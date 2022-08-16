namespace Aequus
{
    public struct TileKey
    {
        public ushort TileType;
        public int TileStyle;

        public TileKey(ushort type, int style)
        {
            TileType = type;
            TileStyle = style;
        }

        public TileKey(int type, int style) : this((ushort)type, style)
        {
        }

        public TileKey(int type) : this((ushort)type, 0)
        {
        }

        public override int GetHashCode()
        {
            return new { TileType, TileStyle }.GetHashCode();
        }
    }
}