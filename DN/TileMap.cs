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
        
        public void Draw(Rectangle region)
        {
            Size ts = CM.I.tex("wall_tile").Size;
            for (int i = region.Left; i < region.Right; i++)
            {
                for (int j = region.Top; j < region.Bottom; j++)
                {
                    if (_map[i, j] == CellType.Wall)
                    {
                        SpriteBatch.Instance.DrawTexture(CM.I.tex("wall_tile"), i * ts.Width, j * ts.Height, ts.Width, ts.Height, Rectangle.Empty, Color.White, 0, 0, 0);
                        SpriteBatch.Instance.OutlineRectangle(i * ts.Width, j * ts.Height, ts.Width, ts.Height, Color.Gray,1,0,0,0); // debug fraw
                    }
                }
            }
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
