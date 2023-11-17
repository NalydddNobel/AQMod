using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public partial class TrashCompactorSystem {
    public class AnimationItemSpew {
        public readonly Vector2 Location;
        public readonly Point TileLocation;
        public readonly Point TileOrigin;
        public readonly int ItemId;
        public float AnimationTime;
        public bool SpawnedParticles;

        public AnimationItemSpew(Vector2 location, Point tileOrigin, int itemId) {
            Location = location;
            TileOrigin = tileOrigin;
            TileLocation = location.ToTileCoordinates();
            ItemId = itemId;
        }
    }
}