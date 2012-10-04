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

        // public static Size g_screenSize = new Size(1080, 1920);
        //public static Rectangle g_screenRect = new Rectangle(0, 0, 1080, 1920);
        public static Size g_screenSize = new Size(640, 480);
        public static Rectangle g_screenRect = new Rectangle(0, 0, 640, 480);

        public static KeyboardDevice g_Keyboard;
        public static MouseDevice g_Mouse;
        public static GamepadState g_Gamepad;
        #endregion
        private GameWorld gameWorld;

        public Game()
            : base(g_screenSize.Width, g_screenSize.Height, GraphicsMode.Default, "Devil's nightmare")
        {
            GraphicsDevice.Instance.Initialize(g_screenSize.Width, g_screenSize.Height);
        }

        protected override void OnLoad(EventArgs e)
        {
            g_Keyboard = Keyboard;
            g_Mouse = Mouse;
            g_Gamepad = new GamepadState(GamepadIndex.One);
      
            GL.ClearColor(Color4.Black);

            LoadTextures();
            gameWorld = new GameWorld(50, 50);

            Keyboard.KeyRepeat = false;

            base.OnLoad(e);

        }

        private void LoadTextures()
        {
            CM.I.LoadTexture("wall_tile", Path.Combine("Content", "Textures", "wall_tile.png"));
            CM.I.LoadTexture("hero_tile", Path.Combine("Content", "Textures", "hero_tile.png"));
            CM.I.LoadTexture("stair_tile", Path.Combine("Content", "Textures", "stair_tile.png"));
            CM.I.LoadTexture("sword_sprite", Path.Combine("Content", "Textures", "Weapons", "Sword.png"));
            CM.I.LoadTexture("bat_sprite", Path.Combine("Content", "Textures", "Enemies", "Bat.png"));
            CM.I.LoadTexture("troll_sprite", Path.Combine("Content", "Textures", "Enemies", "Troll.png"));

            CM.I.LoadFont("Big", Path.Combine("Content", "Fonts", "monofur.ttf"), 48);
            CM.I.LoadFont("Middle", Path.Combine("Content", "Fonts", "monofur.ttf"), 24);
       }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (g_Keyboard[Key.Escape])
                Exit();
            g_Gamepad.Update();
            gameWorld.Update((float)e.Time);
            base.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
         //   SpriteBatch.Instance.Begin();
        //    SpriteBatch.Instance.OutlineRectangle(new RectangleF(5,5,10,40),  Color.White, 10f, 1f, new Vector2(0.5f, 0.5f));
         //   SpriteBatch.Instance.End();

            gameWorld.Draw((float) e.Time);

            SwapBuffers();
            base.OnRenderFrame(e);

        }
    }
}
