namespace Aequus.Tiles
{
    public class TileHooks
    {
        public interface IDontRunVanillaRandomUpdate
        {
        }

        public interface IOnPlaceTile
        {
            bool? OnPlaceTile(int i, int j, bool mute, bool forced, int plr, int style);
        }
    }
}