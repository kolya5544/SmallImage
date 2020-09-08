using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace SmallPicture
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SmallPicture.exe <img picture>");
            } else
            {
                string filename = args[0];
                byte[] contents = File.ReadAllBytes(filename);

                byte PicSize = contents[0];
                int Width = 0, Height = 0;
                Width = (PicSize & 0xF0) >> 4;
                Height = (PicSize & 0x0F);
                Console.WriteLine("Height: " + Height + ". Width: " + Width);

                int BytesLeft = (int)Math.Ceiling((Width * Height)/(decimal)8);

                Bitmap output = new Bitmap(Width, Height);

                int location = 0;

                for (int i = 0; i<BytesLeft; i++)
                {
                    byte current = contents[i+1];
                    for (byte k = 8; k>0; k--)
                    {
                        int Y = location / Width;
                        int X = location % Width;
                        
                        int bit = BitInt(current, (byte)(k-1));
                        try
                        {
                            output.SetPixel(X, Y, Color.FromArgb(bit * 255, bit * 255, bit * 255));
                            location++;
                        } catch
                        {
                            break;
                        }
                    }
                }

                output.Save("output.png");

                System.Diagnostics.ProcessStartInfo procInfo = new System.Diagnostics.ProcessStartInfo();
                procInfo.FileName = ("mspaint.exe");
                procInfo.Arguments = Path.GetFullPath("output.png");
                Process.Start(procInfo);
            }
        }

        public static int BitInt(byte b, byte i)
        {
            return GetBit(b, i) ? 1 : 0;
        }

        private static bool GetBit(byte b, byte i)
        {
            return (b & (1 << i)) != 0;
        }
    }
}
