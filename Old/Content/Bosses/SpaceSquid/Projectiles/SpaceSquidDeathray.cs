﻿using Aequu2.Old.Core;
using System;

namespace Aequu2.Old.Content.Bosses.SpaceSquid.Projectiles;

public class SpaceSquidDeathray : ModProjectile {
    public const int DEATHRAY_LENGTH = 2000;

    public TrailRenderer prim;
    public TrailRenderer smokePrim;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults() {
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.hostile = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.netImportant = true;
        Projectile.manualDirectionChange = true;
        Projectile.coldDamage = true;
    }

    public override void AI() {
        if (Projectile.direction == 0) {
            if (Main.netMode == NetmodeID.SinglePlayer) {
                Projectile.direction = Main.npc[(int)(Projectile.ai[0] - 1)].direction;
            }
            Projectile.netUpdate = true;
        }
        if (Main.expertMode) {
            if ((int)Projectile.ai[1] == 0) {
                Projectile.width = (int)(Projectile.width * 1.5f);
                Projectile.height = (int)(Projectile.height * 1.5f);
                Projectile.ai[1]++;
                Projectile.netUpdate = true;
            }
        }
        if ((int)(Projectile.ai[0] - 1) > -1) {
            if (!Main.npc[(int)(Projectile.ai[0] - 1)].active) {
                Projectile.Kill();
            }
            if (Main.npc[(int)(Projectile.ai[0] - 1)].ai[1] > 300f) {
                Projectile.height -= 2;
                if (Main.expertMode) {
                    Projectile.height -= 1;
                    Projectile.netUpdate = true;
                }
                if (Projectile.height < 2 || Main.npc[(int)(Projectile.ai[0] - 1)].ai[1] > 328f) {
                    Projectile.Kill();
                }
            }
            Projectile.Center = (Main.npc[(int)(Projectile.ai[0] - 1)].ModNPC as SpaceSquid).GetEyePos() + new Vector2(Projectile.direction * 10f, 0f);
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        if (Projectile.direction == -1) {
            projHitbox.X -= DEATHRAY_LENGTH + projHitbox.Width;
            projHitbox.Width = DEATHRAY_LENGTH;
            if (targetHitbox.Intersects(projHitbox)) {
                return true;
            }
        }
        else {
            projHitbox.Width += DEATHRAY_LENGTH;
            if (targetHitbox.Intersects(projHitbox)) {
                return true;
            }
        }
        return base.Colliding(projHitbox, targetHitbox);
    }

    public override bool PreDraw(ref Color lightColor) {
        var drawPos = Projectile.Center - Main.screenPosition + new Vector2(Projectile.direction * 40f, 0f);
        var drawColor = new Color(10, 200, 80, 0);
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
        var arr = new Vector2[] {
                drawPos,
                drawPos + new Vector2(Main.screenWidth * Projectile.direction, 0f),
                drawPos + new Vector2(Main.screenWidth * 2f * Projectile.direction, 0f), };
        if (prim == null) {
            prim = new TrailRenderer(Aequu2Textures.Trail.Value, "Texture", (p) => new Vector2(Projectile.height * (1f - p * p)), (p) => drawColor * (1f - p), obeyReversedGravity: false, worldTrail: false);
        }
        if (smokePrim == null) {
            smokePrim = new TrailRenderer(Aequu2Textures.Trail3.Value, "Texture", (p) => new Vector2(Projectile.height), (p) => drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f) * (1f - p), obeyReversedGravity: false, worldTrail: false);
        }
        if (Main.LocalPlayer.gravDir == -1) {
            Helper.ScreenFlip(arr);
        }
        var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f);
        int amount = (int)(5 * (Aequu2.HighQualityEffects ? 1f : 0.5f));
        var initialArr = new Vector2[amount];
        var center = Projectile.Center;

        initialArr[0] = arr[0];
        for (int i = 1; i < amount; i++) {
            initialArr[i] = drawPos + new Vector2(60f / amount * i * -Projectile.direction, 0f);
        }
        if (Main.LocalPlayer.gravDir == -1) {
            Helper.ScreenFlip(initialArr);
        }
        // funny prim shenanigans
        prim.Draw(initialArr);
        smokePrim.Draw(initialArr, -Main.GlobalTimeWrappedHourly * 2f, 4f);
        prim.Draw(arr);
        smokePrim.Draw(arr, -Main.GlobalTimeWrappedHourly, 2f);

        var spotlight = Aequu2Textures.Bloom;
        Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.4f, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * (Projectile.height / 32f), SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.5f * (Projectile.height / 32f), SpriteEffects.None, 0f);
        return false;
    }
}