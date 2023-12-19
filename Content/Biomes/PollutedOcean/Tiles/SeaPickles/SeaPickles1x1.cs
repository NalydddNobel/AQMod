using Aequus.Core.Graphics.Animations;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;

[LegacyName("SeaPickle", "SeaPickleTile")]
public class SeaPickles1x1 : ModTile {
    public override void SetStaticDefaults() {
        Main.tileLighted[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;

        AddMapEntry(new(120, 180, 40));
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

        r = anim.LightMagnitude * 0.5f;
        g = anim.LightMagnitude * 0.75f;
        b = anim.LightMagnitude * 0.1f;
    }
}
