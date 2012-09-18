using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using DN;
using DN.Creatures;
using OpenTK;

namespace PeoplePolder.Creatures.PathFinding
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
        const bool DiagonalMovesAllowed = false;



        byte[,] pointMap;
        BinaryHeap OpenList;

        public AStar(TileMap tileMap, Creature creature = null)
        {
            _tileMap = tileMap;
            Creature = creature;
        }

        public void FindWayAsync(Point startPosition, Point endPosition)
        {
            HasEnded = false;
            var caller = new AsyncMethodCaller(FindPath);
            caller.BeginInvoke(startPosition, endPosition, null, null);
        }
        public void FindPath(Point startPosition, Point endPosition)
        {
            path = new Path();

            List<Vector2> wayList = FindCellWay(startPosition, endPosition);
            if (wayList != null)
            {
                foreach (Vector2 p in wayList)
                    path.AddPoint(new Vector2(p.X * 64,p.Y * 64));
            }
            HasEnded = true;
        }

        public List<Vector2> FindCellWay(Point startCell, Point endCell)
        {
            pointMap = new byte[_tileMap.Width, _tileMap.Height];
            OpenList = new BinaryHeap();

            WayPoint startPoint = new WayPoint(startCell, null, true);
            startPoint.CalculateCost(Creature, endCell, ImprovedPathFinding);

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

        private bool CheckPassability(int x, int y)
        {
            return _tileMap[x, y] == CellType.Free;
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
            if (!_tileMap.InRange(pos))
                return;
            if (pointMap[pos.X, pos.Y] != 1)
            {
                WayPoint temp = new WayPoint(pos, node, type);
                temp.CalculateCost(Creature, endCell, ImprovedPathFinding);
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