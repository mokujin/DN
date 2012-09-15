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
    public class Hero:Creature
    {
        public Hero(GameWorld world):base(world)
        {
            Game.g_Gamepad.OnButtonPress+=g_Gamepad_OnButtonPress;
            _size = new Size(48, 48);
            StandOnStairs += Hero_StandOnStairs;
        }

        void Hero_StandOnStairs()
        {
            _speed.Y = 0;
        }

        private void g_Gamepad_OnButtonPress(object sender, GamepadExtension.GamepadButtons e)
        {
            if (e.HasFlag(GamepadExtension.GamepadButtons.A))
            {
                if (_onGround || _onStairs)
                {
                    _speed.Y = -820;
                }
            }
        }
        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex("hero_tile"), Position, Rectangle.Empty, Color4.White);
            SpriteBatch.Instance.OutlineRectangle(Bounds, Color.White); // debug draw

            base.Draw(dt);
        }
        void Move(float dx, float dy)
        {
            if (!CollideWith(new Vector2(dx, dy), CellType.Wall))
            {
                _position.X += dx;
                _position.Y += dy;
            }
            else MoveToContact(new Vector2( dx, dy));
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (Game.g_Gamepad.DPad.Left)
                Move(-250 * dt, 0);
            if (Game.g_Gamepad.DPad.Right)
                Move(250 * dt, 0);
            if (_onStairs)
            {
                if (Game.g_Gamepad.DPad.Up)
                    Move(0, -250 * dt);
                if (Game.g_Gamepad.DPad.Down)
                    Move(0, 250 * dt);
            }
        }
    }
}
