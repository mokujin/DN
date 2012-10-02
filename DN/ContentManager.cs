using System.Drawing;
using Blueberry.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blueberry.Graphics.Fonts;

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

        private CM()
        {
            _textures = new Dictionary<string, Texture>();
            _fonts = new Dictionary<string, BitmapFont>();
        }
        public void LoadTexture(string asset, string file)
        {
            _textures.Add(asset, new Texture(file));
        }
        public void LoadFont(string asset, string file, float size)
        {
            _fonts.Add(asset, new BitmapFont(file, size));
        }

        public Texture tex(string asset)
        {
            return _textures[asset];
        }

        public BitmapFont Font(string asset)
        {
            return _fonts[asset];
        }
    }
}
