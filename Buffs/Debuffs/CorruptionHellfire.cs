using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class CorruptionHellfire : ModBuff
    {
        public static Color FireColor => new Color(100, 28, 160, 10);
        public static Color BloomColor => new Color(20, 2, 80, 10);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.DemonSiegeEnemyImmunity.Add(Type);
        }

        public static void AddStack(NPC npc, int time, int stacksAmt, bool inflictOnFireInsteadIfItsImmune = true)
        {
            if (npc.buffImmune[ModContent.BuffType<CorruptionHellfire>()])
            {
                if (inflictOnFireInsteadIfItsImmune)
                {
                    npc.AddBuff(BuffID.OnFire, time);
                }
                return;
            }

            AequusBuff.InflictAndPlaySound<CorruptionHellfire>(npc, time, BlueFire.InflictDebuffSound.WithPitch(-0.2f));
            npc.Aequus().corruptionHellfireStacks += (byte)stacksAmt;
            npc.netUpdate = true;
        }
    }
}