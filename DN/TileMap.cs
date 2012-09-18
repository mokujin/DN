﻿using Blueberry;
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
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                    _map[i, j] = cellType;
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
                    }
                    /*
                    if (_map[i, j] != CellType.Free)
                        SpriteBatch.Instance.OutlineRectangle(i * ts.Width, j * ts.Height, ts.Width, ts.Height, Color.Gray,1,0,0,0); // debug fraw
                      */  
                }
            }
        }


        public void FillRandom()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _map[i, j] = (CellType)RandomTool.RandByte(2);
                }
            }
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
