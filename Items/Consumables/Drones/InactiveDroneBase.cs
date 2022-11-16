﻿using Aequus.Content.DronePylons;
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
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.useAnimation = 50;
            Item.useTime = 50;
            Item.UseSound = SoundID.Item4;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 4f;
        }

        public override bool? UseItem(Player player)
        {
            int tileX = (int)(player.position.X + player.width / 2f) / 16;
            int tileY = (int)(player.position.Y + player.height / 2f) / 16;
            for (int i = -25; i < 25; i++)
            {
                for (int j = -25; j < 25; j++)
                {
                    int x = tileX + i;
                    int y = tileY + j;
                    if (!WorldGen.InWorld(x, y, 10) && AequusHelpers.IsSectionLoaded(x, y))
                    {
                        continue;
                    }

                    if (DroneWorld.ValidSpot(x, y))
                    {
                        return DroneWorld.FindOrAddDrone(x, y)?.ConsumeSlot<TDroneSlot>(player) == true;
                    }
                }
            }
            return false;
        }
    }
}