using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Blueberry.Audio;
using Blueberry.Diagnostics;
using Blueberry.Graphics;
using Blueberry.Graphics.Fonts;
using DN.GameObjects.Creatures.Enemies;
using DN.LevelGeneration;
using OpenTK.Audio;
using OpenTK.Input;
using Blueberry;

namespace DN.States
{
    public class PlayState:GameState
    {
        private GameWorld _gameWorld;
       // AudioClip clip;
        BitmapFont font;

        public PlayState(StateManager stateManager, GameWorld gameWolrd) : base(stateManager)
        {
            _gameWorld = gameWolrd;
            font = new BitmapFont(new Font("Consolas", 14));
            
       //     AudioClip clip = new AudioClip(Path.Combine("Content", "Sounds", "rainfall.ogg"));
         //   clip.Play();
        }

        internal override void Init()
        {
            new DiagnosticsCenter();

            _gameWorld.InsertHero();
            _gameWorld.InitGui();
            for (int i = 0; i < 0; i++)
            {
                Enemy enemy = EnemiesFabric.CreateEnemy(_gameWorld,RandomTool.RandBool()? EnemyType.Troll : EnemyType.Bat);
                enemy.Cell = _gameWorld.GetRandomPoint();
            }
            _gameWorld.InitTextures();
        }

        internal override void LoadContent()
        {
            CM.I.LoadTexture("wall_tile", Path.Combine("Content", "Textures", "wall_tile.png"));
            CM.I.LoadTexture("hero_tile", Path.Combine("Content", "Textures", "hero_tile.png"));
            CM.I.LoadTexture("stair_tile", Path.Combine("Content", "Textures", "stair_tile.png"));
            CM.I.LoadTexture("sword_sprite", Path.Combine("Content", "Textures", "Weapons", "Sword.png"));
            CM.I.LoadTexture("bat_sprite", Path.Combine("Content", "Textures", "Enemies", "Bat.png"));
            CM.I.LoadTexture("troll_sprite", Path.Combine("Content", "Textures", "Enemies", "Troll.png"));
            CM.I.LoadTexture("heart", Path.Combine("Content", "Textures", "Gui", "heart.png"));
            CM.I.LoadTexture("potion", Path.Combine("Content", "Textures", "GameObjects", "Potion.png"));
            CM.I.LoadTexture("arrow_sprite", Path.Combine("Content", "Textures", "Weapons", "arrow.png"));
            CM.I.LoadTexture("bow_sprite", Path.Combine("Content", "Textures", "Weapons", "Bow.png"));
        }

        internal override void UnloadContent()
        {
        }

        internal override void Update(float dt)
        {

            if (Game.g_Keyboard[Key.Tilde])
                if (DiagnosticsCenter.Instance.Visible) DiagnosticsCenter.Instance.Hide();
                else DiagnosticsCenter.Instance.Show();
            _gameWorld.Update(dt);
            DiagnosticsCenter.Instance.Update(dt);
        }

        internal override void Draw(float dt)
        {
                _gameWorld.Draw(dt);
                DiagnosticsCenter.Instance.Draw(dt);
        }
    }
}
