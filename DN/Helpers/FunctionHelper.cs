using System;
using System.Drawing;
using OpenTK;

namespace DN.Helpers
{ 
    static public class FunctionHelper
    {
        static public float GetDirectionFromVelocity(Vector2 velocity)
        {
            velocity.Normalize();
          //  float t1 = (float)Math.Acos(velocity.X / Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y));
            float t1 = (float)Math.Atan2(velocity.Y, velocity.X);
            return t1;
        }

        static public bool GetLineOfSight(TileMap tileMap, Point startCell, Point endCell)
        {
            return GetLineOfSight(tileMap, startCell.X, startCell.Y, endCell.X, endCell.Y);
        }

        static public bool GetLineOfSight(TileMap tileMap, float x1, float y1, float x2, float y2)
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
        static public sbyte GetSign(float number)
        {
            if (number < 0)
                return -1;
            if (number > 0)
                return 1;
            else 
                return 0;
        }

    }
}
