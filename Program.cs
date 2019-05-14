using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Video_Sorter
{
    class Program
    {
        static void Main(string[] args)
        {
            String osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            String slash = @"/";

            if (osNameAndVersion.ToLower().Contains("windows"))
            {
                slash = @"\";
            }

            string NewPathError = Directory.GetCurrentDirectory() + slash + "@Error";
            string NewPath854x480p = Directory.GetCurrentDirectory() + slash + "@854x480p";
            string NewPath768x480p = Directory.GetCurrentDirectory() + slash + "@768x480p";
            string NewPathKeepRes = Directory.GetCurrentDirectory() + slash + "@Keep res";
            string NewPathLong = Directory.GetCurrentDirectory() + slash + "@Long";

            MediaFile MediaFile = null;

            int MovedCount = 0;
            int ParsedCount = 0;

            DirectoryInfo CurrentDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            Console.WriteLine("\nFile search started.");


            FileInfo[] Files = CurrentDirectoryInfo.GetFiles("*.mp4", SearchOption.TopDirectoryOnly);
            Console.WriteLine("File search completed. \n");

            Directory.CreateDirectory(NewPathError);
            Directory.CreateDirectory(NewPath854x480p);
            Directory.CreateDirectory(NewPath768x480p);
            Directory.CreateDirectory(NewPathKeepRes);
            Directory.CreateDirectory(NewPathLong);

            Console.WriteLine("File sort started. \n");

            try
            {
                foreach (FileInfo File in Files)
                {
                    MediaFile = new MediaFile(File.FullName);
                    ParsedCount++;

                    if (MediaFile.VideoStream == null)
                    {
                        MovedCount++;
                                                
                        File.MoveTo(NewPathError + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    if (MediaFile.VideoStream.ResolutionX == 1280 && MediaFile.VideoStream.ResolutionY == 720 && MediaFile.Bitrate >= 1000)
                    {
                        MovedCount++;

                        File.MoveTo(NewPath854x480p + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    if (MediaFile.VideoStream.ResolutionX == 1920 && MediaFile.VideoStream.ResolutionY == 1080 && MediaFile.Bitrate >= 1000)
                    {
                        MovedCount++;

                        File.MoveTo(NewPath854x480p + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    if (MediaFile.VideoStream.ResolutionX == 1152 && MediaFile.VideoStream.ResolutionY == 720 && MediaFile.Bitrate >= 1000)
                    {
                        MovedCount++;

                        File.MoveTo(NewPath768x480p + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    if (MediaFile.VideoStream.ResolutionX == 960 && MediaFile.VideoStream.ResolutionY == 540 && MediaFile.Bitrate >= 1000)
                    {
                        MovedCount++;

                        File.MoveTo(NewPath854x480p + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    if (MediaFile.VideoStream.ResolutionY <= 480 && MediaFile.Bitrate >= 1000)
                    {
                        MovedCount++;

                        File.MoveTo(NewPathKeepRes + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    if (MediaFile.Duration >= new TimeSpan(0, 45, 0) && MediaFile.Bitrate <= 900)
                    {
                        MovedCount++;

                        File.MoveTo(NewPathLong + slash + File.Name);
                        Console.WriteLine("Number of files moved: " + MovedCount.ToString());
                    }

                    Console.Title = "Processed " + ParsedCount + " of " + Files.Length + " files";
                }
            }
            catch
            {
                Console.WriteLine("Error: " + MediaFile.Filename);
            }

            if (MovedCount == 0)
            {
                Console.WriteLine("No matching files found!\n");
            }
            else
            {
                Console.WriteLine("\nFile sort completed!\n");
            }
        }
    }
}