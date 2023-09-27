using Aequus.Common.NPCs;
using Aequus.Core.Utilities;
using System;
using Terraria;

namespace Aequus {
    public partial class AequusItem {
        /// <summary>
        /// How long the item should float for. Set to 255 for permanent duration.
        /// </summary>
        public byte itemGravityCheck = 0;
        public float itemGravityMultiplier = 1f;

        public void SetNoGrav(Item item, byte duration) {
            var aequus = item.GetGlobalItem<AequusItem>();
            if (aequus.itemGravityCheck > Math.Max(duration - 15, 15)) {
                return;
            }
            CheckNoGravity(item);
            aequus.itemGravityCheck = duration;
        }

        private void CheckNoGravity(Item item) {
            itemGravityMultiplier = TileHelper.IsShimmerBelow(item.Center.ToTileCoordinates(), 12) ? 1f : 0f;
        }

        private void CheckNoGravity(Entity parent) {
            //if (parent is not NPC npc || !npc.GetGlobalNPC<AequusNPC>().noGravityDrops || Helper.FindFloor(parent.Center, 20) != -1) {
            //    return;
            //}

            //itemGravityCheck = 255;
        }

        private void UpdateZeroGravity(Item item, ref float gravity) {
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