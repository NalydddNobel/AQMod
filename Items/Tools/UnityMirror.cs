using AQMod.Common;
using AQMod.Items.BuffItems.Foods;
using AQMod.Items.Placeable;
using AQMod.Items.TagItems.Starbyte;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class UnityMirror : ModItem, IUpdatePiggybank
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Blue;
        }

        private void UpdateUnityMirror(AQPlayer aQPlayer)
        {
            aQPlayer.unityMirror = true;
        }

        public override void UpdateInventory(Player player)
        {
            UpdateUnityMirror(player.GetModPlayer<AQPlayer>());
        }

        void IUpdatePiggybank.UpdatePiggyBank(Player player, int i)
        {
            UpdateUnityMirror(player.GetModPlayer<AQPlayer>());
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddIngredient(ModContent.ItemType<SpicyEel>(), 2);
            r.AddIngredient(ItemID.Bone, 15);
            r.AddIngredient(ModContent.ItemType<Molite>(), 5);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 9);
            r.AddIngredient(ModContent.ItemType<ExoticCoral>(), 30);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}