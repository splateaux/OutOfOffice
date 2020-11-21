using Microsoft.VisualBasic;
using Microsoft.Win32.SafeHandles; 
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Windows.Forms;

///  <summary>
///  For communicating with HID-class USB devices.
///  Includes routines for sending and receiving reports via control transfers and to 
///  retrieve information about and configure a HID.
///  </summary>
///  

namespace GenericHid
{    
	internal sealed partial class Hid  
	{         
		//  Used in error messages.
		
		private const String MODULE_NAME = "Hid"; 
		
		internal HIDP_CAPS Capabilities; 
		internal HIDD_ATTRIBUTES DeviceAttributes; 
		
		///  <summary>
		///  Remove any Input reports waiting in the buffer.
		///  </summary>
		///  
		///  <param name="hidHandle"> a handle to a device.   </param>
		///  
		///  <returns>
		///  True on success, False on failure.
		///  </returns>
		
		internal Boolean FlushQueue( SafeFileHandle hidHandle ) 
		{             
			Boolean success = false; 
			
			try 
			{ 
				success = HidD_FlushQueue( hidHandle ); 
				
				return success;                 
			} 
			catch ( Exception ex ) 
			{ 
				DisplayException( MODULE_NAME, ex ); 
				throw ; 
			}             
		}         
		
		///  <summary>
		///  Retrieves a structure with information about a device's capabilities. 
		///  </summary>
		///  
		///  <param name="hidHandle"> a handle to a device. </param>
		///  
		///  <returns>
		///  An HIDP_CAPS structure.
		///  </returns>
		
