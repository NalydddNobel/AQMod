using Aequus.Content.DronePylons;
using Aequus.Projectiles.Misc.Drones;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Drones
{
    public class InactivePylonCleanser : ModItem
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
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 15);
            Item.shootSpeed = 4f;
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

            for (int i = -25; i < 25; i++)
            {
                for (int j = -25; j < 25; j++)
                {
                    int x = tileX + i;
                    int y = tileY + j;
                    if (!WorldGen.InWorld(x, y, 10))
                    {
                        continue;
                    }

                    if (DroneSystem.ValidSpot(x, y))
                    {
                        if (DroneSystem.FindOrAddDrone(x, y)?.AddDrone<CleanserDroneType>() == true)
                        {
                            if (Main.myPlayer == player.whoAmI)
                            {
                                var spawnPosition = player.Center;
                                var mousePosition = Main.MouseWorld;
                                var p = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), spawnPosition,
                                    Vector2.Normalize(mousePosition - spawnPosition) * Item.shootSpeed, ModContent.ProjectileType<CleanserDrone>(), 0, 0f, Main.myPlayer);
                                p.ModProjectile<CleanserDrone>().pylonSpot = new Point(x, y);
                                p.ModProjectile<CleanserDrone>().spawnInAnimation = -1;
                            }
                            return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }
    }
}