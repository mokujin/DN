using System;
using System.Collections.Generic;
using System.Drawing;
using Blueberry;

namespace DN.LevelGeneration
{
    internal enum Resourse:byte
    {
        Nothingness = 0,
        Dirt = 1,
        Iron = 2,
        Gold = 3,
    }
    internal class ResourseMap
    {
        private Resourse[,] _map;

        private int _width, _height;

        public Resourse this[int i, int j]
        {
            get 
            {
                return _map[i,j];
            }
        }

        public ResourseMap(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new Resourse[width, height];
            Generate();
            PrintDebug();
        }

        private void Generate()
        {
            byte[,] digitMap = new byte[_width, _height];

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    digitMap[i, j] = RandomTool.RandByte(1, 9);
                }
            }
  
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if(false)
                    {
                        byte k = GetAvarageTile(GetTilesAround(digitMap, i, j));

                        if (k == 0 || k >= 4)
                            _map[i, j] = Resourse.Dirt;
                        else if (k == 2)
                            _map[i, j] = Resourse.Gold;
                        else
                            _map[i, j] = Resourse.Iron;
                    }
                    else
                    {
                        _map[i, j] = (Resourse)digitMap[i, j];
                    }
                }
            }
        }


        public void PrintDebug()
        {
            for (int j = 0; j < _height; j++)
            {
                Console.WriteLine();
                for (int i = 0; i < _width; i++)
                {
                    Console.Write((byte)_map[i, j]);
                }
            }
        }

        private byte GetAvarageTile(byte[,] tiles)
        {
            byte av = 0;
            byte t = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tiles[i, j] != 255)
                    {
                        av += tiles[i, j];
                        t++;
                    }
                }
            }

            av /= t;

            return av;
        }

        private byte[,] GetTilesAround(byte[,] map, int x, int y)
        {
            byte[,] tiles = new byte[3,3];

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    PutInTile(tiles, map, x, y, i, j);

            return tiles;
        }

        private void PutInTile(byte[,] tiles, byte[,] map, int x, int y, int offsetX, int offsetY)
        {
            int X = x + offsetX;
            int Y = y + offsetY;
            int posX = 1 + offsetX;
            int posY = 1 + offsetY;

            if (X >= 0 && Y >= 0 && X < _width && Y < _height)
                tiles[posX, posY] = map[X, Y];
            else
                tiles[posX, posY] = 255;
        }

        public void GatherResourses(Miner miner)
        {
           // if (_map[miner.Cell.X, miner.Cell.Y] == Resourse.Gas)
           // {
           //    Explode(miner.Cell);
           // }
            _map[miner.Cell.X, miner.Cell.Y] = Resourse.Nothingness;
        }

        private void Explode(Point cell)
        { 
            //
        }

    }
}