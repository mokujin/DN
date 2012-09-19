using Blueberry.Graphics;
using DN.GameObjects.Weapons;
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

namespace DN.GameObjects.Creatures
{
    public class Hero:Creature
    {
        private Weapons.Weapon _weapon;

        private float _dt = 0;

        public Hero(GameWorld gameWorld):base(gameWorld)
        {
            Game.g_Gamepad.OnButtonPress += g_Gamepad_OnButtonPress;
            Game.g_Keyboard.KeyRepeat = true;
            Size = new Size(48, 48);
            StandOnStairs += Hero_StandOnStairs;
            _weapon = new Sword(gameWorld, this)
                          {
                              AttackSpeed = 15,
                              TimeToFinishAttack = 0.5f
                          };
        }

        void Hero_StandOnStairs()
        {
            Speed.Y = 0;
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
        }
        void Move(float dx, float dy)
        {
            if (!CollideWith(new Vector2(dx, dy), CellType.Wall))
            {
                X += dx;
                Y += dy;
            }
            else MoveToContact(new Vector2( dx, dy));
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            _dt = dt;

            if(_weapon != null)
                _weapon.Update(dt);
            
            UpdateControlls(dt);
        }

        private void UpdateControlls(float dt)
        {
            if (LeftKeyPressed())
            {
                Move(-250*dt, 0);
                Direction = MovementDirection.Left;
            }
            if (RightKeyPressed())
            {
                Move(250*dt, 0);
                Direction = MovementDirection.Right;
            }

            if (_weapon != null)
                if (AttackKeyPressed())
                    _weapon.StartAttack();

            if (OnStairs)
            {
                if (UpKeyPressed())
                    Move(0, -250 * dt);
                if (DownKeyPressed())
                    Move(0, 250 * dt);
            }
            if (JumpKeyPressed())
                Jump();
        }

        private static bool JumpKeyPressed()
        {
            return Game.g_Keyboard[Key.Up];
        }

        private static bool DownKeyPressed()
        {
            return Game.g_Gamepad.DPad.Down || Game.g_Keyboard[Key.Down];
        }

        private static bool UpKeyPressed()
        {
            return Game.g_Gamepad.DPad.Up || Game.g_Keyboard[Key.Up];
        }

        private static bool AttackKeyPressed()
        {
            return Game.g_Keyboard[Key.Z];
        }

        private static bool RightKeyPressed()
        {
            return Game.g_Gamepad.DPad.Right || Game.g_Keyboard[Key.Right];
        }

        private static bool LeftKeyPressed()
        {
            return Game.g_Gamepad.DPad.Left || Game.g_Keyboard[Key.Left];
        }

        private void Jump()
        {
            if (OnGround || OnStairs)
            {
                Speed.Y = -420;
            }
        }
    }
}
