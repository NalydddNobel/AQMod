using Aequu2.Core.Entities.Tiles;
using Terraria.GameContent;

namespace Aequu2.Old.Content.Items.Materials.MonoGem;

public class MonoGemTile : BaseGemTile {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Main.tileLighted[Type] = true;

        AddMapEntry(new Color(66, 55, 55), Lang.GetItemName(ModContent.ItemType<MonoGem>()));
        DustType = DustID.Ambient_DarkBrown;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.2f;
        g = 0.2f;
        b = 0.2f;
    }


    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        Vector2 worldCoordinates = new Vector2(i * 16f, j * 16f);
        Vector2 drawCoordinates = worldCoordinates + TileHelper.DrawOffset - Main.screenPosition;

        if (!Main.tile[i, j].IsTileInvisible) {
            ulong seed = Helper.TileSeed(i, j);
            float pulse = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 0.1f + Utils.RandomFloat(ref seed) * 20f, 0.7f, 1f);

            Main.spriteBatch.Draw(
                Aequu2Textures.Bloom,
                drawCoordinates + new Vector2(8f),
                null,
                Color.Black * pulse * 0.2f,
                0f,
                Aequu2Textures.Bloom.Size() / 2f,
                0.45f * pulse + 0.33f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                drawCoordinates,
                new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16),
                Color.White);
        }

        if (tModLoaderExtended.ExtendedMod.GameWorldActive && Main.rand.NextBool(10)) {
            Vector2 randomLocation = worldCoordinates + new Vector2(8f) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(50f);
            float scale = Main.rand.NextFloat(0.5f, 0.9f);
            Vector2 velocity = Vector2.UnitY * 0.25f * scale;
            Color color = (Color.Gray * (1f - scale * 0.5f)) with { A = 255 };
            Dust d = Dust.NewDustPerfect(randomLocation, DustID.TintableDustLighted, velocity, Alpha: 255, newColor: color, Scale: scale);
            d.fadeIn = d.scale + 0.1f;
            d.noGravity = true;
            d.noLight = true;
            d.noLightEmittence = true;
            d.rotation = 0f;
        }

        MonoGemRenderer.Instance.Enqueue(new Point(i, j));
        return false;
    }
}