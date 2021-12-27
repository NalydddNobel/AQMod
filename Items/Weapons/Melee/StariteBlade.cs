using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class StariteBlade : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private readonly BasicOverlay _overlay = new BasicOverlay(AQUtils.GetPath<StariteBlade>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetStaticDefaults()
        {
            AQItem.CreativeMode.SingleItem(this);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = AQItem.Rarities.StariteWeaponRare;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.value = AQItem.Prices.GlimmerWeaponValue;
            Item.damage = 38;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4.5f;
            Item.scale = 1.25f;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 15);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenSword)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}