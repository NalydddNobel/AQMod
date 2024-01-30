﻿namespace Aequus.Common.Projectiles;

public class ProjectileAISnippets {
    public static void GenericPetGroundMovement(Projectile projectile, Vector2 to, System.Single moveSpeed = 0.08f, System.Single moveSpeedCap = 6.5f) {
        System.Boolean flag8 = false;
        System.Boolean flag9 = false;
        System.Boolean flag11 = false;
        System.Single num68 = moveSpeed;
        System.Single num69 = moveSpeedCap;
        if (to.X < projectile.position.X + projectile.width / 2f - 16) {
            flag8 = true;
        }
        else if (to.X > projectile.position.X + (System.Single)(projectile.width / 2f) + 16) {
            flag9 = true;
        }
        if (flag8) {
            if (projectile.velocity.X > -3.5f) {
                projectile.velocity.X -= num68;
            }
            else {
                projectile.velocity.X -= num68 * 0.25f;
            }
        }
        else if (flag9) {
            if (projectile.velocity.X < 3.5f) {
                projectile.velocity.X += num68;
            }
            else {
                projectile.velocity.X += num68 * 0.25f;
            }
        }
        else {
            projectile.velocity.X *= 0.9f;
            if (projectile.velocity.X >= 0f - num68 && projectile.velocity.X <= num68) {
                projectile.velocity.X = 0f;
            }
        }
        if (flag8 || flag9) {
            System.Int32 num70 = (System.Int32)(projectile.position.X + projectile.width / 2) / 16;
            System.Int32 j2 = (System.Int32)(projectile.position.Y + projectile.height / 2) / 16;
            if (flag8) {
                num70--;
            }
            if (flag9) {
                num70++;
            }
            num70 += (System.Int32)projectile.velocity.X;
            if (WorldGen.SolidTile(num70, j2)) {
                flag11 = true;
            }
        }
        Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY);
        if (projectile.velocity.Y == 0f) {
            if (flag11) {
                System.Int32 num72 = (System.Int32)(projectile.position.X + projectile.width / 2) / 16;
                System.Int32 num73 = (System.Int32)(projectile.position.Y + projectile.height) / 16;
                if (WorldGen.SolidTileAllowBottomSlope(num72, num73) || Main.tile[num72, num73].IsHalfBlock || Main.tile[num72, num73].Slope > 0) {
                    try {
                        num72 = (System.Int32)(projectile.position.X + projectile.width / 2) / 16;
                        num73 = (System.Int32)(projectile.position.Y + projectile.height / 2) / 16;
                        if (flag8) {
                            num72--;
                        }
                        if (flag9) {
                            num72++;
                        }
                        num72 += (System.Int32)projectile.velocity.X;
                        if (!WorldGen.SolidTile(num72, num73 - 1) && !WorldGen.SolidTile(num72, num73 - 2)) {
                            projectile.velocity.Y = -5.1f;
                        }
                        else if (!WorldGen.SolidTile(num72, num73 - 2)) {
                            projectile.velocity.Y = -7.1f;
                        }
                        else if (WorldGen.SolidTile(num72, num73 - 5)) {
                            projectile.velocity.Y = -11.1f;
                        }
                        else if (WorldGen.SolidTile(num72, num73 - 4)) {
                            projectile.velocity.Y = -10.1f;
                        }
                        else {
                            projectile.velocity.Y = -9.1f;
                        }
                    }
                    catch {
                        projectile.velocity.Y = -9.1f;
                    }
                }
            }
        }
        if (projectile.velocity.X > num69) {
            projectile.velocity.X = num69;
        }
        if (projectile.velocity.X < 0f - num69) {
            projectile.velocity.X = 0f - num69;
        }
        if (projectile.velocity.X < 0f) {
            projectile.direction = -1;
        }
        if (projectile.velocity.X > 0f) {
            projectile.direction = 1;
        }
        if (projectile.velocity.X > num68 && flag9) {
            projectile.direction = 1;
        }
        if (projectile.velocity.X < 0f - num68 && flag8) {
            projectile.direction = -1;
        }
    }
}