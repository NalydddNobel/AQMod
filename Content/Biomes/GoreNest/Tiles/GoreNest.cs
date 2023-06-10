using Aequus.Common.DataSets;
using Aequus.Common.Effects;
using Aequus.Common.Rendering.Tiles;
using Aequus.Content.Events.DemonSiege;
using Aequus.NPCs.Town.OccultistNPC;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.GoreNest.Tiles {
    public class GoreNest : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestTile>());
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
        }
    }

    public class GoreNestTile : ModTile, ISpecialTileRenderer {
        public static int BiomeCount;
        public static MiscShaderWrap<GoreNestShaderData> GoreNestPortal { get; private set; }

        public override void Load() {
            if (!Main.dedServ) {
                GoreNestPortal = new MiscShaderWrap<GoreNestShaderData>("Aequus/Assets/Effects/GoreNestPortal", "GoreNestPortal", "DemonicPortalPass", (effect, pass) => new GoreNestShaderData(effect, pass));
            }
        }

        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileSets.PreventsSlopesBelow.Add(Type);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
            TileObjectData.addTile(Type);
            DustType = DustID.Blood;
            AdjTiles = new int[] { TileID.DemonAltar };
            MineResist = 110;
            AddMapEntry(new Color(175, 15, 15), TextHelper.GetItemName<GoreNest>());
        }

        public override void Unload() {
            GoreNestPortal = null;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
            if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0) {
                return false;
            }
            return GetUsableDemonSiegeItem(settings.player) != null;
        }

        public override void MouseOver(int i, int j) {
            if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(TopLeft(i, j), out var s) && s.PreStart <= 0) {
                return;
            }
            var player = Main.LocalPlayer;
            var upgradeableItem = GetUsableDemonSiegeItem(player);
            if (upgradeableItem != null) {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = upgradeableItem.type;
            }
        }

        public override bool AutoSelect(int i, int j, Item item) {
            return DemonSiegeSystem.RegisteredSacrifices.ContainsKey(item.type);
        }

        public override bool RightClick(int i, int j) {
            var topLeft = TopLeft(i, j);
            var item = GetUsableDemonSiegeItem(Main.LocalPlayer);
            if (item == null) {
                return false;
            }
            if (DemonSiegeSystem.NewInvasion(topLeft.X, topLeft.Y, item, Main.myPlayer)) {
                ConsumeGoreNestItem(item);
            }
            else if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(topLeft, out var sacrifice)) {
                sacrifice.PreStart = Math.Min(sacrifice.PreStart, 60);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketSystem.Send(sacrifice.SendStatusPacket, PacketType.DemonSiegeSacrificeStatus);
            }
            return true;
        }
        public static bool ConsumeGoreNestItem(Item item) {
            //if (item.type != ModContent.ItemType<VoidRing>())
            //{
            //    return false;
            //}
            item.stack--;
            if (item.stack <= 0) {
                item.TurnToAir();
            }
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<GoreNest>());
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
            if (drawData.tileFrameX % 48 != 0 || drawData.tileFrameY % 48 != 0) {
                return;
            }
            SpecialTileRenderer.Add(i, j, TileRenderLayer.PostDrawMasterRelics);
        }

        public static void InnerDrawPortal(Point ij, Vector2 position) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            LegacyEffects.DrawShader(GoreNestPortal.ShaderData, Main.spriteBatch, position, Color.White, scale: 82f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            if (DemonSiegeSystem.ActiveSacrifices.TryGetValue(ij, out var invasion)) {
                InnerDrawPorter_DoDust(position + Main.screenPosition, invasion);

                float opacity = 1f;
                float upgradeOpacity = 0f;
                if (invasion.PreStart > 0) {
                    if (invasion.PreStart < 60) {
                        opacity = 1f - invasion.PreStart / 60f;
                    }
                    else {
                        opacity = 0f;
                    }
                }
                if (invasion.TimeLeft < 360) {
                    float time = (360 - invasion.TimeLeft) / 30f;
                    upgradeOpacity = Helper.Wave(time, 1f, 0f);
                    opacity = Helper.Wave(time, 0f, 1f);
                }
                if (invasion.Items.Count == 1) {
                    InnerDrawPortalItem(invasion.Items[0], position, opacity, upgradeOpacity);
                }
                else if (invasion.Items.Count != 0) {
                    float outwards = 40f + invasion.Items.Count * 4f + Helper.Wave(Main.GlobalTimeWrappedHourly * 2.5f, -4f, 4f);
                    var c = Helper.CircularVector(invasion.Items.Count, Main.GlobalTimeWrappedHourly);
                    for (int i = 0; i < invasion.Items.Count; i++) {
                        InnerDrawPortalItem(invasion.Items[i], position + c[i] * outwards, opacity, upgradeOpacity);
                    }
                }
            }
        }
        public static void InnerDrawPorter_DoDust(Vector2 where, DemonSiegeSacrifice invasion) {
            if (Aequus.GameWorldActive) {
                if (invasion.PreStart > 0) {
                    if (invasion.PreStart < 75) {
                        var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(40f, 80f), ModContent.DustType<MonoDust>(),
                            newColor: new Color(200, 120 + Main.rand.Next(-60, 40), 30, 25));
                        d.velocity = (where - d.position) / 20f;
                    }
                    else {
                        var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * (Main.rand.NextFloat(40f, 80f) + (invasion.PreStart - 75f) / 2f), ModContent.DustType<MonoSparkleDust>(),
                            newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f));
                        d.velocity = (where - d.position) / 20f;
                        d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                    }
                }
                else if (invasion.TimeLeft < 360 || Main.rand.NextBool(50)) {
                    var d = Dust.NewDustPerfect(where + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f, 240f), ModContent.DustType<MonoSparkleDust>(),
                        newColor: new Color(158, 70 + Main.rand.Next(-10, 30), 10, 25) * Main.rand.NextFloat(0.9f, 1.5f));
                    d.velocity = (where - d.position) / 20f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.9f, 1.1f);
                }
            }
        }
        public static void InnerDrawPortalItem(Item i, Vector2 where, float opacity, float upgradeOpacity, float rotation = 0f) {
            Main.instance.LoadItem(i.netID);

            var texture = TextureAssets.Item[i.type].Value;
            i.GetItemDrawData(out var frame);
            float scale = 1f;
            int largest = texture.Width > texture.Height ? texture.Width : texture.Height;
            if (largest > 32) {
                scale = 32f / largest;
            }
            var origin = frame.Size() / 2f;
            var backColor = Color.Lerp(Color.Red * 0.5f, Color.OrangeRed * 0.75f, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));
            for (int k = 0; k < 4; k++) {
                Main.spriteBatch.Draw(texture, where + (k * MathHelper.PiOver2 + rotation).ToRotationVector2() * 2f, frame, backColor, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, where, frame, Color.White * opacity, rotation, origin, scale, SpriteEffects.None, 0f);

            if (upgradeOpacity > 0f) {
                DemonSiegeSystem.RegisteredSacrifices.TryGetValue(i.netID, out var upgrade);
                Main.instance.LoadItem(upgrade.NewItem);

                texture = TextureAssets.Item[upgrade.NewItem].Value;
                Helper.GetItemDrawData(upgrade.NewItem, out frame);
                scale = 1f;
                largest = texture.Width > texture.Height ? texture.Width : texture.Height;
                if (largest > 32) {
                    scale = 32f / largest;
                }
                scale = MathHelper.Lerp(scale, 1f, upgradeOpacity);
                origin = frame.Size() / 2f;
                backColor = Color.Lerp(Color.Red * 0.6f, Color.OrangeRed * 0.8f, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 1f));

                for (int k = 0; k < 4; k++) {
                    Main.spriteBatch.Draw(texture, where + (k * MathHelper.PiOver2 + rotation).ToRotationVector2() * 2f, frame, backColor * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.Draw(texture, where, frame, Color.White * upgradeOpacity, rotation, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public static Point TopLeft(int x, int y) {
            return new Point(x - Main.tile[x, y].TileFrameX / 18, y - Main.tile[x, y].TileFrameY / 18);
        }

        public static Item GetUsableDemonSiegeItem(Player player) {
            if (DemonSiegeSystem.RegisteredSacrifices.ContainsKey(player.HeldItemFixed().type)) {
                return player.HeldItemFixed();
            }
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
                if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(player.inventory[i].type, out var val) && val.OriginalItem != val.NewItem) {
                    return player.inventory[i];
                }
            }
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++) {
                if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(player.inventory[i].type, out var val)) {
                    return player.inventory[i];
                }
            }
            return null;
        }

        public static bool IsGoreNest(Point p) {
            return IsGoreNest(p.X, p.Y);
        }
        public static bool IsGoreNest(int x, int y) {
            return Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<GoreNestTile>();
        }

        void ISpecialTileRenderer.Render(int i, int j, byte layer) {
            OccultistHostile.CheckSpawn(i, j, Main.myPlayer);
            InnerDrawPortal(new Point(i, j), new Vector2(i * 16f + 24f, j * 16f + 8f + Helper.Wave(Main.GlobalTimeWrappedHourly / 4f, -5f, 5f) - 40f) - Main.screenPosition);
        }

        public override void RandomUpdate(int i, int j) {
            int x = i + WorldGen.genRand.Next(-10, 10);
            int y = j + WorldGen.genRand.Next(-10, 10);
            if (!WorldGen.InWorld(x, y, 5)) {
                return;
            }

            TileHelper.KillLiquid(x, y, quiet: false);
        }
    }

    public class GoreNestShaderData : MiscShaderData {
        public GoreNestShaderData(Ref<Effect> shader, string passName) : base(shader, passName) {
            UseColor(new Vector3(5f, 0f, 0f)).UseSecondaryColor(new Vector3(4f, 0, 2f))
                .UseSaturation(1f).UseOpacity(1f);
        }

        public override void Apply() {
            Shader.Parameters["colorLerpMult"].SetValue(0.45f + MathF.Sin(Main.GlobalTimeWrappedHourly * 10 * 0.1f));
            Shader.Parameters["thirdColor"].SetValue(new Vector3(2f, 4f, 0));
            base.Apply();
        }
    }

    public class GoreNestDummy : ModTile {
        public override string Texture => Helper.GetPath<GoreNestTile>();

        public override void SetStaticDefaults() {
            AdjTiles = new int[] { TileID.DemonAltar };
            AddMapEntry(new Color(175, 15, 15), TextHelper.GetText("MapObject.GoreNest"));
        }
    }
}