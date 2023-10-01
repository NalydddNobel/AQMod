using Aequus;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.Yoyo.Pistachiyo;

public class PistachiyoVineProj : ModProjectile {
    public override string Texture => AequusTextures.NPC(NPCID.PlanterasTentacle);

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = Main.npcFrameCount[NPCID.PlanterasTentacle];
    }

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
    }

    private int FindYoyo() {
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].friendly && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].aiStyle == ProjAIStyleID.Yoyo && !Main.projectile[i].counterweight) {
                return i;
            }
        }
        return -1;
    }

    public override void AI() {
        //Main.NewText($"{Projectile.whoAmI}: {Projectile.ai[0]}, {Projectile.ai[1]}, {Projectile.localAI[0]}, {Projectile.localAI[1]}", Main.DiscoColor);
        int yoyoParent = FindYoyo();
        if (yoyoParent == -1 || Main.projectile[yoyoParent].ai[0] == -1f) {
            Projectile.Kill();
            if (Main.myPlayer == Projectile.owner) {
                Main.player[Projectile.owner].Heal(Pistachiyo.HealAmount);
            }
            return;
        }

        Projectile.localAI[0]--;
        if (Projectile.localAI[0] <= 0f) {
            Projectile.localAI[0] = 60f;
            if (Main.myPlayer == Projectile.owner) {
                var vector = Main.rand.NextVector2Unit() * Main.rand.NextFloat(30f, 120f);
                Projectile.ai[0] = vector.X;
                Projectile.ai[1] = vector.Y;
            }
        }

        var gotoLocation = Main.projectile[yoyoParent].Center + new Vector2(Projectile.ai[0], Projectile.ai[1]);
        if (Projectile.Distance(gotoLocation) < 10f) {
            Projectile.velocity *= 0.95f;
        }
        else {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(gotoLocation) * 30f, 0.2f);
        }
        if (Projectile.frameCounter++ > 4) {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= Main.projFrames[Type]) {
                Projectile.frame = 0;
            }
        }

        Projectile.rotation = (Main.projectile[yoyoParent].Center - Projectile.Center).ToRotation();
    }

    public override bool PreDraw(ref Color lightColor) {
        int yoyoParent = FindYoyo();
        if (yoyoParent == -1) {
            return true;
        }

        var chainTexture = TextureAssets.Chain27.Value;
        var chainOrigin = chainTexture.Size() / 2f;
        var projectileCenter = Projectile.Center;
        var chainPosition = Main.projectile[yoyoParent].Center;
        var chainDifference = projectileCenter - chainPosition;
        var chainVelocity = Vector2.Normalize(chainDifference) * chainTexture.Height;
        for (int i = 0; i < 200; i++) {
            Main.EntitySpriteDraw(chainTexture, chainPosition - Main.screenPosition, null, Lighting.GetColor(chainPosition.ToTileCoordinates()), chainVelocity.ToRotation() + MathHelper.PiOver2, chainOrigin, 1f, SpriteEffects.None, 0f);
            if (Vector2.DistanceSquared(chainPosition, projectileCenter) < 1024f) {
                break;
            }

            chainPosition += chainVelocity;
        }

        var texture = TextureAssets.Projectile[Type].Value;
        var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}