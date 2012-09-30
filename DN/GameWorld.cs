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
using DN.Effects;
using OpenTK.Graphics.OpenGL;
using DN.GameObjects.Weapons;

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

        public float G = 15f; // gravity acceleration;
        public Vector2 GravityDirection = new Vector2(0, 1);
        private List<GameObject> _gameObjects;
        private Queue<GameObject> _addNewObjectsQueue;
        private Queue<GameObject> _deleteObjectsQueue;

        private Camera camera;
        public Camera Camera { get { return camera; } }
        ParallaxBackground background;

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
                             RoomsMaxWidth = 10,
                             RoomsMaxHeight = 15,
                             RoomCount = 2,
                             Scale = 0.5f
                         };
            lg.Generate(this);

            InsertHero();

            background = new ParallaxBackground(this);
            for (int i = 0; i < 10; i++)
            {
             
            Creature bat = EnemiesFabric.CreateEnemy(this, EnemyType.Bat);
            bat.Cell = GetRandomPoint();   
            }
        }

        public void InsertHero()
        {
            Hero = new Hero(this);
            Point p = GetRandomPoint();
            Hero.Position = new Vector2((p.X * 64)+32, (p.Y * 64)+32);
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
            float angle = (float)Math.Atan2(g1.Y - g2.Y, g1.X - g2.X);
            return new Vector2(-(float)Math.Cos(angle), -(float)Math.Sin(angle));  
        }

        public void Update(float dt)
        {
            camera.MoveTo(Hero.Position);

            foreach (var gameObject in _gameObjects)
                gameObject.Update(dt);

            if (Game.g_Keyboard[Key.Plus])
                camera.ScaleOn(0.01f);
            if (Game.g_Keyboard[Key.Minus])
                camera.ScaleOn(-0.01f);

            camera.Update(dt);
            UpdateObjectsEnqueues();
           background.Update(dt);
        }

        private void UpdateObjectsEnqueues()
        {
            while (_addNewObjectsQueue.Count > 0)
                _gameObjects.Add(_addNewObjectsQueue.Dequeue());

            while (_deleteObjectsQueue.Count > 0)
                _gameObjects.Remove(_deleteObjectsQueue.Dequeue());
        }

        Texture back = new Texture(Game.g_screenSize);
        public void Draw(float dt)
        {
            
            
            SpriteBatch.Instance.Begin(camera.GetViewMatrix());

            RenderTiles(dt);
            GL.ClearColor(0, 0, 0, 0);

            SpriteBatch.Instance.End(back, true, true);
            
            GL.ClearColor(0, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            background.Draw(dt);

            SpriteBatch.Instance.Begin();
            
            SpriteBatch.Instance.DrawTexture(back, Game.g_screenRect,Rectangle.Empty, Color.White,0,Vector2.Zero,false,true);
            
            SpriteBatch.Instance.End();

            SpriteBatch.Instance.Begin();
            SpriteBatch.Instance.FillCircle(new PointF(200, 200), 20, Color.Red, 10);
            SpriteBatch.Instance.End();

            SpriteBatch.Instance.Begin(camera.GetViewMatrix());
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
           // Console.SetCursorPosition(0,1);
            //Console.Write("tiles in view: {0}   ",rect.Width * rect.Height);
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

        internal List<GameObject> GetCollisionsWithObjects(GameObject gameObject)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gO in _gameObjects)
            {
                if (gO != gameObject)
                {
                    if (gO.Bounds.IntersectsWith(gameObject.Bounds))
                    {
                        list.Add(gO);
                    }
                    
                }
            }
            return list;
        }

        internal List<CollidedCell> GetCollisionsWithTiles(RectangleF rectangle)
        {
            return GetCollisionsWithTiles(new Rectangle((int)Math.Round(rectangle.X),
                                               (int)Math.Round(rectangle.Y),
                                               (int)Math.Round(rectangle.Width),
                                               (int)Math.Round(rectangle.Height)));
        }

        internal List<CollidedCell> GetCollisionsWithTiles(Rectangle rectangle)
        {

            var tilesToCheck = TileMap.GetRectanglesAround(new Point(rectangle.X / 64,
                                                                     rectangle.Y / 64));

            var list = new List<CollidedCell>();
            foreach (Rectangle rect in tilesToCheck)
            {
                if (rect.IntersectsWith(rectangle)) 
                    list.Add(new CollidedCell(rect, TileMap[rect.X / 64, rect.Y / 64]));
            }
            return list;
        }
    }

    public class CollidedCell
    {
        public Rectangle Rectangle;
        public CellType CellType;

        /// <summary>
        /// Determines in which directection object collided this tile
        /// </summary>
        public Point Direction;

        public CollidedCell(Rectangle rect, CellType cellType)
        {
            CellType = cellType;
            Rectangle = rect;
        }
    }
}
