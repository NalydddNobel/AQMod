﻿using Aequus.CrossMod;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus {
    public static partial class Helper {
        public static bool DebugKeyPressed =>
#if DEBUG
            Main.keyState.IsKeyDown(Keys.LeftShift);
#else
            false;
#endif

        #region Files
        public static string SourceFilePath => string.Join(Path.DirectorySeparatorChar, Main.SavePath, "ModSources", "Aequus", "");
        public static string DebugFilePath => string.Join(Path.DirectorySeparatorChar, Main.SavePath, "Mods", "Aequus", "");

        public static FileStream CreateSourceFile(string name) {
            string path = SourceFilePath;
            Directory.CreateDirectory(path);
            return File.Create($"{path}{Path.DirectorySeparatorChar}{name}");
        }
        public static FileStream CreateDebugFile(string name) {
            string path = DebugFilePath;
            Directory.CreateDirectory(path);
            return File.Create($"{path}{Path.DirectorySeparatorChar}{name}");
        }
        public static void OpenDebugFolder() {
            Utils.OpenFolder(DebugFilePath);
        }
        #endregion

        public static List<string> GetListOfActiveDifficulties() {
            List<string> difficultyDetection = new();

            if (Main.LocalPlayer.difficulty == PlayerDifficultyID.Creative) {
                difficultyDetection.Add("Journey Mode");
            }
            if (!Main.masterMode && !Main.expertMode) {
                difficultyDetection.Add("Classic Mode");
            }
            else {
                if (Main.expertMode) {
                    difficultyDetection.Add("Expert Mode");
                }
                if (Main.masterMode) {
                    difficultyDetection.Add("Master Mode");
                }
            }
            if (CalamityMod.Revengeance) {
                difficultyDetection.Add("Revengeance Mode");
            }
            if (CalamityMod.Death) {
                difficultyDetection.Add("Death Mode");
            }
            if (CalamityMod.BossRush) {
                difficultyDetection.Add("Boss Rush in Progress");
            }
            return difficultyDetection;
        }

        public static List<string> GetListOfDrops(List<IItemDropRule> dropTable) {
            var tooltips = new List<string>();
            if (dropTable.Count == 0) {
                return tooltips;
            }

            foreach (var rule in dropTable) {
                var drops = new List<DropRateInfo>();
                rule.ReportDroprates(drops, new DropRateInfoChainFeed(1f));
                tooltips.Add(rule.GetType().FullName + ":");
                foreach (var drop in drops) {
                    string text = "* " + TextHelper.ItemCommand(drop.itemId);
                    if (drop.stackMin == drop.stackMax) {
                        if (drop.stackMin > 1) {
                            text += $" ({drop.stackMin})";
                        }
                    }
                    else {
                        text += $" ({drop.stackMin} - {drop.stackMax})";
                    }
                    text += " " + (int)(drop.dropRate * 10000f) / 100f + "%";
                    tooltips.Add(text);
                    if (drop.conditions != null && drop.conditions.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftControl)) {
                        foreach (var cond in drop.conditions) {
                            if (cond == null) {
                                continue;
                            }

                            string extraDesc = cond.GetConditionDescription();
                            string condText = Main.keyState.IsKeyDown(Keys.LeftShift) ? cond.GetType().FullName : cond.GetType().Name;
                            if (!string.IsNullOrEmpty(extraDesc))
                                condText = $"{condText} '{extraDesc}': {cond.CanDrop(info: new DropAttemptInfo() { IsInSimulation = false, item = -1, npc = Main.npc[0], player = Main.LocalPlayer, rng = Main.rand, IsExpertMode = Main.expertMode, IsMasterMode = Main.masterMode })}";
                            tooltips.Add(condText);
                        }
                    }
                }
            }
            return tooltips;
        }

        internal static void DebugSpawnNPC<T>(Vector2 where) where T : ModNPC {
#if DEBUG
            NPC.NewNPC(null, (int)where.X, (int)where.Y, ModContent.NPCType<T>());
#endif
        }

        public static void DebugTextDraw(string text, Vector2 where, float scale = 1f) {
#if DEBUG
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, where, Color.White, 0f, Vector2.Zero, Vector2.One * scale);
#endif
        }
        public static void DebugTextDraw(object text, Vector2 where, float scale = 1f) {
            DebugTextDraw(text.ToString(), where, scale);
        }

        public static void DebugDustLine(Vector2 start, Vector2 end, int amt, int dustType = 6) {
            var dustTravel = (start - end) / amt;
            for (int i = 0; i < amt; i++) {
                DebugDust(start - dustTravel * i, dustType);
            }
        }

        public static Dust DebugDustDirect(Vector2 where, int dustType = DustID.Torch) {
#if DEBUG
            var d = Dust.NewDustPerfect(where, dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            return d;
#else
            return null;
#endif
        }

        public static void DebugDustColor(Vector2 where, Color color) {
            var dust = DebugDustDirect(where, ModContent.DustType<MonoSparkleDust>());
            if (dust != null) {
                dust.color = color;
            }
        }

        public static void DebugDust(Vector2 where, int dustType = DustID.Torch) {
            DebugDustDirect(where, dustType);
        }

        public static void DebugDustRectangle(Point where, int dustType = DustID.Torch) {
            DebugDustRectangle(where.X, where.Y, dustType);
        }
        public static void DebugDustRectangle(int x, int y, int dustType = DustID.Torch) {
#if DEBUG
            var rect = new Rectangle(x * 16, y * 16, 16, 16);
            for (int i = 0; i < 4; i++) {
                i *= 4;
                var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                i /= 4;
            }
#endif
        }
        public static void DebugDustRectangle(Rectangle rect, int dustType = DustID.Torch) {
#if DEBUG
            int amt = rect.Width / 2;
            for (int i = 0; i < amt; i++) {
                i *= 2;
                var d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + i, rect.Y + rect.Height), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                i /= 2;
            }
            amt = rect.Height / 2;
            for (int i = 0; i < amt; i++) {
                i *= 2;
                var d = Dust.NewDustPerfect(new Vector2(rect.X, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                d = Dust.NewDustPerfect(new Vector2(rect.X + rect.Width, rect.Y + i), dustType);
                d.noGravity = true;
                d.fadeIn = d.scale * 2f;
                d.velocity = Vector2.Zero;
                i /= 2;
            }
#endif
        }
    }
}