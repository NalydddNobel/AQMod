using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content
{
    public sealed class EquivalenceMachineItemManager : GlobalItem
    {
        public byte noGravity;
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public override void UpdateInventory(Item item, Player player)
        {
            noGravity = 0;
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (noGravity > 0)
            {
                item.velocity.Y *= 0.95f;
                gravity = 0f;
                noGravity--;
            }
        }

        public static void AntiGravityNearbyItems(Vector2 position, float distanceMultiplier, byte duration, Player player)
        {
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active && !ItemID.Sets.ItemNoGravity[Main.item[i].type]
                    && Vector2.Distance(Main.item[i].Center, position) < AQItem.GetGrabRange(Main.item[i], player) * distanceMultiplier)
                {
                    Main.item[i].GetGlobalItem<EquivalenceMachineItemManager>().noGravity = duration;
                }
            }
        }
    }
}