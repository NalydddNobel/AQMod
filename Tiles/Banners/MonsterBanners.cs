using Aequus.Common.Rendering.Tiles;
using Aequus.Content.Boss.UltraStariteMiniboss;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.CrabCrevice;
using Aequus.NPCs.Monsters.Night;
using Aequus.NPCs.Monsters.Night.Glimmer;
using Aequus.NPCs.Monsters.Sky.GaleStreams;
using Aequus.NPCs.Monsters.Underground;
using Aequus.NPCs.Monsters.Underworld;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Banners {
    public class MonsterBanners : ModTile {
        public const int StariteBanner = 0;
        public const int SuperStariteBanner = 1;
        public const int HyperStariteBanner = 2;
        public const int CrabFishBanner = 3;
        public const int HijivarchCrabBanner = 4;
        public const int SoldierCrabBanner = 5;
        public const int StriderCrabBanner = 6;
        public const int CinderaBanner = 7;
        public const int MagmabubbleBanner = 8;
        public const int TrapperImpBanner = 9;
        public const int VraineBanner = 10;
        public const int WhiteSlimeBanner = 11;
        public const int Unused_RedSpriteBanner = 12;
        public const int Unused_SpaceSquidBanner = 13;
        public const int UltraStariteBanner = 14;
        public const int CoconutCrabBanner = 15;
        public const int BreadofCthulhuBanner = 16;
        public const int BloodMimicBanner = 17;
        public const int TrapSkeletonBanner = 18;

        public static int BannerToItem(int style) {
            int npc = BannerToNPC(style);
            if (npc > NPCID.Count) {
                return NPCLoader.GetNPC(npc).BannerItem;
            }
            return 0;
        }

        public override void SetStaticDefaults() {
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
            AddMapEntry(new Color(13, 88, 130), TextHelper.GetText("MapObject.Banners"));
            if (!Main.dedServ) {
                SpecialTileRenderer.ModHangingVines.Add(Type, 3);
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) {
            int item = BannerToItem(frameX / 18);
            if (item != 0) {
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, item);
            }
        }

        public override void NearbyEffects(int i, int j, bool closer) {
            if (closer) {
                int style = Main.tile[i, j].TileFrameX / 18;
                int npcType = BannerToNPC(style);
                if (npcType != 0) {
                    int bannerItem = NPCLoader.GetNPC(npcType).BannerItem;
                    if (ItemID.Sets.BannerStrength.IndexInRange(bannerItem) && ItemID.Sets.BannerStrength[bannerItem].Enabled) {
                        Main.SceneMetrics.NPCBannerBuff[npcType] = true;
                        Main.SceneMetrics.hasBanner = true;
                        if (npcType == ModContent.NPCType<SummonerCrab>()) {
                            Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<SummonerCrabMinion>()] = true;
                            Main.SceneMetrics.hasBanner = true;
                        }
                    }
                }
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
            if (i % 2 == 1) {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
            if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0) {
                SpecialTileRenderer.AddSpecialPoint(Main.instance.TilesRenderer, i, j, 5);
            }

            return false;
        }

        public static int BannerToNPC(int style) {
            switch (style) {
                case StariteBanner:
                    return ModContent.NPCType<Starite>();
                case SuperStariteBanner:
                    return ModContent.NPCType<SuperStarite>();
                case HyperStariteBanner:
                    return ModContent.NPCType<SuperStarite>();
                case CinderaBanner:
                    return ModContent.NPCType<Cindera>();
                case MagmabubbleBanner:
                    return ModContent.NPCType<Magmabubble>();
                case VraineBanner:
                    return ModContent.NPCType<Vraine>();
                case WhiteSlimeBanner:
                    return ModContent.NPCType<WhiteSlime>();
                case UltraStariteBanner:
                    return ModContent.NPCType<UltraStarite>();
                case CrabFishBanner:
                    return ModContent.NPCType<CrabFish>();
                case HijivarchCrabBanner:
                    return ModContent.NPCType<SummonerCrab>();
                case SoldierCrabBanner:
                    return ModContent.NPCType<SoldierCrab>();
                case CoconutCrabBanner:
                    return ModContent.NPCType<CoconutCrab>();
                case BreadofCthulhuBanner:
                    return ModContent.NPCType<BreadOfCthulhu>();
                case BloodMimicBanner:
                    return ModContent.NPCType<BloodMimic>();
                case TrapSkeletonBanner:
                    return ModContent.NPCType<TrapSkeleton>();
            }
            return 0;
        }
    }
}