using AequusRemake.Content.Biomes.PollutedOcean;
using AequusRemake.Content.Items.Materials;
using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Graphics.Animations;
using AequusRemake.Core.Util.Helpers;
using Terraria.DataStructures;

namespace AequusRemake.Content.Tiles.Furniture.Trash;

public class SeaPickleTorch : UnifiedModTorch {
    public override int TorchIngredient => ModContent.ItemType<CompressedTrash>();
    public override int TorchCraftAmount => 20;

    public override Vector3 LightColor => new Vector3(0.5f, 1f, 1f);

    public override bool AllowWaterPlacement => true;

    public override void Load() {
        base.Load();
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override float GetTorchLuck(Player player) {
        return player.InModBiome<PollutedOceanBiomeUnderground>() ? 1f : -1f;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        AnimationSystem.GetValueOrAddDefault<AnimationSeaPickleTorch>(new Point16(i, j));
        return true;
    }
}

public class AnimationSeaPickleTorch : ITileAnimation {
    public bool Update(int x, int y) {
        var tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<SeaPickleTorch>() && Cull.ClipIJ(x, y);
    }
}
