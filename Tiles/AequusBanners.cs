using Aequus.NPCs.Monsters.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles
{
    public sealed class AequusBanners : ModTile
    {
        public const int Starite = 0;
        public const int SuperStarite = 1;
        public const int HyperStarite = 2;
        public const int Unused_ArrowCrab = 3;
        public const int Unused_HermitCrab = 4;
        public const int SoliderCrabs = 5;
        public const int Unused_StriderCrab = 6;
        public const int Cindera = 7;
        public const int Magmabubble = 8;
        public const int TrapperImp = 9;
        public const int VraineBanner = 10;
        public const int WhiteSlimeBanner = 11;
        public const int Unused_RedSprite = 12;
        public const int Unused_SpaceSquid = 13;

        public static List<int> BannerTypesHack;

        #region Special thanks to turingcomplete30 for writing the code to support modded banners swaying in the wind!

        private static string SpecialThanks = "Special thanks to turingcomplete30 for writing the code to support modded banners swaying in the wind!";
        private static string SpecialThanks2 = "This entire code region is mostly taken from his Banner Tile code.";

        public override void Load()
        {
            SpecialThanks.Trim();
            SpecialThanks2.Trim();
            try
            {
                _addSpecialPointSpecialPositions = typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
                _addSpecialPointSpecialsCount = typeof(TileDrawing).GetField("_specialsCount", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            catch (Exception e)
            {
                Logging.PublicLogger.Debug(e);
            }

            BannerTypesHack = new List<int>();
            IL.Terraria.GameContent.Drawing.TileDrawing.DrawMultiTileVines += TileDrawing_DrawMultiTileVines;
        }
        private static void TileDrawing_DrawMultiTileVines(ILContext il)
        {

            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(9),
                i => i.MatchLdnull(),
                i => i.MatchCall(out _),
                i => i.MatchBrfalse(out _),
                i => i.MatchLdloca(9),
                i => i.MatchCall(out _),
                i => i.MatchBrfalse(out _)
                ))
                return;

            c.Emit(OpCodes.Ldloc, 9);
            c.EmitDelegate((Tile tile) => {
                if (BannerTypesHack.Contains(tile.TileType))
                {
                    return 3;
                }
                return 1;
            });
            c.Emit(OpCodes.Stloc, 8);
        }

        public override void Unload()
        {
            BannerTypesHack?.Clear();
            BannerTypesHack = null;
            _addSpecialPointSpecialPositions = null;
            _addSpecialPointSpecialsCount = null;
        }

        private static FieldInfo _addSpecialPointSpecialPositions;
        private static FieldInfo _addSpecialPointSpecialsCount;

        public static void AddSpecialPoint(TileDrawing renderer, int x, int y, int type)
        {
            if (_addSpecialPointSpecialPositions?.GetValue(renderer) is Point[][] _specialPositions)
            {
                if (_addSpecialPointSpecialsCount?.GetValue(renderer) is int[] _specialsCount)
                {
                    _specialPositions[type][_specialsCount[type]++] = new Point(x, y);
                }
            }
        }

        public static int BannerToItem(int style)
        {
            int npc = BannerToNPC(style);
            if (npc > Main.maxNPCTypes)
            {
                return NPCLoader.GetNPC(npc).BannerItem;
            }
            return 0;
        }

        #endregion

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            DustType = -1;
            TileID.Sets.DisableSmartCursor[Type] = true;
            AddMapEntry(new Color(13, 88, 130), CreateMapEntryName("Banners"));
            BannerTypesHack.Add(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int item = BannerToItem(frameX / 18);
            if (item != 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, item);
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                int style = Main.tile[i, j].TileFrameX / 18;
                int npcType = BannerToNPC(style);
                if (npcType != 0)
                {
                    int bannerItem = NPCLoader.GetNPC(npcType).BannerItem;
                    if (ItemID.Sets.BannerStrength.IndexInRange(bannerItem) && ItemID.Sets.BannerStrength[bannerItem].Enabled)
                    {
                        Main.SceneMetrics.NPCBannerBuff[npcType] = true;
                        Main.SceneMetrics.hasBanner = true;
                    }
                }
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            /*Main.LightingEveryFrame*/
            if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0)
            {
                AddSpecialPoint(Main.instance.TilesRenderer, i, j, 5);
            }

            return false;
        }

        public static int BannerToNPC(int style)
        {
            switch (style)
            {
                case VraineBanner:
                    return ModContent.NPCType<Vraine>();
                case WhiteSlimeBanner:
                    return ModContent.NPCType<WhiteSlime>();
            }
            return 0;
        }
    }
}