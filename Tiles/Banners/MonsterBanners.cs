using Aequus.Common.Drawing;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.BloodMoon;
using Aequus.NPCs.Monsters.DemonSiege;
using Aequus.NPCs.Monsters.GaleStreams;
using Aequus.NPCs.Monsters.Glimmer;
using Aequus.NPCs.Monsters.Glimmer.UltraStarite;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Aequus.Tiles.Banners;
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

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.Platform, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.StyleWrapLimit = 111;
        TileObjectData.addTile(Type);
        DustType = -1;
        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(13, 88, 130), TextHelper.GetText("MapObject.Banners"));
        TileDrawHooks.Instance.HangingTile[Type] = 3;
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

#if !CRAB_CREVICE_DISABLE
                    if (npcType == ModContent.NPCType<global::Aequus.NPCs.Monsters.CrabCrevice.SummonerCrab>()) {
                        Main.SceneMetrics.NPCBannerBuff[ModContent.NPCType<global::Aequus.NPCs.Monsters.CrabCrevice.SummonerCrabMinion>()] = true;
                        Main.SceneMetrics.hasBanner = true;
                    }
#endif
                }
            }
        }
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
        if (i % 2 == 1) {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0) {
            TileDrawHooks.Instance.AddSpecialPoint(i, j, 5);
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
#if !CRAB_CREVICE_DISABLE
            case CrabFishBanner:
                return ModContent.NPCType<global::Aequus.NPCs.Monsters.CrabCrevice.CrabFish>();
            case HijivarchCrabBanner:
                return ModContent.NPCType<global::Aequus.NPCs.Monsters.CrabCrevice.SummonerCrab>();
            case SoldierCrabBanner:
                return ModContent.NPCType<global::Aequus.NPCs.Monsters.CrabCrevice.SoldierCrab>();
            case CoconutCrabBanner:
                return ModContent.NPCType<global::Aequus.NPCs.Monsters.CrabCrevice.CoconutCrab>();
#endif
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