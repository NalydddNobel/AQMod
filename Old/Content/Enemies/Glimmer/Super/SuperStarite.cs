using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Common.NPCs.Components;
using Aequus.Core.ContentGeneration;
using Aequus.Old.Content.Critters;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.Materials;
using Aequus.Old.Content.Particles;
using Aequus.Old.Content.Potions.NeutronYogurt;
using Aequus.Old.Content.StatusEffects;
using Aequus.Old.Core.Utilities;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Old.Content.Enemies.Glimmer.Super;

[ModBiomes(typeof(GlimmerZone))]
[AutoloadBanner]
public class SuperStarite : ModNPC, ITrackTimeBetweenHits {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 2;

        NPCSets.TrailingMode[Type] = 7;
        NPCSets.TrailCacheLength[Type] = 15;
        NPCSets.ImmuneToRegularBuffs[Type] = true;

        NPCMetadata.FromGlimmer.Add(Type);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StariteMaterial>(), minimumDropped: 1, maximumDropped: 2));
        npcLoot.Add(ItemDropRule.Common(ItemID.Megaphone, chanceDenominator: 50));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NeutronYogurt>(), chanceDenominator: 2));
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void SetDefaults() {
        NPC.width = 80;
        NPC.height = 80;
        NPC.lifeMax = 75;
        NPC.damage = 25;
        NPC.defense = 9;
        NPC.HitSound = SoundID.NPCHit5;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(silver: 8);
        NPC.npcSlots = 1.5f;
    }

    public override Color? GetAlpha(Color drawColor) {
        return Color.White;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        float x = Math.Abs(NPC.velocity.X) * hit.HitDirection;
        if (NPC.life <= 0) {
            for (int i = 0; i < 35; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            }
            for (int i = 0; i < 50; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            }
            for (int i = 0; i < 8; i++) {
                Gore.NewGore(NPC.GetSource_OnHurt(null), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
            }
        }
        else {
            for (int i = 0; i < 6; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
            }
            int d1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
            Main.dust[d1].velocity.X += x;
            Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            if (Main.rand.NextBool())
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
        }
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        NPC.damage = (int)(NPC.damage * 0.75f);
    }

    private bool playerCheck() {
        NPC.TargetClosest(faceTarget: false);
        if (!NPC.HasValidTarget || Main.player[NPC.target].dead) {
            NPC.ai[0] = -1f;
            return false;
        }
        else {
            return true;
        }
    }

    public override void AI() {
        Vector2 center = NPC.Center;
        const float collisionMult = 0.75f;
        bool collisonEffects = false;
        if (NPC.collideX) {
            if (Math.Abs(NPC.oldVelocity.X) > 2f) {
                NPC.velocity.X = -NPC.oldVelocity.X * collisionMult;
                collisonEffects = true;
            }
        }
        if (NPC.collideY) {
            if (Math.Abs(NPC.oldVelocity.Y) > 2f) {
                NPC.velocity.Y = -NPC.oldVelocity.Y * collisionMult;
                collisonEffects = true;
            }
        }
        if (collisonEffects) {
            for (int i = 0; i < 10f; i++) {
                int d = Dust.NewDust(center + NPC.oldVelocity, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity = NPC.velocity * 0.65f;
            }
        }

        if ((int)NPC.ai[0] == -2f) {
            if (NPC.localAI[0] == 0) {
                NPC.localAI[0] = Main.rand.Next(100);
            }
            NPC.velocity *= 0.97f;
            NPC.rotation += 0.1f * (1f + NPC.ai[2] / 60f);
            if (NPC.ai[3] > 0f)
                NPC.ai[3] = 0f;
            NPC.ai[3] -= 1.5f - NPC.ai[2] / 60f;
            for (int i = 0; i < Main.rand.Next(2, 6); i++) {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[3] * Main.rand.NextFloat(0.2f, 1f) * 3f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Math.Min(Main.rand.NextFloat(1f) - NPC.ai[3] / 60f, 1f)) with { A = 0 });
                d.velocity *= 0.2f;
                d.velocity += (NPC.Center - d.position) / 8f;
                d.scale = Main.rand.NextFloat(0.3f, 2f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            if (NPC.ai[3] < -60f) {
                for (int i = 0; i < 60; i++) {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * 50f * Main.rand.NextFloat(0.01f, 1f), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Main.rand.NextFloat(1f)) with { A = 0 });
                    d.velocity *= 0.2f;
                    d.velocity += (d.position - NPC.Center) / 2f;
                    d.scale = Main.rand.NextFloat(0.3f, 2.5f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                NPC.life = -33333;
                NPC.HitEffect();
                NPC.checkDead();
            }
            return;
        }

        if (NPC.TryGetGlobalNPC(out DropsGlobalNPC drops)) {
            drops.noOnKillEffects = Main.dayTime && this.TimeSinceLastHit() > 60;
        }

        if (Main.rand.NextBool(20)) {
            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
            Main.dust[d].velocity = NPC.velocity * 0.01f;
        }
        if (Main.rand.NextBool(40)) {
            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
            Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
            Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
        }
        if (Main.rand.NextBool(40)) {
            int g = Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16 + Main.rand.Next(2));
            Main.gore[g].scale *= 0.6f;
        }
        Lighting.AddLight(NPC.Center, new Vector3(0.8f, 0.8f, 0.45f));

        if (NPC.ai[0] == -1f) {
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.965f;
            if (NPC.velocity.Y > 0f) {
                NPC.velocity.Y *= 0.985f;
            }

            NPC.velocity.Y -= 0.055f;
            if (NPC.timeLeft > 100) {
                NPC.timeLeft = 100;
            }

            return;
        }
        if (NPC.ai[0] == 0f) {
            NPC.TargetClosest(faceTarget: false);
            if (NPC.HasValidTarget) {
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) || NPC.life < NPC.lifeMax) {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                }
                else {
                    NPC.ai[1]++;
                    if (NPC.ai[1] >= 1200f) {
                        NPC.timeLeft = 0;
                        NPC.ai[0] = -1f;
                    }
                    NPC.velocity *= 0.96f;
                    return;
                }
            }
            else {
                if (Main.player[NPC.target].dead) {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = -0f;
                }
                NPC.ai[1]++;
                if (NPC.ai[1] >= 1200f) {
                    NPC.timeLeft = 0;
                    NPC.ai[0] = -1f;
                }
                NPC.velocity *= 0.96f;
                return;
            }
        }
        if (NPC.ai[0] == 1f) {
            NPC.ai[1]++;
            NPC.velocity.Y -= 0.35f;
            if (NPC.velocity.Y < -8f || NPC.ai[1] > 50f) {
                NPC.ai[0] = 2f;
                NPC.ai[1] -= 50f;
            }
            return;
        }
        if (NPC.ai[0] == 2f) {
            NPC.ai[1]++;
            NPC.velocity.Y += 0.15f;
            if (NPC.velocity.Y >= 0f || NPC.ai[1] > 50f) {
                if (playerCheck()) {
                    NPC.velocity.Y = 0f;
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                }
            }
            return;
        }
        Player player = Main.player[NPC.target];
        Vector2 plrCenter = player.Center;
        bool doRotate = true;
        if (NPC.ai[0] == 3f) {
            NPC.ai[1]++;
            float oldSpeed = NPC.velocity.Length();
            if (NPC.ai[2] != 0f && NPC.velocity.X != NPC.ai[2]) {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.ai[2], 0.035f);
                if (Math.Abs(NPC.velocity.X - NPC.ai[2]) < 0.1f) {
                    NPC.velocity.X = NPC.ai[2];
                    NPC.ai[2] = 0f;
                }
            }
            if (NPC.ai[3] != 0f && NPC.velocity.Y != NPC.ai[3]) {
                NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, NPC.ai[3], 0.035f);
                if (Math.Abs(NPC.velocity.Y - NPC.ai[3]) < 0.1f) {
                    NPC.velocity.Y = NPC.ai[3];
                    NPC.ai[3] = 0f;
                }
            }
            if (NPC.velocity.Length() < oldSpeed) {
                NPC.velocity = Vector2.Normalize(NPC.velocity) * oldSpeed;
            }
            if (NPC.ai[1] > 50f) {
                if (playerCheck()) {
                    Vector2 difference = plrCenter + new Vector2(0f, player.height * -5f) - center;
                    float length = difference.Length();
                    Vector2 velocity = Vector2.Normalize(difference) * Main.rand.NextFloat(4f, 8f);
                    NPC.netUpdate = true;
                    if (length < 500f) {
                        NPC.ai[1] = -125f;
                    }
                    else if (length < 800f) {
                        NPC.ai[1] = -100f;
                    }
                    else if (length < 1500f) {
                        NPC.ai[1] = -50f;
                    }
                    else if (length < 2000f) {
                        NPC.timeLeft = 0;
                    }
                    NPC.ai[2] = velocity.X;
                    NPC.ai[3] = velocity.Y;
                }
            }
            if (NPC.ai[1] >= 0f && NPC.ai[1] % 50 == 0) {
                SoundEngine.PlaySound(SoundID.Item84 with { Volume = 0.25f, Pitch = 0.2f }, NPC.Center);
                const float twoPiOver5 = MathHelper.TwoPi / 5f;
                int damage = Main.expertMode ? 15 : 20;
                int type = ModContent.ProjectileType<SuperStariteBullet>();
                float length = (float)Math.Sqrt(NPC.width * NPC.width + NPC.height * NPC.height) / 2f;
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    for (int i = 0; i < 5; i++) {
                        Vector2 normal = (twoPiOver5 * i + NPC.rotation).ToRotationVector2();
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), center + normal * length, normal * 9f, type, damage, 1f, Main.myPlayer);
                    }
                }
            }
            NPC.velocity *= 0.995f;
            NPC.rotation += 0.0314f;
        }
        if (doRotate) {
            NPC.rotation += NPC.velocity.Length() * 0.0157f;
        }
    }

    public override bool? CanFallThroughPlatforms() {
        return true;
    }

    public override void UpdateLifeRegen(ref int damage) {
        if (Main.dayTime && (int)NPC.ai[0] != -2f && !OldHelper.ShadedSpot(NPC.Center) && !Main.remixWorld) {
            NPC.lifeRegen = -30;
            damage = 5;
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool(Main.expertMode ? 1 : 2)) {
            target.AddBuff(ModContent.BuffType<BlueFire>(), 240);
        }

        if (Main.rand.NextBool(Main.expertMode ? 1 : 4)) {
            target.AddBuff(BuffID.Blackout, 600);
        }

        if (Main.rand.NextBool(Main.expertMode ? 4 : 12)) {
            target.AddBuff(BuffID.Silenced, 120);
        }
    }

    public override bool CheckDead() {
        if (NPC.ai[0] == -2f) {
            return true;
        }

        NPC.ai[0] = -2f;
        NPC.ai[1] = 0f;
        NPC.ai[2] = 0f;
        NPC.ai[3] = 0f;
        NPC.velocity *= 0.5f;
        NPC.dontTakeDamage = true;
        NPC.life = NPC.lifeMax;
        return false;
    }

    public override void OnKill() {
        NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<DwarfStarite>());
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var origin = new Vector2(59f, 64f);
        var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
        float mult = 1f / NPCSets.TrailCacheLength[NPC.type];
        float rotation = NPC.rotation;
        var coreFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height);

        Color trailColor = GlimmerColors.Red * 0.27f;
        Color bloomColor = GlimmerColors.Yellow;
        drawColor = NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor));

        if (!NPC.IsABestiaryIconDummy) {
            for (int i = 0; i < NPCSets.TrailCacheLength[NPC.type]; i++) {
                trailColor *= mult * (NPCSets.TrailCacheLength[NPC.type] - i);
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - screenPos, NPC.frame, trailColor, NPC.oldRot[i], origin, NPC.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - screenPos, coreFrame, trailColor, 0f, origin, NPC.scale, SpriteEffects.None, 0f);
            }
        }

        Main.spriteBatch.Draw(texture, NPC.position + offset - screenPos + new Vector2(1f, 4f), NPC.frame, drawColor, rotation, origin, NPC.scale, SpriteEffects.None, 0f);

        var bloom = AequusTextures.Bloom;
        Main.spriteBatch.Draw(bloom, NPC.position + offset - screenPos, null, bloomColor * 0.5f, 0f, bloom.Size() / 2f, NPC.scale * 0.6f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(bloom, NPC.position + offset - screenPos, null, bloomColor * 0.25f, 0f, bloom.Size() / 2f, NPC.scale * 0.9f, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(texture, NPC.position + offset - screenPos, coreFrame, drawColor, 0f, NPC.frame.Size()/2f, NPC.scale, SpriteEffects.None, 0f);
        if ((int)NPC.ai[0] == -2) {
            DrawDeathExplosion(NPC.position + offset - screenPos);
        }
        return false;
    }

    public void DrawDeathExplosion(Vector2 drawPos) {
        float scale = (float)Math.Min(NPC.scale * (-NPC.ai[3] / 60f), 1f) * 1.5f;
        var shineColor = new Color(200, 40, 150, 0) * scale * NPC.Opacity;

        Texture2D lightRay = AequusTextures.LightRayFlat;
        var lightRayOrigin = lightRay.Size() / 2f;

        FastRandom r = new FastRandom(NPC.whoAmI).WithModifier((ulong)NPC.whoAmI);

        float rotation = Main.GlobalTimeWrappedHourly * 1.8f + NPC.localAI[0];
        int rayCount = r.Next(6, 11);
        for (int i = 0; i < rayCount; i++) {
            float f = i / (float)rayCount * MathHelper.TwoPi + rotation;
            var rayScale = new Vector2(Helper.Oscillate(r.NextFloat(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * r.NextFloat(1f, 5f) * 0.5f, 0.3f, 1f) * r.NextFloat(0.5f, 1.25f));
            rayScale.X *= 0.1f;
            rayScale.X *= (float)Math.Pow(scale, Math.Min(rayScale.Y, 1f));
            Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, f, lightRayOrigin, scale * rayScale * 2f, SpriteEffects.None, 0f);
        }

        var bloom = AequusTextures.Bloom;
        var bloomOrigin = bloom.Size() / 2f;
        scale *= 0.5f;
        Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * scale * NPC.Opacity, 0f, bloomOrigin, scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, 0f, bloomOrigin, scale * 1.4f, SpriteEffects.None, 0f);

        Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
        var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
        var shineOrigin = shine.Size() / 2f;
        Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * scale, SpriteEffects.None, 0);
    }
}