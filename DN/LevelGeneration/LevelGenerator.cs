﻿using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
using Blueberry;
using Blueberry.Graphics;

namespace DN.LevelGeneration
{
    public enum Stage:sbyte
    {
        Nothing = -1,
        Miners = 0,
        Nature = 1,
        Adventurer = 2,
        WayChecker = 3,
        Final = 4
    }

    public delegate void FinishGenerationEventHandler();

    public class LevelGenerator
    {


        public Stage Stage { get;private set; }
        public event FinishGenerationEventHandler GenerationFinishedEvent;
        public bool Finished
        {
            get { return Stage == Stage.Final; }
        }

        public bool Skip = false;



        public int RoomCount;
        public int RoomsMaxWidth;
        public int RoomsMaxHeight;

        public float Scale = 0.5f;
        /// <summary>
        /// percantage of chance to smooth walls, 0 - 100
        /// </summary>
        public float WallSmoothing = 50;
        internal TileMap TileMap;
        internal ResourseMap ResourseMap;
        private List<Miner> _miners;

        internal CellType[,] Map;
        internal int Width;
        internal int Height;


        private Texture _wallTexture = CM.I.tex("mini_wall");
        private Texture _ladderTexture = CM.I.tex("mini_ladder");
        private Texture _minerTexture = CM.I.tex("mini_miner");

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

                Width = (int)Math.Round(TileMap.Width * Scale);
                Height = (int)Math.Round(TileMap.Height * Scale);

                Map = new CellType[Width, Height];
                Stage = Stage.Nothing;
                _miners.Clear();

