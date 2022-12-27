using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class AethersWrath : ModBuff
    {
        public static SoundStyle InflictDebuffSound => Aequus.GetSound("inflictaetherfire", volume: 1f, pitch: 0.2f, variance: 0.3f);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.AddStaticImmunity(NPCID.MoonLordCore, Type);
            AequusBuff.AddStaticImmunity(NPCID.MoonLordHand, Type);
            AequusBuff.AddStaticImmunity(NPCID.MoonLordHead, Type);
            AequusBuff.AddStaticImmunity(NPCID.Pixie, Type);
            AequusBuff.AddStaticImmunity(NPCID.Unicorn, Type);
            AequusBuff.AddStaticImmunity(NPCID.LightMummy, Type);
            AequusBuff.AddStaticImmunity(NPCID.ChaosElemental, Type);
            AequusBuff.AddStaticImmunity(NPCID.RainbowSlime, Type);
            AequusBuff.AddStaticImmunity(NPCID.Gastropod, Type);
            AequusBuff.AddStaticImmunity(NPCID.HallowBoss, Type);
            AequusBuff.AddStaticImmunity(NPCID.BigMimicHallow, Type);
            AequusBuff.AddStaticImmunity(NPCID.DesertGhoulHallow, Type);
            AequusBuff.AddStaticImmunity(NPCID.PigronHallow, Type);
            AequusBuff.AddStaticImmunity(NPCID.SandsharkHallow, Type);
            AequusBuff.AddStaticImmunity(NPCID.QueenSlimeBoss, Type);
            AequusBuff.AddStaticImmunity(NPCID.QueenSlimeMinionBlue, Type);
            AequusBuff.AddStaticImmunity(NPCID.QueenSlimeMinionPink, Type);
            AequusBuff.AddStaticImmunity(NPCID.QueenSlimeMinionPurple, Type);
            AequusBuff.IsFire.Add(Type);
        }
    }
}