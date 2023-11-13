using Aequus.Common.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

[LegacyName("SedimentaryRockWallWall", "SedimentaryRockWallPlaced")]
public class SedimentaryRockWall : AequusModWall {
    public override bool Friendly => false;

    internal override int DustId => 209;

    internal override Color MapEntry => new(70, 40, 20);
}