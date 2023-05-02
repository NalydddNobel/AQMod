using Aequus.Common;
using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.Pyramid.Tiles {
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
        public Dictionary<Duality, List<OfferingInfo>> Items;
        public int SelectionLoopTime = 120;

        public record struct OfferingInfo(int ItemID, Func<Player, int, int, bool, bool> AcceptOffer);

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

        public override void Load() {
            Items = new();
            for (int i = 0; i < 4; i++) {
                Items[(Duality)i] = new();
            }

            AddNeutralOffering(new(ItemID.GoldCoin, (player, x, y, selected) => {
                if (!player.CanAfford(Item.gold)) {
                    return false;
                }

                player.BuyItem(Item.gold);
                SoundEngine.PlaySound(SoundID.Coins);
                SoundEngine.PlaySound(SoundID.Item4);

                player.AddBuff(GetMoonDualism() switch {
                    Duality.Light => BuffID.Shine,
                    Duality.Dark => BuffID.NightOwl,
                    _ => BuffID.Lucky,
                }, Item.luckPotionDuration1);
                return true;
            }));

            AddLightOffering(new(ItemID.LightShard, (player, x, y, selected) => {
                if (!player.ConsumeItem(ItemID.LightShard)) {
                    return false;
                }

                SoundEngine.PlaySound(SoundID.Item4);

                player.AddBuff(BuffID.Archery, Item.luckPotionDuration1);
                return true;
            }));
            AddDarkOffering(new(ItemID.DarkShard, (player, x, y, selected) => {
                if (!player.ConsumeItem(ItemID.DarkShard)) {
                    return false;
                }

                SoundEngine.PlaySound(SoundID.Item4);

                player.AddBuff(BuffID.Silenced, Item.luckPotionDuration1);
                return true;
            }));
        }

        public void AddNeutralOffering(OfferingInfo offering) {
            Items[Duality.Neutral].Add(offering);
            Items[Duality.Dark].Add(offering);
            Items[Duality.Light].Add(offering);
        }
        public void AddLightOffering(OfferingInfo offering) {
            Items[Duality.Neutral].Add(offering);
            Items[Duality.Light].Add(offering);
        }
        public void AddDarkOffering(OfferingInfo offering) {
            Items[Duality.Neutral].Add(offering);
            Items[Duality.Dark].Add(offering);
        }

        public Duality GetMoonDualism(MoonPhase moonPhase) {
            switch (Main.GetMoonPhase()) {
                case MoonPhase.Full:
                case MoonPhase.ThreeQuartersAtLeft:
                case MoonPhase.ThreeQuartersAtRight:
                    return Duality.Light;

                case MoonPhase.Empty:
                case MoonPhase.QuarterAtLeft:
                case MoonPhase.QuarterAtRight:
                    return Duality.Dark;
            }

            return Duality.Neutral;
        }
        public Duality GetMoonDualism() {
            return GetMoonDualism(Main.GetMoonPhase());
        }

        public void GetHoverItem(out List<OfferingInfo> list, out int index) {
            list = Items[GetMoonDualism(Main.GetMoonPhase())];
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
            GetHoverItem(out var list, out var index);
            if (list[index].AcceptOffer(Main.LocalPlayer, i, j, true)) {
                return true;
            }

            for (int k = 0; k < list.Count; k++) {
                if (list[k].AcceptOffer(Main.LocalPlayer, k, j, false)) {
                    return true;
                }
            }
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            //var tile = Main.tile[i, j];
            //if (tile.TileFrameX == 54 && tile.TileFrameY == 108) {
            //    DrawMoon(i, j, spriteBatch);
            //}

            switch (GetMoonDualism(Main.GetMoonPhase())) {
                case Duality.Light:
                    ManualDraw(i, j, spriteBatch, 1);
                    return false;

                case Duality.Dark:
                    ManualDraw(i, j, spriteBatch, 2);
                    return false;
            }
            return true;
        }
    }
    public class PyramidStatueBroken1Tile : PyramidStatueTileBase {
    }
    public class PyramidStatueBroken2Tile : PyramidStatueTileBase {
    }
}