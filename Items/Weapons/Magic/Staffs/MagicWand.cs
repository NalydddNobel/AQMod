using AQMod.Assets.ItemOverlays;
using AQMod.Assets.Textures;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic.Staffs
{
    public class MagicWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            if (!Main.dedServ)
                ItemOverlayLoader.Register(new Glowmask(GlowID.MagicWand, new Color(128, 128, 128, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.magic = true;
            item.damage = 68;
            item.knockBack = 2.25f;
            item.mana = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 32;
            item.useTime = 32;
            item.UseSound = SoundID.Item9;
            item.rare = ItemRarityID.LightRed;
            item.value = AQItem.OmegaStariteWeaponValue;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.MagicBolt>();
            item.autoReuse = true;
        }
    }
}