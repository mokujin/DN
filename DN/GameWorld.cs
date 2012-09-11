using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DN
{
    public class GameWorld
    {
        public readonly int Width;
        public readonly int Height;

        public readonly TileMap TileMap;

        public GameWorld(int width, int height)
        {
            Width = width;
            Height = height;
            TileMap = new TileMap(Width, Height);
        }

        public void Update(float dt)
        {

        }

        public void Draw(float dt)
        {
            RenderTiles(dt);
        }

        private void RenderTiles(float dt)
        {

        }


        public bool InRange(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool InRange(Point cell)
        {
            return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
        }
    }
}
