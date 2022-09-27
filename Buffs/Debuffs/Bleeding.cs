using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class Bleeding : ModBuff
    {
        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Bleeding;

        public static SoundStyle InflictDebuffSound => Aequus.GetSound("inflict_blood").WithVolume(0.5f);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.AddStaticImmunity(NPCID.MartianWalker, Type);
            AequusBuff.AddStaticImmunity(NPCID.ForceBubble, Type);
            AequusBuff.AddStaticImmunity(NPCID.MartianDrone, Type);
            AequusBuff.AddStaticImmunity(NPCID.Probe, Type);
            AequusBuff.AddStaticImmunity(NPCID.PossessedArmor, Type);
            AequusBuff.AddStaticImmunity(NPCID.TheDestroyer, Type);
            AequusBuff.AddStaticImmunity(NPCID.TheDestroyerBody, Type);
            AequusBuff.AddStaticImmunity(NPCID.TheDestroyerTail, Type);
            AequusBuff.AddStaticImmunity(NPCID.SkeletronPrime, Type);
            AequusBuff.AddStaticImmunity(NPCID.PrimeCannon, Type);
            AequusBuff.AddStaticImmunity(NPCID.PrimeLaser, Type);
            AequusBuff.AddStaticImmunity(NPCID.PrimeSaw, Type);
            AequusBuff.AddStaticImmunity(NPCID.PrimeVice, Type);
            AequusBuff.AddStaticImmunity(NPCID.Wraith, Type);
            AequusBuff.AddStaticImmunity(NPCID.Mimic, Type);
            AequusBuff.AddStaticImmunity(NPCID.BigMimicCorruption, Type);
            AequusBuff.AddStaticImmunity(NPCID.BigMimicCrimson, Type);
            AequusBuff.AddStaticImmunity(NPCID.BigMimicHallow, Type);
            AequusBuff.AddStaticImmunity(NPCID.BigMimicJungle, Type);
            AequusBuff.AddStaticImmunity(NPCID.IceMimic, Type);
            AequusBuff.AddStaticImmunity(NPCID.PresentMimic, Type);
            AequusBuff.AddStaticImmunity(NPCID.SantaNK1, Type);
            foreach (var n in ContentSamples.NpcsByNetId)
            {
                if (NPCID.Sets.DebuffImmunitySets.TryGetValue(n.Key, out var buff) && buff != null && buff.SpecificallyImmuneTo != null && buff.SpecificallyImmuneTo.ContainsAny(BuffID.Bleeding))
                {
                    AequusBuff.AddStaticImmunity(n.Key, false, Type);
                }
            }
        }
    }
}