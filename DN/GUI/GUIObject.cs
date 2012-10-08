using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;

namespace DN.GUI
{
    public abstract class GUIObject
    {
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

        private Size _size;
        public Size Size
        {
            get { return new Size(_size.Width, _size.Height); }
            set { _size.Width = value.Width; _size.Height = value.Height; }
        }

        public virtual RectangleF Bounds
        {
            get
            {
                return new RectangleF(X, Y, Size.Width, Size.Height);
            }
        }

        public abstract void Update(float dt);
        public abstract void Draw(float dt);
    }
}
