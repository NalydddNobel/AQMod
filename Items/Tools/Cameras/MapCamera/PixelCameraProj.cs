using Aequus.Items.Tools.Cameras.CarpenterCamera;
using Aequus.Items.Tools.Cameras.MapCamera.Clip;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Cameras.MapCamera;

public class PixelCameraProj : CameraShootProj {
    public class StateID {
        public const int State_2x2 = 0;
        public const int State_2x3 = 1;
        public const int State_3x2 = 2;
        public const int State_3x3 = 3;
        public const int State_6x4 = 4;

        public const int Count = 5;
    }

    public readonly static Point[] StateDimensions = new Point[StateID.Count] {
        new Point(2, 2),
        new Point(2, 3),
        new Point(3, 2),
        new Point(3, 3),
        new Point(6, 4),
    };

    /// <summary>
    /// A better naming scheme for how this camera functions
    /// </summary>
    public int PhotoState => Math.Clamp((int)Projectile.ai[0] - 1, 0, StateDimensions.Length);

    public override int PhotoSizeX => StateDimensions[PhotoState].X * 8;
    public override int PhotoSizeY => StateDimensions[PhotoState].Y * 8;

    public void SpawnClipItem(Rectangle tilesCaptured) {
        Item item;
        if (Main.netMode != NetmodeID.SinglePlayer) {
            item = new(ModContent.ItemType<PixelCameraClip>());
        }
        else {
            int i = Item.NewItem(Main.player[Projectile.owner].GetSource_ItemUse_WithPotentialAmmo(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.useAmmo), Main.player[Projectile.owner].getRect(),
                ModContent.ItemType<PixelCameraClip>());
            if (i == -1) {
                return;
            }
            item = Main.item[i];
        }

        var modItem = item.ModItem as PixelCameraClip;
        modItem.SetClip(tilesCaptured);
        modItem.photoState = PhotoState;
        if (Main.netMode == NetmodeID.SinglePlayer) {
            modItem.OnMissingTooltipTexture();
        }
        else {
            var p = Aequus.GetPacket(PacketType.SpawnPixelCameraClip);
            p.Write(Projectile.owner);
            p.Write(PhotoState);
            modItem.NetSend(p);
            p.Send();
        }
    }

    protected override void Initialize() {
    }

    protected override void SnapPhoto() {
        var player = Main.player[Projectile.owner];
        player.Aequus().SetCooldown(300, ignoreStats: true, Main.player[Projectile.owner].HeldItemFixed());
        if (Main.myPlayer == Projectile.owner && player.ConsumeItem(ModContent.ItemType<PixelCameraClipAmmo>())) {
            SpawnClipItem(Area);
        }
    }

    protected override void UpdateScrollWheel(int scrollAmount) {
        Projectile.ai[0] = (int)Projectile.ai[0] + Math.Sign(scrollAmount);
        if (Projectile.ai[0] <= 0f) {
            Projectile.ai[0] = StateDimensions.Length;
        }
        else if (Projectile.ai[0] > StateDimensions.Length) {
            Projectile.ai[0] = 1f;
        }
    }
}