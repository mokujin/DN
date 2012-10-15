using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Blueberry.Graphics;
using DN.GameObjects.Creatures.Enemies;
using DN.LevelGeneration;
using OpenTK.Input;

namespace DN.States
{
    public class LevelGenerationState:GameState
    {
        private LevelGenerator levelGenerator;
        private GameWorld _gameWorld;

        public LevelGenerationState(StateManager stateManager) : base(stateManager)
        {
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
        }

        internal override void Draw(float dt)
        {
            SpriteBatch.Instance.Begin();
            levelGenerator.Draw(dt);
            SpriteBatch.Instance.End();
        }


        private void OnFinishGeneration()
        {
            StateManager.SetState(new PlayState(StateManager, _gameWorld));
        }

        private void GKeyboardOnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            levelGenerator.Skip = true;
        }

    }
}
