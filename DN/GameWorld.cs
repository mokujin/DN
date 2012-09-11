using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DN
{
    public class GameWorld
    {
        private byte[,] _map;
        private int _width, _height;

        public GameWorld(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new byte[_width, _height];
        }
    }
}
