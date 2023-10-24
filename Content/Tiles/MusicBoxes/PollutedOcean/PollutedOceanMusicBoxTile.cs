using Aequus.Common.Tiles;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.MusicBoxes.PollutedOcean;

public class PollutedOceanMusicBoxTile : BaseMusicBoxTile {
    public override int MusicBoxItemId => ModContent.ItemType<PollutedOceanMusicBoxItem>();
}