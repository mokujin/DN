using Blueberry.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DN
{
    /// <summary>Content Manager</summary>
    public class CM
    {
        private static CM _instance;

        /// <summary>Instance</summary>
        public static CM I { get { if (_instance == null) _instance = new CM(); return _instance; } }

        private Dictionary<string, Texture> _textures;

        private CM()
        {
            _textures = new Dictionary<string, Texture>();
        }
        public void LoadTexture(string asset, string file)
        {
            _textures.Add(asset, new Texture(file));
        }
        public Texture tex(string asset)
        {
            return _textures[asset];
        }
    }
}
