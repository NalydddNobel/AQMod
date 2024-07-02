using Aequus.Core.Entities.Tiles.Rubblemaker;
using Aequus.Core.Graphics.Animations;
using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Ambient.SeaPickles;

internal abstract class SeaPicklesTileBase : RubblemakerTile {
    protected float LightMagnitudeMultiplier = 1f;
    protected int _frameHeight;

    protected SeaPicklesTileBase() : base() {
    }
    protected SeaPicklesTileBase(string name, string texture, bool natural) : base(name, texture, natural) {
    }

    public override void SafeSetStaticDefaults() {
        base.SafeSetStaticDefaults();
        Main.tileLighted[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.DrawYOffset = 2;

        HitSound = SoundID.NPCDeath1;

        AddMapEntry(new(120, 180, 40));

        _frameHeight = TileObjectData.newTile.CoordinateFullHeight;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        if (Main.tile[i, j].LiquidAmount <= 0) {
            r = g = b = 0f;
            return;
        }

        i -= Main.tile[i, j].TileFrameX % 36 / 18;
        j -= Main.tile[i, j].TileFrameY % 36 / 18;
        var anim = AnimationSystem.GetValueOrAddDefault<SeaPickleLightTracker>(i, j);
        anim.DespawnTimer = 0;
        GetDrawData(i, j, out var pickleColor);

        float lightMagnitude = anim.LightMagnitude * LightMagnitudeMultiplier * 0.75f;
        var v3 = pickleColor.ToVector3() * lightMagnitude;
        r = v3.X;
        g = v3.Y;
        b = v3.Z;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var tile = Main.tile[i, j];
        var texture = TextureAssets.Tile[Type].Value;
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var drawCoordinates = new Vector2(i * 16f, j * 16f + 2f) - Main.screenPosition + TileHelper.DrawOffset;
        var lightColor = Lighting.GetColor(i, j);
        spriteBatch.Draw(texture, drawCoordinates, frame, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        int left = i - tile.TileFrameX % 36 / 18;
        int top = j - tile.TileFrameY % 36 / 18;
        if (Main.tile[left, top].LiquidAmount > 0) {
            GetDrawData(i, j, out var pickleColor);
            spriteBatch.Draw(texture, drawCoordinates, frame with { Y = frame.Y + _frameHeight }, pickleColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        return false;
    }

    public static void GetDrawData(int i, int j, out Color pickleColor) {
        var tile = Main.tile[i, j];
        int left = i;
        int top = j;
        if (TileLoader.GetTile(tile.TileType) is SeaPicklesTileBase) {
            left -= tile.TileFrameX % 36 / 18;
            top -= tile.TileFrameY % 36 / 18;
        }
        pickleColor = new Color(255, 255, 30);
        if (AnimationSystem.TryGet<SeaPickleLightTracker>(left, top, out var lightTracker)) {
            pickleColor = lightTracker.SeaPickleColor;
        }
    }
}