using Aequus.Common.Utilities;
using Aequus.Items.Accessories;
using Aequus.Projectiles;
using Aequus.Tiles;
using Aequus.Tiles.Furniture;
using Aequus.Tiles.PhysicistBlocks;
using Aequus.UI;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus
{
    public class AequusSystem : ModSystem
    {
        /// <summary>
        /// Caches <see cref="Main.invasionSize"/>
        /// </summary>
        public static ValueCache<int> Main_invasionSize { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.invasionType"/>
        /// </summary>
        public static ValueCache<int> Main_invasionType { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.bloodMoon"/>
        /// </summary>
        public static ValueCache<bool> Main_bloodMoon { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.eclipse"/>
        /// </summary>
        public static ValueCache<bool> Main_eclipse { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.dayTime"/>
        /// </summary>
        public static ValueCache<bool> Main_dayTime { get; private set; }

        public static FieldInfo Field_Main_swapMusic { get; private set; }

        public static int ReversedGravityCheck;

        public override void Load()
        {
            Field_Main_swapMusic = typeof(Main).GetField("swapMusic", BindingFlags.NonPublic | BindingFlags.Static);
            Main_invasionSize = new ValueCache<int>(() => ref Main.invasionSize);
            Main_invasionType = new ValueCache<int>(() => ref Main.invasionType);
            Main_bloodMoon = new ValueCache<bool>(() => ref Main.bloodMoon);
            Main_eclipse = new ValueCache<bool>(() => ref Main.eclipse);
            Main_dayTime = new ValueCache<bool>(() => ref Main.dayTime);
        }

        public override void Unload()
        {
            Main_invasionSize = null;
            Main_invasionType = null;
            Main_bloodMoon = null;
            Main_eclipse = null;
            Main_dayTime = null;
        }

        public void ResetCaches()
        {
            AequusProjectile.pWhoAmI = -1;
            AequusProjectile.pIdentity = -1;
            AequusProjectile.pNPC = -1;
            AequusPlayer.PlayerContext = -1;
            AequusUI.CurrentItemSlot = new AequusUI.ItemSlotContext();
            if (Main_invasionSize.IsCaching)
            {
                Main_invasionSize.EndCaching();
            }
            if (Main_invasionType.IsCaching)
            {
                Main_invasionType.EndCaching();
            }
            if (Main_bloodMoon.IsCaching)
            {
                Main_bloodMoon.EndCaching();
            }
            if (Main_eclipse.IsCaching)
            {
                Main_eclipse.EndCaching();
            }
            if (Main_dayTime.IsCaching)
            {
                Main_dayTime.EndCaching();
            }
        }

        public override void PreUpdateEntities()
        {
            AequusTile.UpdateIndestructibles();
            CelesteTorus.RenderPoints?.Clear();
            ArmFloaties.EquippedCache?.Clear();
            ResetCaches();

            ReversedGravityCheck--;
            if (ReversedGravityCheck <= 0)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                for (int i = 0; i < Main.maxItems; i++)
                {
                    if (Main.item[i].active && !ItemID.Sets.ItemNoGravity[Main.item[i].type])
                    {
                        Main.item[i].Aequus().CheckGravityTiles(Main.item[i]);
                    }
                }
                stopWatch.Stop();
                ReversedGravityCheck = Math.Min((int)stopWatch.ElapsedMilliseconds, 30);
            }
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Enabled = false;
                AdvancedRulerInterface.Instance.Holding = false;
                OmniPaintUI.Instance.Enabled = false;
                ChestLensInterface.Enabled = false;
                NecromancyInterface.SelectedGhost = -1;
            }
            if (StariteBottleTile.blessedPlayerDelay > 0)
                StariteBottleTile.blessedPlayerDelay--;
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = false;
        }

        public override void PostUpdatePlayers()
        {
            ResetCaches();
        }

        public override void PostUpdateTime()
        {
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
        }
    }
}