using Aequus.Buffs;
using Aequus.Items.Misc.Fish;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.BuffPotions
{
    public class BloodthirstPotion : ModItem
    {
        public override void Load()
        {
            On.Terraria.NPC.NPCLoot_DropHeals += Hook_BloodthirstDropHeals;
        }

        private void Hook_BloodthirstDropHeals(On.Terraria.NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.HasBuff<BloodthirstBuff>())
            {
                Item.NewItem(self.GetSource_Loot(), self.getRect(), 58);
                return;
            }
            orig(self, closestPlayer);
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(234, 0, 83, 0), new Color(162, 0, 80, 0), };
            SacrificeTotal = 20;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.HeartreachPotion);
            Item.buffType = ModContent.BuffType<BloodthirstBuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Leecheel>()
                .AddIngredient(ItemID.Deathweed)
                .AddTile(TileID.Bottles)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.HeartreachPotion));
        }
    }
}