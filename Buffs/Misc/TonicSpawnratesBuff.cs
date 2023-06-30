using Aequus.Common.Buffs;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Misc {
    public class TonicSpawnratesBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, Color.Green);
            AequusBuff.ConcoctibleBuffIDsBlacklist.Add(Type);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.buffTime[buffIndex] <= 2 && !player.buffImmune[ModContent.BuffType<TonicSpawnratesDebuff>()])
            {
                player.buffType[buffIndex] = ModContent.BuffType<TonicSpawnratesDebuff>();
                player.buffTime[buffIndex] = 1200;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
    public class TonicSpawnratesDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, Color.Green);
            Main.debuff[Type] = true;
        }
    }
}