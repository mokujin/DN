using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry;

namespace DN.LevelGeneration
{
    public partial class LevelGenerator
    {
        private class Miner
        {
            private Point _cell;
            private Point _direction;
            private byte _steps = 0;

            private readonly byte[,] _map;
            private readonly int _mapWidth;
            private readonly int _mapHeight;

            public Miner(byte[,] map, int x, int y)
            {
                _cell.X = x;
                _cell.Y = y;

                _map = map;
                _mapWidth = _map.GetLength(0);
                _mapHeight = _map.GetLength(1);
            }

            public void Step()
            {
                if (_steps > 0)
                {
                    var nextPosition = new Point(_cell.X + _direction.X,
                                                 _cell.Y + _direction.Y);
                    if (InRange(nextPosition))
                    {

                    }
                }
                else
                {
                    _direction = GetRandomDirection();
                    _steps = RandomTool.RandByte(2, 5);
                }
            }

            /// <returns>returns non diagonal direction</returns>
            private Point GetRandomDirection()
            {
                var a = RandomTool.RandBool() ? RandomTool.RandSign() : 0;
                var b = a == 0 ? 0 : RandomTool.RandSign();
                return new Point(a, b);
            }

            private bool IsPossibleToMove(Point p)
            {
                if (_map[p.X, p.Y + 1] == 0) return false;

                return false;
            }

            bool InRange(Point p)
            {
                return p.X >= 0 && p.X < _mapWidth && p.Y >= 0 && p.Y < _mapHeight;
            }
        }
    }
}
