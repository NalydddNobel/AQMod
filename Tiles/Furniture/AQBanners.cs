using AQMod.Items.Placeable.Banners;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters.CrabSeason;
using AQMod.NPCs.Monsters.DemonSiege;
using AQMod.NPCs.Monsters.GaleStreams;
using AQMod.NPCs.Monsters.GlimmerEvent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public sealed class AQBanners : ModTile
    {
        public const int Starite = 0;
        public const int SuperStarite = 1;
        public const int HyperStarite = 2;
        public const int ArrowCrab = 3;
        public const int HermitCrab = 4;
        public const int SoliderCrabs = 5;
        public const int StriderCrab = 6;
        public const int Cindera = 7;
        public const int Magmabubble = 8;
        public const int TrapperImp = 9;
        public const int Vraine = 10;
        public const int SolarSlime = 11;
        public const int RedSprite = 12;
        public const int SpaceSquid = 13;

        public override void SetDefaults()
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
            dustType = -1;
            disableSmartCursor = true;
            AddMapEntry(new Color(13, 88, 130), CreateMapEntryName("Banners"));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            switch (frameX / 18)
            {
                case 0:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<StariteBanner>());
                    break;
                case 1:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<SuperStariteBanner>());
                    break;
                case 2:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<HyperStariteBanner>());
                    break;
                case ArrowCrab:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<ArrowCrabBanner>());
                    break;
                case HermitCrab:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<HermitCrabBanner>());
                    break;
                case SoliderCrabs:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<SoliderCrabsBanner>());
                    break;
                case StriderCrab:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<SoliderCrabsBanner>());
                    break;
                case Cindera:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<CinderaBanner>());
                    break;
                case Magmabubble:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<MagmabubbleBanner>());
                    break;
                case TrapperImp:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<TrapperImpBanner>());
                    break;
                case Vraine:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<VraineBanner>());
                    break;
                case SolarSlime:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<WhiteSlimeBanner>());
                    break;
                case RedSprite:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<RedSpriteBanner>());
                    break;
                case SpaceSquid:
                    Item.NewItem(i * 16, j * 16, 16, 48, ModContent.ItemType<SpaceSquidBanner>());
                    break;
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Player player = Main.LocalPlayer;
                switch (Main.tile[i, j].frameX / 18)
                {
                    case Starite:
                        player.NPCBannerBuff[ModContent.NPCType<Starite>()] = true;
                        break;
                    case SuperStarite:
                        player.NPCBannerBuff[ModContent.NPCType<SuperStarite>()] = true;
                        break;
                    case HyperStarite:
                        player.NPCBannerBuff[ModContent.NPCType<HyperStarite>()] = true;
                        break;
                    case ArrowCrab:
                        player.NPCBannerBuff[ModContent.NPCType<ArrowCrab>()] = true;
                        break;
                    case HermitCrab:
                        player.NPCBannerBuff[ModContent.NPCType<HermitCrab>()] = true;
                        break;
                    case SoliderCrabs:
                        player.NPCBannerBuff[ModContent.NPCType<SoliderCrabs>()] = true;
                        break;
                    case StriderCrab:
                        player.NPCBannerBuff[ModContent.NPCType<StriderCrab>()] = true;
                        break;
                    case Cindera:
                        player.NPCBannerBuff[ModContent.NPCType<Cindera>()] = true;
                        break;
                    case Magmabubble:
                        player.NPCBannerBuff[ModContent.NPCType<Magmalbubble>()] = true;
                        break;
                    case TrapperImp:
                        player.NPCBannerBuff[ModContent.NPCType<TrapImp>()] = true;
                        player.NPCBannerBuff[ModContent.NPCType<Trapper>()] = true;
                        break;

                    case Vraine:
                        player.NPCBannerBuff[ModContent.NPCType<Vraine>()] = true;
                        break;

                    case SolarSlime:
                        player.NPCBannerBuff[ModContent.NPCType<WhiteSlime>()] = true;
                        break;

                    case RedSprite:
                        player.NPCBannerBuff[ModContent.NPCType<RedSprite>()] = true;
                        break;

                    case SpaceSquid:
                        player.NPCBannerBuff[ModContent.NPCType<SpaceSquid>()] = true;
                        break;

                    default:
                        return;
                }
                player.hasBanner = true;
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}