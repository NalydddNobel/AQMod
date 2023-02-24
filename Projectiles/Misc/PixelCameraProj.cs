using Aequus.Items;
using Aequus.NPCs.CarpenterNPC.Rewards;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class PixelCameraProj : ShutterstockerProj
    {
        public class StateID
        {
            public const int State_2x2 = 0;
            public const int State_2x3 = 1;
            public const int State_3x2 = 2;
            public const int State_3x3 = 3;
            public const int State_6x4 = 4;

            public const int Count = 5;
        }

        public static Point[] DimensionsForState;

        /// <summary>
        /// A better naming scheme for how this camera functions
        /// </summary>
        public virtual int PhotoState => (int)PhotoSize;

        public override float PhotoSize { get => base.PhotoSize; set => Projectile.ai[0] = (value + StateID.Count) % StateID.Count; }

        public override int PhotoSizeX => DimensionsForState[PhotoState].X * 8;
        public override int PhotoSizeY => DimensionsForState[PhotoState].Y * 8;
        public override int ClipPaddingX => 0;
        public override int ClipPaddingY => 0;

        public override void Load()
        {
            base.Load();
            DimensionsForState = new Point[StateID.Count]
            {
                new Point(2, 2),
                new Point(2, 3),
                new Point(3, 2),
                new Point(3, 3),
                new Point(6, 4),
            };
        }

        public override void SpawnClipItem(Rectangle tilesCaptured)
        {
            Item item;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                item = AequusItem.SetDefaults<PixelCameraClip>(checkMaterial: false);
            }
            else
            {
                int i = Item.NewItem(Main.player[Projectile.owner].GetSource_ItemUse_WithPotentialAmmo(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.useAmmo, "Shutterstock Photo Creation"), Main.player[Projectile.owner].getRect(),
                    ModContent.ItemType<PixelCameraClip>());
                if (i == -1)
                {
                    return;
                }
                item = Main.item[i];
            }

            item.ModItem<PixelCameraClip>().SetClip(tilesCaptured);
            item.ModItem<PixelCameraClip>().photoState = PhotoState;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                item.ModItem<PixelCameraClip>().OnMissingTooltipTexture();
            }
            else
            {
                var p = Aequus.GetPacket(PacketType.SpawnPixelCameraClip);
                p.Write(Projectile.owner);
                p.Write(PhotoState);
                item.ModItem<PixelCameraClip>().NetSend(p);
                p.Send();
            }
        }
    }
}