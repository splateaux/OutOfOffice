using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Touchless.Vision.Camera;

namespace BaldHead
{
	public partial class MainForm : Form
	{
		// Application mode.
		Enums.Mode _appMode;

		// Class holding all items persisted to disk.
		public PersistedSettings _settings;

		// Config.cs to WebCam communication variable.
		//  THIS PROPERTY CONTROLS THE TYPE OF WEBCAM DRAWING THAT OCCURS WHEN STREAMING.
		internal ConfigScreenEnum ConfigScreen { get; set; }

		// Score parsed from command line and parsed to TakePicture.
		public int Score { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args">Command line args</param>
		public MainForm(string[] args)
		{
			bool success = PersistedSettings.ReadFromDisk(out _settings);
			if (!success)
			{
				// Object creation failed.  Create a default PersistedSettings object.
				_settings = new PersistedSettings();
				_settings.SetValidDefaultState();
			}

			InitializeComponent();
			Score = -1;
			ParseCommandLineArgs(args);
		}

		/// <summary>
		/// Wrapper for PersistedSettings.WriteToDisk()
		/// </summary>
		public void PersistStateToDisk()
		{
			_settings.WriteToDisk();
		}

		/// <summary>
		/// Parses the command line arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		void ParseCommandLineArgs(string[] args)
		{
			_appMode = Enums.Mode.ExitNow;

			if (args.Count() == 1)
			{
				string cmdSwitch = args[0].ToLower();

				if (cmdSwitch.StartsWith("/config") && cmdSwitch.Length == 7)
				{
					_appMode = Enums.Mode.Config;
				}
				else if (cmdSwitch.StartsWith("/llama") && cmdSwitch.Length == 6)
				{
					_appMode = Enums.Mode.Llama;
				}
				else if (cmdSwitch.StartsWith("/kiosk") && cmdSwitch.Length == 6)
				{
					_appMode = Enums.Mode.Kiosk;
					throw new NotImplementedException();
				}
				else if (cmdSwitch.StartsWith("/score:") && cmdSwitch.Length > 7)
				{
					// Parse score value.
					int value;
					bool success = Int32.TryParse(cmdSwitch.Substring(7), out value);
					if (success)
					{
						Score = value;
					}

					_appMode = Enums.Mode.Score;
				}
			}
		}

		/// <summary>
		/// Exit application if command line error was detected in Form1 ctor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Activated(object sender, EventArgs e)
		{
			// Never display the main form.
			this.Visible = false;
			Form form = new TextBoxDisplay(this, Consts.AppHelp);

			// Go to mode specific screen.
			switch (_appMode)
			{
				case Enums.Mode.ExitNow:
					// Default case is to display user help.
					break;
				case Enums.Mode.Config:
					form = new Config(this);
					break;
				case Enums.Mode.Llama:
					form = new Llama(this);
					break;
				case Enums.Mode.Kiosk:
					break;
				case Enums.Mode.Score:
					form = new TakePicture(this);
					break;
				default:
					break;
			}

			// Display for for desired class and exit when modal dialog class exits.
			form.ShowDialog();
			Application.Exit();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}


	}
}
