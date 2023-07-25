using Aequus.Items.Materials.PearlShards;
using Aequus.Items.Weapons.Ranged.Misc.BlockGlove;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.DataSets {
    public class TileSets : DataSet {
        public static Dictionary<int, bool> CraftableTileLookup { get; private set; }
        public static List<PearlsTile> OceanPearlsToGenerate = new();
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

        public static bool IsTileIDCraftable(int tileID) {
            if (CraftableTileLookup.TryGetValue(tileID, out var val)) {
                return val;
            }

            foreach (var rec in Main.recipe.Where((r) => r != null && !r.Disabled && r.createItem != null && r.createItem.createTile == tileID && r.createItem.consumable && (r.requiredItem.Count > 1 || !r.HasCondition(Condition.NearWater) && !r.HasCondition(Condition.NearLava) && !r.HasCondition(Condition.NearHoney) && !r.HasCondition(Condition.NearShimmer) && r.requiredItem[0].createWall <= WallID.None))) {
                foreach (var i in rec.requiredItem) {
                    foreach (var rec2 in Main.recipe.Where((r) => r != null && !r.Disabled && r.createItem != null && r.createItem.type == i.type)) {
                        foreach (var i2 in rec2.requiredItem) {
                            if (i2.type == rec.createItem.type) {
                                goto Continue;
                            }
                        }
                    }
                }
                CraftableTileLookup.Add(tileID, true);
                return true;

            Continue:
                continue;
            }
            CraftableTileLookup.Add(tileID, false);
            return false;
        }
    }
}