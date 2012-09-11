using System;
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
namespace DN
{
    public class Game:GameWindow
    {
        private GameWorld gameWorld;

        public Game()
            : base(640, 480, GraphicsMode.Default, "Devil's nightmare")
        {
            GraphicsDevice.Instance.Initialize(640, 480); // i seeked this problem along whole hour x__x
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Black);

            CM.I.LoadTexture("wall_tile", Path.Combine("Content", "Textures", "wall_tile.png"));
            gameWorld = new GameWorld(10, 10);
            base.OnLoad(e);

        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            gameWorld.Update((float)e.Time);
            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            gameWorld.Draw((float) e.Time);

            SpriteBatch.Instance.Begin();
            SpriteBatch.Instance.DrawTexture(CM.I.tex("wall_tile"), 50,50, 32, 32, Rectangle.Empty, Color.White);
            SpriteBatch.Instance.End();

            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}
