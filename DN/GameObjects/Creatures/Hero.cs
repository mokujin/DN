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
using DN.GameObjects.Weapons;
using DN.Effects;

namespace DN.GameObjects.Creatures
{
    public class Hero:Creature
    {
        private Weapon _currentWeapon;

        private float _dt = 0;

        DustPointEmitter dustEffect;

        public Hero(GameWorld gameWorld):base(gameWorld)
        {
            Game.g_Gamepad.OnButtonPress += g_Gamepad_OnButtonPress;
            Game.g_Keyboard.KeyRepeat = true;
            Size = new Size(48, 48);
            StandOnStairs += HeroStandOnStairs;

            _currentWeapon = new Sword(gameWorld, this)
                          {
                              AttackSpeed = 0.3f,
                              TimeToFinishAttack = 0.2f
                          };
            dustEffect = new DustPointEmitter(Position, Vector2.UnitX, 0.5f);
            dustEffect.Initialise(60, 1);
        }

        void HeroStandOnStairs()
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
            dustEffect.Draw(dt);
        }
        

        public override void Update(float dt)
        {
            base.Update(dt);

            _dt = dt;

            if(_currentWeapon != null)
                _currentWeapon.Update(dt);
            
            UpdateControlls(dt);

            dustEffect.Position = new Vector2(Position.X, Bounds.Bottom);
            dustEffect.Update(dt);
            
        }

        private void UpdateControlls(float dt)
        {
            if (LeftKeyPressed())
            {
                Move(-250*dt, 0);
                Direction = MovementDirection.Left;
                dustEffect.Direction = MathUtils.RotateVector2(Vector2.UnitX, 0.5f);
                dustEffect.Trigger(dt);
            }
            if (RightKeyPressed())
            {
                Move(250*dt, 0);
                Direction = MovementDirection.Right;
                dustEffect.Direction = MathUtils.RotateVector2(Vector2.UnitX, 0.5f);
                dustEffect.Direction = new Vector2(-dustEffect.Direction.X, dustEffect.Direction.Y);
                dustEffect.Trigger(dt);
            }

            if (_currentWeapon != null)
                if (AttackKeyPressed())
                    _currentWeapon.StartAttack();

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
