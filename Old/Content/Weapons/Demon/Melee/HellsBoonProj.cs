using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Old.Content.Weapons.Demon.Melee;

public class HellsBoonProj : ModProjectile {
    private const Int32 GO_OUT_TIME = 15;

    public override void SetStaticDefaults() {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.hide = true;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
    }

    public override Boolean? CanCutTiles() => Projectile.ai[1] > 15f && Projectile.ai[1] < 15f + GO_OUT_TIME;

    public override void AI() {
        if (Projectile.frameCounter == 0) {
            Projectile.frameCounter++;
            Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
            Projectile.spriteDirection = Main.rand.NextBool() ? -1 : 1;
        }
        if (Projectile.localAI[0] == 0f) {
            Projectile.localAI[0] = Projectile.position.X;
            Projectile.localAI[1] = Projectile.position.Y;
        }
        if (Projectile.ai[1] > 15f) {
            if (Projectile.ai[1] < 15f + GO_OUT_TIME) {
                Projectile.ai[1] += Main.rand.NextFloat(1f, 2.5f);
                if (Main.netMode != NetmodeID.Server && (Int32)Projectile.ai[1] >= 15 + GO_OUT_TIME) {
                    SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.5f, Pitch = 0.05f }, Projectile.Center);
                }
                Single progress = (Projectile.ai[1] - 15f) / GO_OUT_TIME;
                Projectile.ai[0] = MathHelper.Lerp(0f, 80f, progress);
                Projectile.timeLeft += Main.rand.Next(12);
            }
        }
        else {
            if (Projectile.ai[1] == 0f) {
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
            }
            Projectile.ai[1] += Main.rand.NextFloat(1f) + 0.01f;
            if (Main.rand.NextBool(6)) {
                Int32 d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 36 : 17);
                Main.dust[d].velocity = Projectile.velocity * 2f;
                Main.dust[d].noGravity = true;
            }
        }
        Projectile.position = new Vector2(Projectile.localAI[0] + Projectile.velocity.X * Projectile.ai[0], Projectile.localAI[1] + Projectile.velocity.Y * Projectile.ai[0]);
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override void OnKill(Int32 timeLeft) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.25f, Pitch = 0.5f }, Projectile.Center);

        for (Int32 i = 0; i < 10; i++) {
            var d = Dust.NewDustDirect(Projectile.position - Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(90f), Projectile.width, Projectile.height, Main.rand.NextBool() ? 36 : 17, Scale: Main.rand.NextFloat(0.6f, 1f));
            if (Main.rand.NextBool()) {
                d.noGravity = true;
            }
        }
    }

    public override Boolean PreDraw(ref Color lightColor) {
        lightColor = ExtendLight.Get(Projectile.Center);
        var texture = TextureAssets.Projectile[Type].Value;
        Int32 frameWidth = texture.Width / Main.projFrames[Projectile.type];
        Int32 frameHeight = (Int32)Projectile.ai[0] + 8;
        if (frameHeight > texture.Height) {
            frameHeight = texture.Height;
        }
        var drawData = new DrawData(texture, Projectile.Center - Main.screenPosition, new Rectangle(frameWidth * Projectile.frame, 0, frameWidth - 2, frameHeight), lightColor.MaxRGBA(200), Projectile.rotation + MathHelper.PiOver2, new Vector2(frameWidth / 2f, 16f), Projectile.scale, SpriteEffects.None, 0);
        if (Projectile.ai[0] > 42f) {
            drawData.scale.Y += (Projectile.ai[0] - 42f) / 66f;
        }

        //if (Aequus.highQualityEffects) {
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.BeginWorld(shader: true);
        //    var effect = SpikeFade.ShaderData;
        //    effect.UseOpacity(1f / texture.Height * frameHeight + _portaloffset);
        //    effect.UseColor(new Vector3(0.65f, 0.3f, 1f));
        //    effect.Apply(drawData);
        //    drawData.Draw(Main.spriteBatch);
        //    drawData.Draw(Main.spriteBatch);
        //    Main.spriteBatch.End();
        //    Main.spriteBatch.BeginWorld(shader: false); ;
        //}
        //else {
        Single alpha = 0f;
        if (Projectile.ai[1] > 15f) {
            if (Projectile.ai[1] < 15f + GO_OUT_TIME) {
                alpha = (Projectile.ai[1] - 15f) / GO_OUT_TIME;
            }
            else {
                alpha = 1f;
            }
        }
        drawData.color *= alpha;
        drawData.Draw(Main.spriteBatch);
        //}
        return false;
    }

    public override void DrawBehind(Int32 index, List<Int32> behindNPCsAndTiles, List<Int32> behindNPCs, List<Int32> behindProjectiles, List<Int32> overPlayers, List<Int32> overWiresUI) {
        behindNPCsAndTiles.Add(index);
    }
}

public class HellsBoonSpawner : ModProjectile {
    public override String Texture => AequusTextures.TemporaryBuffIcon;

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.friendly = true;
        Projectile.hide = true;
        Projectile.timeLeft = 2;
    }

    public override Boolean? CanDamage() {
        return false;
    }

    public override void AI() {
        if (Main.myPlayer != Projectile.owner) {
            return;
        }
        Int32 size = (Int32)(35f * Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee));
        Int32 projectileCount = size / 4;
        var tilePos = Projectile.Center.ToTileCoordinates();
        var sizeCorner = new Point(tilePos.X - size / 2, tilePos.Y - size / 2);

        sizeCorner.X = Math.Clamp(sizeCorner.X, size + 5, Main.maxTilesX - size - 5);
        sizeCorner.Y = Math.Clamp(sizeCorner.Y, size + 5, Main.maxTilesY - size - 5);

        var validSpots = new List<Point>();
        for (Int32 i = 0; i < size; i++) {
            for (Int32 j = 0; j < size; j++) {
                Int32 tileX = sizeCorner.X + i;
                Int32 tileY = sizeCorner.Y + j;
                if (Main.tile[tileX, tileY].IsSolid()) {
                    if (Main.tile[tileX + 1, tileY].IsSolid() &&
                        Main.tile[tileX - 1, tileY].IsSolid() &&
                        Main.tile[tileX, tileY + 1].IsSolid() &&
                        Main.tile[tileX, tileY - 1].IsSolid()) {
                        continue;
                    }
                    var pos = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f);
                    pos += Vector2.Normalize(Projectile.Center - pos) * 18f;
                    if (Collision.CanHitLine(Projectile.Center, 2, 2, pos, 0, 0)) {
                        validSpots.Add(new Point(tileX, tileY));
                    }
                }
            }
        }
        if (validSpots.Count <= 0) {
            return;
        }
        if (validSpots.Count < projectileCount) {
            projectileCount = validSpots.Count;
        }
        var source = Projectile.GetSource_FromAI();
        for (Int32 i = 0; i < projectileCount; i++) {
            Int32 random = Main.rand.Next(validSpots.Count);
            var spawnPosition = new Vector2(validSpots[random].X * 16 + 8f, validSpots[random].Y * 16f + 8f);
            Projectile.NewProjectile(source, spawnPosition, Vector2.Normalize(Projectile.Center - spawnPosition).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<HellsBoonProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        Projectile.Kill();
    }
}
