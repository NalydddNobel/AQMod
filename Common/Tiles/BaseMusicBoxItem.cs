using Aequus.Core.Assets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public abstract class BaseMusicBoxItem : ModItem {
    public abstract SoundAsset Music { get; }
    public abstract int MusicBoxTileId { get; }

    public override void SetStaticDefaults() {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, Music.ModPath), Type, Item.createTile);
    }

    public override void SetDefaults() {
        Item.DefaultToMusicBox(MusicBoxTileId, 0);
    }
}