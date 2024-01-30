using Terraria.Audio;

namespace Aequus.Common.Tiles;

public abstract class BaseMusicBoxItem : ModItem {
    public abstract SoundStyle Music { get; }
    public abstract System.Int32 MusicBoxTileId { get; }

    public override void SetStaticDefaults() {
        ItemID.Sets.CanGetPrefixes[Type] = false;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;
        MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, Music.SoundPath.Remove(7)), Type, Item.createTile);
    }

    public override void SetDefaults() {
        Item.DefaultToMusicBox(MusicBoxTileId, 0);
    }
}