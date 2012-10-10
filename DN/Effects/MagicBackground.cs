using Blueberry.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public class MagicBackground
    {
        Shader shader;
        VertexBuffer buffer;
        float time = 0;

        public MagicBackground()
        {
            shader = new Shader();

            float v = Shader.Version;

            shader.LoadVertexFile(Path.Combine("Effects", v < 3.3f ? "v12" : "v33", "magic_background.vert"));
            shader.LoadFragmentFile(Path.Combine("Effects", v < 3.3f ? "v12" : "v33", "magic_background.frag"));

            shader.Link();

            buffer = new VertexBuffer(4);
            buffer.DeclareNextAttribute("vposition", 2);
            buffer.DeclareNextAttribute("vtexcoord", 2);
            buffer.DeclareNextAttribute("vcolor", 4);

            buffer.Attach(shader);
            
            buffer.AddVertex(-1, -1, 0, 0, 1f, 1f, 1f, 1f);
            buffer.AddVertex(1, -1, 1, 0, 1f, 1f, 1f, 1f);
            buffer.AddVertex(1, 1, 1, 1, 1, 1, 1, 1);
            buffer.AddVertex(-1, 1, 0, 1, 1, 1, 1, 1);
            buffer.AddIndices(0, 1, 2, 0, 2, 3);
            buffer.UpdateBuffer();
        }
        int way = 1;
        public void Draw(float dt)
        {
            time += way*dt/4;
            if (time >= 1 || time <= 0) { way *= -1; };
            //Console.SetCursorPosition(0, 0);
            //Console.Write("{0}     ", Math.Round(time, 3));

            shader.Use();
            buffer.Bind();
            shader.SetUniform("time",time);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.UseProgram(0);
        }

    }
}
