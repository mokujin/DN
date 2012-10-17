using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DN.GameObjects.Creatures.Enemies;
using DN.LevelGeneration;
using DN.States;
using OpenTK;
using Blueberry;
using Blueberry.Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using OpenTK.Input;
using GamepadExtension;
using OpenTK.Audio;
using OggStream;
using Blueberry.Audio;
using System.Threading;
using Blueberry.Diagnostics;
using Blueberry.Graphics.Fonts;
using DN.GameObjects.Creatures.Enemies;


namespace DN
{
    public class Game:GameWindow
    {
        #region GLOBAL
        
        public static Size g_screenSize;
        public static Rectangle g_screenRect;

        public static KeyboardDevice g_Keyboard;
        public static MouseDevice g_Mouse;
        public static GamepadState g_Gamepad;
        #endregion

        private StateManager _stateManager;
        AudioContext audioContext;

        public Game(Size screenSize, bool fullscreen)
            : base(screenSize.Width, screenSize.Height, GraphicsMode.Default, "Devil's nightmare", fullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.Default)
        {
            g_screenSize = screenSize;
            g_screenRect = new Rectangle(0, 0, screenSize.Width, screenSize.Height);

            GraphicsDevice.Instance.Initialize(g_screenSize.Width, g_screenSize.Height);
            VSync = VSyncMode.On;
            audioContext = new AudioContext();
            new AudioManager(16, 8, 4096, true);
        }



        protected override void OnLoad(EventArgs e)
        {

            GL.ClearColor(Color4.Black);
            LoadContent();

            g_Keyboard = Keyboard;
            g_Mouse = Mouse;
            g_Gamepad = new GamepadState(GamepadIndex.One);

            _stateManager = new StateManager();
            _stateManager.SetState(new LevelGenerationState(_stateManager));



            Keyboard.KeyRepeat = false;


        }


        private void LoadContent()
        {
            CM.I.LoadFont("Big", Path.Combine("Content", "Fonts", "monofur.ttf"), 48);
            CM.I.LoadFont("Middle", Path.Combine("Content", "Fonts", "monofur.ttf"), 24);
            CM.I.LoadFont("Small", Path.Combine("Content", "Fonts", "monofur.ttf"), 14);
            CM.I.LoadFont("speculum16", Path.Combine("Content", "Fonts", "speculum.ttf"), 16);
            CM.I.LoadFont("consolas24", Path.Combine("Content", "Fonts", "consola.ttf"), 24);

            CM.I.LoadSound("swordA", Path.Combine("Content", "Sounds", "steelsword.ogg"));
            CM.I.LoadSound("swordB", Path.Combine("Content", "Sounds", "wv-sword.ogg"));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            float dt = (float)e.Time;
            if (g_Keyboard[Key.Escape])
                Exit();
            _stateManager.Update(dt);

            base.OnUpdateFrame(e);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            float dt = (float)e.Time;
            g_Gamepad.Update(dt);

            _stateManager.Draw(dt);

            SwapBuffers();
            base.OnRenderFrame(e);

        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            audioContext.Dispose();

            base.OnClosing(e);
        }
    }
}
