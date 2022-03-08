using AQMod.Common.Graphics;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Recipes;
using AQMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class Resonance : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 12;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.melee = true;
            item.damage = 36;
            item.knockBack = 4.11f;
            item.value = AQItem.Prices.OmegaStariteWeaponValue;
            item.UseSound = SoundID.Item1;
            item.rare = AQItem.RarityOmegaStarite;
            item.channel = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<ResonanceProjectile>();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(Main.itemTexture[item.type], position, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 3f, 0f, 0.3f), 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            AQGraphics.Rendering.FallenStarAura(item, spriteBatch, scale, new Color(40, 40, 40, 40), new Color(100, 70, 70, 100));
            Rectangle frame;
            if (Main.itemAnimations[item.type] != null)
            {
                frame = Main.itemAnimations[item.type].GetFrame(Main.itemTexture[item.type]);
            }
            else
            {
                frame = Main.itemTexture[item.type].Frame();
            }
            var origin = frame.Size() / 2f;
            var drawCoordinates = item.position - Main.screenPosition + origin + new Vector2(item.width / 2 - origin.X, item.height - frame.Height);
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawCoordinates, frame, item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Main.itemTexture[item.type], drawCoordinates, frame, new Color(255, 255, 255, 0) * AQUtils.Wave(Main.GlobalTime * 6f, 0f, 0.5f), rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<StariteSpinner>(), 1);
            r.AddRecipeGroup(AQRecipeGroups.CascadeOrHelfire, 1);
            r.AddIngredient(ModContent.ItemType<CosmicEnergy>(), 5);
            r.AddIngredient(ModContent.ItemType<LightMatter>(), 8);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}