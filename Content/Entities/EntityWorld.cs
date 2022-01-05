using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Entities
{
    public sealed class EntityWorld : ModWorld
    {
        public override void Initialize()
        {
            CrabPot.Initialize();
        }

        public override void PreUpdate()
        {
            for (int i = 0; i < CrabPot.maxCrabPots; i++)
            {
                CrabPot.crabPots[i].Update();
            }
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound()
            {
                ["Version"] = AQMod.GetInstance().Version.ToString(),
            };
            CrabPot.SignSaveData(tag);
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            if (!tag.ContainsKey("Version"))
            {
                return;
            }
            try
            {
                var version = new Version(tag.GetString("Version"));
                CrabPot.LoadData(tag, version);
            }
            catch
            {
            }
        }
    }
}