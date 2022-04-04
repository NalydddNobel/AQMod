using Aequus.Items.Recipes;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class Slice : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<SliceProj>(10);
            Item.SetWeaponValues(40, 6f);
            Item.width = 20;
            Item.height = 20;
            Item.autoReuse = true;
            Item.rare = ItemRarities.GaleStreams;
            Item.value = ItemPrices.GaleStreamsValue;
            Item.UseSound = SoundID.Item1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(120);
        }

        public override void AddRecipes()
        {
            AequusRecipes.SpaceSquidDrop(this, ModContent.ItemType<CrystalDagger>());
        }
    }
}