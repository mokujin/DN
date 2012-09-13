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
        Wall = 1,
        Ladder = 2
    }

    public class TileMap
    {
        private CellType[,] _map;

        public CellType this[int i, int j]
        {
            get { return _map[i, j]; }
            set { _map[i, j] = value; }
        }

        public readonly int Width;
        public readonly int Height;

        public TileMap(int width, int height)
        {
            Width = width;
            Height = height;
            _map = new CellType[Width, Height];
        }
        
        public void FillWith(CellType cellType)
        {
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                    _map[i, j] = cellType;
        }

        public bool IsFree(Point p)
        {
            return _map[p.X, p.Y] != CellType.Wall;
        }

        public bool InRange(Point cell)
        {
            return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
        }

        public void PrintDebug()
        {
            for (int j = 0; j < Height; j++)
            {
                Console.WriteLine();
                for (int i = 0; i < Width; i++)
                {
                    Console.Write((byte)_map[i,j]);
                }
            }
        }
    }
}