                ResourseMap = new ResourseMap(Width, Height);
                TileMap.FillWith(Map, Width, Height, CellType.Wall);
            }
           catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                TileMap.PrintDebug();
                goto restart;
            }
        }

        public void Update(float dt)
        {
            if(Finished)
                return;
            do
            {
                UpdateMiners();
                if (_miners.Count == 0)
                {
                    Stage += 1;
                    SetCurrentStage();
                    if (Stage == Stage.Final)
                    {
                        GenerationFinishedEvent();
                        break;
                    }
                }
            } while (Skip || Stage == Stage.WayChecker);
        }
        private void SetCurrentStage()
        {
            Point p = Point.Empty;
            switch (Stage)
            {
                case Stage.Miners:
                    var m = new Miner(this, Width/4, Height - 2);
                    m.Init();
                    _miners.Add(m);
                    m = new Miner(this, Width/2, Height - 2);
                    _miners.Add(m);
                    m.Init();
                    break;
                case Stage.Nature:

                    for (int i = 0; i < RoomCount; i++)
                        AddRoomAtRandomPosition();

                    MakeConnection(new Point(Width/4 - 1, Height - 2),
                                   new Point(Width/2 + 1, Height - 2));
                    CopyScaledMap();
                    MakeCorosion();
                    break;
                case Stage.Adventurer:
                    p = GetFreeGroundCell();
                    var adv = new Adventurer(this, p.X, p.Y);
                    _miners.Add(adv);
                    break;
                case Stage.WayChecker:
                    p = GetFreeGroundCell();
                    Stage = Stage.WayChecker;
                    _miners.Add(new WayChecker(this, p.X, p.Y));
                    break;
            }
        }


        public void Draw(Rectangle area, float dt)
        {
            area.X /= 16;
            area.Y /= 16;
            area.Width /= 16;
            area.Height /= 16;

            area.Width += 2;
            area.Height += 2;

            Texture texture = null;

            if((sbyte)Stage < (sbyte)Stage.Nature) 
                CopyScaledMap();

            for (int i = Math.Max(0, area.X); i <Math.Min(area.Right, TileMap.Width); i++)
            {
                for (int j = Math.Max(0, area.Y); j <Math.Min(area.Bottom, TileMap.Height); j++)
                {
                    texture = null;
                    switch (TileMap[i, j])
                    {
                        case CellType.Wall:
                            texture = _wallTexture;
                            break;
                        case CellType.Ladder:
                            texture = _ladderTexture;
                            break;
                    }
                    if(texture != null)
                    SpriteBatch.Instance.DrawTexture(texture,
                                                     new RectangleF(i * texture.Size.Width,
                                                                    j * texture.Size.Height,
                                                                    texture.Size.Width,
                                                                    texture.Size.Height),
                                                     Color.White);
                }
            }

            foreach (var miner in _miners)
            {
                float w = 1;
                float h = 1;

                if(Stage == Stage.Miners)
                {
                    w = (float)TileMap.Width / (float)Width;
                    h = (float)TileMap.Height / (float)Height;
                }

                SpriteBatch.Instance.DrawTexture(_minerTexture,
                                                 new RectangleF(miner.Cell.X * _minerTexture.Size.Width * w,
                                                                miner.Cell.Y * _minerTexture.Size.Height * h,
                                                                _minerTexture.Size.Width,
                                                                _minerTexture.Size.Height),
                                                 Color.White);
            }
        }

        public bool InRange(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        private void MakeCorosion()
        {
            for (int i = 2; i < TileMap.Width - 2; i++)
            {
                for (int j = 2; j < TileMap.Height - 2; j++)
                {

                    if (TileMap[i, j] == CellType.Wall)
                    {
                        int wallCount = GetCellCountAround(i, j, CellType.Wall);
                        if (wallCount == 4)
                        {
                            if (RandomTool.RandBool(WallSmoothing))
                            {
                                TileMap[i, j] = CellType.Free;
                            }
                        }
                    }
                }
            }
        }

        private void MakeConnection(Point p1, Point p2)
        {
            for (int i = p1.X; i < p2.X; i++)
            {
                Map[i, p1.Y] = CellType.Free;
            }
        }

        private void CopyScaledMap()
        {
            double C2 = (double) TileMap.Width/(double) Width;
            double C1 = (double) TileMap.Height/(double) Height;

            int w = (int) (Map.GetLength(0)*C1);
            int h = (int) (Map.GetLength(1)*C2);


            for (int row = 0; row < Width; row++)
                for (int element = 0; element < Height; element++)
                    for (int y = (int) (row*C2); y < (int) ((row + 1)*C2); y++)
                        for (int x = (int) (element*C1); x < (int) ((element + 1)*C1); x++)
                        {
                            TileMap[x, y] = Map[element, row];
                        }
        }

        private void UpdateMiners()
        {
            foreach (var miner in _miners)
                miner.Step();

            for (int i = 0; i < _miners.Count; i++)
                if (_miners[i].Cell.Y <= 0)
                {
                    _miners[i].Remove();
                    _miners.Remove(_miners[i]);
                    i--;
                }
        }

        private void RemoveAloneCells()
        {
            for (int i = 2; i < TileMap.Width - 2; i++)
            {
                for (int j = 2; j < TileMap.Height - 2; j++)
                {
                    if (GetCellCountAround(i, j, CellType.Wall) == 1)
                        TileMap[i, j] = 0;
                }
            }
        }

        private int GetCellCountAround(int x , int y, CellType type = CellType.Free)
        {
            int count = 0;

            for (int i = x - 1; i <= x + 1; i++)
                for (int j = y - 1; j <= y + 1; j++)
                    if (TileMap[i, j] == type)
                        count++;
            return count;
        }

        private void AddRoomAtRandomPosition()
        {
            int x, y, width, height;

            do
            {
                width = RandomTool.RandInt(0, RoomsMaxWidth);
                height = RandomTool.RandInt(0, RoomsMaxHeight);
                x = RandomTool.RandInt(2, Width - width - 2);
                y = RandomTool.RandInt(2, Height - height - 2);

                //PrintDebug();
              //  Console.ReadKey();
            } while (Map[x, y] == CellType.Wall);
            AddRoom(x, y, width, height);
        }

        private void AddRoom(int x, int y, int width, int height)
        {

            for (int i = x; i <= x + width; i++)
                for (int j = y; j <= y + height; j++)
                    Map[i, j] = CellType.Free;
        }

        private int LadderLength(int x, int y)
        {
            int length = 0;

            while (TileMap[x, ++y] == CellType.Ladder);
            while (TileMap[x, --y] == CellType.Ladder)
                length++;

            return length;
        }


        private Point GetFreeGroundCell()
        {
            while (true)
            {
                Point p = new Point(RandomTool.RandInt(1, TileMap.Width - 1),
                                    RandomTool.RandInt(1, TileMap.Height - 1));

                if (TileMap[p.X, p.Y] == CellType.Free && TileMap[p.X, p.Y + 1] == CellType.Wall)
                    return p;
            }
        }

        public void PrintDebug()
        {
            for (int j = 0; j < Height; j++)
            {
                Console.WriteLine();
                for (int i = 0; i < Width; i++)
                {
                    Console.Write((byte)Map[i, j]);
                }
            }
        }
    }
}
