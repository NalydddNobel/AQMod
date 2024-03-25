﻿using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Common.NPCs.Components;
using Aequus.Core.ContentGeneration;
using Aequus.DataSets;
using Aequus.Old.Common.Graphics;
using Aequus.Old.Content.Critters;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.Materials;
using Aequus.Old.Content.Particles;
using Aequus.Old.Content.Potions.NeutronYogurt;
using Aequus.Old.Content.StatusEffects;
using Aequus.Old.Core.Utilities;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace Aequus.Old.Content.Enemies.Glimmer.Hyper;

[ModBiomes(typeof(GlimmerZone))]
[AutoloadBanner]
public class HyperStarite : ModNPC, ITrackTimeBetweenHits {
    public const int STATE_ARMS_IN = 4;
    public const int STATE_ARMS_OUT = 3;
    public const int STATE_CHASE = 2;
    public const int STATE_FLYUP = 1;
    public const int STATE_IDLE = 0;
    public const int STATE_GOODBYE = -1;
    public const int STATE_DEAD = -2;

    public static Color SpotlightColor => GlimmerColors.Yellow * 0.4f;

    public int State { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
    public float ArmsLength { get => NPC.ai[3]; set => NPC.ai[3] = value; }

    public float[] oldArmsLength;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 3;
        NPCSets.TrailingMode[Type] = 7;
        NPCSets.TrailCacheLength[Type] = 15;
        ItemSets.KillsToBanner[BannerItem] = 25;
        NPCSets.ImmuneToRegularBuffs[Type] = true;
        NPCSets.NPCBestiaryDrawOffset.Add(Type, new() {
            Scale = 0.6f,
        });

        NPCDataSet.FromGlimmer.Add(Type);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StariteMaterial>(), minimumDropped: 2, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.Megaphone, chanceDenominator: 50));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NeutronYogurt>(), minimumDropped: 1, maximumDropped: 2));
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void SetDefaults() {
        NPC.width = 50;
        NPC.height = 50;
        NPC.lifeMax = 200;
        NPC.damage = 80;
        NPC.defense = 8;
        NPC.HitSound = SoundID.NPCHit5;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(silver: 30);
        NPC.npcSlots = 3f;

        oldArmsLength = new float[NPCSets.TrailCacheLength[Type]];
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: balance -> balance (bossAdjustment is different, see the docs for details) */
    {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * numPlayers);
        NPC.damage = (int)(NPC.damage * 0.66f);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        float x = Math.Abs(NPC.velocity.X) * hit.HitDirection;
        if (NPC.life <= 0) {
            if (Main.netMode != NetmodeID.Server && State == STATE_DEAD) {
                ViewHelper.LegacyScreenShake(15f, where: NPC.Center);
                ScreenFlash.Instance.Set(NPC.Center, 0.1f);
            }
            for (int i = 0; i < 50; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            }
            for (int i = 0; i < 70; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            }
            for (int i = 0; i < 16; i++) {
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
            }
        }
        else {
            for (int i = 0; i < 7; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
            }
            int d1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
            Main.dust[d1].velocity.X += x;
            Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
        }
    }

    private bool PlayerCheck() {
        NPC.TargetClosest(faceTarget: false);
        if (!NPC.HasValidTarget || Main.player[NPC.target].dead || NPC.Distance(Main.player[NPC.target].Center) > 2000f) {
            NPC.ai[0] = -1f;
            return false;
        }
        else {
            return true;
        }
    }

    public override void AI() {
        var aequus = NPC.GetGlobalNPC<AequusNPC>();
        if (State == STATE_DEAD) {
            if (NPC.localAI[0] == 0) {
                NPC.localAI[0] = Main.rand.Next(100);
            }
            NPC.velocity *= 0.97f;
            NPC.rotation += 0.1f * (1f + NPC.ai[2] / 60f);
            if (NPC.ai[2] > 0f) {
                NPC.ai[2] = 0f;
            }

            NPC.ai[2] -= 1f - NPC.ai[2] / 60f;
            for (int i = 0; i < Main.rand.Next(2, 6); i++) {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[2] * Main.rand.NextFloat(0.2f, 1f) * 3f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Math.Min(Main.rand.NextFloat(1f) - NPC.ai[2] / 60f, 1f)) with { A = 0 });
                d.velocity *= 0.2f;
                d.velocity += (NPC.Center - d.position) / 8f;
                d.scale = Main.rand.NextFloat(0.3f, 2f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            if (NPC.ai[2] < -60f) {
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
            drops.noOnKillEffects = Main.dayTime && State != STATE_DEAD && this.TimeSinceLastHit() > 60;
        }

        if (Main.rand.NextBool(8)) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
            d.velocity = (d.position - NPC.Center) / 8f;
        }
        if (Main.rand.NextBool(10)) {
            var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16);
            g.velocity = (g.position - NPC.Center) / 8f;
            g.scale *= 0.6f;
        }
        Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 0.5f));
        Vector2 center = NPC.Center;
        if (NPC.ai[0] == -1f) {
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.95f;
            if (NPC.velocity.Y > 0f) {
                NPC.velocity.Y *= 0.96f;
            }

            NPC.velocity.Y -= 0.075f;

            NPC.timeLeft = Math.Min(NPC.timeLeft, 100);
            NPC.rotation += NPC.velocity.Length() * 0.0157f;
            return;
        }

        Player player = Main.player[NPC.target];
        Vector2 plrCenter = player.Center;
        float armsWantedLength = 320f;
        oldArmsLength[0] = NPC.ai[3];
        OldHelper.UpdateCacheList(oldArmsLength);
        switch (State) {
            case STATE_IDLE: {
                    NPC.TargetClosest(faceTarget: false);
                    if (NPC.HasValidTarget) {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) || NPC.life < NPC.lifeMax) {
                            NPC.ai[0] = STATE_FLYUP;
                            NPC.ai[1] = 0f;
                            for (int i = 0; i < 5; i++) {
                                int damage = Main.expertMode ? 30 : 55;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<HyperStariteProj>(), damage, 1f, Main.myPlayer, NPC.whoAmI + 1, i);
                            }
                            NPC.netUpdate = true;
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
                            NPC.netUpdate = true;
                        }
                        NPC.ai[1]++;
                        if (NPC.ai[1] >= 1200f) {
                            NPC.timeLeft = 0;
                            NPC.ai[0] = -1f;
                            NPC.netUpdate = true;
                        }
                        NPC.velocity *= 0.96f;
                        return;
                    }
                }
                break;

            case STATE_FLYUP: {
                    NPC.ai[1]++;
                    NPC.velocity.Y -= 0.45f;
                    if (NPC.ai[1] > 20f && PlayerCheck()) {
                        State = STATE_CHASE;
                        NPC.ai[1] = 0f;
                    }
                }
                break;

            case STATE_CHASE: {
                    if (!PlayerCheck()) {
                        return;
                    }
                    NPC.ai[1]++;
                    if (NPC.ai[1] < 50f) {
                        NPC.velocity *= 0.96f;
                    }
                    else {
                        float wantedDistance = 500f;
                        var difference = Main.player[NPC.target].Center - NPC.Center;
                        if (difference.Length() > wantedDistance) {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, difference / 100f, 0.035f);
                        }
                        else {
                            NPC.velocity *= 0.96f;
                            NPC.ai[1] += 4;
                        }
                    }
                    if (NPC.ai[1] > 600f) {
                        State = STATE_ARMS_OUT;
                        NPC.ai[1] = 0f;
                    }
                    NPC.rotation += 0.01f + NPC.velocity.Length() * 0.01f;
                }
                break;

            case STATE_ARMS_OUT: {
                    if (NPC.velocity.Length() > 1f)
                        NPC.velocity *= 0.9f;
                    NPC.ai[1]++;
                    float progress = MathHelper.Clamp(NPC.ai[1] / 120f, 0f, 1f);
                    ArmsLength = (float)Math.Sin(progress * MathHelper.Pi * (2 - MathHelper.Pi / (MathHelper.TwoPi + 2f)) - MathHelper.Pi) * armsWantedLength;
                    if (ArmsLength < 0f)
                        ArmsLength /= 25f * (1f - progress * progress);
                    NPC.rotation += Math.Max(0.06f * progress, 0.01f) + NPC.velocity.Length() * 0.01f;

                    if (NPC.ai[1] > 300f) {
                        State = STATE_ARMS_IN;
                        NPC.ai[1] = 0f;
                    }
                }
                break;

            case STATE_ARMS_IN: {
                    NPC.ai[1]++;
                    if (ArmsLength <= 0.1f) {
                        ArmsLength = 0f;
                    }
                    float progress = NPC.ai[1] / 90f;
                    ArmsLength *= 0.995f - progress * 0.1f;
                    ArmsLength -= 2f;
                    NPC.rotation += Math.Max(0.06f * (1f - progress), 0.01f) + NPC.velocity.Length() * 0.01f;
                    if (NPC.ai[1] > 90f) {
                        State = STATE_CHASE;
                        NPC.ai[1] = 0f;
                    }
                }
                break;
        }
        if (NPC.velocity.Length() < 1.5f && center.Y + 160f > plrCenter.Y && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            NPC.velocity.Y -= 0.6f;
    }

    public override void UpdateLifeRegen(ref int damage) {
        if (Main.dayTime && State != STATE_DEAD && !OldHelper.ShadedSpot(NPC.Center) && !Main.remixWorld) {
            NPC.lifeRegen = -50;
            damage = 8;
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
        if (State == STATE_DEAD) {
            return true;
        }

        State = STATE_DEAD;
        NPC.ai[1] = 0f;
        NPC.ai[2] = 0f;
        NPC.velocity *= 0.5f;
        NPC.dontTakeDamage = true;
        NPC.life = NPC.lifeMax;
        return false;
    }

    public override void OnKill() {
        NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<DwarfStarite>());
    }

    public override int SpawnNPC(int tileX, int tileY) {
        return NPC.NewNPC(null, tileX * 16 + 8, tileY * 16 - 80, NPC.type);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var origin = NPC.frame.Size() / 2f;
        var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
        float mult = 1f / NPCSets.TrailCacheLength[NPC.type];
        var armFrame = NPC.frame;
        var coreFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height * 2, NPC.frame.Width, NPC.frame.Height);
        var bloom = AequusTextures.Bloom.Value;
        var bloomFrame = new Rectangle(0, 0, bloom.Width, bloom.Height);
        var bloomOrigin = bloomFrame.Size() / 2f;

        var armLength = (NPC.height + 56f) * NPC.scale;
        if (NPC.IsABestiaryIconDummy) {
            armLength -= 24f * NPC.scale;
        }

        bool dying = State == STATE_DEAD;
        Main.spriteBatch.Draw(bloom, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), bloomFrame, SpotlightColor, 0f, bloomOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        if (!dying && !NPC.IsABestiaryIconDummy) {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginWorld(shader: true);

            int trailLength = NPCSets.TrailCacheLength[Type];
            int armTrailLength = (int)(trailLength * MathHelper.Clamp((float)Math.Pow(ArmsLength / 240f, 1.2f), 0f, 1f));
            var armPositions = new List<Vector2>[5];

            for (int j = 0; j < 5; j++) {
                armPositions[j] = new List<Vector2>();
            }

            for (int i = 0; i < trailLength; i++) {
                var pos = NPC.oldPos[i] + offset - screenPos;
                float progress = 1f - i / (float)trailLength;
                Color color = new Color(45, 35, 60, 0) * (mult * (NPCSets.TrailCacheLength[NPC.type] - i));
                Main.spriteBatch.Draw(texture, pos.Floor(), coreFrame, color, 0f, origin, NPC.scale * progress * progress, SpriteEffects.None, 0f);
                color = new Color(30, 25, 140, 4) * (mult * (NPCSets.TrailCacheLength[NPC.type] - i)) * 0.6f;
                if (i > armTrailLength || i > 1 && Math.Abs(NPC.oldRot[i] - NPC.oldRot[i - 1]) < 0.002f) {
                    continue;
                }

                for (int j = 0; j < 5; j++) {
                    float rotation = NPC.oldRot[i] + MathHelper.TwoPi / 5f * j;
                    var armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * (armLength + oldArmsLength[i]) - screenPos;
                    armPositions[j].Add(armPos + screenPos);
                    //Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, color, rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            for (int j = 0; j < 5; j++) {
                Vector2[] array = armPositions[j].ToArray();
                float[] rotationsArray = OldDrawHelper.GenerateRotationArr(array);
                DrawHelper.DrawBasicVertexLine(TextureAssets.Extra[ExtrasID.RainbowRodTrailShape].Value, armPositions[j].ToArray(), rotationsArray,
                    (p) => GlimmerColors.Red with { A = 0 } * (1f - p),
                    (p) => 46f
                , -Main.screenPosition);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.BeginWorld(shader: false); ;
        }
        var armSegmentFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height);

        float segmentLength = (NPC.height - 10f) * NPC.scale;
        if (NPC.IsABestiaryIconDummy) {
            segmentLength -= 10f * NPC.scale;
        }
        for (int i = 0; i < 5; i++) {
            float rotation = NPC.rotation + MathHelper.TwoPi / 5f * i;
            if (dying)
                rotation += Main.rand.NextFloat(-0.2f, 0.2f) * Main.rand.NextFloat(NPC.ai[2] / 60f);
            var n = (rotation - MathHelper.PiOver2).ToRotationVector2();
            var armPos = NPC.position + offset + n * (armLength + NPC.ai[3]) - screenPos;
            if (dying) {
                armPos += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 8f), Main.rand.NextFloat(NPC.ai[2] / 8f));
                armPos += n * NPC.ai[2] / 2f;
            }
            Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, Color.White, rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            rotation += MathHelper.TwoPi / 10f;
            armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * segmentLength - screenPos;
            if (dying)
                armPos += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 4f), Main.rand.NextFloat(NPC.ai[2] / 4f));
            Main.spriteBatch.Draw(texture, armPos.Floor(), armSegmentFrame, Color.White, rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }
        if (dying) {
            offset += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 4f), Main.rand.NextFloat(NPC.ai[2] / 4f));
        }

        Main.spriteBatch.Draw(texture, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), coreFrame, new Color(255, 255, 255, 255), 0f, origin, NPC.scale, SpriteEffects.None, 0f);
        if (dying) {
            DrawDeathExplosion(NPC.position + offset - screenPos);
        }
        return false;
    }

    public void DrawDeathExplosion(Vector2 drawPos) {
        float scale = (float)Math.Min(NPC.scale * (-NPC.ai[2] / 60f), 1f) * 2f;
        var shineColor = GlimmerColors.Pink * scale * NPC.Opacity;

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
        scale *= 0.7f;
        Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * scale * NPC.Opacity, 0f, bloomOrigin, scale, SpriteEffects.None, 0f);
        Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * 0.5f * scale * NPC.Opacity, 0f, bloomOrigin, scale * 1.4f, SpriteEffects.None, 0f);

        Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
        var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
        var shineOrigin = shine.Size() / 2f;
        Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * scale, SpriteEffects.None, 0);
        Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * scale, SpriteEffects.None, 0);
    }
}