using Blueberry;
using Blueberry.Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DN.Effects
{
    public class BloodSystem
    {
        GameWorld world;

        Shader blood_blend_shader;

        class BloodSpot
        {
            public Vector2 position;
            public float life;
            public float size;
        }
        List<BloodEmitter> blood_emitters = new List<BloodEmitter>();
        List<BloodSpot> visible_spots = new List<BloodSpot>();
        List<BloodSpot> invisible_spots = new List<BloodSpot>();
        Texture blood = new Texture(Game.g_screenSize); // texture with blood spots
        Texture back;                                   // texture to blend blood with
        VertexBuffer blend_buffer;

        public BloodSystem(GameWorld world)
        {
            this.world = world;
        }
        public void Init()
        {
            blood_blend_shader = new Shader();
            blood_blend_shader.LoadVertexFile(Path.Combine("Effects", "v33", "blood_blend.vert"));
            blood_blend_shader.LoadFragmentFile(Path.Combine("Effects", "v33", "blood_blend.frag"));
            blood_blend_shader.Link();

            blend_buffer = new VertexBuffer(4);
            blend_buffer.DeclareNextAttribute("vposition", 2);
            blend_buffer.DeclareNextAttribute("vtexcoord", 2);
            blend_buffer.DeclareNextAttribute("vcolor", 4);


            blend_buffer.Attach(blood_blend_shader);
            blend_buffer.AddVertex(-1, -1, 0, 0, 1f, 1f, 1f, 1f);
            blend_buffer.AddVertex(1, -1, 1, 0, 1f, 1f, 1f, 1f);
            blend_buffer.AddVertex(1, 1, 1, 1, 1, 1, 1, 1);
            blend_buffer.AddVertex(-1, 1, 0, 1, 1, 1, 1, 1);
            blend_buffer.AddIndices(0, 1, 2, 0, 2, 3);
            blend_buffer.UpdateBuffer();
        }
        public void BlendWith(Texture tex)
        {
            back = tex;
        }
        public void Update(float dt)
        {
            for (int i = 0; i < blood_emitters.Count; i++)
            {
                blood_emitters[i].Update(dt);
            }
        }
        public void DrawParticles(float dt)
        {
            for (int i = 0; i < blood_emitters.Count; i++)
            {
                blood_emitters[i].Draw(dt);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="intensity">how many particles will be released along one spread</param>
        /// <param name="freequency">time between spreads</param>
        /// <param name="countOfSpreads"></param>
        /// <returns></returns>
        public BloodEmitter InitEmitter(Vector2 position, Vector2 direction, int intensity = 3, float freequency = 0.4f, int countOfSpreads = 100)
        {
            BloodEmitter e;
            for (int i = 0; i < blood_emitters.Count; i++)
            {
                if (blood_emitters[i].ActiveParticlesCount == 0 && !blood_emitters[i].Enabled)
                {
                    e = blood_emitters[i];
                    e.Position = position;
                    e.Direction = direction;
                    e.ReleaseQuantity = intensity;
                    e.triggerInterval = freequency;
                    e.countOfSpreads = countOfSpreads;
                    e.Enabled = true;
                    e.Initialise();
                    return e;
                }
            }
            e = new BloodEmitter(position, direction, 1);
            e.OnParticleDeath += e_OnParticleDeath;
            e.ReleaseQuantity = intensity;
            e.triggerInterval = freequency;
            e.countOfSpreads = countOfSpreads;
            e.Initialise(30, 0.7f);
            blood_emitters.Add(e);
            return e;
        }
        void e_OnParticleDeath(Emitter sender, Particle particle)
        {
            if (RandomTool.RandBool(0.25f)) return;
            if (invisible_spots.Count == 0)
            {
                BloodSpot s = new BloodSpot() { position = particle.Position, size = RandomTool.RandFloat(4, 16), life = 15 };
                visible_spots.Add(s);
            }
            else
            {
                BloodSpot s = invisible_spots[invisible_spots.Count - 1];
                invisible_spots.RemoveAt(invisible_spots.Count - 1);
                s.position = particle.Position;
                s.size = RandomTool.RandFloat(4, 16);
                s.life = 15;
                visible_spots.Add(s);
            }
        }

        public void DrawBackground(float dt)
        {
            blood_blend_shader.Use();
            blend_buffer.Bind();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, back.ID);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, blood.ID);

            GL.ActiveTexture(TextureUnit.Texture0);

            blood_blend_shader.SetUniform("tex1", 0);
            blood_blend_shader.SetUniform("tex2", 1);


            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.UseProgram(0);
        }
        public void PredrawBloodTexture(float dt)
        {
            BloodSpot spot;
            SpriteBatch.Instance.Begin(world.Camera.GetViewMatrix());
            for (int i = 0; i < visible_spots.Count; i++)
            {
                spot = visible_spots[i];
                SpriteBatch.Instance.FillCircle(spot.position, spot.size, Color4.Red, 5);
                spot.life -= dt;
                if (spot.life <= 0)
                {
                    invisible_spots.Add(spot);
                    visible_spots.RemoveAt(i);
                    i--;
                }
            }
            SpriteBatch.Instance.End(blood, true, true);
        }
    }
}
