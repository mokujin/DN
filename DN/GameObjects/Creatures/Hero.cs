﻿using Blueberry.Graphics;
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

        public Hero(GameWorld gameWorld):base(gameWorld)
        {
            Game.g_Gamepad.OnButtonPress += g_Gamepad_OnButtonPress;
            Game.g_Gamepad.OnButtonUp += g_Gamepad_OnButtonUp;
            Game.g_Keyboard.KeyDown += g_Keyboard_KeyDown;
            Game.g_Keyboard.KeyUp += g_Keyboard_KeyUp;
            Game.g_Keyboard.KeyRepeat = true;


            Inventory = new LettersInventory();

            Size = new Size(48, 40);
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
        }

        #region Procedural animations
        //Blueberry.Graphics.Fonts.BitmapFont _font = CM.I.Font("consolas32");

        #endregion
        public override void Draw(float dt)
        {
            //SpriteBatch.Instance.dr
            SpriteBatch.Instance.DrawTexture(CM.I.tex("hero_tile"),
                                             Position,
                                             Rectangle.Empty,
                                             Invulnerable ? new Color4(255, 1, 1, RandomTool.RandByte(255)) : Color4.White);
            //SpriteBatch.Instance.OutlineRectangle(Bounds, Color.White); // debug draw

            DustEffect.Draw(dt);
        }


        public override void Update(float dt)
        {
            base.Update(dt);

            _dt = dt;


            UpdateControlls(dt);
            DustEffect.Position = new Vector2(HDirection == GameObjects.HDirection.Left ?Bounds.Right:Bounds.Left, Bounds.Bottom);
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
                    float q = 1/ (Velocity.Length * 5f);
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
                if (UpKeyPressed())
                {
                    if (Velocity.Y >= 0 || ClimbLadder)
                    {
                        Move(new Vector2(0, -1),  100*dt);
                        ClimbLadder = true;
                    }
                }
                if (DownKeyPressed())
                {
                    if (ClimbLadder)
                    {
                       Move(new Vector2(0, 1), 100 * dt);
                    }
                }

                if (LeftKeyPressed() && ClimbLadder)
                {
                    Move(new Vector2(-1, 0), 100 * dt);
                }

                if (RightKeyPressed() && ClimbLadder)
                {
                    Move(new Vector2(1, 0), 100 * dt);
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
    }
}
