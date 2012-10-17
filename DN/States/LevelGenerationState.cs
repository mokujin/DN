using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blueberry;
using Blueberry.Graphics;
using DN.GameObjects.Creatures.Enemies;
using DN.LevelGeneration;
using OpenTK;
using OpenTK.Input;

namespace DN.States
{
    public class LevelGenerationState : GameState
    {
        private LevelGenerator levelGenerator;
        private GameWorld _gameWorld;
        private Camera _camera;
        private Vector2 _nextCameraPosition;

        public LevelGenerationState(StateManager stateManager) : base(stateManager)
        {
            _camera = new Camera(Game.g_screenSize, new Vector2(0, 0), true) {MoveSpeed = 6};
            _nextCameraPosition = new Vector2(0, 0);
        }

        internal override void LoadContent()
        {
            CM.I.LoadTexture("mini_wall", Path.Combine("Content", "Textures", "LevelGeneration", "mini_wall.png"));
            CM.I.LoadTexture("mini_ladder", Path.Combine("Content", "Textures", "LevelGeneration", "mini_ladder.png"));
            CM.I.LoadTexture("mini_miner", Path.Combine("Content", "Textures", "LevelGeneration", "mini_miner.png"));
        }

        internal override void UnloadContent()
        {
            Game.g_Keyboard.KeyDown -= GKeyboardOnKeyDown;
            CM.I.UnloadTexture("mini_wall");
            CM.I.UnloadTexture("mini_ladder");
            CM.I.UnloadTexture("mini_miner");
        }

        internal override void Init()
        {
            _gameWorld = new GameWorld(100, 100);
            levelGenerator = new LevelGenerator
                                 {
                                     RoomsMaxWidth = 10,
                                     RoomsMaxHeight = 15,
                                     RoomCount = 0,
                                     Scale = 0.5f,
                                     WallSmoothing = 0.5f
                                 };
            levelGenerator.GenerationFinishedEvent += OnFinishGeneration;
            levelGenerator.Generate(_gameWorld);

            Game.g_Keyboard.KeyDown += GKeyboardOnKeyDown;
        }

        internal override void Update(float dt)
        {
            if (!levelGenerator.Finished)
                levelGenerator.Update(dt);

            UpdateControlls(dt);

            _camera.MoveTo(_nextCameraPosition);
            _camera.Update(dt);
        }

        private void UpdateControlls(float dt)
        {
            Vector2 dir = Vector2.Zero;

            if (Game.g_Gamepad.LeftStick.Position != Vector2.Zero)
            {
                dir = Game.g_Gamepad.LeftStick.Position;
            }
            if (Game.g_Keyboard[Key.Left] || Game.g_Gamepad.DPad.Left)
            {
                dir.X = -1;
            }
            if (Game.g_Keyboard[Key.Right] || Game.g_Gamepad.DPad.Right)
            {
                dir.X = 1;
            }
            if (Game.g_Keyboard[Key.Down] || Game.g_Gamepad.DPad.Down)
            {
                dir.Y = 1;
            }
            if (Game.g_Keyboard[Key.Up] || Game.g_Gamepad.DPad.Up)
            {
                dir.Y = -1;
            }

            if (Game.g_Keyboard[Key.Plus])
            {
                _camera.ScaleTo(_camera.Scaling - dt);
            }
            if (Game.g_Keyboard[Key.Minus])
            {
                _camera.ScaleTo(_camera.Scaling + dt);
            }


            _nextCameraPosition += dir*15;
        }

        internal override void Draw(float dt)
        {
            SpriteBatch.Instance.Begin(_camera.GetViewMatrix());
            levelGenerator.Draw(_camera.BoundingRectangle, dt);
            SpriteBatch.Instance.End();
        }


        private void OnFinishGeneration()
        {
            StateManager.SetState(new PlayState(StateManager, _gameWorld));
        }

        private void GKeyboardOnKeyDown( object sender, KeyboardKeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                levelGenerator.Skip = true;
        }

    }
}
