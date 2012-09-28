using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using DN;
using DN.GameObjects.Creatures;
using OpenTK;

namespace DN.PathFinding
{
    public delegate void AsyncMethodCaller(Point startPosition, Point endPosition);

    public class AStar
    {
        private Path path;

        public Path Path
        {
            get
            {
                if (HasEnded)
                {
                    return path;
                }
                return null;
            }
        }

        public Creature Creature;

        private TileMap _tileMap;

        public byte[,] FogMap;

        public bool HasEnded = false;
        public bool ImprovedPathFinding = false;
        public bool IgnoreObstacles = false;
        public bool DiagonalMovesAllowed = false;



        byte[,] pointMap;
        BinaryHeap OpenList;

        public AStar() { }

        public AStar(TileMap tileMap, Creature creature = null)
        {
            _tileMap = tileMap;
            this.Creature = creature;
        }

        public void FoundWayAsync(Point startPosition, Point endPosition)
        {
            HasEnded = false;
            AsyncMethodCaller caller = new AsyncMethodCaller(FindPath);
            caller.BeginInvoke(startPosition, endPosition, null, null);
        }
        public void FindPath(Point startPosition, Point endPosition)
        {
            path = new Path();

            List<Vector2> wayList = FindCellWay(startPosition, endPosition);
            if (wayList != null)
            {
                foreach (Vector2 p in wayList)
                    path.AddPoint(new Vector2(p.X * 64,// + Settings.CellSize/2,
                                              p.Y * 64));// + Settings.CellSize/2));
            }
            HasEnded = true;
        }

        public List<Vector2> FindCellWay(Point startCell, Point endCell)
        {
            pointMap = new byte[_tileMap.Width, _tileMap.Height];
            OpenList = new BinaryHeap();

            WayPoint startPoint = new WayPoint(startCell, null, true);
            startPoint.CalculateCost(_tileMap, Creature, endCell,0, ImprovedPathFinding);

            OpenList.Add(startPoint);

            while (OpenList.Count != 0)
            {
                WayPoint node = OpenList.Get();
                if (node.PositionX == endCell.X && node.PositionY == endCell.Y)
                {
                    WayPoint nodeCurrent = node;
                    List<Vector2> points = new List<Vector2>();

                    while (nodeCurrent != null)
                    {
                        points.Insert(0, new Vector2(nodeCurrent.PositionX, nodeCurrent.PositionY));
                        nodeCurrent = nodeCurrent.Parent;
                    }
                    return points;
                }

                OpenList.Remove(); 
                Point temp = new Point(node.PositionX, node.PositionY);
                if (DiagonalMovesAllowed)
                {
                    if (CheckPassability(temp.X - 1, temp.Y) && CheckPassability(temp.X, temp.Y - 1))
                        AddNode(node, -1, -1, false, endCell);
                    if (CheckPassability(temp.X + 1, temp.Y) && CheckPassability(temp.X, temp.Y - 1))
                        AddNode(node, 1, -1, false, endCell);
                    if (CheckPassability(temp.X - 1, temp.Y) && CheckPassability(temp.X, temp.Y + 1))
                        AddNode(node, -1, 1, false, endCell);
                    if (CheckPassability(temp.X + 1, temp.Y) && CheckPassability(temp.X, temp.Y + 1))
                        AddNode(node, 1, 1, false, endCell);
                }

                AddNode(node, -1, 0, true, endCell);
                AddNode(node, 0, -1, true, endCell);
                AddNode(node, 1, 0, true, endCell);
                AddNode(node, 0, 1, true, endCell);
            }
            return null;
        }

