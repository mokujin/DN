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
using OpenTK;
using DN.GameObjects.Creatures.Enemies;
namespace DN
{
    public class GameWorld
    {


        public int Width{get; private set;}
        public int Height{get; private set;}
        public TileMap TileMap { get; private set; }

        public Hero Hero
        {
            get;
            private set;
        }

        public float g = 120f; // gravity acceleration;

        private List<GameObject> _gameObjects;
        private Queue<GameObject> _addNewObjectsQueue;
        private Queue<GameObject> _deleteObjectsQueue;

        private Camera camera;

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
            
            var lg = new LevelGenerator
                         {
                             RoomsMaxWidth = 3,
                             RoomsMaxHeight = 5,
                             RoomCount = 2
                         };
            lg.Generate(this);

            InsertHero();
            Creature bat = EnemiesFabric.CreateEnemy(this, EnemyType.Bat);
            bat.Cell = GetRandomPoint();
            AddObject(bat);
        }

        public void InsertHero()
        {
            Hero = new Hero(this);
            Point p = GetRandomPoint();
            Hero.Position = new OpenTK.Vector2((p.X * 64)+32, (p.Y * 64)+32);
            camera.MoveTo(Hero.Position);
        }
        public Point GetRandomPoint()
        {
            Point p;
            do
            {
                p = new Point(RandomTool.RandInt(0, Width), RandomTool.RandInt(0, Height));
            } while (!TileMap.IsFree(p));

            return p;
        }

        public void AddObject(GameObject gameObject)
        {
            _addNewObjectsQueue.Enqueue(gameObject);
        }
        public void RemoveObject(GameObject gameObject)
        {
            _deleteObjectsQueue.Enqueue(gameObject);
        }

        public float DistanceToObject(GameObject g1, GameObject g2)
        { 
            return (float)Math.Sqrt(Math.Pow(g1.X - g2.X,2) + Math.Pow(g1.Y - g2.Y,2));;
        }
        public Vector2 DirectionToObject(GameObject g1, GameObject g2)
        {
            float angle = (float)Math.Atan2(g1.X - g2.X, g1.Y - g2.Y);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));  
        }

        public void Update(float dt)
        {
            camera.MoveTo(Hero.Position);

            foreach (var gameObject in _gameObjects)
                gameObject.Update(dt);

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
            foreach (var gameObject in _gameObjects)
                gameObject.Draw(dt);

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
