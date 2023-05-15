using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class BattleAxeBleeding : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Bleeding;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.SetImmune(NPCID.MartianWalker, Type);
            AequusBuff.SetImmune(NPCID.ForceBubble, Type);
            AequusBuff.SetImmune(NPCID.MartianDrone, Type);
            AequusBuff.SetImmune(NPCID.Probe, Type);
            AequusBuff.SetImmune(NPCID.PossessedArmor, Type);
            AequusBuff.SetImmune(NPCID.TheDestroyer, Type);
            AequusBuff.SetImmune(NPCID.TheDestroyerBody, Type);
            AequusBuff.SetImmune(NPCID.TheDestroyerTail, Type);
            AequusBuff.SetImmune(NPCID.SkeletronPrime, Type);
            AequusBuff.SetImmune(NPCID.PrimeCannon, Type);
            AequusBuff.SetImmune(NPCID.PrimeLaser, Type);
            AequusBuff.SetImmune(NPCID.PrimeSaw, Type);
            AequusBuff.SetImmune(NPCID.PrimeVice, Type);
            AequusBuff.SetImmune(NPCID.Wraith, Type);
            AequusBuff.SetImmune(NPCID.Mimic, Type);
            AequusBuff.SetImmune(NPCID.BigMimicCorruption, Type);
            AequusBuff.SetImmune(NPCID.BigMimicCrimson, Type);
            AequusBuff.SetImmune(NPCID.BigMimicHallow, Type);
            AequusBuff.SetImmune(NPCID.BigMimicJungle, Type);
            AequusBuff.SetImmune(NPCID.IceMimic, Type);
            AequusBuff.SetImmune(NPCID.PresentMimic, Type);
            AequusBuff.SetImmune(NPCID.SantaNK1, Type);
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (NPCID.Sets.DebuffImmunitySets.TryGetValue(n.Key, out var buff) && buff != null && buff.SpecificallyImmuneTo != null && buff.SpecificallyImmuneTo.ContainsAny(BuffID.Bleeding))
                {
                    AequusBuff.SetImmune(n.Key, false, Type);
                }
            }
            AequusBuff.PlayerDoTBuff.Add(Type);
        }
    }
}