using Aequus.Content.DronePylons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class GunnerDronePack : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 2;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.consumable = true;
            Item.maxStack = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.UseSound = SoundID.Item4;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 25);
        }

        public override bool? UseItem(Player player)
        {
            int tileX = (int)(player.position.X + player.width / 2f) / 16;
            int tileY = (int)(player.position.Y + player.height / 2f) / 16;

            bool closeToAPylon = false;

            foreach (var p in DroneSystem.Drones)
            {
                if (Vector2.Distance(p.Key.ToWorldCoordinates(), player.Center) < 1000f)
                {
                    if (closeToAPylon)
                    {
                        return true;
                    }
                    closeToAPylon = true;
                }
            }

            for (int i = -10; i < 10; i++)
            {
                for (int j = -10; j < 10; j++)
                {
                    int x = tileX + i;
                    int y = tileY + j;
                    if (!WorldGen.InWorld(x, y, 10))
                    {
                        continue;
                    }

                    if (DroneSystem.ValidSpot(x, y))
                    {
                        return DroneSystem.FindOrAddDrone(x, y)?.AddDrone<GunnerDroneType>();
                    }
                }
            }
            return true;
        }
    }
}