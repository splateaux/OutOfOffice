using System;
using System.Runtime.InteropServices; 

namespace GenericHid
{
	internal sealed partial class DeviceManagement
	{
		///<summary >
		// API declarations relating to device management (SetupDixxx and 
		// RegisterDeviceNotification functions).   
		/// </summary>

		// from dbt.h

		internal const Int32 DBT_DEVICEARRIVAL = 0X8000;
		internal const Int32 DBT_DEVICEREMOVECOMPLETE = 0X8004;
		internal const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
		internal const Int32 DBT_DEVTYP_HANDLE = 6;
		internal const Int32 DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		internal const Int32 DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		internal const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
		internal const Int32 WM_DEVICECHANGE = 0X219;

		// from setupapi.h

		internal const Int32 DIGCF_PRESENT = 2;
		internal const Int32 DIGCF_DEVICEINTERFACE = 0X10;

		// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

		// Use this one in the call to RegisterDeviceNotification() and
		// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:

		[StructLayout(LayoutKind.Sequential)]
		internal class DEV_BROADCAST_DEVICEINTERFACE
		{
			internal Int32 dbcc_size = 0;
			internal Int32 dbcc_devicetype = 0;
			internal Int32 dbcc_reserved = 0;
			internal Guid dbcc_classguid = Guid.Empty;
			internal Int16 dbcc_name = 0;
		}

		// Use this to read the dbcc_name String and classguid:

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal class DEV_BROADCAST_DEVICEINTERFACE_1
		{
			internal Int32 dbcc_size = 0;
			internal Int32 dbcc_devicetype = 0;
			internal Int32 dbcc_reserved = 0;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
            internal Byte[] dbcc_classguid = new Byte[0];
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			internal Char[] dbcc_name = new Char[0];
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class DEV_BROADCAST_HDR
		{
			internal Int32 dbch_size = 0;
			internal Int32 dbch_devicetype = 0;
			internal Int32 dbch_reserved = 0;
		}

// Disable "Field 'xxx' is never assigned to, and will always have its default value of 0'
#pragma warning disable 0649

		internal struct SP_DEVICE_INTERFACE_DATA
		{
			internal Int32 cbSize;
			internal System.Guid InterfaceClassGuid;
			internal Int32 Flags;
			internal IntPtr Reserved;
		}

		internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			internal Int32 cbSize;
			internal String DevicePath;
		}

		internal struct SP_DEVINFO_DATA
		{
			internal Int32 cbSize;
			internal System.Guid ClassGuid;
			internal Int32 DevInst;
			internal Int32 Reserved;
		}

#pragma warning restore 0649

        /// <summary>
        /// Request to receive notification messages when a device in an interface class
        //  is attached or removed.
        /// </summary>
        /// <param name="hRecipient">Handle to the window that will receive device events.</param>
        /// <param name="NotificationFilter">Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of 
        //  device to send notifications for.</param>
        /// <param name="Flags">DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.</param>
        /// <returns>Device notification handle or NULL on failure.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

        // UNUSED.
		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern Int32 SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, Int32 hwndParent);

        /// <summary>
        /// Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.
        /// </summary>
        /// <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs.</param>
        /// <returns>True on success, False on failure.</returns>
		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        /// <summary>
        /// Retrieves a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.
        //  On return, MyDeviceInterfaceData contains the handle to a
        //  SP_DEVICE_INTERFACE_DATA structure for a detected device.
        /// </summary>
        /// <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs.</param>
        /// <param name="DeviceInfoData">Optional SP_DEVINFO_DATA structure that defines a device instance 
        //  that is a member of a device information set.</param>
        /// <param name="InterfaceClassGuid">Device interface GUID.</param>
        /// <param name="MemberIndex">Index to specify a device in a device information set.</param>
        /// <param name="DeviceInterfaceData">Pointer to a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.</param>
        /// <returns>True on success, False on failure.</returns>
		[DllImport("setupapi.dll", SetLastError = true)]
		internal static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref System.Guid InterfaceClassGuid, Int32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

		/// <summary>
		/// Retrieves a device information set for a specified group of devices.
		//  SetupDiEnumDeviceInterfaces uses the device information set.
		/// </summary>
		/// <param name="ClassGuid">Interface class GUID.</param>
		/// <param name="Enumerator">Null to retrieve information for all device instances.</param>
		/// <param name="hwndParent">Optional handle to a top-level window (unused here).</param>
		/// <param name="Flags">Flags to limit the returned information to currently present devices 
		//  and devices that expose interfaces in the class specified by the GUID.</param>
		/// <returns>Handle to a device information set for the devices.</returns>
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr SetupDiGetClassDevs(ref System.Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

		/// <summary>
		/// Retrieves an SP_DEVICE_INTERFACE_DETAIL_DATA structure
		//  containing information about a device.
		//  To retrieve the information, call this function twice.
		//  The first time returns the size of the structure.
		//  The second time returns a pointer to the data.
		/// </summary>
		/// <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs
		/// SP_DEVICE_INTERFACE_DATA structure returned by SetupDiEnumDeviceInterfaces</param>
		/// <param name="DeviceInterfaceData">A returned pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA.</param>
		/// <param name="DeviceInterfaceDetailData">Structure to receive information about the specified interface.</param>
		/// <param name="DeviceInterfaceDetailDataSize">The size of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.</param>
		/// <param name="RequiredSize">Pointer to a variable that will receive the returned required size of the 
		//  SP_DEVICE_INTERFACE_DETAIL_DATA structure.</param>
		/// <param name="DeviceInfoData">Returned pointer to an SP_DEVINFO_DATA structure to receive information about the device.</param>
		/// <returns></returns>
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern Boolean SetupDiGetDeviceInterfaceDetail(
			IntPtr DeviceInfoSet,
			ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, 
			IntPtr DeviceInterfaceDetailData,
			Int32 DeviceInterfaceDetailDataSize, 
			ref Int32 RequiredSize,
			IntPtr DeviceInfoData);
		
		/// <summary>
		/// Stop receiving notification messages.
		/// </summary>
		/// <param name="Handle">Handle returned previously by RegisterDeviceNotification.</param>
		/// <returns>True on success, False on failure.</returns>
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern Boolean UnregisterDeviceNotification(IntPtr Handle);
	}
}
