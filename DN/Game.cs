﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Blueberry;
using Blueberry.Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using OpenTK.Input;
using GamepadExtension;

namespace DN
{
    public class Game:GameWindow
    {
        #region GLOBAL

        public static Size g_screenSize = new Size(640, 480);
        public static KeyboardDevice g_Keyboard;
        public static MouseDevice g_Mouse;
        public static GamepadState g_Gamepad;
        #endregion
        private GameWorld gameWorld;

        public Game()
            : base(g_screenSize.Width, g_screenSize.Height, GraphicsMode.Default, "Devil's nightmare")
        {
            GraphicsDevice.Instance.Initialize(g_screenSize.Width, g_screenSize.Height); // i seeked this problem along whole hour x__x
        }

        protected override void OnLoad(EventArgs e)
        {
            g_Keyboard = Keyboard;
            g_Mouse = Mouse;
            g_Gamepad = new GamepadState(GamepadIndex.One);

            GL.ClearColor(Color4.Black);

            CM.I.LoadTexture("wall_tile", Path.Combine("Content", "Textures", "wall_tile.png"));
            CM.I.LoadTexture("hero_tile", Path.Combine("Content", "Textures", "hero_tile.png"));
            gameWorld = new GameWorld(30, 30);
            gameWorld.InsertHero();
            base.OnLoad(e);

        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            g_Gamepad.Update();
            gameWorld.Update((float)e.Time);
            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            gameWorld.Draw((float) e.Time);
            SpriteBatch.Instance.End();

            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}
