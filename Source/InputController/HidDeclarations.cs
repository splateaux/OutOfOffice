using Microsoft.Win32.SafeHandles; 
using System;
using System.Runtime.InteropServices; 

namespace GenericHid
{    
	internal sealed partial class Hid  
	{         
		//  API declarations for HID communications.
		
		//  from hidpi.h
		//  Typedef enum defines a set of integer constants for HidP_Report_Type
		
		internal const Int16 HidP_Input = 0; 
		internal const Int16 HidP_Output = 1; 
		internal const Int16 HidP_Feature = 2;

// Disable "Field 'xxx' is never assigned to, and will always have its default value of 0'
#pragma warning disable 0649

        [ StructLayout( LayoutKind.Sequential ) ]
		internal struct HIDD_ATTRIBUTES 
		{ 
			internal Int32 Size;
			internal UInt16 VendorID;
			internal UInt16 ProductID;
			internal UInt16 VersionNumber; 
		}  
				
		internal struct HIDP_CAPS 
		{ 
			internal Int16 Usage; 
			internal Int16 UsagePage; 
			internal Int16 InputReportByteLength; 
			internal Int16 OutputReportByteLength; 
			internal Int16 FeatureReportByteLength; 
			[ MarshalAs( UnmanagedType.ByValArray, SizeConst=17 ) ]internal Int16[] Reserved; 
			internal Int16 NumberLinkCollectionNodes; 
			internal Int16 NumberInputButtonCaps; 
			internal Int16 NumberInputValueCaps; 
			internal Int16 NumberInputDataIndices; 
			internal Int16 NumberOutputButtonCaps; 
			internal Int16 NumberOutputValueCaps; 
			internal Int16 NumberOutputDataIndices; 
			internal Int16 NumberFeatureButtonCaps; 
			internal Int16 NumberFeatureValueCaps; 
			internal Int16 NumberFeatureDataIndices;             
		}         
		
		//  If IsRange is false, UsageMin is the Usage and UsageMax is unused.
		//  If IsStringRange is false, StringMin is the String index and StringMax is unused.
		//  If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.
		
		internal struct HidP_Value_Caps 
		{ 
			internal Int16 UsagePage; 
			internal Byte ReportID; 
			internal Int32 IsAlias; 
			internal Int16 BitField; 
			internal Int16 LinkCollection; 
			internal Int16 LinkUsage; 
			internal Int16 LinkUsagePage; 
			internal Int32 IsRange; 
			internal Int32 IsStringRange; 
			internal Int32 IsDesignatorRange; 
			internal Int32 IsAbsolute; 
			internal Int32 HasNull; 
			internal Byte Reserved; 
			internal Int16 BitSize; 
			internal Int16 ReportCount; 
			internal Int16 Reserved2; 
			internal Int16 Reserved3; 
			internal Int16 Reserved4; 
			internal Int16 Reserved5; 
			internal Int16 Reserved6; 
			internal Int32 LogicalMin; 
			internal Int32 LogicalMax; 
			internal Int32 PhysicalMin; 
			internal Int32 PhysicalMax; 
			internal Int16 UsageMin; 
			internal Int16 UsageMax; 
			internal Int16 StringMin; 
			internal Int16 StringMax; 
			internal Int16 DesignatorMin; 
			internal Int16 DesignatorMax; 
			internal Int16 DataIndexMin; 
			internal Int16 DataIndexMax; 
		}

#pragma warning restore 0649

        /// <summary>
		/// Removes any Input reports waiting in the buffer.
		/// </summary>
		/// <param name="HidDeviceObject">Handle to the device</param>
		/// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_FlushQueue( SafeFileHandle HidDeviceObject );

        /// <summary>
        /// Frees the buffer reserved by HidD_GetPreparsedData.
        /// </summary>
        /// <param name="PreparsedData">A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_FreePreparsedData( IntPtr PreparsedData );        
		
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_GetAttributes( SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes );

