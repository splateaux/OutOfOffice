using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Touchless.Vision.Camera;
using BaldHead.ScoreKeeper;
using System.Drawing.Imaging;

namespace BaldHead
{
	// Used in BaldHead.Config.cs
	public enum ConfigScreenEnum
	{
		StretchFill,
		UnstretchFill,
		Middle,
		DoNotDraw
	}

	public partial class Config : Form
	{
		MainForm _mainForm;

		// Filename without folder.  It will be installed by default.
		string _bitmapFilename = "Sheep.bmp";

		// Set on entry and when value changes to prevent screen activation from
		//	generating change event.
		Camera _selectedCamera;

		// Streaming camera.
		WebCam _webCam;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="mainForm"></param>
		public Config(MainForm mainForm)
		{
			_mainForm = mainForm;
			InitializeComponent();
			chkFlipImageHorizontally.Checked = _mainForm._settings.FlipImageHorizontally;
			_webCam = new WebCam(mainForm);
			_webCam.SetViewControl(pnlWebCam);

			// Call event handlers.  Used for validation for current values.
			radioFullScaled_CheckedChanged(null, new EventArgs());
			txtName_TextChanged(null, new EventArgs());
			txtScore_TextChanged(null, new EventArgs());

			// Create folder and image path name.  Then send it to image loader.
			_bitmapFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _bitmapFilename);
			SetImage(_bitmapFilename);
			
			// Initialize service client instance.
			ScoreServiceClient item = new ScoreKeeper.ScoreServiceClient();
			lblUrl.Text = item.Endpoint.ListenUri.ToString();
		}

		/// <summary>
		/// Test WCF service by pinging it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnTestConnection_Click(object sender, EventArgs e)
		{
			lblTime.Text = "(Testing for service response)";
			try
			{
				ScoreServiceClient item = new ScoreServiceClient();
				lblTime.Text = item.GetTime();
			}
			catch (Exception ex)
			{
				MessageBox.Show(Utilities.GetAllExceptions(ex), "Exception attempting to get time from server.");
				lblTime.Text = "*****  Exception  *****";
			}
		}

		/// <summary>
		/// Test WCF service by sending info to it.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSendItem_Click(object sender, EventArgs e)
		{
			if (AreSendErrorsDetected())
			{
				MessageBox.Show("Cannot send item until all errors fixed.");
				return;
			}

			Score item = new Score();
			item.Name = txtName.Text;
			item.Points = int.Parse(txtScore.Text);

			if (picImage.Image != null)
			{
				using (MemoryStream memory = new MemoryStream())
				{
					picImage.Image.Save(memory, ImageFormat.Jpeg);
					item.Picture = memory.ToArray();
				}
			}

			try
			{
				using (ScoreServiceClient client = new ScoreServiceClient())
				{
					client.Add(item);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(Utilities.GetAllExceptions(ex), "Exception attempting to send info to server.");
			}
		}

		/// <summary>
		/// Handler - String value changed in Name text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtName_TextChanged(object sender, EventArgs e)
		{
			errorProvider.SetError(txtName,
				txtName.Text.Length > 0 ? null : "Name must be non-zero length.");

			// Update Send button enable/disable;
			AreSendErrorsDetected();
		}

		/// <summary>
		/// Handler - String value changed in Score text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtScore_TextChanged(object sender, EventArgs e)
		{
			int score;
			bool success = int.TryParse(txtScore.Text, out score);

			bool scoreValid = success && score >= -1;

			errorProvider.SetError(txtScore,
				scoreValid ? null : "Score must be integer >= -1.");

			// Update Send button enable/disable;
			AreSendErrorsDetected();
		}

		/// <summary>
		/// If all errorProvider items are OK, enable the Send item button.
		/// </summary>
		/// <returns></returns>
		private bool AreSendErrorsDetected()
		{
			// If there is no image to send, we fail.
			bool errorFound = (picImage.Image == null);

			errorProvider.SetError(picImage,
				errorFound ? "Must supply a bitmap image." : null);

			// If no error yet, test other items.
			if (!errorFound)
			{
				errorFound =
					this.grpSendItemTest.Controls.OfType<TextBox>()
						.Any(t => !String.IsNullOrWhiteSpace(errorProvider.GetError(t)))
					|| !String.IsNullOrWhiteSpace(errorProvider.GetError(picImage));
			}

			btnSendItem.Enabled = !errorFound;

			return errorFound;
		}

		/// <summary>
		/// Handler - Browse for bitmap image.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				SetImage(ofd.FileName);
			}

