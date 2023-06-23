using Aequus.Common.Effects.RenderBatches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    [AutoloadGlowMask]
    public class ZombieScepter : SceptreBase {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(HealAmount);

        public override void SetDefaults() {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(10, 1f, 0);
            Item.shootSpeed = 9f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
            HealAmount = 2;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 4)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .AddCondition(Condition.InGraveyard)
                .TryRegisterAfter(ItemID.RainbowRod)
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .TryRegisterAfter(ItemID.RainbowRod);
        }

        public override void DrawToLayer(RenderLayerBatch layer, SpriteBatch spriteBatch) {
            for (int i = 0; i < npcRenders.Count; i++) {
                EnemyRender n = npcRenders[i];

                var drawPosition = n.Location - Main.screenPosition;
                var bloomColor = Color.Blue with { A = 0 };
                spriteBatch.Draw(AequusTextures.Bloom6, drawPosition, null, bloomColor * n.Opacity, 0f, AequusTextures.Bloom6.Size() / 2f, 0.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AequusTextures.Bloom6, drawPosition, null, bloomColor * n.Opacity * 0.3f, 0f, AequusTextures.Bloom6.Size() / 2f, 2f, SpriteEffects.None, 0f);

                var flare = AequusTextures.Flare.Value;
                var flareOrigin = flare.Size() / 2f;
                var flareColor = bloomColor.HueAdd(Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, -0.05f, 0f)) with { A = 100 };
                float flareOffset = (n.Size.X + flare.Width / 2f) * n.Opacity;
                float flareRotation = Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, -0.1f, 0.1f);
                Vector2 flareScale = new Vector2(0.6f, 1.2f);
                spriteBatch.Draw(flare, drawPosition + new Vector2(flareOffset, 0f).RotatedBy(flareRotation), null, flareColor * n.Opacity, flareRotation + MathHelper.PiOver2, flareOrigin, flareScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(flare, drawPosition - new Vector2(flareOffset, 0f).RotatedBy(flareRotation), null, flareColor * n.Opacity, flareRotation + MathHelper.PiOver2, flareOrigin, flareScale, SpriteEffects.None, 0f);
            }
        }
    }
}