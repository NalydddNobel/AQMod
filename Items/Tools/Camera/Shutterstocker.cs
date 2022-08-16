using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Camera
{
    public class Shutterstocker : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadExtraRange[Type] = 400;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
            Item.useAmmo = ShutterstockerClipAmmo.AmmoID;
            Item.shoot = ModContent.ProjectileType<ShutterstockerProj>();
            Item.shootSpeed = 1f;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }
    }
}