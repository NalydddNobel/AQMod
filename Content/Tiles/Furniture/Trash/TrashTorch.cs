using Aequus.Common.Tiles;
using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.Materials;
using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using Terraria.DataStructures;

namespace Aequus.Content.Tiles.Furniture.Trash;

public class TrashTorch : ModTorch {
    public override int TorchIngredient => ModContent.ItemType<CompressedTrash>();
    public override int TorchCraftAmount => 20;

    public override Vector3 LightColor => new Vector3(0.5f, 1f, 1f);

    public override bool AllowWaterPlacement => true;

    public ModTile CampfireTile { get; private set; }
    public ModItem CampfireItem { get; private set; }

    public override void Load() {
        base.Load();
        InstancedCampfireTile campfire = new InstancedCampfireTile(this);
        CampfireTile = campfire;
        Mod.AddContent(CampfireTile);
        CampfireItem = campfire.Item;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override float GetTorchLuck(Player player) {
        return player.InModBiome<PollutedOceanBiome>() ? 1f : -1f;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        AnimationSystem.GetValueOrAddDefault<AnimationTrashTorch>(new Point16(i, j));
        return true;
    }
}

public class AnimationTrashTorch : ITileAnimation {
    public bool Update(int x, int y) {
        var tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<TrashTorch>() && Cull2D.Tile(x, y);
    }
}
