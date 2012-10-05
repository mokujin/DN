using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DN.PathFinding;

namespace DN.LevelGeneration
{
    class WayChecker:Adventurer
    {
        private readonly byte[,] _reachedPoints;
        private readonly Point _startCell;

        public WayChecker(LevelGenerator levelGenerator, int x, int y) : base(levelGenerator, x, y)
        {
            _reachedPoints = new byte[levelGenerator.TileMap.Width, levelGenerator.TileMap.Height];
            _startCell = new Point(x, y);
        }

        public override void Step()
        {
           // PrintDebug();
            base.Step();
        }

        protected override List<OpenTK.Vector2> GetPath()
        {
            return _astar.FindPlatformerCellWay(_cell, _nextPoint);
        }

        protected override void OnMove()
        {
            _reachedPoints[_cell.X, _cell.Y] = 1;
        }
        protected override void CellWasReached(Point cell)
        {
            base.CellWasReached(cell);
            _cell = _startCell;
        }
        public override void Remove()
        {
            for (int i = 0; i < _levelGenerator.TileMap.Width; i++)
            {
                for (int j = 0; j < _levelGenerator.TileMap.Height; j++)
                {
                    if(_reachedPoints[i,j] == 0)
                        if(_levelGenerator.TileMap[i,j] == CellType.Ladder)
                            _levelGenerator.TileMap[i, j] = CellType.Free;
                }
            }
        }


    }
}
