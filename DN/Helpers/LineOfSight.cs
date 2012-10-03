using System;
using System.Drawing;

namespace DN.Helpers
{ 
    // I guess it's quite stupid to have one class for one method. In future must be moved somewhere else
    static class LineOfSight
    {
        static internal bool Get(TileMap tileMap, Point startCell, Point endCell)
        {
            return Get(tileMap, startCell.X, startCell.Y, endCell.X, endCell.Y);
        }

        static internal bool Get(TileMap tileMap, float x1, float y1, float x2, float y2)
        {
            float deltaX = Math.Abs(x2 - x1);
            float deltaY = Math.Abs(y2 - y1);
            float signX = x1 < x2 ? 1 : -1;
            float signY = y1 < y2 ? 1 : -1;
            float error = deltaX - deltaY;

            while (true)
            {
                if (tileMap[(int)x1, (int)y1] == CellType.Wall) return false;

                if (x1 == x2 && y1 == y2)
                    return true;

                float error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                else if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                }
            }
        }
    }
}
