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

namespace DN.GameObjects.Creatures
{
    public enum MovementDirection:sbyte
    {
        Left = -1,
        Right = 1
    }

    public abstract class Creature:GameObject
    {
        protected Vector2 Speed; // for gravity and jumps
        protected bool OnStairs;
        protected bool OnGround;
        public MovementDirection Direction
        {
            get;
            protected set;
        }


        public Creature(GameWorld gameWorld)
            :base(gameWorld)
        {
            Direction = MovementDirection.Right;
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
        public bool CollideWith(Vector2 shift, CellType type)
        {
            Point tile = new Point((int)((Position.X + shift.X) / 64), (int)((Position.Y + shift.Y) / 64));
            Rectangle bsh = GetBoundsWithShift(shift);
            Rectangle range = new Rectangle(tile.X - 2, tile.Y - 2, tile.X + 2, tile.Y + 2);
            for (int i = Math.Max(0, range.Left); i < Math.Min(range.Right, _world.Width); i++)
            {
                for (int j = Math.Max(0, range.Top); j < Math.Min(range.Bottom, _world.Height); j++)
                {
                    if (_world.TileMap[i, j] == type)
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
            if (!fx) X += shift.X;
            if (!fy) Y += shift.Y;
        }

        public bool CheckStairs()
        {
            Point cur = new Point((int)((X) / 64), (int)((Y) / 64)); // current cell
            return _world.TileMap[cur.X, cur.Y] == CellType.Ladder || _world.TileMap[cur.X, cur.Y] == CellType.VRope;
             
        }


        public event Action StandOnStairs;
        public event Action StandOutStairs;
        public event Action StandOnGround;
        public event Action StandOutGround;

        public override void Update(float dt)
        {
            if (dt >= 0.1f) return; // duct tape xD sometimes dt is extremly big, so that cause movement lag
                                    //lol

            if (CheckStairs())
            {
                if (!OnStairs && StandOnStairs != null)
                    StandOnStairs();
                OnStairs = true;
            }
            else
            {
                if (OnStairs && StandOutStairs != null)
                    StandOutStairs();
                OnStairs = false;
            }
            if (CheckGround())
            {
                if (!OnGround && StandOnGround != null)
                    StandOnGround();
                OnGround = true;
            }
            else
            {
                if (OnGround && StandOutGround != null)
                    StandOutGround();
                OnGround = false;
            }


            if (!OnGround && !OnStairs)
                Speed.Y += _world.g * dt * 10;
            if (OnStairs && Speed.Y < 0)
                Speed.Y = Math.Min(0, Speed.Y + _world.g * dt * 10);
            //else
            //    _speed.Y = 0;
            Vector2 shift = Speed * dt;
            if (!CollideWith(shift, CellType.Wall) && shift != Vector2.Zero)
            {
                Position += shift;
            }
            else if (Speed != Vector2.Zero)
            {
                Speed = Vector2.Zero;
                MoveToContact(shift);
            }

        }

        public bool CheckGround()
        {
            Point cur = new Point((int)((X) / 64), (int)((Y) / 64)); // current cell
            Rectangle range = new Rectangle(cur.X - 2, cur.Y - 2, 4, 4); // range of cells to test on collision
            for (int i = Math.Max(0, range.Left); i < Math.Min(range.Right, _world.Width); i++)
            {
                for (int j = Math.Max(0, range.Top); j < Math.Min(range.Bottom, _world.Height); j++)
                {
                    Rectangle test = _world.TileMap.GetRect(i, j); // just get rectangle of each tile
                    if (_world.TileMap[i, j] == CellType.Wall)
                    {
                        if (Bounds.Bottom == test.Top && Bounds.Left < test.Right && Bounds.Left > test.Left - Size.Width)
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
    }
}
