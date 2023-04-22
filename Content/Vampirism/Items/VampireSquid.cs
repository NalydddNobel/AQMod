using Aequus.Common.Recipes;
using Aequus.Content.Vampirism.Buffs;
using Aequus.Items.Consumables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Vampirism.Items {
    public class VampireSquid : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
            ItemID.Sets.FoodParticleColors[Type] = new[] { Color.Red, Color.DarkRed };
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Red, };
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 3);
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item2;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.buffTime = 10800;
            Item.buffType = ModContent.BuffType<VampirismBuff>();
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanUseItem(Player player) {
            return !player.GetModPlayer<AequusPlayer>().IsVampire;
        }

        public override bool? UseItem(Player player) {
            var aequus = player.Aequus();
            if (aequus.IsVampire) {
                return false;
            }

            if (aequus.HasVampirism) {
                aequus._vampirismData /= 2;
            }
            else {
                aequus.GiveVampirism(Item.buffTime);
            }
            return true;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<PalePufferfish>());
        }
    }
}