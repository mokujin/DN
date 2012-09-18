using System.Collections.Generic;
using System;
using System.Drawing;
using Blueberry;

namespace DN.LevelGeneration
{
    public partial class LevelGenerator
    {
        internal TileMap TileMap;
        internal ResourseMap ResourseMap;
        private List<Miner> _miners;
       


        public LevelGenerator()
        {

            _miners = new List<Miner>();
        }

        public void Generate(GameWorld gameWorld)
        {

            TileMap = gameWorld.TileMap;

            ResourseMap = new ResourseMap(TileMap.Width, TileMap.Height);
            TileMap.FillWith(CellType.Wall);



            _miners.Add(new Miner(this, TileMap.Width / 2, TileMap.Height -1));
            //_miners.Add(new Miner(this, TileMap.Width / 2, (TileMap.Height/2) - 1));
            UpdateMiners();

            Point p = GetFreeCell();
            Adventurer adv = new Adventurer(this, p.X, p.Y);
            _miners.Add(adv);

            UpdateMiners();

            PolishMap();

            ClearJunk();

            TileMap.PrintDebug();
            Console.ReadKey();
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
                            TileMap[i, j] = CellType.Free;
                        }
                    }
                }
            }
        }
        private int LadderLength(int x, int y)
        {
            int length = 0;

            while (TileMap[x, ++y] == CellType.Ladder){}
            while (TileMap[x, --y] == CellType.Ladder)
            {
                length++;
            }

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

        private void PolishMap()
        {
            for (int i = 1; i < TileMap.Width - 1; i++)
            {
                for (int j = 1; j < TileMap.Height - 1; j++)
                {
                    if(TileMap[i,j] == CellType.Ladder)
                    {
                        if(TileMap[i, j + 1] == CellType.Wall ||
                           TileMap[i, j - 1] == CellType.Wall)
                        {
                            //TileMap[i, j] = CellType.Free;
                        }
                    }
                }
            }
        }
    }
}