			AreSendErrorsDetected();
		}

		/// <summary>
		/// Load image in path into PictureBox.
		/// </summary>
		/// <param name="path"></param>
		private void SetImage(string path)
		{
			try
			{
				// Selected a new file.
				_bitmapFilename = path;
				lblBitmapFilename.Text = Path.GetFileName(_bitmapFilename);
				picImage.Load(_bitmapFilename);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				picImage.Image = null;
			}
			finally
			{
				AreSendErrorsDetected();
			}
		}

		/// <summary>
		/// Handler - Form load.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Config_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				// Refresh the list of available cameras
				cmbCameras.Items.Clear();
				foreach (Camera cam in _webCam.GetCameras())
				{
					cmbCameras.Items.Add(cam);
				}

				// Find string value of last setting.  If not found, set it to index 0.
				int cmbIndex = 0;
				for (int i = 0; i < cmbCameras.Items.Count; i++)
				{
					if (((Camera)cmbCameras.Items[i]).Name == _mainForm._settings.SelectedCameraDisplayName)
					{
						cmbIndex = i;
					}
				}

				// Set combo box index. It will be the selected camera or zero.
				if (cmbCameras.Items.Count > 0)
				{
					// Restore persisted camera.
					cmbCameras.SelectedIndex = cmbIndex;
					_webCam.SetCamera((Camera)cmbCameras.SelectedItem);
			
					// If there was not persisted camera, then the cmbIndex will
					//  be zero.  If the selected cmbIndex name doesn't match
					//  the persisted one (it won't), set the flag to enable
					//  user to save this "new" selection.
					if (_mainForm._settings.SelectedCameraDisplayName
						!= ((Camera)cmbCameras.SelectedItem).Name)
					{
						// Save camera and dirty flag.
						_mainForm._settings.SelectedCameraDisplayName = ((Camera)cmbCameras.SelectedItem).Name;
						btnSaveSettings.Enabled = true;
					}
				}

				timer.Enabled = true;
			}

			// Enable GUI events now that all is set up.
			cmbCameras.SelectedIndexChanged
				+= new EventHandler(this.cmbCameras_SelectedIndexChanged);
		}

		/// <summary>
		/// Save settings to disk.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSaveSettings_Click(object sender, EventArgs e)
		{
			_mainForm.PersistStateToDisk();
			btnSaveSettings.Enabled = false;
		}

		/// <summary>
		/// Make sure the selected value is different from value stored
		/// in this class before changing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmbCameras_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox combo = (ComboBox)sender;

			if (_webCam.GetCamera() != (Camera)combo.SelectedItem)
			{
				_mainForm._settings.SelectedCameraDisplayName = ((Camera)combo.SelectedItem).Name;

				// We have camera object and it is different than the current setting.
				//	Stop current streaming (if any) and set new camera.
				_webCam.Stop();
				_selectedCamera = (Camera)combo.SelectedItem;
				_webCam.SetCamera(_selectedCamera);
                _webCam.FlipImageHorizontally(_mainForm._settings.FlipImageHorizontally);

				// Set dirty flag.
				btnSaveSettings.Enabled = true;
			}
		}

		/// <summary>
		/// Start streaming from WebCam.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStart_Click(object sender, EventArgs e)
		{
			_webCam.Start();
            _webCam.FlipImageHorizontally(_mainForm._settings.FlipImageHorizontally);
		}

		/// <summary>
		/// Stop streaming from WebCam.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStop_Click(object sender, EventArgs e)
		{
			_webCam.Stop();
		}

		private void btnConfig_Click(object sender, EventArgs e)
		{
			_webCam.ShowPropertiesDialogBox();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (_webCam.IsCapturing)
			{
				txtCameraStats.Text = _webCam.GetCameraStatistics();
			}
		}

		private void radioFullScaled_CheckedChanged(object sender, EventArgs e)
		{
			_mainForm.ConfigScreen = ConfigScreenEnum.StretchFill;
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			_mainForm.ConfigScreen = ConfigScreenEnum.UnstretchFill;
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			_mainForm.ConfigScreen = ConfigScreenEnum.Middle;
		}

		private void chkFlipImageHorizontally_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)sender;
			
			if (_webCam != null && _webCam.IsCapturing)
			{
				_mainForm._settings.FlipImageHorizontally = cb.Checked;
				_webCam.FlipImageHorizontally(cb.Checked);


				// Set dirty flag.
				btnSaveSettings.Enabled = true;
			}

		}
	}
}
