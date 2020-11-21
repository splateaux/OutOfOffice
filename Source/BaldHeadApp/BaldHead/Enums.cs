using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaldHead
{
	internal class Enums
	{
		internal enum Mode
		{
			/// <summary>
			/// Bad command line argument configuration.  Display usage then exit app.
			/// </summary>
			ExitNow,

			/// <summary>
			/// Enter configuration and network test mode.
			/// </summary>
			Config,

			/// <summary>
			/// Testing mod only.  Do not use.
			/// </summary>
			Llama,

			/// <summary>
			/// Enter game completion mode where user will enter name and create picture
			/// for upload to score server.  Score passed to score server will be -1.
			/// </summary>
			Kiosk,

			/// <summary>
			/// Enter game completion mode where user will enter name and create picture
			/// for upload to score server.  Command line score will be passed to score server.
			/// Note that no white space is allowed between colon and adjoining characters
			/// </summary>
			Score
		}
	}
}
