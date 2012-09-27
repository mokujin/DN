using System.Collections.Generic;
using System;
using System.Drawing;
using Blueberry;

namespace DN.LevelGeneration
{
    public class LevelGenerator
    {
        public int RoomCount;
        public int RoomsMaxWidth;
        public int RoomsMaxHeight;

        internal TileMap TileMap;
        internal ResourseMap ResourseMap;
        private List<Miner> _miners;


        public LevelGenerator()
        {
            _miners = new List<Miner>();
        }

        public void Generate(GameWorld gameWorld)
        {
            restart:
            try
            {
                TileMap = gameWorld.TileMap;
                _miners.Clear();

                ResourseMap = new ResourseMap(TileMap.Width, TileMap.Height);
                TileMap.FillWith(CellType.Wall);

                _miners.Add(new Miner(this, TileMap.Width / 2, TileMap.Height - 1));
                _miners.Add(new Miner(this, TileMap.Width / 2, (TileMap.Height / 2) - 1));
                UpdateMiners();

                MakeTunnelsWider();

                for (int i = 0; i < RoomCount; i++)
                    AddRoomAtRandomPosition();

                var p = GetFreeCell();
                var adv = new Adventurer(this, p.X, p.Y);
                _miners.Add(adv);

                UpdateMiners();

                ClearJunk();
            }
            catch (Exception)
            {
                Console.WriteLine("generation");
                goto restart;
            }
        }

        private void UpdateMiners()
        {
            while (_miners.Count > 0)
            {
                foreach (var miner in _miners)
                    miner.Step();

                for (int i = 0; i < _miners.Count; i++)
                    if (_miners[i].Cell.Y <= 0)
                    {
                        _miners.Remove(_miners[i]);
                        i--;
                    }
            }
        }

        private void MakeTunnelsWider()
        {
            byte[,] newMap = new byte[TileMap.Width, TileMap.Height];

            for (int i = 2; i < TileMap.Width - 2; i++)
            {
                for (int j = 2; j < TileMap.Height - 2; j++)
                {
                    if (FreeCellsAround(i, j) == 2)
                        newMap[i, j] = 1;
                }
            }

            for (int i = 2; i < TileMap.Width - 2; i++)
            {
                for (int j = 2; j < TileMap.Height - 2; j++)
                {
                    if (newMap[i, j] == 1)
                        AddRoom(i - 1, j - 1, 2, 2);
                }
            }
        }

        private int FreeCellsAround(int x , int y)
        {
            int count = 0;

            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                    if (TileMap[i, j] == CellType.Free)
                        count++;
            return count;
        }

        private void AddRoomAtRandomPosition()
        {
            int x, y, width, height;
            width = RandomTool.RandInt(0, RoomsMaxWidth);
            height = RandomTool.RandInt(0, RoomsMaxHeight);

            do
            {
                x = RandomTool.RandInt(2, TileMap.Width - width);
                y = RandomTool.RandInt(2, TileMap.Height - height);
            } while (TileMap[x, y] == CellType.Wall);
            AddRoom(x, y, width, height);
        }

        private void AddRoom(int x, int y, int width, int height)
        {

            for (int i = x; i < x + width; i++)
                for (int j = y; j < y + height; j++)
                    TileMap[i, j] = CellType.Free;
        }



        private void ClearJunk()
        {
            for (int i = 0; i < TileMap.Width; i++)
            {
                for (int j = 0; j < TileMap.Height; j++)
                {
                    if (TileMap[i, j] == CellType.Ladder)
                    {
                        int length = LadderLength(i, j);
                        if (length == 1)
                        {
                            if(TileMap[i,j - 1] != CellType.Wall)
                                TileMap[i, j] = CellType.Free;
                        }
                    }
                }
            }
        }
        private int LadderLength(int x, int y)
        {
            int length = 0;

            while (TileMap[x, ++y] == CellType.Ladder);
            while (TileMap[x, --y] == CellType.Ladder)
                length++;

            return length;
        }


        private Point GetFreeCell()
        {
            while (true)
            {
                Point p = new Point(RandomTool.RandInt(1, TileMap.Width - 1),
                                    RandomTool.RandInt(1, TileMap.Height - 1));

                if (TileMap[p.X, p.Y] == CellType.Free && TileMap[p.X, p.Y + 1] == CellType.Wall)
                    return p;
            }
        }
    }
}
