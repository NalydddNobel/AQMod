using AQMod.Items.DrawOverlays;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public sealed class Skrawler : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<Skrawler>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 48;
            item.magic = true;
            item.useTime = 48;
            item.useAnimation = 48;
            item.width = 32;
            item.height = 32;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = AQItem.Rarities.GoreNestRare;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.Skrawler>();
            item.shootSpeed = 10f;
            item.mana = 12;
            item.autoReuse = true;
            item.UseSound = SoundID.Item20;
            item.value = AQItem.Prices.DemonSiegeWeaponValue;
            item.knockBack = 2f;
        }
    }
}