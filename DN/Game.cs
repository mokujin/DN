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
namespace DN
{
    public class Game:GameWindow
    {
        private GameWorld gameWorld;

        public Game()
            : base(640, 480, GraphicsMode.Default, "Devil's nightmare")
        {
 
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.White);
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

            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}
