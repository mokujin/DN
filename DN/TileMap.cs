using Blueberry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DN
{
    public enum CellType:byte
    {
        Free = 0,
        Wall = 1
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
            return _map[p.X, p.Y] != CellType.Wall; // strange logic when we have CellType.Free
        }

        public void FillRandom()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _map[i, j] = (CellType)RandomTool.RandByte(2);
                }
            }
        }
    }
}
