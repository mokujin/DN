using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry;

namespace DN.Creatures
{
    public class Creature
    {
        protected Vector2 _position; // relative to center

        public Vector2 Position { get { return _position; }  set { _position = value; } }
        protected GameWorld _world;
        protected Size _size = new Size(56, 56); // creature size
        public Rectangle Bounds 
        {
            get
            {
                Rectangle r = new Rectangle((int)(_position.X - _size.Width / 2), (int)(_position.Y - _size.Height / 2), _size.Width, _size.Height);
                return r;
            }
        }
        public int Left { get { return Bounds.Left; } set { _position.X = value + _size.Width / 2; } }
        public int Right { get { return Bounds.Right; } set { _position.X = value - _size.Width / 2; } }
        public int Top { get { return Bounds.Top; } set { _position.Y = value + _size.Height / 2; } }
        public int Bottom { get { return Bounds.Bottom; } set { _position.Y = value - _size.Height / 2; } }
        protected Vector2 _speed; // for gravity and jumps

        public Creature(GameWorld world)
        {
            _world = world;
        }

        public bool Collide(Vector2 shift)
        {
            Point tile = new Point((int)((Position.X + shift.X) / 64), (int)((Position.Y + shift.Y) / 64));
            Rectangle bsh = GetBoundsWithShift(shift);
            Rectangle range = new Rectangle(tile.X - 2, tile.Y - 2, tile.X + 2, tile.Y + 2);
            for (int i = Math.Max(0, range.Left); i < Math.Min(range.Right, _world.Width); i++)
            {
                for (int j = Math.Max(0, range.Top); j < Math.Min(range.Bottom, _world.Height); j++)
                {
                    if (!_world.TileMap.IsFree(new Point(i, j)))
                        if (_world.TileMap.GetRect(i, j).IntersectsWith(bsh))
                        {
                            return true;
                        }
                }
            }
            return false;
        }

        public void MoveToContact(Vector2 shift)
        {
            Point tile = new Point((int)((Position.X + shift.X) / 64), (int)((Position.Y + shift.Y) / 64)); // current tile
            Rectangle bshx = GetBoundsWithShift(shift.X, 0); // next rectangle with x-shift
            Rectangle bshy = GetBoundsWithShift(0, shift.Y); // nect rectangle with y-shift
            Rectangle range = new Rectangle(tile.X - 2, tile.Y - 2, 4, 4); // range of search
            bool fx = false, fy = false; // flags, to know if we already done shift along axis
            for (int i = Math.Max(0, range.Left); i < Math.Min(range.Right, _world.Width); i++)
            {
                for (int j = Math.Max(0, range.Top); j < Math.Min(range.Bottom, _world.Height); j++)
                {
                    Rectangle test = _world.TileMap.GetRect(i, j);
                    if (!_world.TileMap.IsFree(new Point(i, j)))
                    {
                        if (test.IntersectsWith(bshx))
                        {
                            if (shift.X > 0 && this.Right < test.Center().X)
                            {
                                this.Right = test.Left;
                                fx = true;
                            }
                            else if (shift.X < 0 && this.Left > test.Center().X)
                            {
                                this.Left = test.Right;
                                fx = true;
                            }
                        }

                        if (test.IntersectsWith(bshy))
                        {
                            if (shift.Y > 0 && this.Bottom < test.Center().Y)
                            {
                                this.Bottom = test.Top;
                                fy = true;
                            }
                            else if (shift.Y < 0 && this.Top > test.Center().Y)
                            {
                                this.Top = test.Bottom;
                                fy = true;
                            }
                        }
                        if (fx && fy) // if all shifts done, then nothing to do
                            return;
                    }
                }
            }
            if (!fx) _position.X += shift.X;
            if (!fy) _position.Y += shift.Y;
        }

        public virtual void Update(float dt)
        {
            if (dt >= 0.1f) return; // duct tape xD sometimes dt is extremly big, so that cause movement lag

            if(!OnGround())
                _speed.Y += _world.g * dt * 10;
            Vector2 shift = _speed * dt;
            if (!Collide(shift) && shift != Vector2.Zero)
            {
                Position += shift;
            }
            else if (_speed != Vector2.Zero)
            {
                _speed = Vector2.Zero;
                MoveToContact(shift);
            }
        }

        public bool OnGround()
        {
            Point cur = new Point((int)((_position.X) / 64), (int)((_position.Y) / 64)); // current cell
            Rectangle range = new Rectangle(cur.X - 2, cur.Y - 2, 4, 4); // range of cells to test on collision
            for (int i = Math.Max(0, range.Left); i < Math.Min(range.Right, _world.Width); i++)
            {
                for (int j = Math.Max(0, range.Top); j < Math.Min(range.Bottom, _world.Height); j++)
                {
                    Rectangle test = _world.TileMap.GetRect(i, j); // just get rectangle of each tile
                    if (!_world.TileMap.IsFree(new Point(i, j)))
                    {
                        if (Bounds.Bottom == test.Top && Bounds.Left < test.Right && Bounds.Left > test.Left - _size.Width)
                            return true;
                    }
                }
            }
            return false;
        }
        // get bounding rectangle with some shift
        public Rectangle GetBoundsWithShift(float sx, float sy)
        {
            Rectangle r = Bounds;
            r.X = (int)(Left + sx);
            r.Y = (int)(Top + sy);
            return r;
        }
        public Rectangle GetBoundsWithShift(Vector2 shift)
        {
            return GetBoundsWithShift(shift.X, shift.Y);
        }
        public virtual void Draw(float dt)
        {
            
        }
    }
}
