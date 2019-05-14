using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Video_Sorter
{
    public class MediaFile
    {
        //private AudioStream[] _AudioStream;

        private bool VideoStreamDiscovered = false;
       
        public string Filename
        {
            get;
            private set;
        }

        public long FileSize
        {
            get;
            private set;
        }

        public TimeSpan Duration
        {
            get;
            private set;
        }

        public int Bitrate
        {
            get;
            private set;
        }

        public VideoStream VideoStream
        {
            get;
            private set;
        }


        public MediaFile(string filename)
        {
            if (File.Exists(filename))
            {
                this.Filename = filename;
                this.FileSize = new System.IO.FileInfo(filename).Length;

                GetMediaInfo(filename);
            }
            else
            {
                throw new FileNotFoundException("The file specified does not exist.", filename);
            }
        }



        private void GetMediaInfo(string filename)
        {
            Process ProcessFFmpeg = new Process();

            ProcessFFmpeg.StartInfo.FileName = "ffmpeg.exe";
            ProcessFFmpeg.StartInfo.Arguments = "-i " + "\"" + filename + "\"";

            ProcessFFmpeg.StartInfo.UseShellExecute = false;
            ProcessFFmpeg.StartInfo.RedirectStandardOutput = true;
            ProcessFFmpeg.StartInfo.RedirectStandardError = true;
            ProcessFFmpeg.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ProcessFFmpeg.StartInfo.CreateNoWindow = true;

            ProcessFFmpeg.OutputDataReceived += MediaInfoDataReceived;
            ProcessFFmpeg.ErrorDataReceived += MediaInfoDataReceived;

            ProcessFFmpeg.Exited += (object s, EventArgs e) =>
            {
                ProcessFFmpeg.Close();
                ProcessFFmpeg.Dispose();
            };

            ProcessFFmpeg.Start();
            ProcessFFmpeg.BeginOutputReadLine();
            ProcessFFmpeg.BeginErrorReadLine();
            ProcessFFmpeg.WaitForExit();
        }



        private void MediaInfoDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Length < 1)
            {
                return;
            }


            try
            {
                string[] Parts = e.Data.Split(',');



                //Get Media Duration & Bitrate
                if (e.Data.StartsWith("  Duration: ") && e.Data.Contains(", bitrate: "))
                {
                    string[] Duration = Parts[0].Replace("  Duration: ", "").Split(':');

                    int Hours = int.Parse(Duration[0]);
                    int Minutes = int.Parse(Duration[1]);
                    int Seconds = 0;
                    int Miliseconds = 0;

                    if (Duration[2].Contains("."))
                    {
                        Seconds = int.Parse(Duration[2].Split('.')[0]);
                        Miliseconds = int.Parse(Duration[2].Split('.')[1]);
                    }
                    else
                    {
                        Seconds = int.Parse(Duration[2]);
                    }

                    this.Duration = TimeSpan.FromMilliseconds((Hours * 3600 * 1000) + (Minutes * 60 * 1000) + (Seconds * 1000) + Miliseconds);
                    this.Bitrate = int.Parse(Parts[2].Replace(" bitrate: ", "").Replace(" kb/s", ""));
                }




                //Get Video Stream
                if (e.Data.StartsWith("    Stream #0:") && e.Data.Contains(": Video: ") && !VideoStreamDiscovered)
                {
                    //Get Video Stream Index  
                    int vIndex = int.Parse(Parts[0].Substring("    Stream #0:".Length, 1));

                    //Get Video Codec
                    string vCodec = Parts[0].Split(' ')[7];

                    //Get Video Pixel Format
                    string vPixelFormat = Parts[1].Split(' ')[1].Split('(')[0];

                    int vResolutionX = 0;
                    int vResolutionY = 0;
                    int vBitrate = 0;

                    if (Parts[2].Contains('x'))
                    {
                        //Get Video Resolution
                        vResolutionX = int.Parse(Parts[2].Split(' ')[1].Split('x')[0]);
                        vResolutionY = int.Parse(Parts[2].Split(' ')[1].Split('x')[1]);

                        //Get Video Bitrate
                        vBitrate = int.Parse(Parts[3].Split(' ')[1]);
                    }

                    if (Parts[3].Contains('x'))
                    {
                        //Get Video Resolution
                        vResolutionX = int.Parse(Parts[3].Split(' ')[1].Split('x')[0]);
                        vResolutionY = int.Parse(Parts[3].Split(' ')[1].Split('x')[1]);

                        //Get Video Bitrate
                        vBitrate = int.Parse(Parts[4].Split(' ')[1]);
                    }
                    
                    //Get Video Framerate
                    double vFramerate = double.Parse(Parts[5].Split(' ')[1]);

                    this.VideoStream = new VideoStream(vIndex, vCodec, vPixelFormat, vBitrate, vResolutionX, vResolutionY, vFramerate);

                    VideoStreamDiscovered = true;
                }

            }
            catch
            {
                //Split operation failed as Line did not contain any <comma> character
                //OR we tried accessing an item that was out of range             
            }

        }


        public void ShowMediaInfo()
        {
            if (VideoStream != null)
            {
                Console.WriteLine("Media Duration: " + Duration + "\n" +
                                               "Media Bitrate: " + Bitrate + "\n" +
                                               "\n" +
                                               "Video Index: " + VideoStream.Index + "\n" +
                                               "Video Codec: " + VideoStream.Codec + "\n" +
                                               "Video Pixel Format: " + VideoStream.PixelFormat + "\n" +
                                               "Video Resolution: " + VideoStream.ResolutionX + " x " + VideoStream.ResolutionY + "\n" +
                                               "Video Bitrate: " + VideoStream.Bitrate + "\n" +
                                               "Video Framerate: " + VideoStream.Framerate + "\n"
                                               );
            }
        }

    }
}
