using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
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
          //  try
            {
                TileMap = gameWorld.TileMap;
                _miners.Clear();

                ResourseMap = new ResourseMap(TileMap.Width, TileMap.Height);
                TileMap.FillWith(CellType.Wall);

                _miners.Add(new Miner(this, TileMap.Width / 4, TileMap.Height - 2));
                _miners.Add(new Miner(this, TileMap.Width / 2, TileMap.Height - 2));
                UpdateMiners();
                
               
                //TileMap.PrintDebug();
                //Console.WriteLine();
                //Console.ReadKey();

                for (int i = 0; i < RoomCount; i++)
                    AddRoomAtRandomPosition();


                MakeTunnelsWider();


                RemoveAloneCells();


              //  CheckAccessibility();

                MakeConnection(new Point(TileMap.Width / 4, TileMap.Height - 2),
                               new Point(TileMap.Width / 2, TileMap.Height  - 2));


                var p = GetFreeCell();
                var adv = new Adventurer(this, p.X, p.Y);
                _miners.Add(adv);

                UpdateMiners();

                ClearJunk();
            }
          //  catch (Exception)
            {
                Console.WriteLine("generation");
               // goto restart;
            }
        }

        private void MakeConnection(Point p1, Point p2)
        {
            for (int i = p1.X; i < p2.X; i++)
            {
                TileMap[i, p1.Y] = CellType.Free;
            }
        }


        //private delegate void Func<TArg0, TArg1>(TArg0 arg0, TArg1 arg1);
        //private void CheckAccessibility()
        //{
        //    List<byte[,]> zones = new List<byte[,]>();

        //    Point cell = new Point(1,1);
        //    while (NoFreePlacesLeft(cell))
        //    {
        //        cell = GetUndiscoveredFreeCell(zones);

        //        zones.Add(new byte[TileMap.Width, TileMap.Height]);
        //        var curZone = zones[zones.Count - 1];

        //        var OpenList = new List<Point>();
        //        OpenList.Add(cell);

        //        Func<Point, Point> addPoint = (p, o) =>
        //                                          {
        //                                              Point pos = new Point(p.X + o.X, p.Y + o.Y);
        //                                              if(TileMap.InRange(pos))
        //                                              if (TileMap[pos.X, pos.Y] != CellType.Wall)
        //                                                  if (!OpenList.Exists(a => a.X == pos.X && a.Y == pos.Y)
        //                                                      && IsNotInsideZone(pos.X, pos.Y, zones))
        //                                                  {
        //                                                      OpenList.Add(pos);
        //                                                      curZone[pos.X, pos.Y] = 1;
        //                                                  }

        //                                          };

        //        while (OpenList.Count > 0)
        //        {
        //            Point curr = OpenList[0];
        //            addPoint(curr, new Point(0, 1));
        //            addPoint(curr, new Point(1, 0));
        //            addPoint(curr, new Point(0, -1));
        //            addPoint(curr, new Point(-1, 0));
        //            OpenList.Remove(curr);
        //        }
        //    }

        //    if(zones.Count > 1)
        //    {
        //        var cells = zones.Select(GetFreeCell).ToList();

        //        Point p1 = cells[0];

        //        for (int i = 0; i < cells.Count; i++)
        //        {
        //            var p = cells[i];
        //            while (p1.X != p.X && p1.Y != p.Y)
        //            {
        //                TileMap[p.X, p.Y] = 0;

        //                var dir = new Point(Direction(p.X, p1.X),
        //                                    Direction(p.Y, p1.Y));
        //                if (dir.X != 0)
        //                    dir.Y = 0;

        //                p.X += dir.X;
        //                p.Y += dir.Y;
        //            }
        //        }
        //    }
        //}

        //private sbyte Direction(int x1, int x2)
        //{
        //    sbyte t = 0;

        //    if (x1 - x2 < 0) 
        //        t = 1;
        //    else if (x1 - x2 > 0) 
        //        t = -1;
        //    else 
        //        t = 0;

        //    return t;
        //}

        //private bool NoFreePlacesLeft(Point cell)
        //{
        //    return cell.X != -1 && cell.Y != -1;
        //}
        
        //private Point GetFreeCell(byte[,] zone)
        //{
        //    Point p = Point.Empty;

        //    do
        //    {
        //        p = new Point(RandomTool.RandInt(0, TileMap.Width),
        //                      RandomTool.RandInt(0, TileMap.Height));

        //    } while (zone[p.X, p.Y] != 0);

        //    return p;
        //}

        //private Point GetUndiscoveredFreeCell(List<byte[,]> zones)
        //{
        //    int x = 1, y = 1;

        //    while (x < TileMap.Width - 1 && y < TileMap.Height)
        //    {
        //        if(TileMap.IsFree(x,y) && IsNotInsideZone(x,y, zones))
        //        {
        //            return new Point(x, y);
        //        }
        //        x++;
        //        if (x > TileMap.Width)
        //        {
        //            x = 1;
        //            y++;
        //        }
        //    }
        //    return new Point(-1, -1);
        //}

        //private bool IsNotInsideZone(int x, int y, List<byte[,]> zones)
        //{
        //    foreach (var z in zones)
        //    {
        //        if (z[x, y] == 1)
        //            return false;
        //    }
        //    return true;
        //}

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
                    if (GetCellCountAround(i, j) == 2)
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
            width = RandomTool.RandInt(0, RoomsMaxWidth);
            height = RandomTool.RandInt(0, RoomsMaxHeight);

            do
            {
                x = RandomTool.RandInt(2, TileMap.Width - width - 2);
                y = RandomTool.RandInt(2, TileMap.Height - height - 2);
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
