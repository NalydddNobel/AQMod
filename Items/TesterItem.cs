using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Content.Necromancy;
using Aequus.Content.StatSheet;
using Aequus.Content.WorldGeneration;
using Aequus.Items.Weapons.Magic;
using Aequus.NPCs.Monsters.Underworld;
using Aequus.Tiles;
using Hjson;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace Aequus.Items
{
    internal class TesterItem : ModItem
    {
        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.width = 20;
            Item.height = 20;
        }

        public override bool? UseItem(Player player)
        {
            int x = AequusHelpers.tileX;
            int y = AequusHelpers.tileY;
            var f = AequusHelpers.CreateDebugFile("SilkTouchBlocks.txt");
            f.WriteText("[Silk Touch Blocks Database]");
            foreach (var i in AequusTile.TileIDToItemID)
            {
                f.WriteText($"\n[Tile: {i.Key} | {TileID.Search.GetName(i.Key.TileType)}{(i.Key.TileStyle > 0 ? $"/{i.Key.TileStyle}" : "")}, Item: {i.Value} | {Lang.GetItemName(i.Value)}");
            }
            AequusHelpers.OpenDebugFolder();
            return true;
        }

        private static void SpawnPhysicalTestDummies(int npciD)
        {
            var m = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates();
            for (int i = 0; i < 5; i++)
            {
                var n = NPC.NewNPCDirect(null, m + new Vector2(90f * (i - 2), 0f), npciD);
                n.Aequus().noAITest = true;
                n.knockBackResist = 0f;
            }
        }

        private static void SpawnGlimmer()
        {
            GlimmerBiome.TileLocation = Point.Zero;
            GlimmerSystem.BeginEvent();
        }

        private static void KillOfType(int npcID)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == npcID)
                {
                    Main.npc[i].StrikeNPCNoInteraction(99999, 0f, 0, crit: true);
                }
            }
        }
        private static NPC NoAINPC(int npcID)
        {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld, npcID);
            n.Aequus().noAITest = true;
            return n;
        }

        private static void NoAITrapperImp()
        {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld.NumFloor(4), ModContent.NPCType<TrapperImp>());
            n.Aequus().noAITest = true;

            foreach (var v in AequusHelpers.CircularVector(3, -MathHelper.PiOver2))
            {
                var n2 = NPC.NewNPCDirect(n.GetSource_FromAI(), (Main.MouseWorld + v * 125f).NumFloor(4), ModContent.NPCType<Trapper>(), n.whoAmI, ai1: n.whoAmI + 1f);
                n2.Aequus().noAITest = true;
                n2.rotation = v.ToRotation() + MathHelper.PiOver2;
            }
        }

        private static void WriteNPCsInHashSet(HashSet<int> hash)
        {
            foreach (var item in hash)
            {
                Main.NewText(Lang.GetNPCNameValue(item));
            }
        }

        private static void WriteToFileNecromancyWiki()
        {
            var c = Path.DirectorySeparatorChar;
            var path = $"{Main.SavePath}{c}Mods{c}AequusWiki";
            Directory.CreateDirectory(path);
            path += $"{c}NecromancyTiers.txt";
            using (var stream = File.Create(path))
            {
                var l = new List<(int, string, GhostInfo)>();
                foreach (var val in NecromancyDatabase.NPCs)
                {
                    l.Add((val.Key, Lang.GetNPCNameValue(val.Key), val.Value));
                }
                var l2 = l;
                l = new List<(int, string, GhostInfo)>();
                foreach (var val in l2)
                {
                    if (l.FindIndex((a) => a.Item2 == val.Item2) != -1)
                    {
                        continue;
                    }
                    l.Add(val);
                }
                l.Sort((a, b) => a.Item3.PowerNeeded.CompareTo(b.Item3.PowerNeeded));
                var d = new Dictionary<float, List<(int, string, GhostInfo)>>();
                foreach (var val in l)
                {
                    if (!d.ContainsKey(val.Item3.PowerNeeded))
                    {
                        d[val.Item3.PowerNeeded] = new List<(int, string, GhostInfo)>();
                    }
                    d[val.Item3.PowerNeeded].Add(val);
                }

                foreach (var list in d)
                {
                    list.Value.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                    stream.WriteText($"== Tier {list.Key} ==\n");
                    stream.WriteText("{{infocard|class=terraria compact|text=\n");
                    stream.WriteText("{{itemlist|width=18em|class=terraria\n");
                    foreach (var val in list.Value)
                    {
                        stream.WriteText("| {{item|" + (val.Item1 >= Main.maxNPCTypes ? "#" : "") + val.Item2 + "}}\n");
                    }
                    stream.WriteText("}}\n");
                    stream.WriteText("\n");
                }

                Utils.OpenFolder(path);
            }
        }

        private static void WriteStatSheetInfoTest()
        {
            var clr = Color.Red.HueAdd(Main.rand.NextFloat(1f));
            foreach (var s in StatSheetManager.RegisteredStats)
            {
                Main.NewText($"{Language.GetTextValue(s.DisplayName)}: {s.ProvideStatText()}", Color.Lerp(clr, Color.White, 0.75f));
                clr = clr.HueAdd(Main.rand.NextFloat(0.1f));
            }
        }
    }
}