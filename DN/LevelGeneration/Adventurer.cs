using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using DN.PathFinding;


namespace DN.LevelGeneration
{
    class Adventurer:Miner
    {
        private byte[,] _neededPoints;
        Point _nextPoint;

        AStar _astar;
        List<Vector2> _path;
        bool _pathFinished = true;

        public Adventurer(LevelGenerator levelGenerator, int x, int y) : base(levelGenerator, x, y)
        {
            _astar = new AStar(_levelGenerator.TileMap) {DiagonalMovesAllowed = true};

            _neededPoints = new byte[_levelGenerator.TileMap.Width,
                                     _levelGenerator.TileMap.Height];
            DetermineNeededPoints();
        }

        public override void Step()
        {
          //  PrintDebug();
            if (NothingLeftToSearch())
            {
                _cell.Y = -1;
                return;
            }
            if (_pathFinished)
            {
                _nextPoint = GetGlosestPoint();

                _path = _astar.FindPlatformerCellWay(_cell, _nextPoint) ?? _astar.FindCellWay(_cell, _nextPoint);

                _pathFinished = false;
            }
            else
            {
                Point direction = new Point((int)_path[0].X - _cell.X,
                                            (int)_path[0].Y - _cell.Y);


                if(direction.X != 0 || direction.Y != 0)
                {
                    _cell.X += direction.X;
                    _cell.Y += direction.Y;
                    if (_levelGenerator.TileMap[_cell.X, _cell.Y + 1] != CellType.Wall)
                        _levelGenerator.TileMap[_cell.X, _cell.Y] = CellType.Ladder;
                }

                if (_cell.X == _nextPoint.X && _cell.Y == _nextPoint.Y)
                {
                    CellWasReached(_cell);
                    _pathFinished = true;
                }
                _path.RemoveAt(0);
            }

        }

        private void CellWasReached(Point cell)
        {
            _neededPoints[cell.X, cell.Y] = 0;
        }

        private bool NothingLeftToSearch()
        {
            for (int i = 0; i < _levelGenerator.TileMap.Width; i++)
                for (int j = 0; j < _levelGenerator.TileMap.Height; j++)
                    if (_neededPoints[i, j] == 1)
                        return false;
            return true;
        }
        private Point GetGlosestPoint()
        {
            float minDist = float.MaxValue;
            float curDist = float.MaxValue;
            Point minPoint = new Point();


            for (int i = 1; i < _levelGenerator.TileMap.Width - 1; i++)
                for (int j = 1; j < _levelGenerator.TileMap.Height - 1; j++)
                {
                    if (_neededPoints[i, j] == 1)
                    {
                        curDist = (float)Math.Pow(_cell.X - i, 2) + (float)Math.Pow(_cell.Y - j, 2);
                        if (curDist < minDist)
                        {
                            minDist = curDist;
                            minPoint = new Point(i, j);
                        }
                    }
                }
            return minPoint;
        }

        private void DetermineNeededPoints()
        {
            for (int i = 1; i < _levelGenerator.TileMap.Width - 1; i++)
                for (int j = 1; j < _levelGenerator.TileMap.Height - 1; j++)
                    if (_levelGenerator.TileMap[i, j] == CellType.Free &&
                       _levelGenerator.TileMap[i, j + 1] == CellType.Wall)
                    {
                        _neededPoints[i, j] = 1;
                    }
        }
        public void PrintDebug()
        {
            Console.Clear();
            _levelGenerator.TileMap.PrintDebug();

            for (int i = 0; i < _levelGenerator.TileMap.Width; i++)
            {
                for (int j = 0; j < _levelGenerator.TileMap.Height; j++)
                {
                    if (_neededPoints[i, j] == 1)
                    {
                        Console.SetCursorPosition(i, j + 1);
                        Console.Write('N');
                    }
                }

            }
            if(_path != null)
            foreach (var vector2 in _path)
            {
                Console.SetCursorPosition((int)vector2.X, (int)vector2.Y + 1);
                Console.Write('H');
            }

            Console.SetCursorPosition(_cell.X , _cell.Y + 1);
            Console.Write('X');


            Console.ReadKey();

        }
    }

}
