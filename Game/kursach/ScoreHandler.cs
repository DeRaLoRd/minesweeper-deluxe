using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace kursach
{

    internal class ScoreHandler
    {
        public static string Path { get; set; }

        static ScoreHandler()
        {
            Path = "records.dat";
        }

        public static void WriteAll(List<Score> list)
        {
            using (BinaryWriter bwriter = new BinaryWriter(File.Open(Path, FileMode.Create)))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    bwriter.Write(list[i].difficulty);
                    bwriter.Write(list[i].time.minutes);
                    bwriter.Write(list[i].time.seconds);
                }
            }
        }

        public static void ReadAll(List<Score> list)
        {
            using (BinaryReader breader = new BinaryReader(File.Open(Path, FileMode.OpenOrCreate)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Score temp = new Score();
                    try
                    {
                        temp.difficulty = breader.ReadInt32();
                        temp.time.minutes = breader.ReadInt32();
                        temp.time.seconds = breader.ReadInt32();
                    }
                    catch (IOException)
                    {
                        break;
                    }

                    list.Add(temp);
                }
            }
        }
    }
}
