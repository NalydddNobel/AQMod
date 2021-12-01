using AQMod.Assets.ItemOverlays;
using AQMod.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class StariteBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlay(this.GetPath("_Glow"), new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.rare = AQItem.Rarities.StariteWeaponRare;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.Prices.GlimmerWeaponValue;
            item.damage = 38;
            item.melee = true;
            item.knockBack = 4.5f;
            item.scale = 1.25f;
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
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.WoodenSword);
            r.AddIngredient(ItemID.FallenStar, 8);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}