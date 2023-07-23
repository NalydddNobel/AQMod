using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.Cameras;

public abstract class CameraShootProj : ModProjectile {
    public Vector2 mouseWorld;

    public abstract int PhotoSizeX { get; }
    public abstract int PhotoSizeY { get; }
    public Rectangle Area => new((int)(Projectile.Center.X / 16f) - PhotoSizeX / 2, (int)(Projectile.Center.Y / 16f) - PhotoSizeY / 2, PhotoSizeX, PhotoSizeY);

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
    }

    protected abstract void Initialize();

    protected abstract void SnapPhoto();

    protected abstract void UpdateScrollWheel(int scrollAmount);

    public override void AI() {
        var player = Main.player[Projectile.owner];

        Projectile.timeLeft = 2;

        Helper.ShootRotation(Projectile, MathHelper.WrapAngle((Projectile.Center - Main.player[Projectile.owner].Center).ToRotation() + (float)Math.PI / 2f));
        player.itemRotation = Projectile.DirectionTo(player).ToRotation();

        Projectile.velocity *= 0.9f;
        if ((int)Projectile.ai[1] > 0) {
            if ((int)Projectile.ai[1] == 1) {
                SoundEngine.PlaySound(SoundID.Camera);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] > 30f) {
                Projectile.Kill();
                return;
            }

            if (Main.myPlayer != Projectile.owner) {
                return;
            }
            //if ((int)Projectile.ai[1] == 30) {
            //    Projectile.velocity.Y = -10f;
            //}
            //if ((int)Projectile.ai[1] >= 30 && (int)Projectile.ai[1] < 60) {
            //    Projectile.scale *= 1.01f;
            //}
            if ((int)Projectile.ai[1] >= 20) {
                Projectile.scale *= 0.9f;
                Projectile.velocity = (Main.player[Projectile.owner].Center - Projectile.Center) / 10f;
            }
            return;
        }

        Projectile.velocity = Vector2.Zero;
        if ((int)Projectile.ai[0] == 0) {
            Projectile.ai[0] = 1f;
            Initialize();
        }

        if (!player.channel || !player.controlUseItem) {
            Projectile.ai[1] = 1f;
            SnapPhoto();
            return;
        }

        if (Main.myPlayer == Projectile.owner) {
            if (Main.mouseRight && Main.mouseRightRelease) {
                Projectile.Kill();
                return;
            }

            var oldMouseWorld = player.MouseWorld();
            mouseWorld = player.MouseWorld();
            if (mouseWorld.X != oldMouseWorld.X || mouseWorld.Y != oldMouseWorld.Y) {
                Projectile.netUpdate = true;
            }
            int scrollWheel = PlayerInput.ScrollWheelDelta / 120;
            if (scrollWheel != 0) {
                UpdateScrollWheel(scrollWheel);
                Projectile.netUpdate = true;
            }
        }

        Projectile.Center = mouseWorld.ToTileCoordinates().ToWorldCoordinates();
    }

    protected virtual void CustomDraw(Color color, Vector2 camStart, Vector2 camEnd, Vector2 size, Vector2 topLeftEnd) {
    }

    public override bool PreDraw(ref Color lightColor) {
        var player = Main.player[Projectile.owner];
        var color = Color.White with { A = 100 } * 0.6f * Projectile.scale;
        var camStart = player.Center + player.DirectionTo(Projectile) * 20f - Main.screenPosition;
        var camEnd = Projectile.Center - Main.screenPosition;
        if (PhotoSizeX % 2 == 0) {
            camEnd.X -= 8f;
        }
        if (PhotoSizeY % 2 == 0) {
            camEnd.Y -= 8f;
        }

        var size = new Vector2(PhotoSizeX * 16f, PhotoSizeY * 16f) / 2f * Projectile.scale;
        float startOffset = 8f;

        var bottomRightStart = camStart + new Vector2(startOffset, startOffset);
        var bottomRightEnd = camEnd + size;
        var bottomLeftStart = camStart + new Vector2(-startOffset, startOffset);
        var bottomLeftEnd = camEnd + size with { X = -size.X };
        var topRightStart = camStart + new Vector2(startOffset, -startOffset);
        var topRightEnd = camEnd + size with { Y = -size.Y };
        var topLeftStart = camStart + new Vector2(-startOffset, -startOffset);
        var topLeftEnd = camEnd - size;

        Helper.DrawLine(bottomRightStart, bottomRightEnd, 2f, color * 0.1f);
        Helper.DrawLine(bottomLeftStart, bottomLeftEnd, 2f, color * 0.1f);
        Helper.DrawLine(topRightStart, topRightEnd, 2f, color * 0.1f);
        Helper.DrawLine(topLeftStart, topLeftEnd, 2f, color * 0.1f);
        //Helper.DrawLine(topLeftStart, bottomLeftStart, 2f, color);
        //Helper.DrawLine(topRightStart, bottomRightStart, 2f, color);
        //Helper.DrawLine(topRightStart, topLeftStart, 2f, color);
        //Helper.DrawLine(bottomRightStart, bottomLeftStart, 2f, color);
        Helper.DrawLine(topLeftEnd, bottomLeftEnd, 2f, color);
        Helper.DrawLine(topRightEnd, bottomRightEnd, 2f, color);
        Helper.DrawLine(topRightEnd, topLeftEnd, 2f, color);
        Helper.DrawLine(bottomRightEnd, bottomLeftEnd, 2f, color);

        if ((int)Projectile.ai[1] > 0) {
            if (Projectile.ai[1] < 8f) {
                Helper.DrawRectangle(new Rectangle((int)topLeftEnd.X, (int)topLeftEnd.Y, (int)(topRightEnd.X - topLeftEnd.X), (int)(bottomLeftEnd.Y - topLeftEnd.Y)), Color.White * MathF.Sin(1f - Projectile.ai[1] / 8f));
            }

            if (Main.myPlayer != Projectile.owner) {
                return false;
            }

            return false;
        }

        CustomDraw(color, camStart, camEnd, size, topLeftEnd);
        return false;
    }
}