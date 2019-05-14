using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Video_Sorter
{
    public class VideoStream
    {
        public int Index
        {
            get;
            private set;
        }

        public string Codec
        {
            get;
            private set;
        }

        public int Bitrate
        {
            get;
            private set;
        }

        public int ResolutionX
        {
            get;
            private set;
        }

        public int ResolutionY
        {
            get;
            private set;
        }

        public double AspectRatio
        {
            get
            {
                return ((double)ResolutionX / (double)ResolutionY);
            }
        }

        public string PixelFormat
        {
            get;
            private set;
        }

        public double Framerate
        {
            get;
            private set;
        }


        public VideoStream(int index, string codec, string pixelFormat, int bitrate, int resolutionX, int resolutionY, double framerate)
        {
            Index = index;
            Codec = codec;
            PixelFormat = pixelFormat;
            Bitrate = bitrate;
            ResolutionX = resolutionX;
            ResolutionY = resolutionY;
            Framerate = framerate;
        }


    }
}