        /// <summary>
        /// Attempts to read a Feature report from the device.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to a HID.</param>
        /// <param name="lpReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <param name="ReportBufferLength">The size of the buffer.</param>
        /// <returns></returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_GetFeature( SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength );

        /// <summary>
        /// Attempts to read an Input report from the device using a control transfer.
        //  Supported under Windows XP and later only.
        /// </summary>
        /// <param name="HidDeviceObject"></param>
        /// <param name="lpReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <param name="ReportBufferLength">The size of the buffer.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_GetInputReport( SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength );        
		
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern void HidD_GetHidGuid( ref System.Guid HidGuid );

        /// <summary>
        /// Retrieves the number of Input reports the host can store.
        //  Not supported by Windows 98 Gold.
        //  If the buffer is full and another report arrives, the host drops the 
        //  oldest report.
        /// </summary>
        /// <param name="HidDeviceObject"></param>
        /// <param name="NumberBuffers">Number of buffers.</param>
        /// <returns></returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_GetNumInputBuffers( SafeFileHandle HidDeviceObject, ref Int32 NumberBuffers );

        /// <summary>
        /// Retrieves a pointer to a buffer containing information about the device's capabilities.
        //  HidP_GetCaps and other API functions require a pointer to the buffer.
        /// </summary>
        /// <param name="HidDeviceObject">A handle returned by CreateFile.</param>
        /// <param name="PreparsedData">A pointer to a buffer.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_GetPreparsedData( SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData );

        /// <summary>
        /// Attempts to send a Feature report to the device.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to a HID.</param>
        /// <param name="lpReportBuffer">A pointer to a buffer containing the report ID and report.</param>
        /// <param name="ReportBufferLength">The size of the buffer.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_SetFeature( SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength );

        /// <summary>
        /// Sets the number of Input reports the host can store.
        //  If the buffer is full and another report arrives, the host drops the oldest report.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to a HID.</param>
        /// <param name="NumberBuffers">Number of buffers.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_SetNumInputBuffers( SafeFileHandle HidDeviceObject, Int32 NumberBuffers );

        /// <summary>
        /// Attempts to send an Output report to the device using a control transfer.
        //  Requires Windows XP or later.
        /// </summary>
        /// <param name="HidDeviceObject">A handle to a HID.</param>
        /// <param name="lpReportBuffer">A pointer to a buffer containing the report ID and report</param>
        /// <param name="ReportBufferLength"> The size of the buffer.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Boolean HidD_SetOutputReport( SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength );

        /// <summary>
        /// Find out a device's capabilities.
        //  For standard devices such as joysticks, you can find out the specific
        //  capabilities of the device.
        //  For a custom device where the software knows what the device is capable of,
        //  this call may be unneeded.
        /// </summary>
        /// <param name="PreparsedData">A pointer returned by HidD_GetPreparsedData.</param>
        /// <param name="Capabilities">A pointer to a HIDP_CAPS structure.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]
		internal static extern Int32 HidP_GetCaps( IntPtr PreparsedData, ref HIDP_CAPS Capabilities );

        /// <summary>
        /// Retrieves a buffer containing an array of HidP_ValueCaps structures.
        //  Each structure defines the capabilities of one value.
        //  This application doesn't use this data.
        /// </summary>
        /// <param name="ReportType">A report type enumerator from hidpi.h.</param>
        /// <param name="ValueCaps">A pointer to a buffer for the returned array.</param>
        /// <param name="ValueCapsLength">The NumberInputValueCaps member of the device's HidP_Caps structure.</param>
        /// <param name="PreparsedData">A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.</param>
        /// <returns>True on success, False on failure.</returns>
		[ DllImport( "hid.dll", SetLastError=true ) ]       
		internal static extern Int32 HidP_GetValueCaps(Int32 ReportType, Byte[] ValueCaps, ref Int32 ValueCapsLength, IntPtr PreparsedData);
   }   
} 
