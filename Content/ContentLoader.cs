using System.Collections.Generic;

namespace AQMod.Content
{
    public abstract class ContentLoader<T> where T : ContentItem
    {
        protected List<T> _content;
        protected int _contentCount;

        public int Count => _contentCount;

        public virtual void Setup()
        {
            _content = new List<T>();
            _contentCount = 0;
        }

        public virtual void Unload()
        {
            _content = null;
        }

        public virtual int AddContent(T item)
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