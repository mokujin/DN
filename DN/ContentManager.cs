using System.Drawing;
using Blueberry.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry.Graphics.Fonts;
using System.IO;


namespace DN
{
    /// <summary>Content Manager</summary>
    public class CM
    {
        private static CM _instance;

        /// <summary>Instance</summary>
        public static CM I { get { return _instance ?? (_instance = new CM()); }}

        private readonly Dictionary<string, Texture> _textures;
        private readonly Dictionary<string, BitmapFont> _fonts;

        public struct SoundStream
        {
            byte[] buffer;
            List<OggStream.OggStream> streams;
            public int maxStreams;

            public SoundStream(string file)
            {
                buffer = File.ReadAllBytes(file);
                streams = new List<OggStream.OggStream>();
                maxStreams = 5;
            }
            public OggStream.OggStream GetFreeSream()
            {
                foreach (var item in streams)
                {
                    if (item.State != OpenTK.Audio.OpenAL.ALSourceState.Playing)
                        return item;
                }
                if (maxStreams > streams.Count)
                {
                    OggStream.OggStream s = new OggStream.OggStream(new MemoryStream(buffer));
                    streams.Add(s);
                    return s;
                }
                else
                    return null;
            }
        }
        private readonly Dictionary<string, SoundStream> _sounds;

        private CM()
        {
            _textures = new Dictionary<string, Texture>();
            _fonts = new Dictionary<string, BitmapFont>();
            _sounds = new Dictionary<string, SoundStream>();
        }
        public void LoadTexture(string asset, string file)
        {
            _textures.Add(asset, new Texture(file));
        }
        public void LoadFont(string asset, string file, float size)
        {
            _fonts.Add(asset, new BitmapFont(file, size));
        }
        public void LoadSound(string asset, string file)
        {
            _sounds.Add(asset, new SoundStream(file));
        }
        public Texture tex(string asset)
        {
            return _textures[asset];
        }

        public BitmapFont Font(string asset)
        {
            return _fonts[asset];
        }
        public SoundStream Sound(string asset)
        {
            return _sounds[asset];
        }

    }
}
