using Aequus.Buffs;
using Aequus.Buffs.Misc.Empowered;
using Aequus.Common;
using Aequus.Common.Buffs;
using Aequus.Common.ModPlayers;
using Aequus.Content;
using Aequus.Content.ItemPrefixes.Potions;
using Aequus.Items.Potions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions {
    public class VeinminerPotion : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(234, 0, 83, 0), new Color(162, 0, 80, 0), };
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.MiningPotion);
            Item.buffType = ModContent.BuffType<VeinminerBuff>();
            Item.buffTime = 7200;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.SpecularFish)
                .AddIngredient(ItemID.Blinkroot)
                .AddIngredient(ItemID.Waterleaf)
                .AddIngredient(ItemID.SilverOre)
                .AddTile(TileID.Bottles)
                .TryRegisterAfter(ItemID.MiningPotion);

            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient(ItemID.SpecularFish)
                .AddIngredient(ItemID.Blinkroot)
                .AddIngredient(ItemID.Waterleaf)
                .AddIngredient(ItemID.TungstenOre)
                .AddTile(TileID.Bottles)
                .TryRegisterAfter(ItemID.MiningPotion);
        }
    }
}

namespace Aequus.Buffs {
    public class VeinminerBuff : ModBuff {
        public override void SetStaticDefaults() {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(85, 195, 160));
            AequusBuff.AddPotionConflict(Type, BuffID.Mining);
        }

        public override void Update(Player player, ref int buffIndex) {
            var aequus = player.Aequus();
            aequus.veinminerAbility = Math.Max(aequus.veinminerAbility, 1);
        }
    }

    public class VeinminerBuffEmpowered : EmpoweredBuffBase {
        public override int OriginalBuffType => ModContent.BuffType<VeinminerBuff>();

        public override void SetStaticDefaults() {
            EmpoweredPrefix.ItemToEmpoweredBuff.Add(ModContent.ItemType<VeinminerPotion>(), Type);
        }

        public override void Update(Player player, ref int buffIndex) {
            base.Update(player, ref buffIndex);
            var aequus = player.Aequus();
            aequus.extraOresChance *= 0.9f;
            aequus.veinminerAbility = Math.Max(aequus.veinminerAbility, 2);
        }
    }
}

namespace Aequus.Common {
    public struct VeinmineTask {
        /// <summary>
        /// 20
        /// </summary>
        public static int MaxCanMineAtATime = 20;

        public List<Point> addPoints;
        public List<Point> workingPoints;
        public int tileID;
        public int delay;
        public int delayMax;

        public VeinmineTask() {
            addPoints = new();
            workingPoints = new();
            tileID = 0;
            delay = 0;
            delayMax = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DoScan(int x, int y, int altX, int altY) {
            if (WorldGen.InWorld(x, y, fluff: 10) && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == tileID) {
                addPoints.Add(new(x, y));
            }
            else if (WorldGen.InWorld(altX, altY, fluff: 10) && Main.tile[altX, altY].HasTile && Main.tile[altX, altY].TileType == tileID) {
                addPoints.Add(new(altX, altY));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Scan(Player player, int x, int y) {
            DoScan(x, y - 1,
                x - 1, y - 1);
            DoScan(x, y + 1,
                x + 1, y + 1);
            DoScan(x - 1, y,
                x - 1, y + 1);
            DoScan(x + 1, y,
                x + 1, y - 1);
        }

        public void Update(Player player) {
            if (workingPoints.Count == 0) {
                delay = 0;
                delayMax = 0;
                tileID = -1;
                return;
            }

            if (--delay > 0) {
                return;
            }
            bool scanOnly = delay < -1;
            delay = delayMax;

            addPoints.Clear();

            var pickaxe = player.GetBestPickaxe();
            int pickPower = pickaxe?.pick ?? 35;
            int count = Math.Min(workingPoints.Count, MaxCanMineAtATime);
            if (scanOnly) {
                for (int i = 0; i < count; i++) {
                    Scan(player, workingPoints[i].X, workingPoints[i].Y);
                }
            }
            else {
                for (int i = 0; i < count; i++) {
                    Point p = workingPoints[i];
                    if (!Main.tile[p].HasTile) {
                        Scan(player, p.X, p.Y);
                    }
                    else {
                        player.PickTile(p.X, p.Y, pickPower);
                        int index = player.hitTile.HitObject(p.X, p.Y, 1);
                        var hitTile = player.hitTile.data[index];
                        if (hitTile.damage > 0 || !Main.tile[p].HasTile) {
                            addPoints.Add(p);
                        }
                    }
                }
            }
            if (count >= MaxCanMineAtATime) {
                for (int i = count; i < workingPoints.Count; i++) {
                    addPoints.Add(workingPoints[i]);
                }
            }

            var distinctAddPoints = addPoints.Distinct();
            workingPoints.Clear();
            foreach (var point in distinctAddPoints) {
                workingPoints.Add(point);
            }
        }

        public void Activate(Player player, int x, int y, int tileID, int delay) {
            this.delay = -1;
            this.tileID = tileID;
            workingPoints.Clear();
            workingPoints.Add(new(x, y));
            delayMax = delay;
            Update(player);
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public StatChance extraOresChance;
        public int veinminerAbility;
        public VeinmineTask veinmineTask;

        public void ProcVeinminer(int x, int y, int type) {
            if (veinminerAbility > 0 && veinmineTask.workingPoints.Count == 0) {
                if (ModContent.GetInstance<AequusTile>().VeinmineCondition.TryGetValue(type, out var condition) &&
                    !condition(x, y, type, Player, Player.GetBestPickaxe()?.pick ?? 35)) {
                    return;
                }
                veinmineTask.Activate(Player, x, y, type, 5 / veinminerAbility);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PostUpdate_Veinminer() {
            if (veinminerAbility == 0) {
                veinmineTask.workingPoints.Clear();
                veinmineTask.addPoints.Clear();
                return;
            }

            veinmineTask.Update(Player);
        }
    }

    public partial class AequusTile {
        /// <summary>
        /// 1) int | x
        /// <para>2) int | y</para>
        /// <para>3) int | type</para>
        /// <para>4) Player | player</para>
        /// <para>5) int | pickaxe power</para>
        /// <para>returns: Whether the block can be veinmined.</para>
        /// </summary>
        public Dictionary<int, Func<int, int, int, Player, int, bool>> VeinmineCondition { get; private set; } = new();

        public void Load_Veinminer() {
            VeinmineCondition.Add(TileID.Meteorite, (x, y, type, player, pickaxePower) => Main.hardMode);
            VeinmineCondition.Add(TileID.Hellstone, (x, y, type, player, pickaxePower) => Main.hardMode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ProcVeinminer(int i, int j, int type, Player player, AequusPlayer aequus) {
            aequus.ProcVeinminer(i, j, type);
        }
    }
}