using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Old.Content.Tools;

public class PhysicsGun : ModItem {
    public static Dictionary<int, bool> TileSpecialConditions { get; private set; }

    public Asset<Texture2D> GlowTexture => ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad);

    public override void Load() {
        TileSpecialConditions = new Dictionary<int, bool>();
    }

    public override void Unload() {
        TileSpecialConditions?.Clear();
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 5);
        Item.shoot = ModContent.ProjectileType<PhysicsGunProj>();
        Item.shootSpeed = 2f;
        Item.noUseGraphic = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.channel = true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        try {
            var texture = TextureAssets.Item[Type].Value;

            var coloring = Main.mouseColor;
            var glowTexture = GlowTexture.Value;

            spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            for (int i = 0; i < 4; i++) {
                spriteBatch.Draw(glowTexture, position + (i * MathHelper.PiOver2).ToRotationVector2() * scale * 2f, frame, (coloring * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)) with { A = 100 }, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(glowTexture, position, frame, coloring, 0f, origin, scale, SpriteEffects.None, 0f);
        }
        catch {
            return true;
        }
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        try {
            var texture = TextureAssets.Item[Type].Value;
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var drawCoordinates = Item.position - Main.screenPosition + origin + new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
            var itemOrigin = frame.Size() / 2f;

            var coloring = Main.mouseColor;
            var glowTexture = GlowTexture.Value;

            spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);

            for (int i = 0; i < 4; i++) {
                spriteBatch.Draw(glowTexture, drawCoordinates + (i * MathHelper.PiOver2).ToRotationVector2() * scale * 2f, frame, (coloring * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)) with { A = 100 }, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(glowTexture, drawCoordinates, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0f);
        }
        catch {
            return true;
        }
        return false;
    }
}