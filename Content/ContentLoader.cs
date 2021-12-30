using AQMod.Common;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public abstract class ContentLoader<T> : IAutoloadType where T : ListedContentItem
    {
        protected List<T> _content;
        protected int _contentCount;

        public int Count => _contentCount;

        void IAutoloadType.OnLoad()
        {
            ContentInstance.Register(this);
            _content = new List<T>();
            _contentCount = 0;
            Load(AQMod.Instance);
        }

        void IAutoloadType.Unload()
        {
            Unload(AQMod.Instance);
            _content = null;
        }

        public virtual void Load(AQMod aQMod)
        {
        }

        public virtual void Unload(AQMod aQMod)
        {
        }

        public virtual int InitializeContent(T item)
        {
            if (AQMod.Loading)
            {
                if (GetContent(item.Mod, item.Name) == null)
                {
                    item.SetID(_contentCount);
                    _content.Add(item);
                    _contentCount++;
                    return _contentCount - 1;
                }
            }
            return -1;
        }

        public virtual T GetContent(int id)
        {
            return _content[id];
        }

        public T GetContent(string mod, string name)
        {
            return GetContent(mod + ":" + name);
        }

        /// <summary>
        /// Tries to find the content using a key. Returns null if the content wasn't found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T GetContent(string key)
        {
            return _content.Find((c) => c.IsKey(key));
        }

        public int GetContentID(string mod, string name)
        {
            return GetContentID(mod + ":" + name);
        }

        public virtual int GetContentID(string key)
        {
            return _content.FindIndex((c) => c.IsKey(key));
        }
    }
}