using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldHead
{
	public static class Consts
	{
		// TakePicture.cs

		// Button labels when WebCam streaming or paused.
		public const string StartStopButtonLabel_PressToStart = "Press to start webcam";
		public const string StartStopButtonLabel_PressToStop = "Press to pause webcam";

		/// <summary>
		/// Help message.
		/// </summary>
		public static string AppHelp = @"
   BaldHead - Application using webcam to create picture of user with bald head.

   Usage:  BaldHead [switch]

	   Command line switches:

		   (none)    : Display this help.
		   /help     : Display this help.

		   /config   : Enter configuration and network test mode.

		   /llama    : Testing mode only.  Do not use.

		   /kiosk    : Enter game completion mode where user will enter name and
						 create picture for upload to score server.  Score passed
						 to score server will be -1.

		   /game:{score}  : Enter game completion mode where user will enter name
							  and create picture for upload to score server.
							  Command line score will be passed to score server.
							  Note that no white space is allowed between colon
							  and adjoining characters.
";
	}
}
