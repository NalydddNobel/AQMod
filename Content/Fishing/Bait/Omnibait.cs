using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Bait {
    public class Omnibait : ModItem, IModifyFishAttempt
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.LightRed;
        }

        bool IModifyFishAttempt.OnItemRoll(Projectile bobber, ref FishingAttempt fisher)
        {
            var p = Main.player[bobber.owner];
            var aequus = p.Aequus();
            aequus.omnibait = true;
            p.zone1 = (byte)Main.rand.Next(0, byte.MaxValue + 1);
            p.zone2 = (byte)Main.rand.Next(0, byte.MaxValue + 1);
            p.zone3 = (byte)Main.rand.Next(0, byte.MaxValue + 1);
            p.zone4 = (byte)Main.rand.Next(0, byte.MaxValue + 1);
            p.zone5 = (byte)Main.rand.Next(0, byte.MaxValue + 1);
            if (Main.rand.NextBool(4))
            {
                fisher.heightLevel = Main.rand.Next(5);
            }
            if (Main.rand.NextBool(8))
            {
                fisher.CanFishInLava = true;
                fisher.inLava = true;
            }
            else if (Main.rand.NextBool(8))
            {
                fisher.inLava = false;
                fisher.inHoney = true;
            }
            else if (Main.rand.NextBool(8))
            {
                fisher.inLava = false;
                fisher.inHoney = false;
            }

            // Prevents some cheese
            if (!NPC.downedBoss3) {
                p.ZoneDungeon = false;
            }
            if (!Main.hardMode) {
                p.ZoneLihzhardTemple = false;
                p.ZoneHallow = false;
            }
            if (!NPC.downedPlantBoss) {
                p.ZoneLihzhardTemple = false;
            }
            return true;
        }
    }
}