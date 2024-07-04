using AequusRemake.Core.Entities.Players.Drawing;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace AequusRemake.Content.Items.Accessories.EventPrevention;

public class StellarGlasses : ModItem {
    private class StellarGlassesEquipTexture : EquipTexture, IEquipTextureDraw {
        public void Draw(ref PlayerDrawSet drawInfo, Vector2 position, Rectangle frame, Color color, float rotation, Vector2 origin, SpriteEffects effects, int shader) {
            Texture2D texture = TextureAssets.AccFace[drawInfo.drawPlayer.face].Value;
            color = Color.Lerp(color, Color.White with { A = color.A } * (1f - drawInfo.shadow), 0.8f);

            drawInfo.Draw(texture, position, frame, color, rotation, origin, 1f, effects, shader);
            float time = Main.GlobalTimeWrappedHourly * 0.5f % 2f;
            if (time < 1f) {
                float waveFunction = MathF.Sin(time * MathHelper.Pi);
                Color glowColor = color with { A = 0 } * 0.2f * waveFunction;
                for (int i = 0; i < 4; i++) {
                    drawInfo.Draw(texture, position + (i * MathHelper.PiOver2 + time * 5f).ToRotationVector2() * 2f * waveFunction, frame, glowColor, rotation, origin, 1f, effects, shader);
                }
            }
        }
    }

    public override void Load() {
        Item.faceSlot = EquipLoader.AddEquipTexture(Mod, AequusTextures.StellarGlasses_Face.Path, EquipType.Face, this, null, new StellarGlassesEquipTexture());
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 2);
        Item.faceSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<EventDeactivatorPlayer>().accDisableGlimmer = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.8f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient(ItemID.FallenStar, 5)
            //.AddIngredient<Content.Items.Materials.StariteMaterial>(5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
