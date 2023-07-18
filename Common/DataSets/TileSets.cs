using Aequus.Items.Weapons.Ranged.Misc.BlockGlove;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public class TileSets : DataSet {
        public static Dictionary<int, BlockGloveProjectileInfo> ProjectileInfo = new();
        public static readonly HashSet<int> PreventsSlopesBelow = new();
        public static int[] TileRenderConversion;

        public override void OnLoad(Mod mod) {
            TileRenderConversion = new int[0];
            ProjectileInfo.AddArrayRange(
                BlockGloveProjectileInfo.FromTileID(TileID.Meteorite, onAI: BlockGloveProjectileInfo.OnAI_FireBlock, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_FireBlock),
                BlockGloveProjectileInfo.FromTileID(TileID.Hellstone, onAI: BlockGloveProjectileInfo.OnAI_FireBlock, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_FireBlock),
                BlockGloveProjectileInfo.FromTileID(TileID.HellstoneBrick, onAI: BlockGloveProjectileInfo.OnAI_FireBlock, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_FireBlock),
                BlockGloveProjectileInfo.FromTileID(TileID.AncientHellstoneBrick, onAI: BlockGloveProjectileInfo.OnAI_FireBlock, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_FireBlock),
                BlockGloveProjectileInfo.FromTileID(TileID.Spikes, modifyHitNPC: BlockGloveProjectileInfo.ModifyHitNPC_Spikes, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_Spikes),
                BlockGloveProjectileInfo.FromTileID(TileID.WoodenSpikes, modifyHitNPC: BlockGloveProjectileInfo.ModifyHitNPC_Spikes, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_Spikes),
                BlockGloveProjectileInfo.FromTileID(TileID.PoopBlock, onHitNPC: BlockGloveProjectileInfo.OnHitNPC_Poo)
            );
        }

        public override void PostSetupContent() {
            Array.Resize(ref TileRenderConversion, TileLoader.TileCount);
        }

        public static void AddTileRenderConversion(int tileType, int tileConversion) {
            if (tileType >= TileRenderConversion.Length) {
                Array.Resize(ref TileRenderConversion, tileType + 1);
            }
            TileRenderConversion[tileType] = tileConversion;
        }
    }
}