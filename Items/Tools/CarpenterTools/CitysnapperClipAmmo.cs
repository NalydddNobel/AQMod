using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.CarpenterTools
{
    public class CitysnapperClipAmmo : ModItem
    {
        public static int AmmoID => ModContent.ItemType<CitysnapperClipAmmo>();

        public override string Texture => AequusHelpers.GetPath<CitysnapperClip>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 999;
            Item.ammo = AmmoID;
            Item.consumable = true;
        }
    }
}