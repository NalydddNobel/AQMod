using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Bait
{
    public class LegendberryBait : ModItem, IModifyFishAttempt
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 6;
            Item.height = 6;
            Item.bait = 40;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.value = Item.sellPrice(silver: 20);
            Item.rare = ItemRarityID.LightRed;
        }

        bool IModifyFishAttempt.OnItemRoll(Projectile bobber, ref FishingAttempt fisher)
        {
            if (fisher.crate)
            {
                return true;
            }
            fisher.common = false;
            int fishTier = GetFishRarity(fisher);
            int rolledTier = Main.rand.Next(4);
            if (rolledTier > fishTier)
            {
                fishTier = rolledTier;
            }

            fisher.uncommon = false;
            fisher.rare = false;
            fisher.veryrare = false;
            fisher.legendary = false;

            if (fishTier == 3)
            {
                fisher.legendary = true;
            }
            else if (fishTier == 2)
            {
                fisher.veryrare = true;
            }
            else if (fishTier == 1)
            {
                fisher.rare = true;
            }
            else if (fishTier == 0)
            {
                fisher.uncommon = true;
            }
            return true;
        }
        public int GetFishRarity(FishingAttempt fisher)
        {
            if (fisher.legendary)
            {
                return 3;
            }
            if (fisher.veryrare)
            {
                return 2;
            }
            if (fisher.rare)
            {
                return 1;
            }
            return 0;
        }
    }
}