using Scoring.ScoreKeeperService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Scoring
{
    public static class Reporter
    {
        public static void Send(string character, string name, int points)
        {
            byte[] picture = GetGamerAvatar(character);

            using (var client = new ScoreServiceClient())
            {
                client.Add(new Score()
                {
                    Name = name,
                    Points = points,
                    Picture = picture,
                });
            }
        }

        private static byte[] GetGamerAvatar(string player)
        {
            Stream stream;
            if (player.Equals("Don", StringComparison.OrdinalIgnoreCase))
            {
                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Scoring.Resources.Don_CloseupSmall.png");
            }
            else if (player.Equals("McB", StringComparison.OrdinalIgnoreCase))
            {
                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Scoring.Resources.McB_CloseupSmall.png");
            }
            else
            {
                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Scoring.Resources.John_CloseupSmall.png");
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Dispose();
            return buffer;
        }
    }
}
