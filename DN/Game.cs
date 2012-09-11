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
        public Game()
            : base(640, 480, GraphicsMode.Default, "Devil's nightmare")
        {
 
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.White);
            base.OnLoad(e);

        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            SpriteBatch.Instance.Begin();
            SpriteBatch.Instance.FillEllipse(new System.Drawing.PointF(15, 15), 10, 12, Color4.Black, 8);
            SpriteBatch.Instance.End();
            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}
