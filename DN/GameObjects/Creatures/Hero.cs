using Blueberry.Graphics;
using DN.Effects;
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

namespace DN.GameObjects.Creatures
{
    public class Hero:Creature
    {
        private Weapon _currentWeapon;
        private float _dt = 0;

        DustPointEmitter _dustEffect;


        public Hero(GameWorld gameWorld):base(gameWorld)
        {
            Game.g_Gamepad.OnButtonPress += g_Gamepad_OnButtonPress;
            Game.g_Keyboard.KeyDown += g_Keyboard_KeyDown;
            Game.g_Keyboard.KeyRepeat = true;
            Size = new Size(48, 40);
            MaxVelocity = new Vector2(5,15);
            MaxLadderVelocity = new Vector2(5, 5);
            LadderFriction = 40f;
            Friction = 5f;
            this.InvulnerabilityDuration = 1;



            _currentWeapon = new Sword(gameWorld, this)
                          {
                              AttackSpeed = 0.5f,
                              TimeToFinishAttack = 0.3f,
                              Damage = 1
                          };

            _dustEffect = new DustPointEmitter(Position, Vector2.UnitX, 0.5f);
            _dustEffect.Initialise(60, 1);

            Health = 10;
            Direction = Direction.Right;
        }

        void g_Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                Jump();
            }
            if (_currentWeapon != null)
                if (e.Key == Key.Z)
                    _currentWeapon.StartAttack();
        }

        private void g_Gamepad_OnButtonPress(object sender, GamepadExtension.GamepadButtons e)
        {
            if (e.HasFlag(GamepadExtension.GamepadButtons.A))
            {
                Jump();
            }

            if (_currentWeapon != null)
                if (e.HasFlag(GamepadExtension.GamepadButtons.X))
                {
                    _currentWeapon.StartAttack();
                }
        }



        public override void Draw(float dt)
        {
            SpriteBatch.Instance.DrawTexture(CM.I.tex("hero_tile"),
                                             Position,
                                             Rectangle.Empty,
                                             Invulnerable ? new Color4(255, 1, 1, RandomTool.RandByte(255)) : Color4.White);
            //SpriteBatch.Instance.OutlineRectangle(Bounds, Color.White); // debug draw

            _dustEffect.Draw(dt);
            
        }


        public override void Update(float dt)
        {
            base.Update(dt);

            _dt = dt;

            if (_currentWeapon != null)
                _currentWeapon.Update(dt);

            UpdateControlls(dt);
            _dustEffect.Position = new Vector2(Position.X, Bounds.Bottom);
            _dustEffect.Update(dt);

        }

        private void UpdateControlls(float dt)
        {
            if (LeftKeyPressed())
            {
                Move(new Vector2(-1, 0), 10 * dt);
                Direction = Direction.Left;
                if (OnGround)
                {
                    _dustEffect.Direction = MathUtils.RotateVector2(Vector2.UnitX, 0.5f);
                    _dustEffect.Trigger(dt);
                }
            }
            if (RightKeyPressed())
            {
                Move(new Vector2(1, 0), 10 * dt);
                Direction = Direction.Right;
                if (OnGround)
                {
                    _dustEffect.Direction = MathUtils.RotateVector2(Vector2.UnitX, 0.5f);
                    _dustEffect.Direction = new Vector2(-_dustEffect.Direction.X, _dustEffect.Direction.Y);
                    _dustEffect.Trigger(dt);
                }
            }



            if (OnLadder)
            {
                if (UpKeyPressed())
                {
                    if (Velocity.Y >= 2 || ClimbLadder)
                    {
                        Move(new Vector2(0, -1), 45*dt);
                        ClimbLadder = true;
                    }
                }
                if (DownKeyPressed())
                {
                    if (ClimbLadder)
                    {
                        Move(new Vector2(0, 1), 45*dt);
                    }
                }

                if (LeftKeyPressed() && ClimbLadder)
                {
                    Move(new Vector2(-1, 0), 35 * dt);
                }

                if (RightKeyPressed() && ClimbLadder)
                {
                    Move(new Vector2(1, 0), 35 * dt);
                }
            }
        }

        private static bool DownKeyPressed()
        {
            return Game.g_Gamepad.DPad.Down || Game.g_Keyboard[Key.Down];
        }

        private static bool UpKeyPressed()
        {
            return Game.g_Gamepad.DPad.Up || Game.g_Keyboard[Key.Up];
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
            if (OnGround || (OnLadder && ClimbLadder))
            {
                SetMoveY(-7, false);
                ClimbLadder = false;
            }
        }

    }
}
