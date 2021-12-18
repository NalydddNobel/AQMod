using AQMod.Content.CursorDyes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets.Cursors
{
    public struct CursorTextureArray : ITextureComponent
    {
        private readonly string _path;
        private static readonly bool _debug = false;

        private readonly Ref<Texture2D>[] _textures;
        private readonly bool[] _loaded;
        private readonly bool[] _loadedProperly;

        public bool IsNull => false;
        public string Path => _path;

        public bool GetLoaded(int index)
        {
            return _loaded[index];
        }

        public bool GetLoadedProperly(int index)
        {
            return _loadedProperly[index];
        }

        internal CursorTextureArray(string path)
        {
            _path = path;
            int length = (int)CursorType.Count;
            _loaded = new bool[length];
            _loadedProperly = new bool[length];
            _textures = new Ref<Texture2D>[length];
            if (_debug)
            {
                LoadValue(null);
            }
        }

        private string getPath(object context)
        {
            return _path + context.ToString();
        }

        public void LoadValue(object context)
        {
            if (!_loaded[(int)context])
            {
                string path = getPath((int)context);
                _loadedProperly[(int)context] = ModContent.TextureExists(path);
                if (_loadedProperly[(int)context])
                {
                    _textures[(int)context] = new Ref<Texture2D>(ModContent.GetTexture(path));
                }
                _loaded[(int)context] = true;
            }
        }

        public bool GetCursorTexture(int type, out Texture2D texture)
        {
            LoadValue(type);
            texture = null;
            if (_loadedProperly[type])
            {
                texture = _textures[type].Value;
                return true;
            }
            return false;
        }

        public bool GetCursorTexture(CursorType type, out Texture2D texture)
        {
            return GetCursorTexture((int)type, out texture);
        }

        public Ref<Texture2D> GetRef(object context)
        {
            return _textures[(int)context];
        }
    }
}
