using AQMod.Assets.LegacyItemOverlays;
using AQMod.Common.Graphics;
using AQMod.Items.DrawOverlays;
using AQMod.Items.Materials.Energies;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class StariteBlade : ModItem
    {
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

        public override Color? GetAlpha(Color lightColor)
        {
            int b = (int)(255 * AQUtils.Wave(Main.GlobalTime * 6f, 0.9f, 1f));
            return new Color(b, b, b, 255);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            AQGraphics.Rendering.DrawFallenStarAura(item, spriteBatch, scale, new Color(80, 80, 50, 50), new Color(150, 150, 130, 127));
            return true;
        }

        public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 15);
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].fadeIn = 0.2f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
                if (Main.netMode != NetmodeID.Server)
                {
                    AQSound.Play(SoundType.NPCHit, "inflict_bluefire", target.Center, 0.2f);
                }
            }
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