using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DN
{
    public enum CellType
    {
        Free,
        Wall
    }

    public class TileMap
    {
        private CellType[,] _map;
        private readonly int _width;
        private readonly int _height;

        public TileMap(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new CellType[_width, _height];
        }

        public bool IsFree(Point p)
        {
            return _map[p.X, p.Y] != CellType.Wall;
        }

    }
}
