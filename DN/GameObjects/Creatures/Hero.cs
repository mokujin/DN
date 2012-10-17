using Blueberry.Graphics;
using DN.Effects;
using DN.GameObjects.Items.Weapons;
using DN.Helpers;
using GamepadExtension;
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
using System.IO;
using System.Threading;

namespace DN.GameObjects.Creatures
{
    public class Hero:Creature
    {

        public LettersInventory Inventory { get; private set; }

        private float _dt = 0;
        
        public readonly DustPointEmitter DustEffect;

        private Texture _texture = CM.I.tex("hero_tile");

        public Hero(GameWorld gameWorld):base(gameWorld)
        {
            Game.g_Gamepad.OnButtonPress += g_Gamepad_OnButtonPress;
            Game.g_Gamepad.OnButtonUp += g_Gamepad_OnButtonUp;
            Game.g_Keyboard.KeyDown += g_Keyboard_KeyDown;
            Game.g_Keyboard.KeyUp += g_Keyboard_KeyUp;
            Game.g_Keyboard.KeyRepeat = true;
            Game.g_Gamepad.OnLeftStick += g_Gamepad_OnLeftStick;
            _font = CM.I.Font("consolas24");
            _font.Options.Monospacing = Blueberry.Graphics.Fonts.FontMonospacing.Yes;
            Inventory = new LettersInventory();

            Size = new Size(36, 40);
            MaxVelocity = new Vector2(5,15);
            MaxLadderVelocity = new Vector2(3, 3.5f);
            LadderFriction = 40f;
            Friction = 5f;
            InvulnerabilityDuration = 1;

            SetItem(new Bow(gameWorld)
                        {
                            IntervalDuration = 1.5f,
                            Damage = 3,
                            ProjectiveSpeed = 1,
                            Cell = this.Cell
                        });

            DustEffect = new DustPointEmitter(Position, Vector2.UnitX, 2f);
            DustEffect.Initialise(60, 1);


            Health = 36;

            HDirection = HDirection.Right;

            CollisionWithTiles += OnCollisionWithTiles;
        }

        void g_Gamepad_OnLeftStick(object sender, GamepadState.ThumbstickState e, Vector2 delta)
        {
            //Move(e.Position, 10 * dt);
        }

        private void OnCollisionWithTiles(CollidableGameObject sender, CollidedCell collidedCell)
        {

        }


