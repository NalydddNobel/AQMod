using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.CarpenterTools
{
    public class ShutterstockerClipAmmo : ModItem
    {
        public static int AmmoID => ModContent.ItemType<ShutterstockerClipAmmo>();

        public override string Texture => AequusHelpers.GetPath<ShutterstockerClip>();

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