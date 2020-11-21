using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using Touchless.Vision.Camera;

namespace BaldHead
{
    public class PersistedSettings
    {
        private const string PersistFilename = "Settings.xml";

        //--------------------------------------------------------------------
        // BaldHead.Config.cs 

        // WebCam friendly name.
        public string SelectedCameraDisplayName { get; set; }
        public bool FlipImageHorizontally { get; set; }

        //--------------------------------------------------------------------
        // BaldHead.TakePictures.cs

        // WebCam cropping and resize constants.
        //  Percents are off of width or height of frame buffer from camera.
        public float CropWebCam_WidthPercent { get; set; }
        public float CropWebCam_HeightPercent { get; set; }

        public int Haircut_Offset { get; set; }
        public int Face_Height { get; set; }
        public int Face_Width { get; set; }

        public int HeadImage_VertOffset { get; set; }
        public int HeadImage_HorzOffset { get; set; }

        public System.Drawing.Size Size_HeadImageToScoringServer { get; set; }

        public bool WriteToServerDuringSubmit { get; set; }

        /// <summary>
        /// If settings cannot be restored from file (or if this is the first
        /// time the app is started), there is no settings file to set up a valid
        /// state.  Create one.
        /// </summary>
        public void SetValidDefaultState()
        {
            SelectedCameraDisplayName = "";
            FlipImageHorizontally = true;

            CropWebCam_WidthPercent = 0.5f;
            CropWebCam_HeightPercent = 0.8f;

            Haircut_Offset = 40;
            Face_Height = 30;
            Face_Width = 30;
            HeadImage_VertOffset = 50;
            HeadImage_HorzOffset = 50;

            Size_HeadImageToScoringServer = new System.Drawing.Size(120, 131);
            WriteToServerDuringSubmit = true;
        }

        /// <summary>
        /// Write the passed PersistedSettings object to disk.
        /// </summary>
        /// <returns>True if write succeeds.</returns>
        public bool WriteToDisk()
        {
            bool success = true;

            try
            {
                XmlSerializer writer = new XmlSerializer(typeof(PersistedSettings));
                using (StreamWriter file = new StreamWriter(PersistFilename))
                {
                    writer.Serialize(file, this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Utilities.GetAllExceptions(ex), "Exception attempting to save persistance file.");

                //MessageBox.Show("Unable to save application settings to \""
                //    + Path.GetFileName(PersistFilename)
                //    + "\".  "
                //    + Environment.NewLine + Environment.NewLine + ex.Message, ex.GetType().Name);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Create a PersistedSettings object from settings file read from disk.
        /// </summary>
        /// <param name="instance">Uninitialized object.</param>
        /// <returns>True if restore from disk succeeds.</returns>
        public static bool ReadFromDisk(out PersistedSettings instance)
        {
            bool success = true;
            instance = null;

            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(PersistedSettings));
                using (StreamReader file = new StreamReader(PersistFilename))
                {
                    PersistedSettings ps = new PersistedSettings();
                    ps = (PersistedSettings)reader.Deserialize(file);

                    instance = ps;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to restore application settings from \""
                    + Path.GetFileName(PersistFilename)
                    + "\".  Using application defaults.  "
                    + Environment.NewLine + Environment.NewLine + ex.Message, ex.GetType().Name);
                success = false;
            }

            return success;
        }
    }
}
