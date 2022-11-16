using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class NightfallDebuff : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public static SoundStyle InflictDebuffSound => SoundID.Item4.WithPitch(0.6f).WithVolume(0.5f);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            var aequus = npc.Aequus();
            aequus.nightfallStacks = (byte)Math.Min(aequus.nightfallStacks + 1, 20);
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var aequus = npc.Aequus();
            aequus.nightfallStacks = Math.Max(aequus.nightfallStacks, (byte)1);
        }

        public static void AddBuff(NPC npc, int time)
        {
            AequusBuff.ApplyBuff<NightfallDebuff>(npc, time, out bool canPlaySound);
            if (canPlaySound)
            {
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    PacketSystem.SyncSound(SoundPacket.InflictNightfall, npc.Center);
                }
                SoundEngine.PlaySound(InflictDebuffSound, npc.Center);
            }
        }
    }
}