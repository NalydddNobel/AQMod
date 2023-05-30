using Aequus.Buffs;
using Aequus.Common.DataSets;
using Aequus.Common.Rendering.Tiles;
using Aequus.Content.NPCs.Critters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Misc {
    public class StariteBottle : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<StariteBottleTile>());
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient<DwarfStariteItem>()
                .TryRegisterAfter(ItemID.GoldButterflyCage);
        }
    }

    public class StariteBottleTile : ModTile, TileHooks.IGetTileDrawData, TileHooks.IGetLightOverride {
        public override void SetStaticDefaults() {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            DustType = -1;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(20, 166, 200), CreateMapEntryName());

            TileSets.AddTileRenderConversion(Type, TileID.HangingLanterns);
            if (!Main.dedServ) {
                SpecialTileRenderer.ModHangingVines[Type] = 2;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            r = 0.05f;
            g = 0.35f;
            b = 0.75f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<StariteBottle>());
        }

        public override void NearbyEffects(int i, int j, bool closer) {
            if (!Main.LocalPlayer.dead && closer && Main.tile[i, j].TileFrameY == 0) {
                int buffID = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<StariteBottleBuff>());
                if (buffID == -1 || Main.LocalPlayer.buffTime[buffID] < 10)
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<StariteBottleBuff>(), 180);
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        }

        public int GetFrame(int i, int j) {
            return (int)(Main.GameUpdateCount / 8 + i * i / j + j * j) % 2;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 36 == 0) {
                SpecialTileRenderer.AddSpecialPoint(i, j, 5);
            }
            return false;
        }

        void TileHooks.IGetTileDrawData.GetTileDrawData(TileDrawing self, int x, int y, Tile tileCache, ushort typeCache, ref short tileFrameX, ref short tileFrameY, ref int tileWidth, ref int tileHeight, ref int tileTop, ref int halfBrickHeight, ref int addFrX, ref int addFrY, ref SpriteEffects tileSpriteEffect, ref Texture2D glowTexture, ref Rectangle glowSourceRect, ref Color glowColor) {
            glowTexture = AequusTextures.StariteBottleTile_Glow;
            glowColor = Color.White;
            addFrX = 18 * GetFrame(x, y - Main.tile[x, y].TileFrameY / 18);
            glowSourceRect = new(tileFrameX + addFrX, tileFrameY + addFrY, tileWidth, tileHeight);
        }

        Color TileHooks.IGetLightOverride.GetLightOverride(TileDrawing self, int j, int i, Tile tileCache, ushort typeCache, short tileFrameX, short tileFrameY, Color tileLight) {
            return Color.White;
        }
    }
}