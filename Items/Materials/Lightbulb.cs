using AQMod.Assets.ItemOverlays;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Items.Materials
{
    public class Lightbulb : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow"), getGlowmaskColor), item.type);
        }

        private Color getGlowmaskColor()
        {
            var random = new UnifiedRandom((int)Main.GameUpdateCount / 15 * 40 + Main.LocalPlayer.name.GetHashCode());
            random.Next();
            random.Next();
            random.Next();
            random.Next();
            for (int i = 0; i < random.Next(7) + 1; i++)
            {
                random.Next();
            }
            return new Color(250, 250, 250, 10) * random.NextFloat(0.1f, 0.8f);
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 5);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Glass);
            r.AddIngredient(ItemID.FallenStar);
            r.AddIngredient(ItemID.CopperBar);
            r.AddTile(TileID.Anvils);
            r.SetResult(this, 2);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Glass);
            r.AddIngredient(ItemID.FallenStar);
            r.AddIngredient(ItemID.TinBar);
            r.AddTile(TileID.Anvils);
            r.SetResult(this, 2);
            r.AddRecipe();
        }
    }
}