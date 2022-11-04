using Aequus.Content.Necromancy;
using Aequus.Content.StatSheet;
using Aequus.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

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
                    //stream.WriteText("{{infocard|class=terraria compact|text=\n");
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
            //var clr = Color.Red.HueAdd(Main.rand.NextFloat(1f));
            //foreach (var s in StatSheetManager.RegisteredStats)
            //{
            //    Main.NewText($"{Language.GetTextValue(s.DisplayName)}: {s.ProvideStatText()}", Color.Lerp(clr, Color.White, 0.75f));
            //    clr = clr.HueAdd(Main.rand.NextFloat(0.1f));
            //}
            return true;
        }
    }
}