        public List<Vector2> FindPlatformerCellWay(Point startCell, Point endCell)
        {
            pointMap = new byte[_tileMap.Width, _tileMap.Height];
            OpenList = new BinaryHeap();

            WayPoint startPoint = new WayPoint(startCell, null, true);
            startPoint.CalculateCost(_tileMap, Creature, endCell,0, ImprovedPathFinding);

            OpenList.Add(startPoint);

            while (OpenList.Count != 0)
            {
                WayPoint node = OpenList.Get();
                if (node.PositionX == endCell.X && node.PositionY == endCell.Y)
                {
                    WayPoint nodeCurrent = node;
                    List<Vector2> points = new List<Vector2>();

                    while (nodeCurrent != null)
                    {
                        points.Insert(0, new Vector2(nodeCurrent.PositionX, nodeCurrent.PositionY));
                        nodeCurrent = nodeCurrent.Parent;
                    }
                    if (points.Count <= 0)
                    {
                        Console.WriteLine("Fuck this shit");
                    }
                    return points;
                }

                OpenList.Remove();
                Point temp = new Point(node.PositionX, node.PositionY);
                if (DiagonalMovesAllowed)
                {
                    if (CheckPassability(temp.X - 1, temp.Y) || CheckPassability(temp.X, temp.Y - 1))
                        AddPlatformerNode(node, -1, -1, false, endCell);
                    if (CheckPassability(temp.X + 1, temp.Y) || CheckPassability(temp.X, temp.Y - 1))
                        AddPlatformerNode(node, 1, -1, false, endCell);
                    if (CheckPassability(temp.X - 1, temp.Y) || CheckPassability(temp.X, temp.Y + 1))
                        AddPlatformerNode(node, -1, 1, false, endCell);
                    if (CheckPassability(temp.X + 1, temp.Y) || CheckPassability(temp.X, temp.Y + 1))
                        AddPlatformerNode(node, 1, 1, false, endCell);
                }
                AddPlatformerNode(node, -1, 0, true, endCell);
                AddPlatformerNode(node, 0, -1, true, endCell);
                AddPlatformerNode(node, 1, 0, true, endCell);
                AddPlatformerNode(node, 0, 1, true, endCell);
            }
            return null;
        }

        private bool CheckPassability(int x, int y)
        {
            //if (!IgnoreObstacles)
            //{
            //    MapObject obstacle = gameField.things.QueryObject(new Point(x, y));
            //    if (obstacle != null)
            //        if (!obstacle.Passable)
            //            return false;
            //}
            //if(FogMap!= null)
            //    if (FogMap[x, y] == 1) return false;
            if (!_tileMap.InRange(new Point(x, y))) return false;

            return _tileMap[x, y] != CellType.Wall;
        }
        
        private bool CheckPlatformPassability(int x, int y)
        {
            if (!_tileMap.InRange(new Point(x, y)))
                return false;
            if (!_tileMap.InRange(x, y + 1))
                return false;
            if (_tileMap[x, y + 1] == CellType.Free)
                return false;
            return _tileMap[x, y] != CellType.Wall;
        }

        private bool CheckPassability(Point cell)
        {
            return CheckPassability(cell.X, cell.Y);
        }


        private void AddNode(WayPoint node, sbyte offSetX, sbyte offSetY, bool type, Point endCell)
        {
            Point pos = new Point(node.PositionX + offSetX, node.PositionY + offSetY);

            if (!CheckPassability(pos))
                return;
            if (pointMap[pos.X, pos.Y] != 1)
            {
               // byte addCost = _tileMap[pos.X, pos.Y + 1] == CellType.Free ? (byte)20 : (byte)0; 

                WayPoint temp = new WayPoint(pos, node,  type);
                temp.CalculateCost(_tileMap, Creature, endCell, 0, ImprovedPathFinding);
                OpenList.Add(temp);
                pointMap[pos.X, pos.Y] = 1;
            }
        }

        private void AddPlatformerNode(WayPoint node, sbyte offSetX, sbyte offSetY, bool type, Point endCell)
        {
            Point pos = new Point(node.PositionX + offSetX, node.PositionY + offSetY);
            
            if (!CheckPlatformPassability(pos.X, pos.Y))
                return;

            if (pointMap[pos.X, pos.Y] != 1)
            {
                byte addCost = _tileMap[pos.X, pos.Y + 1] == CellType.Free ? (byte)20 : (byte)0;

                WayPoint temp = new WayPoint(pos, node, type);
                temp.CalculateCost(_tileMap, Creature, endCell, addCost, ImprovedPathFinding);
                OpenList.Add(temp);
                pointMap[pos.X, pos.Y] = 1;
            }
        }

        private Point VectorToPoint(Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        private Vector2 PointToVector(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}