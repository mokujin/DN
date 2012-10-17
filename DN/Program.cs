using System;
using System.Drawing;

namespace DN
{
    class Program
    {
        public readonly static Size fullHD = new Size(1920, 1080);
        public readonly static Size HD = new Size(1280, 720);
        public readonly static Size qHD = new Size( 960, 540);
        public readonly static Size nHD = new Size(640, 360);

        static void Main(string[] args)
        {
            using (Game game = new Game(qHD, false))
            {
                game.Run(120.0, 120.0);
            }
        }
    }
}
