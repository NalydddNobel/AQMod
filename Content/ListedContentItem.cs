using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content
{
    public abstract class ListedContentItem
    {
        public readonly string Mod;
        public readonly string Name;
        protected bool unloaded;
        public bool IsUnloadedContent => unloaded;
        public int ID { get; protected set; }
        internal void SetID(int ID)
        {
            this.ID = ID;
        }

        public ListedContentItem(Mod mod, string name)
        {
            Mod = mod.Name;
            Name = name;
        }

        protected ListedContentItem(string mod, string name)
        {
            Mod = mod;
            Name = name;
        }

        public Mod GetMod()
        {
            return Mod == "AQMod" ? AQMod.GetInstance() : ModLoader.GetMod(Mod);
        }

        public string GetKey()
        {
            return Mod + ":" + Name;
        }

        public bool IsKey(string key)
        {
            return key == GetKey();
        }

        protected virtual ListedContentItem Clone()
        {
            return (ListedContentItem)MemberwiseClone();
        }

        protected virtual void Save(TagCompound tag)
        {
        }

        protected virtual void Load(TagCompound tag)
        {
        }

        public TagCompound SaveContent(object serializationContext)
        {
            var tag = new TagCompound();
            Save(tag);
            tag["mod"] = Mod;
            tag["name"] = Mod;
            return tag;
        }

        public static T LoadContent<T>(TagCompound tag) where T : ListedContentItem
        {
            var instance = (T)Activator.CreateInstance(typeof(T), tag.GetString("mod"), tag.GetString("name"));
            try
            {
                if (ModLoader.GetMod(instance.Mod) == null)
                {
                    instance.unloaded = true;
                }
            }
            catch
            {
                instance.unloaded = true;
            }
            instance.Load(tag);
            return instance;
        }
    }
}