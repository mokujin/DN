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
using OpenTK.Input;

namespace DN.Creatures
{
    public class Hero:Creature
    {
        private float _dt = 0;

        public Hero(GameWorld world):base(world)
        {
            Game.g_Gamepad.OnButtonPress+=g_Gamepad_OnButtonPress;
            Game.g_Keyboard.KeyRepeat = true;
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
                Jump();
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
            _dt = dt;
            if (Game.g_Gamepad.DPad.Left || Game.g_Keyboard[Key.Left])
                Move(-250 * dt, 0);
            if (Game.g_Gamepad.DPad.Right || Game.g_Keyboard[Key.Right])
                Move(250 * dt, 0);
            if (_onStairs)
            {
                if (Game.g_Gamepad.DPad.Up || Game.g_Keyboard[Key.Up])
                    Move(0, -250 * dt);
                if (Game.g_Gamepad.DPad.Down || Game.g_Keyboard[Key.Down])
                    Move(0, 250 * dt);
            }
            //else
            {
                if(Game.g_Keyboard[Key.Up])
                    Jump();
            }
        }

        private void Jump()
        {
            if (_onGround || _onStairs)
            {
                _speed.Y = -420;
            }
        }
    }
}
