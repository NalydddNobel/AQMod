using System;
using System.IO;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.Utilities.Debugging
{
    internal static class AutomaticWikiTestStuff
    {
        private static Stream _stream;

        private static Encoding encoding => Encoding.ASCII;

        private static void setupstream(ModItem modItem)
        {
            string path = NCallCommand.DebugFolderPath + Path.DirectorySeparatorChar +
                "WikiItems";
            Directory.CreateDirectory(path);
            _stream = File.Create(path + Path.DirectorySeparatorChar + modItem.Name + ".txt");
        }

        private static byte[] newLineBytes = encoding.GetBytes(Environment.NewLine);

        private static void newline()
        {
            write(newLineBytes);
        }
        private static void write(byte[] buffer)
        {
            _stream.Write(buffer, 0, buffer.Length);
        }
        private static void write(string text)
        {
            write(encoding.GetBytes(text));
        }
        private static void lwrite(string text)
        {
            newline();
            write(encoding.GetBytes(text));
        }

        public static void basicwikipage(ModItem modItem)
        {
            setupstream(modItem);

            write("{{mod sub-page}}");
            lwrite("item name: " + modItem.Name);
            writeitem(modItem.item);
            writeitemrecipe(modItem.item);

            lwrite("{{Aequus/Navbox weapons|show-main=yes}}");

            _stream.Dispose();
        }

        public static void writeitem(Item item)
        {
            lwrite("{{item infobox");
            lwrite("| stack = " + item.stack);
            string type = "";
            if (item.damage > 0)
            {
                lwrite("| damage = " + item.damage);
                writeitemdamagetype(item);
                lwrite("| knockback = " + item.knockBack);
                type = "Weapon";
            }
            item.netID = item.type;
            item.RebuildTooltip();
            if (item.ToolTip != null)
            {
                string tooltip = "";
                for (int k = 0; k < item.ToolTip.Lines; k++)
                {
                    if (k == 0 && item.type >= ItemID.JungleKey && item.type <= ItemID.FrozenKey && !NPC.downedPlantBoss)
                    {
                        tooltip += Lang.tip[59].Value;
                    }
                    else
                    {
                        if (k > 0)
                        {
                            tooltip += "<br/>";
                        }
                        tooltip += item.ToolTip.GetLine(k);
                    }
                }
                lwrite("| tooltip = " + tooltip);
            }
            if (item.useAnimation > 0)
            {
                lwrite("| use = " + item.useAnimation);
            }
            if (item.useAnimation > 0)
            {
                lwrite("| use = " + item.useAnimation);
            }
            if (type != "")
            {
                lwrite("| type = " + type);
            }
            lwrite("| rare = " + item.rare);
            int[] coins = Utils.CoinsSplit(item.value);
            lwrite("| sell = {{value|" + coins[3] + "|" + coins[2] + "|" + coins[1] + "|" + coins[0] + "}}");
            lwrite("}}");
        }

        public static void writeitemdamagetype(Item item)
        {
            if (item.melee)
            {
                lwrite("| damagetype = melee");
            }
            else if (item.ranged)
            {
                lwrite("| damagetype = ranged");
            }
            else if (item.magic)
            {
                lwrite("| damagetype = magic");
            }
            else if (item.summon)
            {
                lwrite("| damagetype = summon");
            }
        }

        public static string recipetile(int type)
        {
            switch (type)
            {
                case TileID.Anvils:
                    return "Anvil";
            }
            return "Unknown";
        }

        public static string itemname(int type)
        {
            if (type > Main.maxItemTypes)
                return "#" + Lang.GetItemName(type).Value;
            return Lang.GetItemName(type).Value;
        }

        public static void writeitemrecipe(Item item)
        {
            var r = new RecipeFinder();
            r.SetResult(item.type);
            var l = r.SearchRecipes();
            if (l.Count > 0)
            {
                lwrite("=== Recipe ===");
                foreach (var rec in l)
                {
                    lwrite("{{crafts top|}}");
                    lwrite("{{crafts row | top=y");

                    if (rec.requiredTile[0] <= 0)
                        lwrite("| tool = " + recipetile(rec.requiredTile[0]));

                    lwrite("| result = " + itemname(rec.createItem.type) + " | " + rec.createItem.stack);

                    for (int i = 0; i < rec.requiredItem.Length; i++)
                    {
                        if (rec.requiredItem[i].type > ItemID.None)
                        {
                            lwrite("| " + itemname(rec.requiredItem[i].type) + " | " + rec.requiredItem[i].stack);
                        }
                    }

                    lwrite("}}");
                    lwrite("{{crafts bottom|}}");
                }
            }
        }
    }
}