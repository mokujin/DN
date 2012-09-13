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
        }

        private void g_Gamepad_OnButtonPress(object sender, GamepadExtension.GamepadButtons e)
        {
            if(e.HasFlag(GamepadExtension.GamepadButtons.A))
                if (OnGround())
                {
                    _speed.Y = -420;
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
            if (!Collide(new Vector2(dx, dy)))
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
                Move(-300 * dt, 0);
            if (Game.g_Gamepad.DPad.Right)
                Move(300 * dt, 0);
            
        }
    }
}
