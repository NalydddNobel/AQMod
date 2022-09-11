using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace Aequus.Content.WorldGeneration
{
    public struct Structures : IEnumerable<KeyValuePair<string, Point>>
    {
        private Dictionary<string, Point> inner;

        public Structures()
        {
            inner = new Dictionary<string, Point>();
        }

        /// <summary>
        /// Adds a structure to the list
        /// </summary>
        /// <param name="name">The name of the structure</param>
        /// <param name="location">The location of the structure in tile coordinates</param>
        internal void Add(string name, Point location)
        {
            inner.Add(name, location);
        }

        public int Count => inner.Count;

        /// <summary>
        /// Gets the location of a structure using a name
        /// </summary>
        /// <param name="name">The name of the structure</param>
        /// <returns></returns>
        public Point? GetLocation(string name)
        {
            if (inner.TryGetValue(name, out var value))
            {
                return value;
            }
            return null;
        }

        public void Save(TagCompound tag, string name = "Structures")
        {
            if (inner == null)
            {
                return;
            }
            if (Count > 0)
            {
                var tag2 = new TagCompound();
                foreach (var pair in inner)
                {
                    if (Aequus.LogMore)
                    {
                        Aequus.Instance.Logger.Debug(pair.Key + ": " + pair.Value);
                    }
                    tag2[pair.Key] = pair.Value.ToVector2();
                }
                tag[name] = tag2;
            }
        }

        public void Load(TagCompound tag, string name = "Structures")
        {
            if (inner == null)
            {
                inner = new Dictionary<string, Point>();
            }
            if (tag.TryGet<TagCompound>(name, out var tag2))
            {
                foreach (var pair in tag2)
                {
                    if (Aequus.LogMore)
                    {
                        Aequus.Instance.Logger.Debug(pair.Key + ": " + pair.Value);
                    }
                    inner.Add(pair.Key, tag2.Get<Vector2>(pair.Key).ToPoint());
                }
            }
        }

        public IEnumerator<KeyValuePair<string, Point>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }
}