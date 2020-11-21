using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SoftProGameEngine.Helpers
{
    public class MapExport
    {
        //MapHolder
        public string Name { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int ViewWidth { get; set; }

        public List<string> Data { get; set; }

        //TileExports
        public List<TileExport> Tiles { get; set; }

        //Powerups
        public List<PowerupExport> Powerups { get; set; }

        public MapExport()
        {
            Tiles = new List<TileExport>();
            Powerups = new List<PowerupExport>();
        }

        public bool Save(string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MapExport));
                TextWriter textWriter = new StreamWriter(fileName);
                serializer.Serialize(textWriter, this);
                textWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Load(ref MapExport var, string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(MapExport));
                    TextReader textReader = new StreamReader(fileName);
                    var = (MapExport)deserializer.Deserialize(textReader);
                    textReader.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public TileExport GetTileExport(int index)
        {
            foreach (TileExport tile in Tiles)
            {
                if (tile.Index == index)
                    return tile;
            }
            return null;
        }
    }
}