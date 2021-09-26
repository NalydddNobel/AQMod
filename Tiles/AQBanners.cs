using AQMod.Items.Placeable.Banners;
using AQMod.NPCs.Glimmer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles
{
    public sealed class AQBanners : ModTile
    {
        public const int Starite = 0;
        public const int SuperStarite = 1;
        public const int HyperStarite = 2;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.addTile(Type);
            dustType = -1;
            disableSmartCursor = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Banner");
            AddMapEntry(new Color(13, 88, 130), name);
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
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Player player = Main.LocalPlayer;
                switch (Main.tile[i, j].frameX / 18)
                {
                    case 0:
                    player.NPCBannerBuff[ModContent.NPCType<Starite>()] = true;
                    break;
                    case 1:
                    player.NPCBannerBuff[ModContent.NPCType<SuperStarite>()] = true;
                    break;
                    case 2:
                    player.NPCBannerBuff[ModContent.NPCType<HyperStarite>()] = true;
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