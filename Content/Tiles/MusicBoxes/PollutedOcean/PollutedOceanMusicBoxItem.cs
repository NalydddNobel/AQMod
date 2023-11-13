using Aequus.Common.Items.Components;
using Aequus.Common.Tiles;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.MusicBoxes.PollutedOcean;

public class PollutedOceanMusicBoxItem : BaseMusicBoxItem, IOnlineLink {
    public string Link => "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    public override SoundStyle Music => AequusSounds.PollutedOcean;
    public override int MusicBoxTileId => ModContent.TileType<PollutedOceanMusicBoxTile>();
}