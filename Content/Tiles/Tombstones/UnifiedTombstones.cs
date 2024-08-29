using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.Generative;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Tombstones;
public abstract class UnifiedTombstones : ModTile {
    public abstract int StyleCount { get; }

    public readonly ModItem[] Items;
    public readonly ModProjectile[] TombProjectile;

    public UnifiedTombstones() {
        Items = new ModItem[StyleCount];
        TombProjectile = new ModProjectile[StyleCount];
    }

    public override void Load() {
        for (int i = 0; i < StyleCount; i++) {
            ModItem nextItem = Items[i] = new InstancedTombstoneItem(this, i);

            Mod.AddContent(nextItem);
        }
    }

    public override void SetStaticDefaults() {
        Main.tileSign[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        AdjTiles = [TileID.Tombstones];
        DustType = DustID.Ash;
    }

    public abstract TombstoneOverride? OverrideTombstoneDrop(Player player, bool gold, long coinsOwned);
}

public record struct TombstoneOverride(int ProjType, string? TombstoneText);

internal class InstancedTombstoneItem(ModTile parent, int style) : InstancedModItem($"Tombstone{style}", parent.Texture) {
    public override LocalizedText DisplayName => parent.GetLocalization($"MapEntry.{style}");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 2;
        if (!Main.dedServ) {
            Main.QueueMainThreadAction(SetTexture);
        }
    }

    void SetTexture() {
        TextureAssets.Item[Type] = TextureGen.New(new EffectAtlasMerge(36 * style, 0, 2, 2, new AtlasInfo(16, 2, [16, 18], 2)), TextureAssets.Item[Type]);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Tombstone);
        Item.createTile = parent.Type;
        Item.placeStyle = style;
    }
}