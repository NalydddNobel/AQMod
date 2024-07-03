using System;
using Terraria.GameContent;

namespace Aequu2.Old.Content.Items.Weapons.Ranged.CrusadersCrossbow;

public class CrusadersCrossbowProj : ModProjectile {
    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.extraUpdates = 2;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Main.teamColor[Main.player[Projectile.owner].team];
    }

    public override void AI() {
        Projectile.velocity.Y += 0.1f;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Main.netMode != NetmodeID.SinglePlayer && Main.myPlayer == Projectile.owner) {
            int playerToHeal = GetBestPlayerWithin(new Rectangle((int)Projectile.position.X - 40, (int)Projectile.position.Y - 40, 80 + Projectile.width, 80 + Projectile.height), Projectile.owner);
            if (playerToHeal != -1 && playerToHeal != Projectile.owner) {
                if (Main.player[playerToHeal].team == Main.player[Projectile.owner].team) {
                    int healing = Main.DamageVar(Projectile.damage / 2f);
                    Main.player[playerToHeal].HealEffect(healing, broadcast: false);
                    Main.player[playerToHeal].statLife += healing;
                    if (Main.player[playerToHeal].statLife > Main.player[playerToHeal].statLifeMax2) {
                        Main.player[playerToHeal].statLife = Main.player[playerToHeal].statLifeMax2;
                    }
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, playerToHeal, healing);
                    }
                    Projectile.Kill();
                }
            }
        }
    }

    private static int GetBestPlayerWithin(Rectangle Hitbox, int myPlayer) {
        int lowestHealth = int.MaxValue;
        int chosenPlayer = -1;
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (i != myPlayer
                && Main.player[i].active && !Main.player[i].DeadOrGhost
                && Main.player[i].team == Main.player[myPlayer].team
                && Main.player[i].Hitbox.Intersects(Hitbox)
                && Main.player[i].statLife < lowestHealth) {
                lowestHealth = Main.player[i].statLife;
                chosenPlayer = i;
            }
        }

        return chosenPlayer;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 2;
        height = 2;
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition;
        var drawColor = Projectile.GetAlpha(lightColor);

        Main.spriteBatch.GraphicsDevice.Textures[0] = Aequu2Textures.Trail.Value;
        DrawHelper.VertexStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot,
            (p) => Projectile.GetAlpha(Color.White) with { A = 0 } * 0.9f * (float)Math.Pow(1f - p, 2f),
            (p) => 6f * (1f - p),
            Projectile.Size / 2f - Main.screenPosition);
        DrawHelper.VertexStrip.DrawTrail();

        var frame = Projectile.Frame();
        var origin = new Vector2(frame.Width, frame.Height / 2f - 1f);
        Main.spriteBatch.Draw(texture, Projectile.position + offset, frame, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, Projectile.position + offset, new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height), Color.White, Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}