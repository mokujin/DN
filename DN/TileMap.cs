using Blueberry;
using Blueberry.Graphics;
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
        Ladder = 2,
        VRope = 3

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
            FillWith(_map, Width, Height, cellType);
        }

        static public void FillWith(CellType[,] map, int width, int height, CellType cellType)
        {
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    map[i, j] = cellType;
        }


        public Rectangle GetRect(int x, int y)
        {
            return new Rectangle(x * 64, y * 64, 64, 64);
        }
        public Rectangle GetRect(Point pos)
        {
            return new Rectangle(pos.Y * 64, pos.Y * 64, 64, 64);
        }
        public bool IsFree(Point p)
        {
            return _map[p.X, p.Y] == CellType.Free; 
        }
        public bool IsFree(int x, int y)
        {
            return _map[x, y] == CellType.Free;
        }
        
        public List<Rectangle> GetRectanglesAround(Point p)
        {
            List<Rectangle> rects = new List<Rectangle>();

            for (int i = p.X - 1; i <= p.X + 1; i++)
            {
                for (int j = p.Y - 1; j <= p.Y + 1; j++)
                {
                    rects.Add(GetRect(i, j));
                }
            }

            return rects;
        }

        public void Draw(Rectangle region)
        {
            Size ts = CM.I.tex("wall_tile").Size;
            for (int i = Math.Max(0, region.Left); i < Math.Min(region.Right, Width); i++)
            {
                for (int j = Math.Max(0, region.Top); j < Math.Min(region.Bottom, Height); j++)
                {
                    switch(_map[i,j])
                    {
                        case CellType.Wall:
                            SpriteBatch.Instance.DrawTexture(CM.I.tex("wall_tile"), i * ts.Width, j * ts.Height, ts.Width, ts.Height, Rectangle.Empty, Color.White, 0, 0, 0);
                            break;
                        case CellType.Ladder:
                            SpriteBatch.Instance.DrawTexture(CM.I.tex("stair_tile"), i * ts.Width, j * ts.Height, ts.Width, ts.Height, Rectangle.Empty, Color.White, 0, 0, 0);
                            break;
                        case CellType.VRope:
                            SpriteBatch.Instance.DrawTexture(CM.I.tex("stair_tile"), i * ts.Width, j * ts.Height, ts.Width, ts.Height, Rectangle.Empty, Color.Brown, 0, 0, 0);
                            break;
                    }
                }
            }
        }

        public bool InRange(Point cell)
        {
            return InRange(cell.X, cell.Y);
        }
        public bool InRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
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
