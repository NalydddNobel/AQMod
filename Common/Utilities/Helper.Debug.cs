using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus
{
    public static partial class Helper
    {
        public static bool DebugKeyPressed => Main.keyState.IsKeyDown(Keys.LeftShift);

        #region Files
        public static string SourceFilePath => string.Join(Path.DirectorySeparatorChar, Main.SavePath, "ModSources", "Aequus", "");
        public static string DebugFilePath => string.Join(Path.DirectorySeparatorChar, Main.SavePath, "Mods", "Aequus", "");

        public static FileStream CreateSourceFile(string name)
        {
            string path = SourceFilePath;
            Directory.CreateDirectory(path);
            return File.Create($"{path}{Path.DirectorySeparatorChar}{name}");
        }
        public static FileStream CreateDebugFile(string name)
        {
            string path = DebugFilePath;
            Directory.CreateDirectory(path);
            return File.Create($"{path}{Path.DirectorySeparatorChar}{name}");
        }
        public static void OpenDebugFolder()
        {
            Utils.OpenFolder(DebugFilePath);
        }
        #endregion

        public static List<string> GetListOfDrops(List<IItemDropRule> dropTable)
        {
            var tooltips = new List<string>();
            if (dropTable.Count == 0)
            {
                return tooltips;
            }

            foreach (var rule in dropTable)
            {
                var drops = new List<DropRateInfo>();
                rule.ReportDroprates(drops, new DropRateInfoChainFeed(1f));
                tooltips.Add(rule.GetType().FullName + ":");
                foreach (var drop in drops)
                {
                    string text = "* " + TextHelper.ItemCommand(drop.itemId);
                    if (drop.stackMin == drop.stackMax)
                    {
                        if (drop.stackMin > 1)
                        {
                            text += $" ({drop.stackMin})";
                        }
                    }
                    else
                    {
                        text += $" ({drop.stackMin} - {drop.stackMax})";
                    }
                    text += " " + (int)(drop.dropRate * 10000f) / 100f + "%";
                    tooltips.Add(text);
                    if (drop.conditions != null && drop.conditions.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftControl))
                    {
                        foreach (var cond in drop.conditions)
                        {
                            if (cond == null)
                            {
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

        internal static void DebugSpawnNPC<T>(Vector2 where) where T : ModNPC
        {
            NPC.NewNPC(null, (int)where.X, (int)where.Y, ModContent.NPCType<T>());
        }

        public static void DebugTextDraw(string text, Vector2 where)
        {
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, where, Color.White, 0f, Vector2.Zero, Vector2.One);
        }

        public static Dust DebugDustDirect(Vector2 where, int dustType = DustID.Torch)
        {
            var d = Dust.NewDustPerfect(where, dustType);
            d.noGravity = true;
            d.fadeIn = d.scale * 2f;
            d.velocity = Vector2.Zero;
            return d;
        }
        public static void DebugDustColor(Vector2 where, Color color)
        {
            var d = DebugDustDirect(where, ModContent.DustType<MonoSparkleDust>());
            d.color = color;
        }
        public static void DebugDust(Vector2 where, int dustType = DustID.Torch)
        {
            DebugDustDirect(where, dustType);
        }
        public static void DebugDust(Point where, int dustType = DustID.Torch)
        {
            DebugDust(where.X, where.Y, dustType);
        }
        public static void DebugDust(int x, int y, int dustType = DustID.Torch)
        {
            var rect = new Rectangle(x * 16, y * 16, 16, 16);
            for (int i = 0; i < 4; i++)
            {
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
        }
        public static void DebugDust(Rectangle rect, int dustType = DustID.Torch)
        {
            int amt = rect.Width / 2;
            for (int i = 0; i < amt; i++)
            {
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
            for (int i = 0; i < amt; i++)
            {
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
        }

        [Obsolete("Only used to help port to 1.4.4 easier.")]
        public static void PrepareDrawnEntityDrawing(this Main instance, Entity entity, int intendedShader, Matrix? bleh)
        {
            instance.PrepareDrawnEntityDrawing(entity, intendedShader);
        }
    }
}