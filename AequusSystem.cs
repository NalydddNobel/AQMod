using Aequus.Buffs;
using Aequus.Common.Utilities;
using Aequus.Items;
using Aequus.Items.Accessories.Combat.Passive;
using Aequus.Items.Tools.Building;
using Aequus.NPCs;
using Aequus.Projectiles;
using Aequus.Tiles.Blocks;
using Aequus.UI;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus {
    public partial class AequusSystem : ModSystem
    {
        /// <summary>
        /// Caches <see cref="Main.invasionSize"/>
        /// </summary>
        public static RefFuncCacher<int> Main_invasionSize { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.invasionType"/>
        /// </summary>
        public static RefFuncCacher<int> Main_invasionType { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.bloodMoon"/>
        /// </summary>
        public static RefFuncCacher<bool> Main_bloodMoon { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.eclipse"/>
        /// </summary>
        public static RefFuncCacher<bool> Main_eclipse { get; private set; }
        /// <summary>
        /// Caches <see cref="Main.dayTime"/>
        /// </summary>
        public static RefFuncCacher<bool> Main_dayTime { get; private set; }

        public static FieldInfo Field_Main_swapMusic { get; private set; }

        public override void Load()
        {
            Field_Main_swapMusic = typeof(Main).GetField("swapMusic", BindingFlags.NonPublic | BindingFlags.Static);
            Main_invasionSize = new RefFuncCacher<int>(() => ref Main.invasionSize);
            Main_invasionType = new RefFuncCacher<int>(() => ref Main.invasionType);
            Main_bloodMoon = new RefFuncCacher<bool>(() => ref Main.bloodMoon);
            Main_eclipse = new RefFuncCacher<bool>(() => ref Main.eclipse);
            Main_dayTime = new RefFuncCacher<bool>(() => ref Main.dayTime);
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
            AequusNPC.doLuckyDropsEffect = false;
            AequusUI.CurrentItemSlot = new AequusUI.ItemSlotContext();
            if (Main_invasionSize.IsCaching)
            {
                Main_invasionSize.ResetValue();
            }
            if (Main_invasionType.IsCaching)
            {
                Main_invasionType.ResetValue();
            }
            if (Main_bloodMoon.IsCaching)
            {
                Main_bloodMoon.ResetValue();
            }
            if (Main_eclipse.IsCaching)
            {
                Main_eclipse.ResetValue();
            }
            if (Main_dayTime.IsCaching)
            {
                Main_dayTime.ResetValue();
            }
        }

        public override void PreUpdateEntities()
        {
            PreUpdateEntities_CheckCrabson();
            AequusTile.UpdateIndestructibles();
            CelesteTorus.ClearDrawData();
            ResetCaches();
            AequusItem.CheckItemGravity();
            AequusItem.CheckItemAbsorber();
        }

        public override void PreUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                AdvancedRulerInterface.Instance.Enabled = false;
                AdvancedRulerInterface.Instance.Holding = false;
                ModContent.GetInstance<OmniPaintUI>().Enabled = false;
                ChestLensInterface.Enabled = false;
            }
            AequusBuff.preventRightClick.Clear();
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = false;
        }

        public override void PostUpdatePlayers()
        {
            ResetCaches();
        }

        public override void PostUpdateItems()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.LocalPlayer.Aequus().CheckThirsts(); // To fix the UI bugging out a lot
            }
        }

        public override void PostUpdateTime()
        {
            Main.tileSolid[ModContent.TileType<EmancipationGrillTile>()] = true;
        }
    }
}