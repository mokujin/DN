using Blueberry;
using Blueberry.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using DN.GameObjects.Creatures;
using DN.LevelGeneration;
using DN.GameObjects;
namespace DN
{
    public class GameWorld
    {


        public readonly int Width;
        public readonly int Height;

        public readonly TileMap TileMap;

        Camera camera;
        Hero hero = null;
        public float g = 120f; // gravity acceleration;

        private List<GameObject> _gameObjects;
        private Queue<GameObject> _addNewObjectsQueue;
        private Queue<GameObject> _deleteObjectsQueue;


        public GameWorld(int width, int height)
        {
            Width = width;
            Height = height;
            TileMap = new TileMap(Width, Height);

            _gameObjects = new List<GameObject>();
            _addNewObjectsQueue = new Queue<GameObject>();
            _deleteObjectsQueue = new Queue<GameObject>();

            camera = new Camera(Game.g_screenSize, new Point(Game.g_screenSize.Width / 2, Game.g_screenSize.Height / 2), true);
            camera.MoveSpeed = 7;
            
            LevelGenerator lg = new LevelGenerator();
            lg.RoomsMaxWidth = 3;
            lg.RoomsMaxHeight = 5;
            lg.RoomCount = 2;
            lg.Generate(this);
        }

        public void InsertHero()
        {
            hero = new Hero(this);
            Point p;
            while(true)
            {
                p = new Point(RandomTool.RandInt(0, Width), RandomTool.RandInt(0, Height));
                if(TileMap.IsFree(p))
                {
                    hero.Position = new OpenTK.Vector2((p.X * 64)+32, (p.Y * 64)+32);
                    break;
                }
            }
            camera.MoveTo(hero.Position);
        }

        public void AddObject(GameObject gameObject)
        {
            _addNewObjectsQueue.Enqueue(gameObject);
        }
        public void RemoveObject(GameObject gameObject)
        {
            _deleteObjectsQueue.Enqueue(gameObject);
        }

        public void Update(float dt)
        {
            float shift = dt * 200;
            if (Game.g_Keyboard[Key.Left])
                camera.Move(-shift, 0);
            if (Game.g_Keyboard[Key.Right])
                camera.Move(shift, 0);
            if (Game.g_Keyboard[Key.Up])
                camera.Move(0, -shift);
            if (Game.g_Keyboard[Key.Down])
                camera.Move(0, shift);
            camera.MoveTo(hero.Position);
            hero.Update(dt);
            camera.Update(dt);

            UpdateObjectsEnqueues();
        }

        private void UpdateObjectsEnqueues()
        {
            while (_addNewObjectsQueue.Count > 0)
                _gameObjects.Add(_addNewObjectsQueue.Dequeue());
            while (_deleteObjectsQueue.Count > 0)
                _gameObjects.Remove(_deleteObjectsQueue.Dequeue());
        }

        public void Draw(float dt)
        {
            SpriteBatch.Instance.Begin(camera.GetViewMatrix());
            RenderTiles(dt);
            hero.Draw(dt);
            SpriteBatch.Instance.End();
        }

        private void RenderTiles(float dt)
        {
            Rectangle rect = camera.BoundingRectangle;
            rect.X /= 64;
            rect.Y /= 64;
            rect.Width /= 64;
            rect.Height /= 64;
            rect.Width+=2;
            rect.Height+=2;

            TileMap.Draw(rect);
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
