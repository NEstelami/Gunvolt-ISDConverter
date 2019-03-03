using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ISDConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> fileNames = GetFileNames();

            // If there already isn't an ogg folder, create one.
            if (!Directory.Exists("ogg"))
                Directory.CreateDirectory("ogg");

            // For every sound file, find the appropriate isd file and decrypt it.
            for (int j = 0; j < fileNames.Count; j++)
            {
                string fileStr = fileNames[j].Split('\t')[0];
                int fileId = Convert.ToInt32(fileNames[j].Split('=')[1].Split('x')[1].Split(';')[0], 16);

                Console.WriteLine("Decrypting " + fileStr + ".ogg");

                // Make sure the isd file in question actually exists.
                if (!File.Exists("stream\\" + fileId + ".isd"))
                {
                    Console.WriteLine(fileStr + " does not exist!");
                }
                else
                {
                    // Read in the isd file, decrypt it with the xor table, then save the decrypted ogg file.
                    byte[] isdData = File.ReadAllBytes("stream\\" + fileId + ".isd");
                    byte[] xorTbl = { 0xE0, 0x00, 0xE0, 0x00, 0xA0, 0x00, 0x00, 0x00, 0xE0, 0x00, 0xE0, 0x80, 0x40, 0x40, 0x40, 0x00 };

                    for (int i = 0; i < isdData.Length; i++)
                    {
                        int ind = (i % xorTbl.Length);
                        isdData[i] ^= xorTbl[ind];
                    }

                    if (!File.Exists("ogg\\" + fileStr + ".ogg"))
                        File.WriteAllBytes("ogg\\" + fileStr + ".ogg", isdData);
                }
            }
        }

        // Reads in the names of sounds from GV_Stream.ish
        // Returns a list of the filenames.
        public static List<string> GetFileNames()
        {
            string[] lines = File.ReadAllLines("GV_steam.ish");
            List<string> fileNames = new List<string>();

            bool startRec = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("// Sound"))
                    startRec = true;

                if (startRec && lines[i].StartsWith("static"))
                {
                    fileNames.Add(lines[i].Split(' ')[4]);
                }
            }

            return fileNames;
        }
    }
}
