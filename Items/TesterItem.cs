using Aequus.Biomes;
using Aequus.Biomes.Glimmer;
using Aequus.Content.Necromancy;
using Aequus.Content.WorldGeneration;
using Aequus.Graphics;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Weapons.Magic;
using Aequus.NPCs.Monsters.Underworld;
using Aequus.Particles.Dusts;
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
using Terraria.Utilities;

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
            int x = AequusHelpers.MouseTileX;
            int y = AequusHelpers.MouseTileY;
            //AequusWorld.hardmodeChests = false;
            //if (Chest.FindChestByGuessing(x, y) != -1)
            //{
            //    Main.chest[Chest.FindChestByGuessing(x, y)].SquishAndStackContents();
            //}
            //Projectile.NewProjectile(null, player.Center + new Vector2(400f, 0f), Vector2.Zero, ModContent.ProjectileType<ModIconAnimation>(), 0, 0f, player.whoAmI);
            if (AequusWorldGenerator.RadonCaves.ValidSpotForCave(x, y))
            {
                AequusWorldGenerator.RadonCaves.CreateCave(x, y);
                //AequusWorldGenerator.RadonCaves.GrowStalactite(x, y, AequusWorldGenerator.RadonCaves.MaxWidth, AequusWorldGenerator.RadonCaves.MaxHeight);
            }
            return true;
        }

        public override void AddRecipes()
        {
            //CreateRecipe().AddIngredient<UltimateEnergy>().Register();
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

        public static void SpawnPhysicalTestDummies(int npciD)
        {
            var m = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates();
            for (int i = 0; i < 5; i++)
            {
                var n = NPC.NewNPCDirect(null, m + new Vector2(90f * (i - 2), 0f), npciD);
                n.Aequus().noAITest = true;
                n.knockBackResist = 0f;
            }
        }

        public static void SpawnGlimmer()
        {
            GlimmerBiome.TileLocation = Point.Zero;
            GlimmerSystem.BeginEvent();
        }

        public static void KillOfType(int npcID)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == npcID)
                {
                    Main.npc[i].StrikeNPCNoInteraction(99999, 0f, 0, crit: true);
                }
            }
        }
        public static NPC NoAINPC(int npcID)
        {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld, npcID);
            n.Aequus().noAITest = true;
            return n;
        }

        public static void NoAITrapperImp()
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

        public static void WriteNPCsInHashSet(HashSet<int> hash)
        {
            foreach (var item in hash)
            {
                Main.NewText(Lang.GetNPCNameValue(item));
            }
        }

        public static void WriteToFileNecromancyWiki()
        {
            var c = Path.DirectorySeparatorChar;
            var path = $"{Main.SavePath}{c}Mods{c}AequusWiki";
            Directory.CreateDirectory(path);
            path += $"{c}NecromancyTiers.txt";
            using var stream = File.Create(path);
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

        public static void WriteStatSheetInfoTest()
        {
            //var clr = Color.Red.HueAdd(Main.rand.NextFloat(1f));
            //foreach (var s in StatSheetManager.RegisteredStats)
            //{
            //    Main.NewText($"{Language.GetTextValue(s.DisplayName)}: {s.ProvideStatText()}", Color.Lerp(clr, Color.White, 0.75f));
            //    clr = clr.HueAdd(Main.rand.NextFloat(0.1f));
            //}
        }

        public class ModIconAnimation : ModProjectile
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
                if (Projectile.ai[1] > 1120f)
                {
                    Projectile.Kill();
                }
            }

            public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
            {
                //overWiresUI.Add(index);
            }

            public void Draw2(Vector2 p, float t, float scale)
            {
                if (t > 300f)
                {
                    var carpenter = TextureAssets.Npc[ModContent.NPCType<NPCs.Friendly.Town.Carpenter>()].Value;
                    var carpenterFrame = carpenter.Frame(verticalFrames: 25, frameY: (int)(Main.GameUpdateCount / 4 % 13) + 2);
                    Main.spriteBatch.Draw(Textures.Bloom[2].Value, p + new Vector2(0f, scale * 4f), null, Color.Cyan.UseA(0) * 0.6f, 0f, Textures.Bloom[2].Value.Size() / 2f, scale, SpriteEffects.None, 0f);

                    Main.spriteBatch.Draw(carpenter, p, carpenterFrame, Color.White, 0f, carpenterFrame.Size() / 2f, scale, SpriteEffects.None, 0f);
                    string text = "Happy";
                    var textColor = Color.Lerp(Color.White, Color.Cyan, 0.33f);
                    var font = FontAssets.MouseText.Value;
                    ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, p - new Vector2(0f, scale * 20f), textColor, 0f, font.MeasureString(text) / 2f, new Vector2(scale));
                    text = "New Years!";
                    ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, p - new Vector2(0f, scale * -30f), textColor, 0f, font.MeasureString(text) / 2f, new Vector2(scale) * 0.9f);
                    return;
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
                            var d = Dust.NewDustPerfect(Projectile.Center + new Vector2((crack.Width / -2f + i % crack.Width) * crackScale, (crack.Height / -2f + j) * crackScale), DustID.AncientLight, Scale: crackScale * 6f);
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
                    DrawRay(ray, p, 0f, clr, scale, rayT);
                    DrawRay(ray, p, 1f, clr, scale, rayT);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 1.1f, clr, scale, rayT * 0.88f);
                    DrawRay(ray, p, -3.11f, clr, scale, rayT * 0.8f);
                    DrawRay(ray, p, 2.11f, clr, scale, rayT * 0.6f);
                    DrawRay(ray, p, 1.34f, clr, scale, rayT * 0.9f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly, clr, scale, rayT * 0.9f - 0.33f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.9f, clr, scale, rayT * 0.9f - 0.33f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.7f, clr, scale, rayT * 0.9f - 0.13f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.88f, clr, scale, rayT * 1.3f - 0.3f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.87f, clr, scale, rayT * 1.3f - 0.3f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.86f, clr, scale, rayT * 1.2f - 0.3f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.85f, clr, scale, rayT * 1.1f - 0.3f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.84f, clr, scale, rayT * 1.11f - 0.3f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.83f, clr, scale, rayT * 1.21f - 0.3f);
                    DrawRay(ray, p, Main.GlobalTimeWrappedHourly * 0.82f, clr, scale, rayT * 1.6f - 0.3f);
                    Main.spriteBatch.Draw(Textures.Bloom[1].Value, p, null, Color.White.UseA(0), 0f, Textures.Bloom[1].Value.Size() / 2f, scale * (float)Math.Pow(rayT, 8), SpriteEffects.None, 0f);
                }
                if (t == 300f)
                {
                    var clrs = crack.Get1DColorArr();
                    for (int i = 0; i < clrs.Length; i++)
                    {
                        if (clrs[i].A > 128 && Main.rand.NextBool(10))
                        {
                            int j = i / crack.Width;
                            var d = Dust.NewDustPerfect(Projectile.Center + new Vector2((crack.Width / -2f + i % crack.Width) * crackScale, (crack.Height / -2f + j) * crackScale), DustID.AncientLight, Scale: crackScale * 3f);
                            d.velocity *= 8f;
                        }
                    }
                }
            }
            public void Draw3(Vector2 p, float t, float scale)
            {
                var icon = ModContent.Request<Texture2D>("Aequus/icon").Value;
                Main.spriteBatch.Draw(icon, p, null, Color.White, 0f, icon.Size() / 2f, scale, SpriteEffects.None, 0f);
                if (t > 30f && t < 190)
                {
                    DrawEnergy(ModContent.ItemType<OrganicEnergy>(), 0f, scale, t, p);
                    DrawEnergy(ModContent.ItemType<AquaticEnergy>(), MathHelper.TwoPi / 5f, scale, t, p);
                    DrawEnergy(ModContent.ItemType<CosmicEnergy>(), MathHelper.TwoPi / 5f * 2f, scale, t, p);
                    DrawEnergy(ModContent.ItemType<DemonicEnergy>(), MathHelper.TwoPi / 5f * 3f, scale, t, p);
                    DrawEnergy(ModContent.ItemType<AtmosphericEnergy>(), MathHelper.TwoPi / 5f * 4f, scale, t, p);
                }
                if (t > 160f && t < 200f)
                {
                    Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);

                    float scale2 = (t - 160f) / 20f;
                    float scale3 = AequusHelpers.Wave(t * 0.2f, 0.7f, 1f);
                    Main.spriteBatch.Draw(Textures.Bloom[2].Value, p, null, Color.White, 0f, Textures.Bloom[1].Value.Size() / 2f, scale * scale2 / 2f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(Textures.Bloom[1].Value, p, null, Color.White, 0f, Textures.Bloom[1].Value.Size() / 2f, scale * scale2, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value, p, null, Color.White.UseA(0), 0f,
                        TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value.Size() / 2f, new Vector2(1f, scale * scale2) * scale3, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value, p, null, Color.White.UseA(0), MathHelper.PiOver2,
                        TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value.Size() / 2f, new Vector2(1f, scale * 2f * scale2) * scale2 * scale3, SpriteEffects.None, 0f);
                }
            }
            public void Draw4(Vector2 p, float t, float scale)
            {
                var icon = ModContent.Request<Texture2D>(t > 210f ? "Aequus/icon2" : "Aequus/icon_bw").Value;
                Main.spriteBatch.Draw(icon, p, null, Color.White, 0f, icon.Size() / 2f, scale, SpriteEffects.None, 0f);
                if (t < 210f)
                {
                    float r = t / 33f;
                    float energyTime = Math.Min(t * 2f, 160f + AequusHelpers.Wave(t / 55f, -20f, 0f));
                    DrawEnergy(ModContent.ItemType<OrganicEnergy>(), r, scale, energyTime, p);
                    DrawEnergy(ModContent.ItemType<AquaticEnergy>(), r + MathHelper.TwoPi / 5f, scale, energyTime, p);
                    DrawEnergy(ModContent.ItemType<CosmicEnergy>(), r + MathHelper.TwoPi / 5f * 2f, scale, energyTime, p);
                    DrawEnergy(ModContent.ItemType<DemonicEnergy>(), r + MathHelper.TwoPi / 5f * 3f, scale, energyTime, p);
                    DrawEnergy(ModContent.ItemType<AtmosphericEnergy>(), r + MathHelper.TwoPi / 5f * 4f, scale, energyTime, p);
                }
                if (t > 190f && t < 230f)
                {
                    var clr = (float)Math.Sin(MathHelper.Pi * (t - 190f) / 40f);
                    AequusHelpers.DrawRectangle(Utils.CenteredRectangle(p, new Vector2(100f, 100f) * scale), Color.White * clr * 0.88f);
                }
                float dustStart = 160f;
                float dustEnd = 240f;
                if (t > dustStart && t < dustEnd + 290f)
                {
                    float progress = (t - dustStart) / (dustEnd - dustStart);
                    var dust = ModContent.Request<Texture2D>($"{AequusHelpers.GetPath<MonoDust>()}");

                    var rand = new FastRandom("SPLIT".GetHashCode());
                    var origin = dust.Value.Frame(verticalFrames: 3).Size() / 2f;
                    for (int i = 0; i < 1000; i++)
                    {
                        if (rand.Next(3) == 0)
                        {
                            continue;
                        }
                        float y = rand.Next(0, 120);
                        float x = rand.Next(0, 100);
                        x += AequusHelpers.Wave(rand.Next(-10, 10) + progress * MathHelper.Pi * (rand.Next(90, 166) / 100f), -10f, 10f);
                        float prog = rand.Next(100, 250) / 100f;
                        if (rand.Next(33) == 0)
                        {
                            prog *= rand.Next(33, 60) / 100f;
                        }
                        float dustScale = rand.Next(22, 166) / 100f;
                        var frame = dust.Value.Frame(verticalFrames: 3, frameY: rand.Next(3));
                        var color = new Color(rand.Next(200, 255), rand.Next(200, 255), rand.Next(200, 255)).HueAdd(rand.Next(700) / 100f);
                        float rotation = rand.Next((int)(MathHelper.TwoPi * 100f)) / 100f;
                        rotation += Main.GlobalTimeWrappedHourly * (rand.Next(33, 366) / 100f);
                        Main.spriteBatch.Draw(dust.Value, p + new Vector2(x * scale - 50f * scale, (y * scale + 50f * scale) * (1f - progress * prog)), frame, color.UseA(rand.Next(100)), rotation, origin, dustScale * scale, SpriteEffects.None, 0f);
                    }
                }
            }
            public void DrawEnergy(int item, float rotation, float scale, float t, Vector2 p)
            {
                Main.instance.LoadItem(item);
                var itemTexture = TextureAssets.Item[item].Value;
                float outwards = (float)Math.Sin((t - 30f) * 0.01f + MathHelper.PiOver2) * 80f * scale;
                Main.spriteBatch.Draw(itemTexture, p + (rotation - MathHelper.PiOver2 + outwards * 0.4f / 60f / scale).ToRotationVector2() * outwards, null, Color.White, 0f, itemTexture.Size() / 2f, scale * 0.75f, SpriteEffects.None, 0f);
            }

            public override bool PreDraw(ref Color lightColor)
            {
                var p = Projectile.Center - Main.screenPosition;
                float t = Projectile.ai[1];
                float scale = 2f;
                AequusHelpers.DrawRectangle(Utils.CenteredRectangle(p, new Vector2(80f * scale * 2f)), Color.Black);
                //Draw2(p, t, scale);
                //Draw3(p, t, scale);
                Draw4(p, t, scale);
                return false;
            }

            public void DrawRay(Texture2D ray, Vector2 p, float rotation, Color clr, float scale, float rayT)
            {
                if (rayT < 0f)
                    return;
                rayT = (float)Math.Pow(rayT, 3);
                Main.spriteBatch.Draw(ray, p, null, clr, rotation, ray.Size() / 2f, new Vector2(scale * rayT, scale * rayT * 1.5f), SpriteEffects.None, 0f);
            }
        }
    }

    internal class AutoLanguageItem : ModItem
    {
        public const bool LoadMe = false;

        public override string Texture => AequusHelpers.GetPath<Gamestar>();

        public override bool IsLoadingEnabled(Mod mod)
        {
            return LoadMe && TesterItem.LoadMe;
        }

        public void WriteTip(FileStream stream, string key, string text, int tabs = 0)
        {
            if (text.Contains('\n'))
            {
                stream.WriteText($"{key}: \n", tabs: tabs);
                stream.WriteText($"'''\n{text}\n'''", tabs: tabs + 1);
            }
            else
            {
                stream.WriteText($"{key}: {text}", tabs: tabs);
            }
        }

        public void LanguageTest()
        {
            using var file = AequusHelpers.CreateDebugFile("en-US-Items.hjson");
            var dict = new Dictionary<string, List<ModItem>>();
            foreach (var item in Aequus.Instance.GetContent<ModItem>())
            {
                string nameSpace = item.GetType().Namespace;
                if (nameSpace.EndsWith(".Items")) // Exclude debug items
                    continue;
                var split = nameSpace.Split('.');
                dict.AddList(string.Join("-", split[2..^0]).Replace('.', '/'), item);
            }
            int itemTabs = 2;
            bool start = false;
            foreach (var value in dict)
            {
                file.WriteText($"{(!start ? "" : "\n")}# {value.Key}\n", tabs: itemTabs - 1);
                foreach (var modItem in value.Value)
                {
                    file.WriteText($"ItemName.{modItem.GetType().Name}: {Lang.GetItemNameValue(modItem.Type)}\n", tabs: itemTabs);
                    string tipKey = $"Mods.Aequus.ItemTooltip.{modItem.GetType().Name}";
                    var tooltip = Language.GetTextValue(tipKey);
                    if (tooltip != tipKey && !string.IsNullOrWhiteSpace(tooltip))
                    {
                        WriteTip(file, $"ItemTooltip.{modItem.GetType().Name}", tooltip, tabs: itemTabs);
                        file.WriteText("\n", tabs: itemTabs);
                    }
                }
                start = true;
            }
            AequusHelpers.OpenDebugFolder();
        }

        public override void AddRecipes()
        {
            LanguageTest();
        }

        public override bool? UseItem(Player player)
        {
            //LanguageTest();
            return true;
        }
    }
}