        private void g_Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if(e.Key == Key.X)
            {
                StopJump();
            }
            else if (e.Key == Key.Z)
            {
                if(InHandItem != null)
                    InHandItem.FinishAction();
            }
        }
        void g_Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                Jump();
            }

            if (e.Key == Key.Z)
            {
                if (Game.g_Keyboard[Key.Down] && !OnLadder)
                    PickUpItem();
                else
                {
                    if (InHandItem != null)
                        InHandItem.DoAction();
                    StopPickingUpItem();
                }
            }
        }


        private void g_Gamepad_OnButtonPress(object sender, GamepadExtension.GamepadButtons e)
        {
            if (e.HasFlag(GamepadButtons.A))
            {
                Jump();
            }

                if (e.HasFlag(GamepadButtons.X))
                {
                    if (Game.g_Gamepad.DPad.Down && !OnLadder)
                        PickUpItem();
                    else
                    {
                        if (InHandItem != null)
                            InHandItem.DoAction();
                        StopPickingUpItem();
                    }
                }
        }
        private void g_Gamepad_OnButtonUp(object sender, GamepadButtons e)
        {
            if (e.HasFlag(GamepadButtons.A))
            {
                StopJump();
            }
            else if (e.HasFlag(GamepadButtons.X))
            {
                if (InHandItem != null)
                    InHandItem.FinishAction();
            }
        }

        #region Procedural animations
        Blueberry.Graphics.Fonts.BitmapFont _font;
        float leg_distance = 10;
        bool phase = false;             // true - left leg rotating, right leg suffering;
        float period = 0.2f;
        float timer = 0;
        
        #endregion
        public override void Draw(float dt)
        {
            SpriteBatch.Instance.PrintSymbol(_font, 'H', this.Position, Color4.White, 0, 1, 1, 1);
            SpriteBatch.Instance.PrintSymbol(_font, 'E', this.Position, Color4.White, 0, 1, 0, 1);
            if ((LeftKeyPressed() || RightKeyPressed()) && OnGround)
            {
                timer += dt;
                if (timer >= period)
                {
                    timer = 0;
                    phase = !phase;

                }
                int direction = HDirection == GameObjects.HDirection.Right ? -1 : 1;
                Vector2 posl, posr;
                if (phase)
                {
                    posl = new Vector2(direction * leg_distance, 0);
                    MathUtils.RotateVector2(ref posl, direction * timer * MathHelper.Pi / period);
                    posl += this.Position;

                    posr = new Vector2(-direction * leg_distance, 0);
                    posr.X += direction * timer * leg_distance * 2 / period;
                    posr += this.Position;
                }
                else
                {
                    posl = new Vector2(-direction * leg_distance, 0);
                    posl.X += direction * timer * leg_distance * 2 / period;
                    posl += this.Position;

                    posr = new Vector2(direction * leg_distance, 0);
                    MathUtils.RotateVector2(ref posr, direction * timer * MathHelper.Pi / period);
                    posr += this.Position;

                }
                SpriteBatch.Instance.PrintSymbol(_font, 'R', posl, Color4.White, 0, 1, 1, 0);
                SpriteBatch.Instance.PrintSymbol(_font, 'O', posr, Color4.White, 0, 1, 0, 0);
            }
            else
            {
                SpriteBatch.Instance.PrintSymbol(_font, 'R', this.Position, Color4.White, 0, 1, 1, 0);
                SpriteBatch.Instance.PrintSymbol(_font, 'O', this.Position, Color4.White, 0, 1, 0, 0);
            }

            /*
            SpriteBatch.Instance.DrawTexture(_texture,
                                             Position,
                                             Rectangle.Empty,
                                             Invulnerable ? new Color4(255, 1, 1, RandomTool.RandByte(255)) : Color4.White);*/
            //SpriteBatch.Instance.OutlineRectangle(Bounds, Color.White); // debug draw
            
            DustEffect.Draw(dt);
        }


        public override void Update(float dt)
        {
            base.Update(dt);
            _dt = dt;

            UpdateControlls(dt);
            DustEffect.Position = new Vector2(HDirection == HDirection.Left ?Bounds.Right:Bounds.Left, Bounds.Bottom);
            DustEffect.Update(dt);
        }

        private void UpdateControlls(float dt)
        {
            if (LeftKeyPressed())
            {
                Move(new Vector2(-1, 0), 10*dt);
                HDirection = HDirection.Left;
                if (OnGround)
                {
                    DustEffect.Direction = Vector2.UnitX;
                    float q = 1/(Velocity.Length*5);
                    DustEffect.TriggerInterval = q;
                    DustEffect.Trigger(dt);
                }
            }
            if (RightKeyPressed())
            {
                Move(new Vector2(1, 0), 10*dt);
                HDirection = HDirection.Right;
                if (OnGround)
                {
                    DustEffect.Direction = -Vector2.UnitX;
                    float q = 1/(Velocity.Length*5f);
                    DustEffect.TriggerInterval = q;
                    DustEffect.Trigger(dt);
                }
            }
            if (UpKeyPressed())
            {
                VDirection = VDirection.Up;
            }
            else if (DownKeyPressed())
            {
                VDirection = VDirection.Down;
            }
            else
            {
                VDirection = VDirection.No;
            }



            if (OnLadder && !OnGround)
            {
                if (Game.g_Gamepad.LeftStick.Position == Vector2.Zero)
                {
                    if (UpKeyPressed())
                    {
                        if (Velocity.Y >= 0 || ClimbLadder)
                        {
                            Move(new Vector2(0, -1), 100*dt);
                            ClimbLadder = true;
                        }
                    }
                    else if (DownKeyPressed())
                    {
                        if (ClimbLadder)
                        {
                            Move(new Vector2(0, 1), 100*dt);
                        }
                    }

                    if (LeftKeyPressed() && ClimbLadder)
                    {
                        Move(new Vector2(-1, 0), 100*dt);
                    }

                    else if (RightKeyPressed() && ClimbLadder)
                    {
                        Move(new Vector2(1, 0), 100*dt);
                    }
                }
                else
                {
                    if (Game.g_Gamepad.LeftStick.Position.Y < 0 && (Velocity.Y >= 0 || ClimbLadder))
                    {
                        ClimbLadder = true;
                    }
                    if (ClimbLadder)
                        Move(Game.g_Gamepad.LeftStick.Position, 100*dt);
                }
            }

            if (Game.g_Gamepad.LeftStick.Position != Vector2.Zero)
            {
                VDirection = (VDirection)(Game.g_Gamepad.LeftStick.Position.Y > 0 ? 1 : -1);
                HDirection = (HDirection)(Game.g_Gamepad.LeftStick.Position.X > 0 ? 1 : -1);
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
            return Game.g_Gamepad.DPad.Right || Game.g_Keyboard[Key.Right] || Game.g_Gamepad.LeftStick.Position.X > 0;
        }

        private static bool LeftKeyPressed()
        {
            return Game.g_Gamepad.DPad.Left || Game.g_Keyboard[Key.Left] || Game.g_Gamepad.LeftStick.Position.X < 0;
        }
    }
}
