using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry;

namespace DN.LevelGeneration
{
    internal class Miner
    {
        public Point Cell
        {
            get { return _cell; }
        }

        protected byte[,] _exploredMap;

        protected Point _cell;
        protected Point _direction;

        protected readonly LevelGenerator _levelGenerator;

        public Miner(LevelGenerator levelGenerator, int x, int y)
        {
            _cell.X = x;
            _cell.Y = y;
            _levelGenerator = levelGenerator; 

            _exploredMap = new byte[levelGenerator.TileMap.Width, levelGenerator.TileMap.Height];
            _exploredMap[_cell.X, _cell.Y] = 1;
        }

        public virtual void Step()
        {
            _direction = GetDirection();
            MoveInDirection();
            CheckBounds();

            _levelGenerator.ResourseMap.GatherResourses(this);
            _levelGenerator.TileMap[_cell.X, _cell.Y] = CellType.Free;
            if (false)
            if(_cell.X != 0 && _cell.Y != 0 && _cell.Y <1500 && _cell.X < 1500)
            {
                _levelGenerator.TileMap[_cell.X - 1, _cell.Y] = CellType.Free;
                _levelGenerator.TileMap[_cell.X + 1, _cell.Y] = CellType.Free;
                _levelGenerator.TileMap[_cell.X, _cell.Y - 1] = CellType.Free;
                _levelGenerator.TileMap[_cell.X, _cell.Y] = CellType.Free;
                _levelGenerator.TileMap[_cell.X - 1, _cell.Y + 1] = CellType.Free;
                _levelGenerator.TileMap[_cell.X - 1, _cell.Y - 1] = CellType.Free;
                _levelGenerator.TileMap[_cell.X + 1, _cell.Y + 1] = CellType.Free;
                _levelGenerator.TileMap[_cell.X + 1, _cell.Y - 1] = CellType.Free;
            }
        }

        private void MoveInDirection()
        {
            _cell.X += _direction.X;
            _cell.Y += _direction.Y;
            _exploredMap[_cell.X, _cell.Y] += 1;
            _exploredMap[_cell.X, _cell.Y] *= 4;
        }

        /// <returns>returns non diagonal direction</returns>
        private Point GetDirection()
        {
            var res = new int[4];
            var p = new Point[4];

            res[0] = GetCellPrice(0, -1);
            p[0] = new Point(0, -1);

            res[1] = GetCellPrice(0,1);
            p[1] = new Point(0, 1);

            res[2] = GetCellPrice(-1, 0);
            p[2] = new Point(-1, 0);

            res[3] = GetCellPrice(1, 0);
            p[3] = new Point(1, 0);

            int max = 0;
            if (res[0] == res[1] && res[0] == res[2] && res[0] == res[3])
                max = RandomTool.RandByte(0, 3);
            else
            for (int i = 0; i < 4; i++)
                if(res[i] != Int32.MinValue)
                    if (res[i] > res[max])
                        max = i;



            return p[max];
        }

        private int GetCellPrice(int offsetX, int offsetY)
        {
            var p = new Point(_cell.X + offsetX, _cell.Y + offsetY);

            if (p.Y <= 0)
                return Int32.MaxValue;
            if (!_levelGenerator.TileMap.InRange(p))
                return Int32.MinValue;
            if(_levelGenerator.TileMap.InRange(new Point(p.X, p.Y + 1)))
                if(offsetY == 0)
                    if (_levelGenerator.TileMap.IsFree(new Point(p.X, p.Y + 1)))
                        return Int32.MinValue;
            if (p.X <= 1 || p.X >= _levelGenerator.TileMap.Width - 1 || p.Y >= _levelGenerator.TileMap.Height - 1)
                return Int32.MinValue;

            return (byte)_levelGenerator.ResourseMap[p.X, p.Y] - _exploredMap[p.X, p.Y];
        }

        private void CheckBounds()
        {
            if (_cell.X < 0)
                _cell.X = 0;
            if (_cell.X > _levelGenerator.TileMap.Width)
                _cell.X = _levelGenerator.TileMap.Width;
            if (Cell.Y > _levelGenerator.TileMap.Height)
                _cell.Y = _levelGenerator.TileMap.Height;
        }
    }
}
