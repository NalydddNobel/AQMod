using Terraria.ModLoader;

namespace AQMod.Content
{
    public abstract class ContentItem
    {
        public readonly string Mod;
        public readonly string Name;
        public int ID { get; protected set; }
        internal void SetID(int ID)
        {
            this.ID = ID;
        }

        public ContentItem(Mod mod, string name)
        {
            Mod = mod.Name;
            Name = name;
        }

        protected ContentItem(string mod, string name)
        {
            Mod = mod;
            Name = name;
        }

        public Mod GetMod()
        {
            return Mod == "AQMod" ? AQMod.Instance : ModLoader.GetMod(Mod);
        }

        public string GetKey()
        {
            return Mod + ":" + Name;
        }

        public bool IsKey(string key)
        {
            return key == GetKey();
        }
    }
}