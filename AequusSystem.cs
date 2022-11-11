using Aequus.Common.Utilities;
using Aequus.Items.Accessories;
using Aequus.Projectiles;
using Aequus.Tiles;
using Aequus.Tiles.Furniture;
using Aequus.Tiles.PhysicistBlocks;
using Aequus.UI;
using Microsoft.Xna.Framework;
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

        public override void Load()
        {
            Field_Main_swapMusic = typeof(Main).GetField("swapMusic", BindingFlags.NonPublic | BindingFlags.Static);
            Main_invasionSize = new ValueCache<int>(() => ref Main.invasionSize);
            Main_invasionType = new ValueCache<int>(() => ref Main.invasionType);
            Main_bloodMoon = new ValueCache<bool>(() => ref Main.bloodMoon);
            Main_eclipse = new ValueCache<bool>(() => ref Main.eclipse);
            Main_dayTime = new ValueCache<bool>(() => ref Main.dayTime);
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            base.ModifySunLightColor(ref tileColor, ref backgroundColor);
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
            if (Field_Main_swapMusic != null)
            {
                Aequus.otherworldMusic = (bool)Field_Main_swapMusic.GetValue(null);
            }
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
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Enabled = false;
                AdvancedRulerInterface.Instance.Holding = false;
                OmniPaintUI.Instance.Enabled = false;
                ChestLensInterface.Enabled = false;
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