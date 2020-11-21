using GenericHid;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace InputController
{
    public class UsbSingleNintendoController : NativeWindow
    {
        const bool WRITE_DEBUG_INFO = false;

        private const Int32 USB_NINTENDO_VID = 0x0079;
        private const Int32 USB_NINTENDO_PID = 0x0011;
        private const Int32 USB_NUMBER_INPUT_REPORT_BUFFERS = 2;
        private const int USB_INPUT_REPORT_LEFT_RIGHT = 4;
        private const int USB_INPUT_REPORT_UP_DOWN = 5;
        private const int USB_INPUT_REPORT_A_B = 6;
        private const int USB_INPUT_REPORT_MASK_A = 0x20;
        private const int USB_INPUT_REPORT_MASK_B = 0x40;
        private const int USB_INPUT_REPORT_START_SELECT = 7;
        private const int USB_INPUT_REPORT_MASK_START = 0x20;
        private const int USB_INPUT_REPORT_MASK_SELECT = 0x10;

        // Constant values were found in the "windows.h" header file.
        private const int WS_CHILD = 0x40000000,
                      WM_ACTIVATEAPP = 0x001C;

        private FileStream fileStreamDeviceData;
        private SafeFileHandle hidHandle;
        private Boolean myDeviceDetected = false;
        private String myDevicePathName = null;
        private DeviceManagement MyDeviceManagement = new DeviceManagement();
        private IntPtr deviceNotificationHandle = IntPtr.Zero;
        private Hid MyHid = new Hid();
        private IntPtr _parentHandle = IntPtr.Zero;
        private Boolean transferInProgress = false;
        private Debugging MyDebugging = new Debugging();
        private readonly object _lockObject = new object();
        private ControllerKey _lastRead = ControllerKey.Stationary;

        public UsbSingleNintendoController(IntPtr parentHandle)
        {
            myDeviceDetected = false;
            _parentHandle = parentHandle;

            // Create a window for its message pump.
            CreateParams cp = new CreateParams();
            cp.Caption = "dummy";
            cp.ClassName = "Button";
            cp.Parent = _parentHandle;
            cp.Style = WS_CHILD;
            this.CreateHandle(cp);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == DeviceManagement.WM_DEVICECHANGE)
            {
                OnDeviceChange(m);
            }

            base.WndProc(ref m);
        }

        public ControllerKey ControllerInput
        {
            get { return _lastRead; }
        }

        /// <summary>
        /// Handler for WndProc WM_DEVICECHANGE message.
        /// </summary>
        /// <param name="m"></param>
        private void OnDeviceChange(Message m)
        {
            if (m.WParam.ToInt32() == DeviceManagement.DBT_DEVICEARRIVAL)
            {
                if (MyDeviceManagement.DeviceNameMatch(m, myDevicePathName))
                {
                    Debug.WriteLineIf(WRITE_DEBUG_INFO, "Nintendo controller attached.");
                }
            }
            else if (m.WParam.ToInt32() == DeviceManagement.DBT_DEVICEREMOVECOMPLETE)
            {
                if (MyDeviceManagement.DeviceNameMatch(m, myDevicePathName))
                {
                    Debug.WriteLineIf(WRITE_DEBUG_INFO, "Nintendo controller removed.");
                    myDeviceDetected = false;
                }
            }
        }

        /// <summary>
        /// Find the Nintendo USB controller. Also save HID capabilities for device for later.
        /// </summary>
        /// <returns></returns>
        public bool FindController()
        {
            bool deviceFound = false;
            Guid hidGuid = Guid.Empty;
            String[] devicePathName = new String[255];
            Int32 memberIndex = 0;
            bool success = false;

            try
            {
                myDeviceDetected = false;
                CloseCommunications();

                // Get the Guid associated with USB HID class.
                Hid.HidD_GetHidGuid(ref hidGuid);

                Debug.WriteLineIf(WRITE_DEBUG_INFO,
                    MyDebugging.ResultOfAPICall("Hid.HidD_GetHidGuid"));

                // Get all attached HIDs.  I am assuming there are a maximum of 128.
                deviceFound = MyDeviceManagement.FindDeviceFromGuid(hidGuid, ref devicePathName);

                if (deviceFound)
                {
                    memberIndex = 0;

                    // Loop through all USB HIDs found and look for my VID/PID.
                    do
                    {
                        // Open HID handle without read nor write access to get info.
                        hidHandle = FileIO.CreateFile(
                            devicePathName[memberIndex],
                            0,
                            FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE,
                            IntPtr.Zero,
                            FileIO.OPEN_EXISTING,
                            0,
                            0);

                        Debug.WriteLineIf(WRITE_DEBUG_INFO,
                            MyDebugging.ResultOfAPICall("FileIO.CreateFile"));

                        if (!hidHandle.IsInvalid)
                        {
                            // Get DeviceAttribute size in bytes.
                            MyHid.DeviceAttributes.Size = Marshal.SizeOf(MyHid.DeviceAttributes);

                            // Get HID attributes.
                            success = Hid.HidD_GetAttributes(hidHandle, ref MyHid.DeviceAttributes);

                            if (success)
                            {
                                if (MyHid.DeviceAttributes.VendorID == USB_NINTENDO_VID
                                    && MyHid.DeviceAttributes.ProductID == USB_NINTENDO_PID)
                                {
                                    myDeviceDetected = true;
                                    myDevicePathName = devicePathName[memberIndex];
                                }
                                else
                                {
                                    // Not a match - close handle.
                                    hidHandle.Close();
                                }
                            }
                            else
                            {
                                // There was a problem trying to retrieve the HID info.
                                Debug.WriteLineIf(WRITE_DEBUG_INFO,
                                    "Error trying to get HID attributes in " +
                                    "UsbSingleNintendoController.FindController() when calling " +
                                    "Hid.HidD_GetAttributes(...)");
                                hidHandle.Close();
                            }

                        }

                        // Prepare to go to next index.
                        memberIndex++;
                    }
                    while (!((myDeviceDetected || (memberIndex == devicePathName.Length))));
                }

                // If device found, wire it up to this class.
                if (myDeviceDetected)
                {
                    // Only allow a very few number of input reports to be queued.
                    MyHid.SetNumberOfInputBuffers(hidHandle, USB_NUMBER_INPUT_REPORT_BUFFERS);

                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        MyDebugging.ResultOfAPICall("MyHid.SetNumberOfInputBuffers"));


                    // Register for notification if HID removed or attached.
                    success = MyDeviceManagement.RegisterForDeviceNotifications(myDevicePathName,
                        this.Handle, hidGuid, ref deviceNotificationHandle);

                    Debug.WriteLineIf(WRITE_DEBUG_INFO && !success,
                            "Failed at: MyDeviceManagement.RegisterForDeviceNotifications");

                    // Get capabilities report for report sizes.
                    MyHid.Capabilities = MyHid.GetDeviceCapabilities(hidHandle);

                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        MyDebugging.ResultOfAPICall("MyHid.GetDeviceCapabilities"));

                    if (success)
                    {
                        // Close handle then reopen in RW mode.
                        hidHandle.Close();
                        hidHandle = FileIO.CreateFile(
                            myDevicePathName,
                            FileIO.GENERIC_READ | FileIO.GENERIC_WRITE,
                            FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE,
                            IntPtr.Zero,
                            FileIO.OPEN_EXISTING,
                            0,
                            0);

#pragma warning disable
                        Debug.WriteLineIf(WRITE_DEBUG_INFO && hidHandle.IsInvalid,
                            "File handle invalid at: FileIO.CreateFile(read/write)");
#pragma warning restore

                        Debug.WriteLineIf(WRITE_DEBUG_INFO,
                            MyDebugging.ResultOfAPICall("FileIO.CreateFile(read/write)"));

                        if (MyHid.Capabilities.InputReportByteLength > 0)
                        {
                            fileStreamDeviceData = new FileStream(
                                hidHandle,
                                FileAccess.ReadWrite,
                                MyHid.Capabilities.InputReportByteLength,
                                false);
                        }

                        MyHid.FlushQueue(hidHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(WRITE_DEBUG_INFO, ex.Message);
                deviceFound = false;
            }

            return deviceFound;
        }

        /// <summary>
        /// Get the controller button status by sending a dummy output report and
        /// getting the keypress input report.
        /// </summary>
        /// <returns><c>true</c> if the device was connected and successfully queried; <c>false</c> otherwise.</returns>
        public bool QueryDevice()
        {
            bool deviceQueried = false;

            //  If the device hasn't been detected, was removed, or timed out on a previous attempt
            //  to access it, look for the device.
            if (!myDeviceDetected)
            {
                myDeviceDetected = FindController();

                if (!myDeviceDetected)
                {
                    // Couldn't find device.  Try again later.
                    return deviceQueried;
                }
            }

            if (transferInProgress || hidHandle.IsInvalid || fileStreamDeviceData == null)
            {
                if (transferInProgress)
                {
                    // Read in progress.  Don't do overlapped reads.
                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        "Skip read...");
                }
                else if (hidHandle.IsInvalid)
                {
                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        "Handle invalid in QueryDevice()");
                }
                else if (fileStreamDeviceData == null)
                {
                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        "Stream fileStreamDeviceDate is null in QueryDevice()");
                }

                return deviceQueried;
            }

            bool success = false;

            // Send output report.  Set first byte to zero to get first report (done by .NET).
            byte[] outputReportBuffer = new byte[MyHid.Capabilities.OutputReportByteLength];
            if (fileStreamDeviceData.CanWrite)
            {
                fileStreamDeviceData.Write(outputReportBuffer, 0, outputReportBuffer.Length);
                success = true;
            }

            // Set up to get input report if output succeeded.
            if (success)
            {
                if (fileStreamDeviceData.CanRead)
                {
                    transferInProgress = true;

                    // Get input report via interrupt transfers with async delegate.
                    byte[] inputReportBuffer = new byte[MyHid.Capabilities.InputReportByteLength];

                    fileStreamDeviceData.BeginRead(
                        inputReportBuffer,
                        0,
                        inputReportBuffer.Length,
                        new AsyncCallback(GetInputReportData),
                        inputReportBuffer);

                    deviceQueried = true;
                }
                else
                {
                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        "Attempt to read report input from device failed.");

                    CloseCommunications();
                }
            }
            else
            {
                // Write failed.  Report to dev, close device and exit.
                Debug.WriteLineIf(WRITE_DEBUG_INFO,
                    "Attempt to write output report to device failed.");

                CloseCommunications();
            }

            return deviceQueried;
        }

        /// <summary>
        /// Called asynchronously with input from USB.
        /// </summary>
        /// <param name="ar"></param>
        private void GetInputReportData(IAsyncResult ar)
        {
            byte[] inputReportBuffer = new byte[0];

            try
            {
                inputReportBuffer = (byte[])ar.AsyncState;

                fileStreamDeviceData.EndRead(ar);

                if (ar.IsCompleted)
                {
                    // Parse controller state and create report for game.
                    ControllerKey key = ControllerKey.Stationary;

                    // Check Left/Right.  Unpressed = 0x7F.
                    if (inputReportBuffer[USB_INPUT_REPORT_LEFT_RIGHT] == 0x00)
                    {
                        key |= ControllerKey.Left;
                    }
                    else if (inputReportBuffer[USB_INPUT_REPORT_LEFT_RIGHT] == 0xFF)
                    {
                        key |= ControllerKey.Right;
                    }

                    // Check Up/Down.  Unpressed = 0x7F.
                    if (inputReportBuffer[USB_INPUT_REPORT_UP_DOWN] == 0x00)
                    {
                        key |= ControllerKey.Up;
                    }
                    else if (inputReportBuffer[USB_INPUT_REPORT_UP_DOWN] == 0xFF)
                    {
                        key |= ControllerKey.Down;
                    }

                    // Remaining non-exclusive bits.
                    if ((inputReportBuffer[USB_INPUT_REPORT_A_B] & USB_INPUT_REPORT_MASK_A)
                        == USB_INPUT_REPORT_MASK_A)
                    {
                        key |= ControllerKey.A;
                    }
                    if ((inputReportBuffer[USB_INPUT_REPORT_A_B] & USB_INPUT_REPORT_MASK_B)
                        == USB_INPUT_REPORT_MASK_B)
                    {
                        key |= ControllerKey.B;
                    }
                    if ((inputReportBuffer[USB_INPUT_REPORT_START_SELECT] & USB_INPUT_REPORT_MASK_SELECT)
                        == USB_INPUT_REPORT_MASK_SELECT)
                    {
                        key |= ControllerKey.Select;
                    }
                    if ((inputReportBuffer[USB_INPUT_REPORT_START_SELECT] & USB_INPUT_REPORT_MASK_START)
                        == USB_INPUT_REPORT_MASK_START)
                    {
                        key |= ControllerKey.Start;
                    }

                    // Save result then clear input buffer to increase input speed.
                    _lastRead = key;
                    MyHid.FlushQueue(hidHandle);

                    Debug.WriteLineIf(WRITE_DEBUG_INFO,
                        MyDebugging.ResultOfAPICall("MyHid.FlushQueue after Async buffer read"));
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                transferInProgress = false;
            }
        }

        /// <summary>
        /// Close the handle and FileStreams for a device.
        /// </summary> 
        private void CloseCommunications()
        {
            if (fileStreamDeviceData != null)
            {
                fileStreamDeviceData.Close();
            }

            if ((hidHandle != null) && (!(hidHandle.IsInvalid)))
            {
                hidHandle.Close();
            }

            // The next attempt to communicate will get new handles and FileStreams.
            myDeviceDetected = false;
        }

        ///  <summary>
        ///  Get number of input reports that can be queued.
        ///  </summary>
        private int GetInputReportBufferSize()
        {
            Int32 numberOfInputBuffers = 0;
            Boolean success = false;

            try
            {
                success = MyHid.GetNumberOfInputBuffers(hidHandle, ref numberOfInputBuffers);

                Debug.WriteLineIf(WRITE_DEBUG_INFO && success,
                    "Max size of input report queue: " + numberOfInputBuffers.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(WRITE_DEBUG_INFO, ex.Message);
            }

            return numberOfInputBuffers;
        }
    }
}
