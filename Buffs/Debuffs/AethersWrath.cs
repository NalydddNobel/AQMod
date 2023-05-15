using Aequus.Common.DataSets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class AethersWrath : ModBuff, IPostAddRecipes {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.SetImmune(NPCID.MoonLordCore, Type);
            AequusBuff.SetImmune(NPCID.MoonLordHand, Type);
            AequusBuff.SetImmune(NPCID.MoonLordHead, Type);
            AequusBuff.SetImmune(NPCID.CultistArcherBlue, Type);
            AequusBuff.SetImmune(NPCID.CultistArcherWhite, Type);
            AequusBuff.SetImmune(NPCID.CultistBoss, Type);
            AequusBuff.SetImmune(NPCID.CultistBossClone, Type);
            AequusBuff.SetImmune(NPCID.CultistDevote, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonHead, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody1, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody2, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody3, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonBody4, Type);
            AequusBuff.SetImmune(NPCID.CultistDragonTail, Type);
            AequusBuff.SetImmune(NPCID.AncientCultistSquidhead, Type);
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public void PostAddRecipes(Aequus aequus) {
            foreach (var i in NPCSets.IsHallow) {
                AequusBuff.SetImmune(i, Type);
            }
            foreach (var i in NPCSets.FromPillarEvent) {
                AequusBuff.SetImmune(i, Type);
            }
        }
    }
}