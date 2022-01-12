using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Players
{
    public sealed class PlayerCounts : ModPlayer
    {
        public int DeathCount { get; private set; }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            DeathCount++;
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["DeathCount"] = DeathCount,
            };
        }

        public override void Load(TagCompound tag)
        {
            DeathCount = tag.GetInt("DeathCount");
        }
    }
}