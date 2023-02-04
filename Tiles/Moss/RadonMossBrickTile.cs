using Terraria.ID;

namespace Aequus.Tiles.Moss
{
    public class RadonMossBrickTile : RadonMossTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.tileMossBrick[Type] = true;
            base.SetStaticDefaults();
            ItemDrop = ItemID.GrayBrick;
        }

        public override bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style)
        {
            return null;
        }
    }
}