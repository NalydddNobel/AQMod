using Aequus.Core.Entities.Projectiles;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Aequus.Content.Items.Weapons.Summon.Whips.DemonCrimsonWhip;

public class VasculashTagProj : ModProjectile {
    public static readonly int IdleTeleportDistance = 2000;
    public static readonly int IdleFadeInDistance = IdleMaxDistance + 34;
    public static readonly int IdleMaxDistance = 96;
    public static readonly int IdleMinDistance = 36;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.timeLeft = 360;
        Projectile.ArmorPenetration = 20;
    }

    public override void AI() {
        Player player = Main.player[Projectile.owner];

        if (player.immune) {
            if (Projectile.ai[0] != 1f) {
                Projectile.ai[0] = Main.rand.NextFloat(64f);
                Projectile.timeLeft = 240;
            }
        }

        if ((int)Projectile.ai[0] == 1) {
            if (Projectile.localAI[1] == 0f) {
                Projectile.localAI[1] = 1f;

                if (Projectile.ai[2] > 0f && Main.myPlayer == player.whoAmI) {
                    Vector2 toMouse = Projectile.DirectionTo(Main.MouseWorld);
                    Projectile.velocity.X += toMouse.X;
                    Projectile.velocity.Y += toMouse.Y;

                    Projectile.ai[2] = 16f;
                }

                Projectile.velocity.Normalize();
                Projectile.velocity *= 12f;

                Projectile.netUpdate = true;

                SoundEngine.PlaySound(SoundID.NPCHit8 with { Volume = 0.5f, Pitch = 0.5f, PitchVariance = 0.5f, MaxInstances = 10 }, Projectile.Center);
            }

            Projectile.ai[0] = 1f;
            Projectile.friendly = true;
            Projectile.localAI[0] += 0.05f;

            AttackingBehavior();

            Vector2 velocityNormal = Vector2.Normalize(Projectile.velocity);
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Unit() * 2f + velocityNormal * 8f, DustID.Blood, -velocityNormal);

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.2f, 0.1f));
        }
        else {
            Projectile.ai[0]--;
            player.statDefense += DemonVasculash.DefensePerPet;

            IdleBehavior();
        }

        if (Projectile.frameCounter++ > 6) {
            Projectile.frameCounter = 0;
            Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
        }

        void AttackingBehavior() {
            Projectile.tileCollide = true;

            if (Projectile.ai[2] <= 0f) {
                int target = Projectile.GetMinionTarget(out _, maxDistance: 1000f, ignoreTilesDistance: null);
                if (target != -1) {
                    Vector2 toTarget = (Main.npc[target].Center - Projectile.Center) * 0.005f;
                    Projectile.velocity.X += toTarget.X;
                    Projectile.velocity.Y += toTarget.Y;

                    if (Math.Sign(toTarget.X) != Math.Sign(Projectile.velocity.X)) {
                        Projectile.velocity.X *= 0.7f;
                    }
                    if (Math.Sign(toTarget.Y) != Math.Sign(Projectile.velocity.Y)) {
                        Projectile.velocity.Y *= 0.7f;
                    }

                    Projectile.tileCollide = false;
                }
            }
            else {
                Projectile.ai[2]--;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        }

        void IdleBehavior() {
            if (player.ownedProjectileCounts[Type] < DemonVasculash.MaxPets) {
                Projectile.timeLeft++;
            }
            else if (Projectile.timeLeft > 1) {
                Projectile.timeLeft -= player.ownedProjectileCounts[Type];
                if (Projectile.timeLeft < 1) {
                    Projectile.timeLeft = 1;
                }
                player.ownedProjectileCounts[Type] = 0;
            }

            Vector2 difference = player.Center - Projectile.Center;
            float length = difference.Length();
            if (length > IdleTeleportDistance) {
                Projectile.Center = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.velocity += Main.rand.NextVector2Unit();
            }
            else if (length > IdleMaxDistance) {
                Vector2 toPlayer = Vector2.Normalize(difference);
                Projectile.velocity.X += toPlayer.X * 0.25f;
                Projectile.velocity.Y += toPlayer.Y * 0.25f;

                if (Math.Sign(difference.X) != Math.Sign(Projectile.velocity.X)) {
                    Projectile.velocity.X *= 0.95f;
                }
                if (Math.Sign(difference.Y) != Math.Sign(Projectile.velocity.Y)) {
                    Projectile.velocity.Y *= 0.95f;
                }
            }
            else if (length < IdleMinDistance) {
                Vector2 toPlayer = Vector2.Normalize(-difference);
                Projectile.velocity.X += toPlayer.X * 0.1f;
                Projectile.velocity.Y += toPlayer.Y * 0.1f;
            }
            else {
                Projectile.velocity *= 0.95f;
                if (Projectile.ai[1]++ > 60f) {
                    Projectile.velocity += Main.rand.NextVector2Unit() * 4f;
                    Projectile.ai[1] = 0f;
                }
            }

            float wantedRotation = length > IdleFadeInDistance ? Projectile.velocity.ToRotation() : difference.ToRotation();

            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, wantedRotation, 0.1f);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Projectile.velocity.X != oldVelocity.X) {
            Projectile.velocity.X = -oldVelocity.X;
        }
        if (Projectile.velocity.Y != oldVelocity.Y) {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        return false;
    }

    public override void OnKill(int timeLeft) {
        if (timeLeft <= 0) {
            SoundEngine.PlaySound(SoundID.Item112 with { Pitch = 0.8f, }, Projectile.Center);
        }

        for (int i = 0; i < 20; i++) {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            d.velocity *= 1.5f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int _);

        Vector2 position = Projectile.position + offset;
        Player player = Main.player[Projectile.owner];
        Vector2 endPosition = player.Center;
        Vector2 difference = endPosition - position;
        float length = difference.Length();
        float fadeInLength = IdleMaxDistance;
        float maxLength = IdleFadeInDistance;

        FastRandom random = new FastRandom(Projectile.whoAmI).WithModifier((ulong)(Main.GlobalTimeWrappedHourly * 6f));
        if (length < maxLength && Projectile.localAI[0] < 1f) {
            Texture2D chainTexture = TextureAssets.Chain12.Value;
            int segmentsCount = 6;
            Vector2 start = position;
            Vector2 segmentDifference = difference / segmentsCount;
            Vector2 chainScale = new Vector2(0.75f, segmentDifference.Length() / chainTexture.Height);
            float rotation = difference.ToRotation() + MathHelper.PiOver2;
            float intensity = Math.Clamp((maxLength - length) / (maxLength - fadeInLength), 0f, 1f);
            intensity *= 1f - Projectile.localAI[0];
            for (int i = 0; i < segmentsCount; i++) {
                SpriteEffects effects = random.Next(2) == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(chainTexture, start - Main.screenPosition, null, lightColor * intensity, rotation, chainTexture.Size() / 2f, chainScale, effects);
                start += segmentDifference;
            }
        }

        Main.EntitySpriteDraw(texture, position - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(30f, frame.Height / 2f - 1f), Projectile.scale, SpriteEffects.None);

        return false;
    }
}
