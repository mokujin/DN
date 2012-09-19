using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Drawing;
namespace DN.GameObjects
{
    public abstract class GameObject
    {
        protected GameWorld _world;
        

        private float _x, _y;
        
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Vector2 Position
        {
            get { return new Vector2(_x, _y); }
            set
            {
                _x = value.X;
                _y = value.Y;
            }
        }

        public Point Cell
        {
            get 
            {
                return new Point((int)(X / 64),(int)( Y / 64)); 
            }
            set 
            {
                _x = value.X * 64; _y = value.Y * 64; 
            }
        }

        private Size _size;
        public Size Size
        {
            get { return new Size(_size.Width, _size.Height); }
            set { _size.Width = value.Width; _size.Height = value.Height; }
        }

        public Rectangle Bounds
        {
            get
            {
                Rectangle r = new Rectangle((int)(X - Size.Width / 2), (int)(Y - Size.Height / 2), Size.Width, Size.Height);
                return r;
            }
        }
        public int Left { get { return Bounds.Left; } set { X = value + Size.Width / 2; } }
        public int Right { get { return Bounds.Right; } set { X = value - Size.Width / 2; } }
        public int Top { get { return Bounds.Top; } set { Y = value + Size.Height / 2; } }
        public int Bottom { get { return Bounds.Bottom; } set { Y = value - Size.Height / 2; } }

        public GameObject(GameWorld gameWorld)
        {
            this._world = gameWorld;
        }
        public abstract void Update(float dt);
        public abstract void Draw(float dt);
    }
}
