﻿using Aequus;
using Aequus.Common.Structures.PhysicsBehaviors;
using Aequus.Common.Utilities;
using Aequus.Common.Utilities.Helpers;
using System;
using Terraria.GameContent;

namespace Aequus.Content.Items.Accessories.FallSpeedHorseshoe;

public class WeightedHorseshoeVisual : ModProjectile {
    public VIStringTwoPoint<VINode> horseshoeAnvilRope;

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        //Projectile.ignoreWater = true;
    }

    public override void AI() {
        var player = Main.player[Projectile.owner];
        var aequus = player.GetModPlayer<AequusPlayer>();
        if (player.DeadOrGhost) {
            aequus.showHorseshoeAnvilRope = false;
        }
        if (aequus.showHorseshoeAnvilRope) {
            Projectile.timeLeft = 2;
        }
        float chainLength = 46f;
        Vector2 anvilAnchor = player.MountedCenter - player.velocity + new Vector2(0f, player.height / 2f - 12f + player.gfxOffY);
        Vector2 gravity = new Vector2(0f, 0.1f);
        float distance = Projectile.Distance(anvilAnchor);
        bool canHitLine = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height);
        if (distance > chainLength * 3f) {
            Projectile.Center = player.Center;
            horseshoeAnvilRope = null; // Reset segments
        }
        else if (distance > chainLength + 4f && !canHitLine || player.shimmering) {
            Projectile.tileCollide = false;
        }
        else {
            Projectile.tileCollide = true;
        }

        Vector2 endPosition = Projectile.Center + new Vector2(0f, Projectile.gfxOffY);
        horseshoeAnvilRope ??= new VIStringTwoPoint<VINode>(anvilAnchor, endPosition, 9, 4.33f, gravity);
        horseshoeAnvilRope.StartPos = anvilAnchor;
        horseshoeAnvilRope.EndPosition = endPosition;
        horseshoeAnvilRope.gravity = gravity;
        horseshoeAnvilRope.damping = Utils.GetLerpValue(20, 0, player.velocity.Length(), true) * 0.05f;
        horseshoeAnvilRope.Update();

        float wantedRotation = (Projectile.Center - horseshoeAnvilRope.segments[horseshoeAnvilRope.segments.Length / 5].Position).ToRotation();
        Projectile.rotation = Projectile.rotation.AngleTowards(wantedRotation, 0.1f);

        var groundTileCoordinates = Projectile.Bottom.ToTileCoordinates();
        var groundTile = Framing.GetTileSafely(groundTileCoordinates);
        if (groundTile.HasUnactuatedTile && Main.tileSolid[groundTile.TileType] && Math.Abs(Projectile.velocity.X) > 0.2f && Main.rand.NextBool(2)) {
            int d = WorldGen.KillTile_MakeTileDust(groundTileCoordinates.X, groundTileCoordinates.Y, groundTile);
            Main.dust[d].position = Projectile.Bottom;
            Main.dust[d].position.X += Main.rand.NextFloat(-4f, 4f);
            Main.dust[d].position.Y += Main.rand.NextFloat(-2f, 2f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 0.1f;
        }
        if (distance > chainLength || !Projectile.tileCollide) {
            Projectile.velocity += (anvilAnchor - Projectile.Center) * 0.03f;
        }
        else {
            if (Projectile.velocity.Y < 0f) {
                Projectile.velocity.Y *= 0.8f;
            }
            Projectile.velocity.X *= 0.8f;
        }

        if (Projectile.shimmerWet) {
            if (Projectile.velocity.Y > 0f) {
                Projectile.velocity.Y = 0f;
            }
            if (Framing.GetTileSafely(Projectile.Top.ToTileCoordinates()).HasShimmer()) {
                Projectile.velocity.Y -= 0.1f;
            }
            else {
                Projectile.velocity.Y *= 0.9f;
            }
        }
        else {
            Projectile.velocity.Y += 0.4f;
        }

        Vector4 collisionVector = Collision.WalkDownSlope(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height, 0.4f);
        Projectile.position.X = collisionVector.X;
        Projectile.position.Y = collisionVector.Y;
        Projectile.velocity.X = collisionVector.Z;
        Projectile.velocity.Y = collisionVector.W;
        Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY, (int)player.gravDir, player.controlUp);
        float terminalVelocity = Math.Max(6f, player.velocity.Y + 0.1f);
        if (Projectile.velocity.Y > terminalVelocity) {
            Projectile.velocity.Y = terminalVelocity;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 7f) {
            var groundTileCoordinates = (Projectile.Bottom + oldVelocity).ToTileCoordinates();
            var groundTile = Framing.GetTileSafely(groundTileCoordinates);
            if (groundTile.HasUnactuatedTile && Main.tileSolid[groundTile.TileType]) {
                for (int i = 0; i < 15; i++) {
                    int d = WorldGen.KillTile_MakeTileDust(groundTileCoordinates.X, groundTileCoordinates.Y, groundTile);
                    Main.dust[d].position = Projectile.Bottom;
                    Main.dust[d].position.X += Main.rand.NextFloat(-4f, 4f);
                    Main.dust[d].position.Y += Main.rand.NextFloat(-2f, 2f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 0f);
                }
            }
        }
        return false;
    }

    private Color GetStringColor(Vector2 stringStart, Vector2 stringEnd, Color baseColor) {
        return (LightingHelper.Get((stringStart + stringEnd) / 2f).MultiplyRGB(baseColor) * 0.75f) with { A = 255, };
    }

    public override bool PreDraw(ref Color lightColor) {
        if (horseshoeAnvilRope == null) {
            return false;
        }

        var player = Main.player[Projectile.owner];
        var AequusPlayer = player.GetModPlayer<AequusPlayer>();
        Main.instance.PrepareDrawnEntityDrawing(Projectile, AequusPlayer.cHorseshoeAnvil, null);
        var stringColor = DrawHelper.GetYoyoStringColor(player.stringColor);
        for (int i = 1; i < horseshoeAnvilRope.segments.Length; i++) {
            var start = horseshoeAnvilRope.segments[i].Position;
            var end = horseshoeAnvilRope.segments[i - 1].Position;
            DrawHelper.DrawLine(Main.EntitySpriteDraw, start - Main.screenPosition, end - Main.screenPosition, 2f, GetStringColor(start, end, stringColor));
        }
        DrawHelper.DrawLine(Main.EntitySpriteDraw, Projectile.Center - Main.screenPosition, horseshoeAnvilRope.segments[^1].Position - Main.screenPosition, 2f, GetStringColor(Projectile.Center, horseshoeAnvilRope.segments[^1].Position, stringColor));
        var texture = TextureAssets.Projectile[Type].Value;
        var effects = Math.Abs(Projectile.rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0f, 4f + Projectile.gfxOffY) - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, effects, 0f);
        return false;
    }
}