		internal HIDP_CAPS GetDeviceCapabilities( SafeFileHandle hidHandle ) 
		{             
			IntPtr preparsedData = new System.IntPtr(); 
			Int32 result = 0; 
			Boolean success = false;  

			try
			{
				success = HidD_GetPreparsedData(hidHandle, ref preparsedData);

				result = HidP_GetCaps(preparsedData, ref Capabilities);
				if ((result != 0))
				{
                    //Debug.WriteLine("");
                    //Debug.WriteLine("  Usage: " + Convert.ToString(Capabilities.Usage, 16));
                    //Debug.WriteLine("  Usage Page: " + Convert.ToString(Capabilities.UsagePage, 16));
                    //Debug.WriteLine("  Input Report Byte Length: " + Capabilities.InputReportByteLength);
                    //Debug.WriteLine("  Output Report Byte Length: " + Capabilities.OutputReportByteLength);
                    //Debug.WriteLine("  Feature Report Byte Length: " + Capabilities.FeatureReportByteLength);
                    //Debug.WriteLine("  Number of Link Collection Nodes: " + Capabilities.NumberLinkCollectionNodes);
                    //Debug.WriteLine("  Number of Input Button Caps: " + Capabilities.NumberInputButtonCaps);
                    //Debug.WriteLine("  Number of Input Value Caps: " + Capabilities.NumberInputValueCaps);
                    //Debug.WriteLine("  Number of Input Data Indices: " + Capabilities.NumberInputDataIndices);
                    //Debug.WriteLine("  Number of Output Button Caps: " + Capabilities.NumberOutputButtonCaps);
                    //Debug.WriteLine("  Number of Output Value Caps: " + Capabilities.NumberOutputValueCaps);
                    //Debug.WriteLine("  Number of Output Data Indices: " + Capabilities.NumberOutputDataIndices);
                    //Debug.WriteLine("  Number of Feature Button Caps: " + Capabilities.NumberFeatureButtonCaps);
                    //Debug.WriteLine("  Number of Feature Value Caps: " + Capabilities.NumberFeatureValueCaps);
                    //Debug.WriteLine("  Number of Feature Data Indices: " + Capabilities.NumberFeatureDataIndices);
					
					Int32 vcSize = Capabilities.NumberInputValueCaps;
					Byte[] valueCaps = new Byte[vcSize];

					result = HidP_GetValueCaps(HidP_Input, valueCaps, ref vcSize, preparsedData);
			
					// (To use this data, copy the ValueCaps byte array into an array of structures.)                   

				}
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
			finally
			{
				if (preparsedData != IntPtr.Zero)
				{
					success = HidD_FreePreparsedData(preparsedData);
				}
			} 
			
			return Capabilities;             
		}

		///  <summary>
		///  reads a Feature report from the device.
		///  </summary>
		///  
		///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>	
		///  <param name="myDeviceDetected"> tells whether the device is currently attached.</param>
		///  <param name="inFeatureReportBuffer"> contains the requested report.</param>
		///  <param name="success"> read success</param>

		internal Boolean GetFeatureReport(SafeFileHandle hidHandle, ref Byte[] inFeatureReportBuffer)
		{
			Boolean success = false;

			try
			{
				success = HidD_GetFeature(hidHandle, inFeatureReportBuffer, inFeatureReportBuffer.Length);

				Debug.Print("HidD_GetFeature success = " + success);
				return success;
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
		}             


		///  <summary>
		///  Creates a 32-bit Usage from the Usage Page and Usage ID. 
		///  Determines whether the Usage is a system mouse or keyboard.
		///  Can be modified to detect other Usages.
		///  </summary>
		///  
		///  <param name="MyCapabilities"> a HIDP_CAPS structure retrieved with HidP_GetCaps. </param>
		///  
		///  <returns>
		///  A String describing the Usage.
		///  </returns>
		
		internal String GetHidUsage( HIDP_CAPS MyCapabilities ) 
		{             
			Int32 usage = 0; 
			String usageDescription = ""; 
			
			try 
			{ 
				//  Create32-bit Usage from Usage Page and Usage ID.
				
				usage = MyCapabilities.UsagePage * 256 + MyCapabilities.Usage; 
				
				if ( usage == Convert.ToInt32( 0X102 ) )
				 { 
					usageDescription = "mouse"; } 
				
				if ( usage == Convert.ToInt32( 0X106 ) )
				 { 
					usageDescription = "keyboard"; }                   
			} 
			catch ( Exception ex ) 
			{ 
				DisplayException( MODULE_NAME, ex ); 
				throw ; 
			} 
			
			return usageDescription;             
		}


		///  <summary>
		///  reads an Input report from the device using a control transfer.
		///  </summary>
		///  
		///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
		///  <param name="myDeviceDetected"> tells whether the device is currently attached. </param>
		///  <param name="inputReportBuffer"> contains the requested report. </param>
		///  <param name="success"> read success </param>

		internal Boolean GetInputReportViaControlTransfer(SafeFileHandle hidHandle, ref Byte[] inputReportBuffer)
		{
			Boolean success = false;

			try
			{
				success = HidD_GetInputReport(hidHandle, inputReportBuffer, inputReportBuffer.Length + 1);

				Debug.Print("HidD_GetInputReport success = " + success);
				return success;
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
		} 
				
		///  <summary>
		///  Retrieves the number of Input reports the host can store.
		///  </summary>
		///  
		///  <param name="hidDeviceObject"> a handle to a device  </param>
		///  <param name="numberOfInputBuffers"> an integer to hold the returned value. </param>
		///  
		///  <returns>
		///  True on success, False on failure.
		///  </returns>
		
		internal Boolean GetNumberOfInputBuffers( SafeFileHandle hidDeviceObject, ref Int32 numberOfInputBuffers ) 
		{             
			Boolean success = false;

			try
			{
				success = HidD_GetNumInputBuffers(hidDeviceObject, ref numberOfInputBuffers);
				return success;
			}
			catch (Exception ex)
			{
				DisplayException( MODULE_NAME, ex ); 
				throw ; 
			}                       
		}
		///  <summary>
		///  writes a Feature report to the device.
		///  </summary>
		///  
		///  <param name="outFeatureReportBuffer"> contains the report ID and report data. </param>
		///  <param name="hidHandle"> handle to the device.  </param>
		///  
		///  <returns>
		///   True on success. False on failure.
		///  </returns>            

		internal Boolean SendFeatureReport(SafeFileHandle hidHandle, Byte[] outFeatureReportBuffer)
		{
			Boolean success = false;

			try
			{
				success = HidD_SetFeature(hidHandle, outFeatureReportBuffer, outFeatureReportBuffer.Length);

				Debug.Print("HidD_SetFeature success = " + success);

				return success;
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
		}             
			///  <summary>
			///  Writes an Output report to the device using a control transfer.
			///  </summary>
			///  
			///  <param name="outputReportBuffer"> contains the report ID and report data. </param>
			///  <param name="hidHandle"> handle to the device.  </param>
			///  
			///  <returns>
			///   True on success. False on failure.
			///  </returns>            
			
			internal Boolean SendOutputReportViaControlTransfer(SafeFileHandle hidHandle, Byte[] outputReportBuffer) 
			{                 
				Boolean success = false; 
				
				try 
				{ 
					success = HidD_SetOutputReport(hidHandle, outputReportBuffer, outputReportBuffer.Length + 1); 
					
					Debug.Print( "HidD_SetOutputReport success = " + success ); 
					
					return success;                     
				} 
				catch ( Exception ex ) 
				{ 
					DisplayException( MODULE_NAME, ex ); 
					throw ; 
				}                 
			}    
 
		///  <summary>
		///  Sets the number of input reports the host will store.
		///  Requires Windows XP or later.
		///  </summary>
		///  
		///  <param name="hidDeviceObject"> a handle to the device.</param>
		///  <param name="numberBuffers"> the requested number of input reports.  </param>
		///  
		///  <returns>
		///  True on success. False on failure.
		///  </returns>
		
		internal Boolean SetNumberOfInputBuffers( SafeFileHandle hidDeviceObject, Int32 numberBuffers ) 
		{              
			try 
			{ 
			    HidD_SetNumInputBuffers( hidDeviceObject, numberBuffers );
				return true;                    
			} 
			catch ( Exception ex ) 
			{ 
				DisplayException( MODULE_NAME, ex ); 
				throw ; 
			}            
		} 
		
		///  <summary>
		///  Provides a central mechanism for exception handling.
		///  Displays a message box that describes the exception.
		///  </summary>
		///  
		///  <param name="moduleName">  the module where the exception occurred. </param>
		///  <param name="e"> the exception </param>
		
		internal static void DisplayException( String moduleName, Exception e ) 
		{
			throw new NotImplementedException();
		}         
	} 
} 
