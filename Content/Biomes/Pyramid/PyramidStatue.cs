using Aequus;
using Aequus.Common;
using Aequus.Common.DataSets;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.Biomes.Pyramid {
    public class PyramidStatue : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PyramidStatueTile>());
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Red;
        }
    }
    public class PyramidStatueBroken1 : ModItem {
        public override string Texture => ModContent.GetInstance<PyramidStatue>().Texture;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PyramidStatueBroken1Tile>());
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }
    }
    public class PyramidStatueBroken2 : PyramidStatueBroken1 {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PyramidStatueBroken2Tile>());
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }
    }

    public abstract class PyramidStatueTileBase : ModTile {
        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileSets.PreventsSlopesBelow.Add(Type);
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 7;
            TileObjectData.newTile.Origin = new Point16(2, 6);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            DustType = DustID.Stone;
            MinPick = 210;
            AddMapEntry(new Color(140, 140, 140), TextHelper.GetItemName<PyramidStatue>());
        }
    }

    public class PyramidStatueTile : PyramidStatueTileBase {
        public int SelectionLoopTime = 60;

        private void DrawMoon(int i, int j, SpriteBatch spriteBatch) {
            var drawPosition = this.GetDrawPosition(i, j) + Helper.TileDrawOffset + new Vector2(-16f, -126f + Helper.Wave(Main.GlobalTimeWrappedHourly, -2f, 2f));

            var moonTexture = TextureAssets.Moon[Main.moonType].Value;
            var moonFrame = moonTexture.Frame(verticalFrames: 8, frameY: Main.moonPhase);
            var moonOrigin = moonFrame.Size() / 2f;
            float moonScale = 0.66f;

            var bloomTexture = AequusTextures.Bloom0;
            var bloomOrigin = bloomTexture.Size() / 2f;

            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.White * 0.1f, 0f,
                bloomOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.Black, 0f,
                bloomOrigin, 0.7f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.Black, 0f,
                bloomOrigin, 0.55f, SpriteEffects.None, 0f);
            spriteBatch.Draw(
                moonTexture, drawPosition,
                moonFrame with { Y = 0 },
                Color.Black * 0.5f, 0f, moonOrigin, moonScale, SpriteEffects.None, 0f
            );
            spriteBatch.Draw(
                moonTexture, drawPosition,
                moonFrame,
                Color.White, 0f, moonOrigin, moonScale, SpriteEffects.None, 0f
            );

            spriteBatch.Draw(bloomTexture, drawPosition, null, Color.White * 0.1f, 0f,
                bloomOrigin, 1f, SpriteEffects.None, 0f);
        }

        private void ManualDraw(int i, int j, SpriteBatch spriteBatch, int frameOffset) {
            var tileTexture = TextureAssets.Tile[Type].Value;
            Rectangle frame = new(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY + frameOffset * 126, 16, 16);
            spriteBatch.Draw(tileTexture, this.GetDrawPosition(i, j) + Helper.TileDrawOffset, frame, Lighting.GetColor(i, j), 0f,
                Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            //var tile = Main.tile[i, j];
            //if (tile.TileFrameX == 54 && tile.TileFrameY == 108) {
            //    DrawMoon(i, j, spriteBatch);
            //}

            var dualism = Helper.GetMoonDualism();
            switch (dualism) {
                case Duality.Light:
                    ManualDraw(i, j, spriteBatch, 1);
                    return false;

                case Duality.Dark:
                    ManualDraw(i, j, spriteBatch, 2);
                    return false;
            }
            return true;
        }

        public void GetHoverItem(out List<PyramidStatueUI.OfferingInfo> list, out int index) {
            list = PyramidStatueUI.Items[Helper.GetMoonDualism()];
            index = (int)Math.Clamp(Main.GameUpdateCount % SelectionLoopTime / (SelectionLoopTime / list.Count), 0, list.Count);
        }

        public override void MouseOver(int i, int j) {
            GetHoverItem(out var list, out var index);
            var player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = list[index].ItemID;
        }

        public override bool RightClick(int i, int j) {
            var tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;
            Aequus.UserInterface.SetState(new PyramidStatueUI(left, top, PyramidStatueUI.Items[Helper.GetMoonDualism()]));
            return false;
        }
    }
    public class PyramidStatueBroken1Tile : PyramidStatueTileBase {
    }
    public class PyramidStatueBroken2Tile : PyramidStatueTileBase {
    }

    public class PyramidStatueUI : AequusUIState {
        public static PyramidStatueUI Instance { get; private set; }
        public static readonly Dictionary<Duality, List<OfferingInfo>> Items = new();

        public int X;
        public int Y;
        public bool Closing = false;
        public Vector2 Center => new(X * 16f + 32f, Y * 16f + 48f);

        public record struct OfferingInfo {
            public int ItemID;
            public string DescriptionKey;
            public Color AuraColor;
            private Func<Player, int, int, bool> acceptOffer;

            public OfferingInfo(int itemID, string descriptionKey, Color auraColor, Func<Player, int, int, bool> AcceptOffer) {
                ItemID = itemID;
                DescriptionKey = descriptionKey;
                AuraColor = auraColor;
                acceptOffer = AcceptOffer;
                Language.GetOrRegister(descriptionKey);
            }

            public bool AcceptOffer(Player player, int i, int j) {
                return acceptOffer(player, i, j);
            }

            public static bool AcceptOffer_Money(Player player, int i, int j) {
                if (!player.CanAfford(Item.gold)) {
                    return false;
                }

                player.BuyItem(Item.gold);
                SoundEngine.PlaySound(SoundID.Coins);
                SoundEngine.PlaySound(SoundID.Item4);

                player.AddBuff(Helper.GetMoonDualism() switch {
                    Duality.Light => BuffID.Shine,
                    Duality.Dark => BuffID.NightOwl,
                    _ => BuffID.Lucky,
                }, Item.luckPotionDuration1);
                return true;
            }
            public static Func<Player, int, int, bool> AcceptOffer_GenericItemOffering(int itemID, int buffID, int buffDuration, SoundStyle? playSound = null) {
                playSound ??= SoundID.Item4;
                return (player, i, j) => {
                    if (!player.ConsumeItem(itemID)) {
                        return false;
                    }

                    SoundEngine.PlaySound(playSound);

                    player.AddBuff(buffID, buffDuration);
                    return true;
                };
            }
        }

        public class PyramidSelectionIcon {
            public OfferingInfo Offering;
            public Item ItemInstance;
            public int shake;
            public float failedOfferAnimation;

            public PyramidSelectionIcon(OfferingInfo offering) {
                Offering = offering;
                ItemInstance = new(offering.ItemID);
            }

            public void Update() {
                if (shake > 0) {
                    shake--;
                }
                if (failedOfferAnimation > 0f) {
                    failedOfferAnimation *= 0.9f;
                    failedOfferAnimation -= 0.01f;
                }
            }
        }

        public float animation;
        public List<PyramidSelectionIcon> Icons;

        private PyramidStatueUI() {
            Icons = new();
            Main.OurFavoriteColor = Color.Red;
        }

        public PyramidStatueUI(int x,int y, List<OfferingInfo> info) : this() {
            X = x;
            Y = y;
            for (int i = 0; i < info.Count; i++) {
                Icons.Add(new(info[i]));
            }
        }

        public override void Load(Mod mod) {
            Instance = this;
            Items.Clear();
            for (int i = 0; i < 4; i++) {
                Items[(Duality)i] = new();
            }
        }

        public override void PostSetupContent(Aequus aequus) {
            AddNeutralOffering(new(
                ItemID.GoldCoin,
                "Mods.Aequus.Interface.PyramidStatue.Offerings.MoneyOffering",
                Color.Gold,
                OfferingInfo.AcceptOffer_Money
            ));
            AddLightOffering(new(
                ItemID.LightShard,
                "Mods.Aequus.Interface.PyramidStatue.Offerings.LightShard",
                Color.White,
                OfferingInfo.AcceptOffer_GenericItemOffering(ItemID.DarkShard, BuffID.Archery, Item.luckPotionDuration1)
            ));
            AddDarkOffering(new(
                ItemID.DarkShard,
                "Mods.Aequus.Interface.PyramidStatue.Offerings.DarkShard",
                Color.Black,
                OfferingInfo.AcceptOffer_GenericItemOffering(ItemID.DarkShard, BuffID.Silenced, Item.luckPotionDuration1)
            ));
        }

        public override void Unload() {
            Instance = null;
            Items.Clear();
        }

        public static void AddNeutralOffering(OfferingInfo offering) {
            Items[Duality.Neutral].Add(offering);
            Items[Duality.Dark].Add(offering);
            Items[Duality.Light].Add(offering);
        }
        public static void AddLightOffering(OfferingInfo offering) {
            Items[Duality.Neutral].Add(offering);
            Items[Duality.Light].Add(offering);
        }
        public static void AddDarkOffering(OfferingInfo offering) {
            Items[Duality.Neutral].Add(offering);
            Items[Duality.Dark].Add(offering);
        }

        public override void OnInitialize() {
            Width.Set(140, 0f);
            Height.Set(140, 0f);
        }

        public override void Update(GameTime gameTime) {
            if (Closing || (Icons?.Count).GetValueOrDefault(0) == 0 || Vector2.Distance(Center, Main.LocalPlayer.Center) > 180f) {
                Close();
                return;
            }
            animation += 1 / 16f;
            foreach (var icon in Icons) {
                icon.Update();
            }
        }

        private void DrawSingleIcon(SpriteBatch spriteBatch, PyramidSelectionIcon offering, int i, Texture2D texture, Vector2 location, Color color, Color itemColor, Vector2 origin) {
            string text = Language.GetTextValue(offering.Offering.DescriptionKey);
            string[] textSnippets = text.Split('\n');

            if (animation > 0.9f && Utils.CenteredRectangle(location, texture.Size()).Contains(Main.mouseX, Main.mouseY)) {
                var player = Main.LocalPlayer;
                Main.instance.MouseText(textSnippets[0] + "?");
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease) {
                    if (!offering.Offering.AcceptOffer(Main.LocalPlayer, X, Y)) {
                        offering.shake = 10;
                        offering.failedOfferAnimation = 1f;
                    }
                }
            }
            location += new Vector2(Main.rand.NextFloat(-offering.shake, offering.shake), Main.rand.NextFloat(-offering.shake, offering.shake));
            var bloomTexture = AequusTextures.Bloom0;
            var auraColor = offering.Offering.AuraColor;

            spriteBatch.Draw(
                texture,
                location,
                null,
                color,
                0f,
                origin,
                Main.inventoryScale,
                SpriteEffects.None,
                0f
            );
            spriteBatch.Draw(
                bloomTexture,
                location,
                null,
                auraColor * 0.3f,
                0f,
                bloomTexture.Size() / 2f,
                Main.inventoryScale * 0.8f,
                SpriteEffects.None,
                0f
            );

            ItemSlotRenderer.Draw(
                spriteBatch,
                offering.ItemInstance,
                location - origin * Main.inventoryScale,
                itemColor
            );

            float iconAnimation = animation - i * 0.33f;
            if (iconAnimation <= 0f) {
                return;
            }

            float textOpacity = 1f;
            float textYMagnitude = 1f;
            if (iconAnimation < 3f) {
                textOpacity *= MathF.Pow(iconAnimation / 3f, 2f);
            }
            if (iconAnimation < 4f) {
                textYMagnitude *= MathF.Pow(iconAnimation / 4f, 3f);
            }
            float textBGOpacity = textOpacity;
            float textScaleMultiplier = textOpacity;
            if (offering.failedOfferAnimation > 0f) {
                if (offering.failedOfferAnimation < 0.1f) {
                    textOpacity *= 1f - offering.failedOfferAnimation / 0.1f;
                }
                else {
                    textOpacity *= (offering.failedOfferAnimation - 0.1f) / 0.9f;
                }
            }

            var font = FontAssets.MouseText.Value;
            var textColor = new Color(auraColor.R + 70, auraColor.G + 70, auraColor.B + 70);
            var measurement = font.MeasureString(text);
            var scale = Vector2.One;
            float textWidth = 220f;
            if (measurement.X > textWidth) {
                scale *= textWidth / measurement.X;
            }
            var textPosition = location + new Vector2(0f, texture.Height / 2f + 2f + 18f * textYMagnitude);
            var textColorReal = textColor * 2f * textOpacity;
            var textScaleReal = scale * Math.Max(textScaleMultiplier, 0.25f);

            Rectangle textBGFrame = new(bloomTexture.Width / 2, 0, 2, bloomTexture.Height);
            spriteBatch.Draw(
                bloomTexture,
                textPosition + new Vector2(-measurement.X / 2f - 8f, 8f) * scale,
                textBGFrame,
                Color.Black * MathF.Pow(textBGOpacity, 3f) * 0.3f,
                MathHelper.PiOver2,
                new Vector2(textBGFrame.Width / 2f, textBGFrame.Height),
                new Vector2(measurement.Y / textBGFrame.Width, measurement.X / textBGFrame.Height),
                SpriteEffects.None,
                0f
            );

            var textPositionReal = textPosition;
            for (int k = 0; k < textSnippets.Length; k++) {
                var snippetMeasurement = font.MeasureString(textSnippets[k]);
                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch,
                    font,
                    textSnippets[k],
                    textPositionReal,
                    textColorReal,
                    0f,
                    snippetMeasurement / 2f,
                    textScaleReal
                );
                textPositionReal.Y += snippetMeasurement.Y;
            }
            if (offering.failedOfferAnimation < 0.7f && offering.failedOfferAnimation > 0f) {
                string failedOfferingText = TextHelper.GetTextValue("Interface.PyramidStatue.Offerings.FailedOffering");
                float failedOfferingOpacity;
                if (offering.failedOfferAnimation < 0.1f) {
                    failedOfferingOpacity = offering.failedOfferAnimation / 0.1f;
                }
                else {
                    failedOfferingOpacity = 1f - (offering.failedOfferAnimation - 0.1f) / 0.6f;
                }
                ChatManager.DrawColorCodedStringWithShadow(
                    spriteBatch,
                    font,
                    failedOfferingText,
                    textPosition + new Vector2(Main.rand.NextFloat(-offering.failedOfferAnimation, offering.failedOfferAnimation), Main.rand.NextFloat(-offering.failedOfferAnimation, offering.failedOfferAnimation)) * 4f,
                    Color.Red * failedOfferingOpacity,
                    0f,
                    font.MeasureString(failedOfferingText) / 2f,
                    Vector2.One
                );
            }
        }

        private void DrawIcons(SpriteBatch spriteBatch, CalculatedStyle calculatedStyle, int count, float iconPosMagnitude, float iconOpacity, float iconRotationStart) {
            var player = Main.LocalPlayer;
            var iconTexture = TextureAssets.InventoryBack.Value;
            var iconOrigin = iconTexture.Size() / 2f;
            var iconColor = Main.inventoryBack * iconOpacity;
            var itemColor = Color.White * iconOpacity;
            var center = Center - Main.screenPosition;
            float iconRotationLoop = MathHelper.TwoPi / count;
            if (Vector2.Distance(Main.MouseScreen, center) < iconPosMagnitude) {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
            for (int i = 0; i < count; i++) {
                DrawSingleIcon(
                    spriteBatch,
                    Icons[i],
                    i,
                    iconTexture,
                    center + (iconRotationStart + iconRotationLoop * i).ToRotationVector2() * iconPosMagnitude,
                    iconColor,
                    itemColor,
                    iconOrigin
                );
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            int count = (Icons?.Count).GetValueOrDefault(0);
            if (count == 0) {
                return;
            }

            var calculatedStyle = GetDimensions();
            float iconPosMagnitude = MathF.Sqrt(calculatedStyle.Width * calculatedStyle.Width + calculatedStyle.Height * calculatedStyle.Height) / 4f * count;
            float iconOpacity = 1f;
            float iconRotationStart = 0f;
            if (count % 2 == 1) {
                iconRotationStart = -MathHelper.PiOver2;
            }

            if (animation < 1f) {
                iconOpacity *= animation;
                iconPosMagnitude *= MathF.Pow(animation, 2f);
            }

            float inventoryScale = Main.inventoryScale;
            Main.inventoryScale = 0.8f;
            try {
                DrawIcons(spriteBatch, calculatedStyle, count, iconPosMagnitude, iconOpacity, iconRotationStart);
            }
            catch {
            }
            Main.inventoryScale = inventoryScale;
        }

        private void Close() {
            Closing = true;
            if (animation <= 0f) {
                CloseThisInterface();
                return;
            }
            animation = Math.Min(animation, 10f);
            animation *= 0.75f;
            animation -= 0.1f;
        }

        public override bool ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType) {
            scaleType = InterfaceScaleType.Game;
            return true;
        }
    }
}