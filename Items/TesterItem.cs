using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Content.Necromancy;
using Aequus.Content.StatSheet;
using Aequus.Graphics;
using Aequus.Items.Weapons.Magic;
using Aequus.NPCs.Monsters.Underworld;
using Aequus.Tiles;
using Aequus.Tiles.Ambience;
using Aequus.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items
{
    internal class TesterItem : ModItem
    {
        public const bool LoadMe = true;

        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override bool IsLoadingEnabled(Mod mod)
        {
            return LoadMe;
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
            PlacePollenExamples(x, y);
            return true;
        }

        public static void PlacePollenExamples(int x, int y)
        {
            WorldGen.PlaceTile(x, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x, y - 1, ModContent.TileType<MorayTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y - 1, ModContent.TileType<MorayTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x + 4, y - 1, ModContent.TileType<MorayTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x, y + 2, ModContent.TileType<ManacleTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x + 2, y + 2, ModContent.TileType<ManacleTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x + 4, y + 2, ModContent.TileType<ManacleTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x + 2, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x + 4, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 9, TileID.Cloud, forced: true);
            WorldGen.PlaceTile(x, y + 8, ModContent.TileType<MistralTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 9, TileID.RainCloud, forced: true);
            WorldGen.PlaceTile(x + 2, y + 8, ModContent.TileType<MistralTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 9, TileID.SnowCloud, forced: true);
            WorldGen.PlaceTile(x + 4, y + 8, ModContent.TileType<MistralTile>(), forced: true, style: 2);
        }

        public static void CreateSilkTouchFile()
        {
            var f = AequusHelpers.CreateDebugFile("SilkTouchBlocks.txt");
            f.WriteText("[Silk Touch Blocks Database]");
            foreach (var i in AequusTile.TileIDToItemID)
            {
                f.WriteText($"\n[Tile: {i.Key} | {TileID.Search.GetName(i.Key.TileType)}{(i.Key.TileStyle > 0 ? $"/{i.Key.TileStyle}" : "")}, Item: {i.Value} | {Lang.GetItemName(i.Value)}");
            }
            AequusHelpers.OpenDebugFolder();
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

    internal class ModIconAnimation_NewYears : ModProjectile
    {
        public override string Texture => $"{Aequus.AssetsPath}Shatter";

        public override bool IsLoadingEnabled(Mod mod)
        {
            return TesterItem.LoadMe;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = 2;
            Projectile.height = 2;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 11200f)
            {
                Projectile.Kill();
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overWiresUI.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var p = Projectile.Center - Main.screenPosition;
            float t = Projectile.ai[1];
            float scale = 2f;
            AequusHelpers.DrawRectangle(Utils.CenteredRectangle(p, new Vector2(80f * scale * 2f)), Color.Black);
            if (t > 300f)
            {
                var carpenter = TextureAssets.Npc[ModContent.NPCType<NPCs.Friendly.Town.Carpenter>()].Value;
                var carpenterFrame = carpenter.Frame(verticalFrames: 25, frameY: (int)(Main.GameUpdateCount / 4 % 13) + 2);
                Main.spriteBatch.Draw(TextureCache.Bloom[2].Value, p + new Vector2(0f, scale * 4f), null, Color.Cyan.UseA(0) * 0.6f, 0f, TextureCache.Bloom[2].Value.Size() / 2f, scale, SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(carpenter, p, carpenterFrame, Color.White, 0f, carpenterFrame.Size() / 2f, scale, SpriteEffects.None, 0f);
                string text = "Happy";
                var textColor = Color.Lerp(Color.White, Color.Cyan, 0.33f);
                var font = FontAssets.MouseText.Value;
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, p - new Vector2(0f, scale * 20f), textColor, 0f, font.MeasureString(text) / 2f, new Vector2(scale));
                text = "New Years!";
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, p - new Vector2(0f, scale * -30f), textColor, 0f, font.MeasureString(text) / 2f, new Vector2(scale) * 0.9f);
                return false;
            }
            var icon = ModContent.Request<Texture2D>("Aequus/icon").Value;
            Main.spriteBatch.Draw(icon, p, null, Color.White, 0f, icon.Size() / 2f, scale, SpriteEffects.None, 0f);

            var crack = TextureAssets.Projectile[Type].Value;
            float crackScale = scale / 3f;
            if ((int)t == 90)
            {
                EffectsSystem.Shake.Set(20f, 0.93f);
                var clrs = crack.Get1DColorArr();
                for (int i = 0; i < clrs.Length; i++)
                {
                    if (clrs[i].A > 128 && Main.rand.NextBool(50))
                    {
                        int j = i / crack.Width;
                        var d = Dust.NewDustPerfect(Projectile.Center + new Vector2((crack.Width / -2f + (i % crack.Width)) * crackScale, (crack.Height / -2f + j) * crackScale), DustID.AncientLight, Scale: crackScale * 6f);
                        d.velocity *= 8f;
                        d.noGravity = true;
                    }
                }
            }
            if (t > 90f)
            {
                Main.spriteBatch.Draw(crack, p, null, Color.White.UseA(0), 0f, crack.Size() / 2f, crackScale, SpriteEffects.None, 0f);
            }
            var ray = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}LightRay").Value;
            if (t > 120f)
            {
                float rayT = (t - 120f) / 140f;
                var clr = new Color(200, 200, 255, 0);
                drawRay(ray, p, 0f, clr, scale, rayT);
                drawRay(ray, p, 1f, clr, scale, rayT);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 1.1f, clr, scale, rayT * 0.88f);
                drawRay(ray, p, -3.11f, clr, scale, rayT * 0.8f);
                drawRay(ray, p, 2.11f, clr, scale, rayT * 0.6f);
                drawRay(ray, p, 1.34f, clr, scale, rayT * 0.9f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly, clr, scale, rayT * 0.9f - 0.33f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.9f, clr, scale, rayT * 0.9f - 0.33f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.7f, clr, scale, rayT * 0.9f - 0.13f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.88f, clr, scale, rayT * 1.3f - 0.3f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.87f, clr, scale, rayT * 1.3f - 0.3f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.86f, clr, scale, rayT * 1.2f - 0.3f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.85f, clr, scale, rayT * 1.1f - 0.3f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.84f, clr, scale, rayT * 1.11f - 0.3f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.83f, clr, scale, rayT * 1.21f - 0.3f);
                drawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.82f, clr, scale, rayT * 1.6f - 0.3f);
                Main.spriteBatch.Draw(TextureCache.Bloom[1].Value, p, null, Color.White.UseA(0), 0f, TextureCache.Bloom[1].Value.Size() / 2f, scale * (float)Math.Pow(rayT, 8), SpriteEffects.None, 0f);
            }
            if (t == 300f)
            {
                var clrs = crack.Get1DColorArr();
                for (int i = 0; i < clrs.Length; i++)
                {
                    if (clrs[i].A > 128 && Main.rand.NextBool(10))
                    {
                        int j = i / crack.Width;
                        var d = Dust.NewDustPerfect(Projectile.Center + new Vector2((crack.Width / -2f + (i % crack.Width)) * crackScale, (crack.Height / -2f + j) * crackScale), DustID.AncientLight, Scale: crackScale * 3f);
                        d.velocity *= 8f;
                    }
                }
            }
            return false;
        }

        public void drawRay(Texture2D ray, Vector2 p, float rotation, Color clr, float scale, float rayT)
        {
            if (rayT < 0f)
                return;
            rayT = (float)Math.Pow(rayT, 3);
            Main.spriteBatch.Draw(ray, p, null, clr, rotation, ray.Size() / 2f, new Vector2(scale * rayT, scale * rayT * 1.5f), SpriteEffects.None, 0f);
        }
    }
}