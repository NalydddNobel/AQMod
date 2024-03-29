﻿using Aequus.Common.CrossMod;
using Aequus.Common.DataSets;
using Aequus.Content.CursorDyes.Items;
using Aequus.Items.Equipment.Accessories.Misc;
using Aequus.Items.Equipment.PetsUtility.Miner;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee.Swords.CrystalDagger;
using Aequus.Items.Weapons.Necromancy.Candles;
using Aequus.Items.Weapons.Ranged.Misc.Slingshot;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.CrossMod {
    internal class CerebralMod : ModSupport<CerebralMod> {
        private void AddCrafterRecipe(string crafterName, int tile = TileID.Anvils, params int[] items) {
            try {
                if (Instance.TryFind<ModItem>(crafterName, out var crafter) && crafter.Type > 0) {
                    foreach (var item in items) {
                        Recipe.Create(item)
                            .AddIngredient(crafter.Type)
                            .AddTile(tile)
                            .Register();
                    }
                }
            }
            catch (Exception ex) {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void AddRecipes() {
            if (Instance == null) {
                return;
            }

            AddCrafterRecipe("GoldenChestCrafter", TileID.Anvils,
                ModContent.ItemType<Bellows>(),
                ModContent.ItemType<GlowCore>(),
                ModContent.ItemType<SwordCursor>(),
                ModContent.ItemType<MiningPetSpawner>());

            AddCrafterRecipe("SkywareChestCrafter", TileID.Anvils,
                ModContent.ItemType<Slingshot>());
#if DEBUG
            AddCrafterRecipe("ShadowOrbCrafter", TileID.Anvils,
                ModContent.ItemType<CorruptionCandle>());

            AddCrafterRecipe("CrimsonHeartCrafter", TileID.Anvils,
                ModContent.ItemType<CrimsonCandle>());
#endif
            AddCrafterRecipe("DungeonChestCrafter", TileID.Anvils,
                ChestLootDataset.AequusDungeonChestLoot.ToArray());
        }
    }
}