using Blueberry;
using Blueberry.Graphics;
using Blueberry.Graphics.Fonts;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public class ParallaxBackground
    {
        private GameWorld world;
        private BitmapFont font;

        FallingLetterEmitter[] emitters;
        const int emitter_count = 20;

        public ParallaxBackground(GameWorld world)
        {
            emitters = new FallingLetterEmitter[emitter_count];
            for (int i = 0; i < emitter_count; i++)
            {
                emitters[i] = new FallingLetterEmitter(new Vector2(RandomTool.RandFloat(-500, world.Width * 64), RandomTool.RandFloat(-500, world.Height * 64)), Vector2.UnitY, 0.6f);
                emitters[i].Initialise(15, 8);
            }
           

            this.world = world;
            font = new BitmapFont(Path.Combine("Content", "Fonts", "monofur.ttf"), 25);
        }
        public void Update(float dt)
        {
            Rectangle particle_bounds = Game.g_screenRect;
            particle_bounds.Inflate(20, 20);
            foreach (var item in emitters)
            {
                item.Update(dt);
                if (!particle_bounds.Contains(world.Camera.ToScreen(item.Position, 0.3f)))
                    item.Position = world.Camera.ToWorld(new Vector2(RandomTool.RandInt(0, Game.g_screenSize.Width), RandomTool.RandInt(-50, 350)), 0.3f);
            }
        }
        public void Draw(float dt)
        {
            /*
            SpriteBatch.Instance.Begin(world.Camera.GetViewMatrix(0.3f));
            Rectangle r = world.Camera.BoundingRectangle;
            Rectangle screen = new Rectangle(0, 0, Game.g_screenSize.Width, Game.g_screenSize.Height);
            int count = 0;
            for (int i = 0; i < particlesCount; i++)
			{
                Letter p = particles[i];
                if (screen.Contains(world.Camera.ToScreen(particles[i].position, 0.3f)))
                {
                    SpriteBatch.Instance.PrintText(font, p.letter.ToString(), p.position, new Color4(1f, 1f, 1f, p.opacity), p.rotation, p.scale);
                    count++;
                }
                
            }
            Console.SetCursorPosition(0, 0);
            Console.Write("background particles count in view: {0}   ",count);
             
            SpriteBatch.Instance.End();
             * */
            SpriteBatch.Instance.Begin(world.Camera.GetViewMatrix(0.3f));
            for (int i = 0; i < emitter_count; i++)
            {
                emitters[i].Draw(dt);
            }

            SpriteBatch.Instance.End();
        }
    }
}
