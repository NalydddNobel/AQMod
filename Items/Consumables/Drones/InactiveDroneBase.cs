using Aequus.Content.DronePylons;
using Aequus.Projectiles.Misc.Drones;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Drones
{
    public abstract class InactiveDroneBase<TDroneSlot> : ModItem where TDroneSlot : DroneSlot
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
            Item.shootSpeed = 4f;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                return false;
            }
            int tileX = (int)(player.position.X + player.width / 2f) / 16;
            int tileY = (int)(player.position.Y + player.height / 2f) / 16;
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

                    if (DroneWorld.ValidSpot(x, y))
                    {
                        if (DroneWorld.FindOrAddDrone(x, y)?.ConsumeSlot<TDroneSlot>(player) == true)
                        {
                            var spawnPosition = player.Center;
                            var mousePosition = Main.MouseWorld;
                            var p = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), spawnPosition,
                                Vector2.Normalize(mousePosition - spawnPosition) * Item.shootSpeed, ModContent.GetInstance<TDroneSlot>().ProjectileType, 0, 0f, Main.myPlayer);
                            p.ModProjectile<TownDroneBase>().pylonSpot = new Point(x, y);
                            p.ModProjectile<TownDroneBase>().spawnInAnimation = -1;
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