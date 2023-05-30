using System;
using Terraria;

namespace Aequus.Items {
    public partial class AequusItem {
        /// <summary>
        /// How long the item should float for. Set to 255 for permanent duration.
        /// </summary>
        public byte itemGravityCheck = 0;
        public float itemGravityMultiplier = 1f;

        public void SetNoGrav(Item item, byte duration) {
            var aequus = item.Aequus();
            if (aequus.itemGravityCheck > Math.Max(duration - 15, 15)) {
                return;
            }
            CheckNoGravity(item);
            aequus.itemGravityCheck = duration;
        }

        private void CheckNoGravity(Item item) {
            itemGravityMultiplier = Helper.IsShimmerBelow(item.Center.ToTileCoordinates(), 12) ? 1f : 0f;
        }

        private void OnSpawn_CheckGravity(Entity parent) {
            if (parent is not NPC npc || Helper.FindFloor(parent.Center, 20) != -1) {
                return;
            }

            itemGravityCheck = (byte)(npc.Aequus().noGravityDrops ? 255 : 0);
        }

        private void Update_CheckGravity(Item item, ref float gravity, ref float maxFallSpeed) {
            if (gravity <= 0f || itemGravityCheck == 0) {
                return;
            }

            if (itemGravityCheck < 255) {
                itemGravityCheck--;
            }
            if (itemGravityCheck == 0 || (itemGravityCheck == 255 && item.timeSinceItemSpawned % 40 == 0)) {
                CheckNoGravity(item);
            }
            item.velocity.Y *= 0.95f + 0.05f * itemGravityMultiplier;
            gravity *= itemGravityMultiplier;
        }
    }
}