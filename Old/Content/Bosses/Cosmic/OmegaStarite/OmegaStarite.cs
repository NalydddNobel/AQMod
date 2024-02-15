using Aequus.Common.NPCs;
using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Bosses;
using Aequus.Content.Bosses.Trophies;
using Aequus.Content.DataSets;
using Aequus.Core;
using Aequus.Core.ContentGeneration;
using Aequus.Old.Common.Graphics;
using Aequus.Old.Common.Graphics.Camera;
using Aequus.Old.Content.Bosses.Cosmic.OmegaStarite.Projectiles;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.Particles;
using Aequus.Old.Content.StatusEffects;
using Aequus.Old.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Utilities;

namespace Aequus.Old.Content.Bosses.Cosmic.OmegaStarite;

[ModBiomes(typeof(GlimmerZone))]
[AutoloadBossHead()]
[AutoloadTrophies(LegacyBossTrophiesTile.OmegaStarite, typeof(OmegaStariteRelicRenderer))]
public class OmegaStarite : AequusBoss {
    public const float BossProgression = 6.99f;

    public const int ACTION_LASER_ORBITAL_2 = 8;
    public const int ACTION_LASER_ORBITAL_1 = 7;
    public const int ACTION_STARS = 6;
    public const int ACTION_ASSAULT = 5;
    public const int ACTION_ORBITAL_3 = 4;
    public const int ACTION_ORBITAL_2 = 3;
    public const int ACTION_ORBITAL_1 = 2;
    public const int ACTION_UNUSED = -2;
    public const int ACTION_DEAD = -3;

    public const float DIAMETER = 120;
    public const float RADIUS = DIAMETER / 2f;
    private const float DEATHTIME = MathHelper.PiOver4 * 134;

    public List<OmegaStariteRing> rings;
    public float starDamageMultiplier;
    private byte _hitShake;

    public override void SetStaticDefaults() {
        NPCSets.TrailingMode[NPC.type] = 7;
        NPCSets.TrailCacheLength[NPC.type] = 60;
        NPCSets.MPAllowedEnemies[Type] = true;
        NPCSets.BossBestiaryPriority.Add(Type);
        NPCSets.NPCBestiaryDrawOffset.Add(Type, new() {
            Position = new Vector2(0f, 2f),
        });
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
        NPCSets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
        NPCSets.SpecificDebuffImmunity[Type][ModContent.BuffType<BlueFire>()] = true;
        //NPCSets.SpecificDebuffImmunity[Type][ModContent.BuffType<BattleAxeBleeding>()] = true;
        Main.npcFrameCount[NPC.type] = 14;

        //SnowgraveCorpse.NPCBlacklist.Add(Type);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void SetDefaults() {
        NPC.width = 120;
        NPC.height = 120;
        NPC.lifeMax = 12000;
        NPC.damage = 45;
        NPC.defense = 18;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(gold: 6);
        NPC.boss = true;
        NPC.npcSlots = 25f;
        NPC.noTileCollide = true;
        NPC.trapImmune = true;
        NPC.lavaImmune = true;

        starDamageMultiplier = 0.8f;

        if (Main.getGoodWorld) {
            NPC.scale *= 0.5f;
            starDamageMultiplier *= 0.5f;
        }

        Music = MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/OmegaStariteOld");
        SceneEffectPriority = SceneEffectPriority.BossLow;
    }

    public override Color? GetAlpha(Color drawColor) {
        return new Color(255, 255, 255, 240);
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        if (Main.expertMode) {
            starDamageMultiplier *= 0.8f;
        }
        NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }
        if (NPC.life == -33333) {
            if (Main.netMode != NetmodeID.Server) {
                ViewHelper.PunchCameraTo(NPC.Center, 6f, 6f, 30);
            }
            for (int k = 0; k < 60; k++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.02f) {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue with { A = 25 }).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.05f) {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold with { A = 25 }).noGravity = true;
            }

            if (Cull2D.Rectangle(NPC.getRect())) {
                for (int k = 0; k < 7; k++) {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }

            for (int i = 0; i < rings.Count; i++) {
                for (int j = 0; j < rings[i].amountOfSegments; j++) {
                    for (int k = 0; k < 30; k++) {
                        Dust.NewDust(rings[i].CachedHitboxes[j].TopLeft(), rings[i].CachedHitboxes[j].Width, rings[i].CachedHitboxes[j].Height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
                    }
                    for (float f = 0f; f < 1f; f += 0.125f) {
                        Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue with { A = 25 }).noGravity = true;
                    }
                    for (float f = 0f; f < 1f; f += 0.25f) {
                        Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold with { A = 25 }).noGravity = true;
                    }
                    if (Cull2D.Rectangle(rings[i].CachedHitboxes[j])) {
                        for (int k = 0; k < 7; k++) {
                            Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                        }
                    }
                }
            }
        }
        else if (NPC.life <= 0) {
            SoundEngine.PlaySound(AequusSounds.OmegaStariteKilled, NPC.Center);
            for (int k = 0; k < 60; k++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.02f) {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue with { A = 25 }).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.05f) {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold with { A = 25 }).noGravity = true;
            }

            if (Cull2D.Rectangle(NPC.getRect())) {
                for (int k = 0; k < 7; k++) {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * 6f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
        }
        else {
            SoundEngine.PlaySound(AequusSounds.OmegaStariteHit0 with { Volume = 0.6f, Pitch = -0.025f, PitchVariance = 0.05f }, NPC.Center);
            byte shake = (byte)MathHelper.Clamp(hit.Damage / 8, 4, 10);
            if (shake > _hitShake) {
                _hitShake = shake;
            }

            float x = Math.Abs(NPC.velocity.X) * hit.HitDirection;
            if (Main.rand.NextBool()) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = Main.rand.NextFloat(2f, 6f);
            }

            if (Cull2D.Rectangle(NPC.getRect()) && Main.rand.NextBool(7)) {
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }
    }

    public void KillFallenStars() {
        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.FallingStar && NPC.Distance(Main.projectile[i].Center) < 2000f) {
                Main.projectile[i].damage = 0;
                Main.projectile[i].noDropItem = true;
                Main.projectile[i].Kill();
            }
        }
    }

    public override void AI() {
        if (Main.dayTime && !Main.remixWorld && Action != ACTION_DEAD) {
            NPC.GetGlobalNPC<DropsGlobalNPC>().noOnKillEffects = true;
        }

        GlimmerZone.omegaStarite = NPC.whoAmI;
        KillFallenStars();
        var center = NPC.Center;
        var player = Main.player[NPC.target];
        var plrCenter = player.Center;
        float speed = NPC.velocity.Length();
        switch ((int)NPC.ai[0]) {
            default: {
                    LerpToDefaultRotationVelocity();
                    NPC.Center = plrCenter + new Vector2(0f, -DIAMETER * 2f);
                }
                break;

            case ACTION_LASER_ORBITAL_2: {
                    if (NPC.ai[1] == 0f) {
                        CullRingRotations();
                    }
                    NPC.ai[1] += 0.0002f;
                    bool allRingsSet = true;

                    rings[0].rotationVelocity *= 0.95f;

                    rings[0].pitch = rings[0].pitch.AngleLerp(MathHelper.PiOver2, 0.025f);
                    rings[0].roll = rings[0].roll.AngleLerp(-MathHelper.PiOver2, 0.025f);

                    if (!CloseEnough(rings[0].pitch, MathHelper.PiOver2, 0.314f) || !CloseEnough(rings[0].roll, -MathHelper.PiOver2, 0.314f)) {
                        allRingsSet = false;
                    }
                    for (int i = 1; i < rings.Count; i++) {
                        rings[i].rotationVelocity *= 0.95f;

                        rings[i].pitch = rings[i].pitch.AngleLerp(0f, 0.025f);
                        rings[i].roll = rings[i].roll.AngleLerp(0f, 0.025f);
                        if (allRingsSet && !CloseEnough(rings[i].pitch, 0f, 0.314f) || !CloseEnough(rings[i].roll, 0f, 0.314f)) {
                            allRingsSet = false;
                        }
                    }

                    if (NPC.ai[1] > 0.0314f) {
                        if (allRingsSet) {
                            NPC.velocity = Vector2.Normalize(plrCenter - center) * NPC.velocity.Length();
                            rings[0].pitch = MathHelper.PiOver2;
                            rings[0].roll = -MathHelper.PiOver2;
                            for (int i = 1; i < rings.Count; i++) {
                                rings[i].pitch = 0f;
                                rings[i].roll = 0f;
                            }
                            if (PlrCheck()) {
                                NPC.ai[0] = ACTION_LASER_ORBITAL_1;
                                NPC.ai[1] = 0f;
                                NPC.ai[3] = 3f + (1f - NPC.life / (float)NPC.lifeMax) * 1.5f;
                            }
                        }
                    }
                    else {
                        rings[0].yaw += 0.0314f - NPC.ai[1];
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += 0.0157f - NPC.ai[1] * 0.5f;
                        }
                    }
                }
                break;

            case ACTION_LASER_ORBITAL_1: {
                    NPC.ai[2]++;
                    if (NPC.ai[2] > 1200f) {
                        if (NPC.ai[1] > 0.0314) {
                            NPC.ai[1] -= 0.0005f;
                        }
                        else {
                            NPC.ai[1] = 0.0314f;
                        }
                        rings[0].yaw += NPC.ai[1];
                        rings[1].yaw += NPC.ai[1] * 0.5f;
                        bool ringsSet = false;
                        if (rings[1].radiusFromOrigin > rings[1].OriginalRadiusFromOrigin) {
                            rings[1].radiusFromOrigin -= MathHelper.PiOver2 * 3f;
                            NPC.localAI[0]++;
                            if (Main.getGoodWorld) {
                                bool shot = false;
                                for (int i = 0; i < rings.Count; i++) {
                                    shot |= ShootProjsFromRing(endingPhase: true, rings[i]);
                                }
                                if (shot) {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                    NPC.localAI[0] = 0f;
                                }
                            }
                            else if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin) {
                                if (ShootProjsFromRing(endingPhase: true, rings[1])) {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                    NPC.localAI[0] = 0f;
                                }
                            }
                        }
                        else {
                            ringsSet = true;
                        }
                        for (int i = 2; i < rings.Count; i++) {
                            rings[i].radiusFromOrigin -= MathHelper.PiOver2 * 3f;
                            if (rings[i].radiusFromOrigin > rings[i].OriginalRadiusFromOrigin) {
                                ringsSet = false;
                            }
                        }
                        if (ringsSet) {
                            ResetRingsRadiusFromOrigin();
                            if (PlrCheck()) {
                                var choices = new List<int>
                                {
                                    ACTION_ASSAULT,
                                    ACTION_STARS,
                                };
                                NPC.ai[0] = choices[Main.rand.Next(choices.Count)];
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                                NPC.ai[3] = 0f;
                                NPC.localAI[0] = 0f;
                                NPC.localAI[1] = 0f;
                                NPC.localAI[2] = 0f;
                            }
                        }
                    }
                    else if ((center - plrCenter).Length() > 1800f) {
                        NPC.ai[2] = 1200f;
                        rings[0].yaw += NPC.ai[1];
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += NPC.ai[1] * 0.5f;
                        }
                    }
                    else {
                        if (NPC.ai[1] >= 0.0628f) {
                            NPC.ai[1] = 0.0628f;
                        }
                        else {
                            NPC.ai[1] += 0.0002f;
                        }
                        rings[0].yaw += NPC.ai[1];
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += NPC.ai[1] * 0.5f;
                            rings[i].radiusFromOrigin = MathHelper.Lerp(rings[i].radiusFromOrigin, rings[i].OriginalRadiusFromOrigin * (NPC.ai[3] + i), 0.025f);
                        }
                        if (NPC.ai[2] > 100f) {
                            if (NPC.localAI[1] == 0f) {
                                if (PlrCheck()) {
                                    NPC.localAI[1] = 1f;
                                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
                                    if (Main.netMode != NetmodeID.Server) {
                                        ViewHelper.PunchCameraTo(NPC.Center);
                                    }
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaStariteDeathray>(), 100, 1f, Main.myPlayer, NPC.whoAmI);
                                    if (Main.getGoodWorld) {
                                        p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<OmegaStariteDeathray>(), 100, 1f, Main.myPlayer, NPC.whoAmI);
                                        ((OmegaStariteDeathray)Main.projectile[p].ModProjectile).rotationOffset = MathHelper.Pi;
                                    }
                                }
                                else {
                                    break;
                                }
                            }
                            if (rings[0].roll > MathHelper.PiOver2 * 6f) {
                                NPC.localAI[2] -= Main.expertMode ? 0.001f : 0.00045f;
                            }
                            else {
                                NPC.localAI[2] += Main.expertMode ? 0.00015f : 0.000085f;
                            }
                            NPC.localAI[0]++;
                            if (Main.getGoodWorld) {
                                bool shot = false;
                                for (int i = 1; i < rings.Count; i++) {
                                    shot |= ShootProjsFromRing(endingPhase: false, rings[i]);
                                }
                                if (shot) {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                    NPC.localAI[0] = 0f;
                                }
                            }
                            else if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin) {
                                if (ShootProjsFromRing(endingPhase: false, rings[1])) {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                    NPC.localAI[0] = 0f;
                                }
                            }
                            rings[0].roll += NPC.localAI[2];
                            if (NPC.soundDelay <= 0) {
                                NPC.soundDelay = 60;
                                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalIdleLoop, NPC.Center);
                            }
                            if (NPC.soundDelay > 0)
                                NPC.soundDelay--;
                            if (rings[0].roll > MathHelper.PiOver2 * 7f) {
                                NPC.soundDelay = 0;
                                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);
                                NPC.ai[2] = 1200f;
                                rings[0].roll = -MathHelper.PiOver2;
                            }
                        }
                        else {
                            const int width = (int)(DIAMETER * 2f);
                            const int height = 900;
                            Vector2 dustPos = center + new Vector2(-width / 2f, 0f);
                            Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerColors.CosmicEnergy, 2f);
                            Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerColors.CosmicEnergy, 2f);
                            Dust.NewDust(dustPos, width, height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, GlimmerColors.CosmicEnergy, 2f);
                        }
                    }
                }
                break;

            case ACTION_STARS: {
                    LerpToDefaultRotationVelocity();

                    NPC.ai[1]++;

                    if (NPC.ai[2] == 0f) {
                        if (Main.expertMode) {
                            NPC.ai[2] = 18f;
                            NPC.ai[3] = 96f;
                        }
                        else {
                            NPC.ai[2] = 7.65f;
                            NPC.ai[3] = 192f;
                        }
                    }

                    if (NPC.ai[1] % NPC.ai[3] == 0f) {
                        if (PlrCheck()) {
                            SoundEngine.PlaySound(AequusSounds.OmegaStariteStarBullets with { Volume = 0.3f, Pitch = 0.5f, PitchVariance = 0.1f }, NPC.Center);

                            int type = ModContent.ProjectileType<OmegaStariteBullet>();
                            float speed2 = Main.expertMode ? 12.5f : 5.5f;
                            int damage = 15;
                            if (Main.expertMode)
                                damage = 10;
                            float rot = MathHelper.TwoPi / (Main.getGoodWorld ? 10f : 5f);
                            for (int i = 0; i < (Main.getGoodWorld ? 3 : 2); i++) {
                                for (float f = 0f; f < MathHelper.TwoPi; f += rot) {
                                    var v = (f - MathHelper.PiOver2).ToRotationVector2();
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                    Main.projectile[p].timeLeft += 120;
                                }
                                speed2 *= 1.2f;
                            }
                        }
                        else {
                            break;
                        }
                    }
                    float distance = (center - plrCenter).Length();
                    if (distance > DIAMETER * 3.75f) {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center) * NPC.ai[2], 0.02f);
                    }
                    else if (distance < DIAMETER * 2.25f) {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(center - plrCenter) * NPC.ai[2], 0.02f);
                    }
                    else {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center).RotatedBy(MathHelper.PiOver2) * NPC.ai[2], 0.02f);
                    }

                    if (NPC.ai[1] > 480f) {
                        NPC.ai[0] = ACTION_ORBITAL_1;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                }
                break;

            case ACTION_ASSAULT:
                Assault(center, plrCenter, player);
                break;

            case ACTION_ORBITAL_3: {
                    NPC.ai[2]++;
                    if (NPC.ai[2] > 300f) {
                        if (NPC.ai[1] > 0.0314) {
                            NPC.ai[1] -= 0.0005f;
                        }
                        else {
                            NPC.ai[1] = 0.0314f;
                        }
                        rings[0].yaw += NPC.ai[1];
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += NPC.ai[1] * 0.5f;
                        }

                        PullInRingsTransition();
                    }
                    else if ((center - plrCenter).Length() > 1800f) {
                        NPC.ai[2] = 300f;
                        rings[0].yaw += NPC.ai[1];
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += NPC.ai[1] * 0.5f;
                        }
                    }
                    else {
                        if (NPC.ai[1] >= 0.0628f) {
                            NPC.ai[1] = 0.0628f;
                        }
                        else {
                            NPC.ai[1] += 0.0002f;
                        }
                        rings[0].yaw += NPC.ai[1];
                        rings[0].radiusFromOrigin = MathHelper.Lerp(rings[0].radiusFromOrigin, rings[0].OriginalRadiusFromOrigin * NPC.ai[3], 0.025f);
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += NPC.ai[1] * 0.5f;
                            rings[i].radiusFromOrigin = MathHelper.Lerp(rings[i].radiusFromOrigin, rings[i].OriginalRadiusFromOrigin * (NPC.ai[3] + i), 0.025f);
                        }
                        if (NPC.ai[2] > 100f) {
                            NPC.localAI[0]++;
                            if (Main.getGoodWorld) {
                                bool shot = false;
                                for (int i = 0; i < rings.Count; i++) {
                                    shot |= ShootProjsFromRing(endingPhase: false, rings[i]);
                                }
                                if (shot) {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                    NPC.localAI[0] = 0f;
                                }
                            }
                            else if (Vector2.Distance(plrCenter, center) > rings[0].radiusFromOrigin) {
                                if (ShootProjsFromRing(endingPhase: false, rings[1])) {
                                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                    NPC.localAI[0] = 0f;
                                }
                            }
                        }
                    }
                }
                break;

            case ACTION_ORBITAL_2: {
                    if (NPC.ai[1] == 0f) {
                        rings[0].pitch %= MathHelper.Pi;
                        rings[0].roll %= MathHelper.Pi;
                        rings[1].pitch %= MathHelper.Pi;
                        rings[1].roll %= MathHelper.Pi;
                    }
                    NPC.ai[1] += 0.0002f;

                    bool allRingsSet = true;
                    for (int i = 0; i < rings.Count; i++) {
                        rings[i].rotationVelocity *= 0.95f;
                        rings[i].pitch = rings[i].pitch.AngleLerp(0f, 0.025f);
                        rings[i].roll = rings[i].roll.AngleLerp(0f, 0.025f);
                        if (allRingsSet && (!CloseEnough(rings[i].pitch, 0f, 0.314f) || !CloseEnough(rings[i].roll, 0f, 0.314f))) {
                            allRingsSet = false;
                        }
                    }
                    if (NPC.ai[1] > 0.0314f) {
                        if (allRingsSet) {
                            NPC.velocity = Vector2.Normalize(plrCenter - center) * NPC.velocity.Length();
                            for (int i = 0; i < rings.Count; i++) {
                                rings[i].pitch = 0f;
                                rings[i].roll = 0f;
                            }
                            if (PlrCheck()) {
                                NPC.ai[0] = ACTION_ORBITAL_3;
                                NPC.ai[1] = 0f;
                                NPC.ai[3] = 3f + (1f - NPC.life / (float)NPC.lifeMax) * 1.5f;
                            }
                        }
                    }
                    else {
                        rings[0].yaw += 0.0314f - NPC.ai[1];
                        for (int i = 1; i < rings.Count; i++) {
                            rings[i].yaw += 0.0157f - NPC.ai[1] * 0.5f;
                        }
                    }
                }
                break;

            case ACTION_ORBITAL_1: {
                    LerpToDefaultRotationVelocity();
                    if (NPC.ai[1] == 0f) {
                        if (PlrCheck()) {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                            NPC.ai[1] = plrCenter.X + player.velocity.X * 20f;
                            NPC.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
                            NPC.netUpdate = true;
                        }
                        else {
                            break;
                        }
                    }
                    if ((center - new Vector2(NPC.ai[1], NPC.ai[2])).Length() < DIAMETER) {
                        if (NPC.velocity.Length() < 2f) {
                            ResetRingsRadiusFromOrigin();
                            if (PlrCheck()) {
                                NPC.velocity *= 0.1f;
                                if (NPC.life / (float)NPC.lifeMax < 0.5f) {
                                    NPC.ai[0] = ACTION_LASER_ORBITAL_2;
                                }
                                else {
                                    NPC.ai[0] = ACTION_ORBITAL_2;
                                }
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                            }
                        }
                        else {
                            NPC.velocity *= 0.925f;
                        }
                    }
                    else {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(new Vector2(NPC.ai[1], NPC.ai[2]) - center) * 30f, 0.025f);
                    }
                }
                break;

            case ACTION_INTRO:
                var r = NPC.getRect();
                r.Inflate(24, 24);
                for (int i = 0; i < Main.maxItems; i++) {
                    if (Main.item[i].active && Main.item[i].type == ItemID.Burger) {
                        if (r.Intersects(Main.item[i].getRect())) {
                            NPC.ai[0] = -100f;
                            NPC.ai[1] = i;
                            Main.item[i].noGrabDelay = 120;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.velocity *= 0.5f;
                            NPC.netUpdate = true;
                            return;
                        }
                    }
                }
                Intro(center, plrCenter);
                break;

            case ACTION_INIT:
                int target = NPC.target;
                if (!NPC.HasValidTarget) {
                    NPC.TargetClosest(faceTarget: false);
                    target = NPC.target;
                }
                Initalize();
                NPC.netUpdate = true;
                NPC.target = target;
                NPC.ai[0] = ACTION_INTRO;
                NPC.ai[2] = plrCenter.Y - DIAMETER * 2.5f;
                break;

            case ACTION_GOODBYE:
                Goodbye();
                break;

            case ACTION_DEAD:
                Die();
                break;

            case -100: {
                    int item = (int)NPC.ai[1];
                    if (!Main.item[item].active) {
                        NPC.ai[0] = ACTION_ASSAULT;
                        return;
                    }
                    Main.item[item].noGrabDelay = 120;
                    NPC.velocity *= 0.95f;
                    for (int i = 0; i < rings.Count; i++) {
                        rings[i].rotationVelocity *= 0.99f;
                    }
                    NPC.ai[3]++;
                    NPC.localAI[3]++;
                    //if ((int)NPC.localAI[3] == 100) {
                    //    SoundEngine.PlaySound(AequusSounds.explosion with { Volume = 0.66f, Pitch = -0.05f }, NPC.Center);
                    //}

                    if (NPC.ai[3] > 60f) {
                        Main.item[item].velocity = Vector2.Zero;
                        if (NPC.ai[3] < 1000f) {
                            float amt = (float)Math.Sin((NPC.ai[3] * 0.02f - 60f) * 0.5f) * 2f;
                            NPC.ai[2] += amt;
                            if (amt < -1f) {
                                NPC.ai[3] = 1000f;
                            }
                        }
                        else {
                            if (NPC.ai[2] < 10f) {
                                if (Main.netMode != NetmodeID.MultiplayerClient) {
                                    //Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ModContent.ItemType<BrickFish>(), 1);
                                    WorldGen.BroadcastText(this.GetLocalization("EasterEgg").ToNetworkText(), CommonColor.TEXT_BOSS);
                                }
                                Main.item[item].stack--;
                                if (Main.item[item].stack <= 0) {
                                    Main.item[item].TurnToAir();
                                    Main.item[item].active = false;
                                }
                                SoundEngine.PlaySound(SoundID.Item2, NPC.Center);
                                NPC.life = -33333;
                                NPC.KillEffects();
                                return;
                            }
                            else {
                                NPC.ai[2] -= 1f + (NPC.ai[3] - 1000f) * 0.01f;
                            }
                        }
                        Main.item[item].Center = Vector2.Lerp(
                            Main.item[item].Center,
                            NPC.Center + Vector2.Normalize(Main.item[item].Center - NPC.Center) * NPC.ai[2],
                            0.075f);
                    }
                    else {
                        NPC.ai[2] = Vector2.Distance(NPC.Center, Main.item[item].Center);
                    }
                }
                break;
        }
        for (int i = 0; i < rings.Count; i++) {
            rings[i].Update(center + NPC.velocity);
        }
        if (NPC.ai[0] != ACTION_DEAD && Action != -100) {
            int chance = 10 - (int)speed;
            if (chance < 2 || Main.rand.NextBool(chance)) {
                if (speed < 2f) {
                    var spawnPos = new Vector2(RADIUS, 0f);
                    int d = Dust.NewDust(center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), 2, 2, DustID.MagicMirror);
                    Main.dust[d].velocity = Vector2.Normalize(spawnPos - center) * speed * 0.25f;
                }
                else {
                    var spawnPos = new Vector2(RADIUS, 0f).RotatedBy(NPC.velocity.ToRotation() - MathHelper.Pi);
                    int d = Dust.NewDust(NPC.Center + spawnPos.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)), 2, 2, DustID.MagicMirror);
                    Main.dust[d].velocity = -NPC.velocity * 0.25f;
                }
            }
            if (Main.rand.NextBool(30)) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
            }
            if (Main.rand.NextBool(30)) {
                int g = Gore.NewGore(new EntitySource_HitEffect(NPC), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
        }
        if (Main.netMode == NetmodeID.Server) {
            return;
        }
        Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 2.2f));
        for (int i = 0; i < rings.Count; i++) {
            for (int j = 0; j < rings[i].amountOfSegments; j++) {
                Lighting.AddLight(new Vector2(rings[i].CachedPositions[i].X, rings[i].CachedPositions[i].Y), new Vector3(0.4f, 0.4f, 1f));
            }
        }
    }

    public void Assault(Vector2 center, Vector2 plrCenter, Player player) {
        LerpToDefaultRotationVelocity();

        if (NPC.ai[1] < 0f) {
            NPC.ai[1]++;
            if (NPC.ai[2] == 0f) {
                if (PlrCheck()) {
                    NPC.ai[2] = Main.expertMode ? 18f : 6f;
                }
                else {
                    return;
                }
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(plrCenter - center) * NPC.ai[2], 0.02f);
        }
        else {
            if (!PlrCheck())
                return;
            if (NPC.ai[1] <= 60f || Vector2.Distance(new Vector2(NPC.ai[1], NPC.ai[2]), plrCenter) > 600f) {
                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
                NPC.ai[1] = plrCenter.X + player.velocity.X * 20f;
                NPC.ai[2] = plrCenter.Y + player.velocity.Y * 20f;
            }
            if ((center - new Vector2(NPC.ai[1], NPC.ai[2])).Length() < DIAMETER) {
                NPC.ai[3]++;
                if (NPC.ai[3] > 5) {
                    NPC.ai[0] = ACTION_ORBITAL_1;
                    NPC.ai[1] = 0f;
                    NPC.ai[3] = 0f;
                }
                else {
                    NPC.ai[1] = -NPC.ai[3] * 16;
                    if (Main.getGoodWorld || Vector2.Distance(plrCenter, center) > 120f) {
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            float lifePercent = NPC.life / (float)NPC.lifeMax;
                            if (Main.getGoodWorld || Main.expertMode && lifePercent < 0.75f || lifePercent < 0.6f) {
                                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                                int type = ModContent.ProjectileType<OmegaStariteBullet>();
                                float speed2 = Main.expertMode ? 12.5f : 5.5f;
                                int damage = 15;
                                if (Main.expertMode)
                                    damage = 10;
                                float rot = MathHelper.TwoPi / (Main.getGoodWorld ? 10f : 5f) + 0.01f;
                                for (float f = 0f; f < MathHelper.TwoPi; f += rot) {
                                    var v = (f - MathHelper.PiOver2).ToRotationVector2();
                                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), center + v * RADIUS, v * speed2, type, damage, 1f, player.whoAmI, -60f, speed2);
                                    Main.projectile[p].timeLeft += 120;
                                }
                            }
                        }
                    }
                    NPC.netUpdate = true;
                }
                NPC.ai[2] = 0f;
            }
            else {
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(new Vector2(NPC.ai[1], NPC.ai[2]) - center) * 20f, 0.025f);
            }
        }
    }
    public void Intro(Vector2 center, Vector2 plrCenter) {
        if (NPC.ai[2] == 0f) {
            NPC.ai[2] = plrCenter.Y;
        }
        LerpToDefaultRotationVelocity();
        if (center.Y > NPC.ai[2]) {
            int[] choices = new int[] { ACTION_ORBITAL_1, ACTION_ASSAULT };
            NPC.ai[0] = choices[Main.rand.Next(choices.Length)];
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.netUpdate = true;
        }
        else {
            float fallSpeed = Main.getGoodWorld ? 56f : 36f;
            NPC.velocity.X = 0f;
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, fallSpeed, 0.025f);
        }
    }
    public void Die() {
        for (int i = 0; i < rings.Count; i++) {
            rings[i].rotationVelocity *= 0f;
        }
        if (NPC.ai[1] > 20f && NPC.ai[1] < DEATHTIME * 1f) {
            for (int i = 0; i < NPC.ai[1] / 40f; i++) {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[1] * Main.rand.NextFloat(0.2f, 3f) * 3f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(Color.HotPink, Color.White, Math.Min(Main.rand.NextFloat(1f) - NPC.ai[1] / 10f, 1f)) with { A = 0 });
                d.velocity *= 0.2f;
                d.velocity += (NPC.Center - d.position) / 8f;
                d.scale = Main.rand.NextFloat(0.3f, 2f + NPC.ai[1] / 30f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }
        NPC.ai[1] += 0.5f;

        //if ((int)(NPC.ai[1] * 2f) == 40) {
        //    SoundEngine.PlaySound(AequusSounds.explosion with { Volume = 0.66f, Pitch = -0.05f }, NPC.Center);
        //}

        if (NPC.ai[1] > DEATHTIME * 1.314f) {
            NPC.life = -33333;
            NPC.HitEffect();
            NPC.checkDead();
        }
    }
    public void Goodbye() {
        if (NPC.timeLeft > 120)
            NPC.timeLeft = 120;
        NPC.velocity.X *= 0.975f;
        NPC.velocity.Y -= 0.2f;

        rings[0].yaw += 0.0314f;
        rings[0].roll += 0.0157f;
        rings[0].pitch += 0.01f;
        rings[1].yaw += 0.0157f;
        rings[1].roll += 0.0314f;
        rings[1].pitch += 0.011f;
    }
    private void Initalize(bool bestiaryDummy = false) {
        if (!bestiaryDummy)
            NPC.TargetClosest(faceTarget: false);
        else if (!Main.getGoodWorld)
            NPC.scale *= 0.5f;
        Initalize_Rings();
    }
    public void Initalize_Rings() {
        var center = NPC.Center;
        rings = new();
        if (Main.expertMode) {
            rings.Add(new(OmegaStariteRing.SEGMENTS_1, DIAMETER, OmegaStariteRing.SCALE_1));
            if (!Main.getGoodWorld) {
                rings.Add(new(OmegaStariteRing.SEGMENTS_2, DIAMETER * OmegaStariteRing.DIAMETERMULT_2_EXPERT, OmegaStariteRing.SCALE_2_EXPERT));
            }
            else {
                rings.Add(new(OmegaStariteRing.SEGMENTS_2, DIAMETER * OmegaStariteRing.DIAMETERMULT_2_EXPERT, OmegaStariteRing.SCALE_2_EXPERT));
                rings.Add(new(OmegaStariteRing.SEGMENTS_3, DIAMETER * OmegaStariteRing.DIAMETERMULT_3, OmegaStariteRing.RING_3_SCALE));
            }
        }
        else {
            rings.Add(new(OmegaStariteRing.SEGMENTS_1, DIAMETER * 0.75f, OmegaStariteRing.SCALE_1));
            rings.Add(new(OmegaStariteRing.SEGMENTS_2, DIAMETER * OmegaStariteRing.DIAMETERMULT_2, OmegaStariteRing.SCALE_2));
        }
        for (int i = 0; i < rings.Count; i++) {
            rings[i].MultScale(NPC.scale);
            rings[i].Update(center);
        }
        if (Main.netMode != NetmodeID.MultiplayerClient && !NPC.IsABestiaryIconDummy) {
            int damage = Main.expertMode ? 12 : 15;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero,
                ModContent.ProjectileType<OmegaStariteProj>(), damage, 1f, Main.myPlayer, NPC.whoAmI + 1);
        }
    }
    private bool PlrCheck() {
        NPC.TargetClosest(faceTarget: false);
        NPC.netUpdate = true;
        if (!NPC.HasValidTarget || NPC.Distance(Main.player[NPC.target].Center) > 4000f) {
            NPC.ai[0] = ACTION_GOODBYE;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.localAI[0] = 0f;
            NPC.localAI[1] = 0f;
            NPC.localAI[2] = 0f;
            NPC.localAI[3] = 0f;
            return false;
        }
        return true;
    }
    private void LerpToDefaultRotationVelocity() {
        rings[0].rotationVelocity = Vector3.Lerp(rings[0].rotationVelocity, new Vector3(0.01f, 0.0157f, 0.0314f), 0.1f);
        rings[1].rotationVelocity = Vector3.Lerp(rings[1].rotationVelocity, new Vector3(0.011f, 0.0314f, 0.0157f), 0.1f);
        if (rings.Count > 2) {
            rings[2].rotationVelocity = Vector3.Lerp(rings[2].rotationVelocity, new Vector3(0.012f, 0.0186f, 0.0214f), 0.1f);
        }
    }
    private bool ShootProjsFromRing(bool endingPhase, OmegaStariteRing ring) {
        int delay = Main.expertMode ? 12 : 60;
        if (!endingPhase && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 1000f) {
            delay /= 2;
        }
        if (NPC.localAI[0] > delay) {
            if (Main.getGoodWorld || NPC.life / (float)NPC.lifeMax < 0.75f) {
                float speed = 7.5f;
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    var diff = new Vector2(ring.CachedPositions[0].X, ring.CachedPositions[0].Y) - NPC.Center;
                    var shootDir = Vector2.Normalize(diff).RotatedBy(MathHelper.PiOver2) * speed;
                    int type = ModContent.ProjectileType<OmegaStariteBullet>();
                    int damage = 12;
                    if (Main.expertMode)
                        damage = 9;
                    for (int i = 0; i < ring.amountOfSegments; i++) {
                        float rot = ring.rotationOrbLoop * i;
                        var position = NPC.Center + diff.RotatedBy(rot);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), position, shootDir.RotatedBy(rot), type, damage, 1f, Main.myPlayer);
                    }
                }
                return true;
            }
        }
        return false;
    }
    public void CullRingRotations() {
        for (int i = 0; i < rings.Count; i++) {
            rings[i].pitch %= MathHelper.TwoPi;
            rings[i].roll %= MathHelper.TwoPi;
        }
    }
    public void ResetRingsRadiusFromOrigin() {
        for (int i = 0; i < rings.Count; i++) {
            rings[i].radiusFromOrigin = rings[i].OriginalRadiusFromOrigin;
        }
    }
    public void PullInRingsTransition() {
        bool allRingsSet = true;
        float[] transitionSpeed = new float[rings.Count];
        transitionSpeed[0] = MathHelper.Pi;
        for (int i = 1; i < rings.Count; i++) {
            transitionSpeed[i] = MathHelper.PiOver2 * (3f + 2.5f * i);
        }
        for (int i = 0; i < rings.Count; i++) {
            if (rings[i].radiusFromOrigin > rings[i].OriginalRadiusFromOrigin) {
                rings[i].radiusFromOrigin -= transitionSpeed[i];
                allRingsSet = false;
            }
        }

        if (allRingsSet && Main.netMode != NetmodeID.MultiplayerClient) {
            for (int i = 0; i < rings.Count; i++) {
                rings[i].radiusFromOrigin = rings[i].OriginalRadiusFromOrigin;
            }
            if (PlrCheck()) {
                var choices = new List<int>
                {
                    ACTION_ASSAULT,
                };
                if (NPC.life / (float)NPC.lifeMax < (Main.expertMode ? 0.5f : 0.33f))
                    choices.Add(ACTION_STARS);
                if (choices.Count == 1) {
                    NPC.ai[0] = choices[0];
                }
                else {
                    NPC.ai[0] = choices[Main.rand.Next(choices.Count)];
                }
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.localAI[1] = 0f;
                NPC.netUpdate = true;
            }
        }
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.ai[0] != ACTION_DEAD) {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6) {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    NPC.frame.Y = 0;
            }
        }
    }

    public override void UpdateLifeRegen(ref int damage) {
        if (Main.dayTime && Action != ACTION_DEAD && !Main.remixWorld) {
            NPC.lifeRegen = -10000;
            damage = 100;
        }
    }

    public override bool CheckDead() {
        if (Action == ACTION_DEAD || Main.dayTime) {
            NPC.lifeMax = -33333;
            return true;
        }
        //NPC.GetGlobalNPC<NoHitting>().preventNoHitCheck = true;
        NPC.ai[0] = ACTION_DEAD;
        NPC.ai[1] = 0f;
        NPC.ai[2] = 0f;
        NPC.ai[3] = 0f;
        NPC.velocity = new Vector2(0f, 0f);
        NPC.dontTakeDamage = true;
        NPC.life = NPC.lifeMax;
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool()) {
            target.AddBuff(ModContent.BuffType<BlueFire>(), 120);
        }

        if (Main.rand.NextBool()) {
            target.AddBuff(BuffID.Blackout, 360);
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return NPC.ai[0] >= 0;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) {
        scale = 1.5f;
        return null;
    }

    private delegate void DrawDelegate(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth);

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        drawColor = NPC.GetAlpha(drawColor);
        if (NPC.IsABestiaryIconDummy) {
            if ((int)NPC.ai[0] == 0) {
                Initalize(bestiaryDummy: true);
                NPC.ai[0]++;
            }
            LerpToDefaultRotationVelocity();
            for (int i = 0; i < rings.Count; i++) {
                rings[i].Update(NPC.Center);
            }
            NPC.scale = 0.5f;
        }
        if (rings == null) {
            return false;
        }
        var viewPos = NPC.IsABestiaryIconDummy ? NPC.Center : new Vector2(screenPos.X + Main.screenWidth / 2f, screenPos.Y + Main.screenHeight / 2f);
        var drawPos = NPC.Center - screenPos;
        drawPos.X = (int)drawPos.X;
        drawPos.Y = (int)drawPos.Y;
        if (NPC.IsABestiaryIconDummy) {
            drawPos.Y += 2f;
        }
        var positions = new List<Vector4>();
        for (int i = 0; i < rings.Count; i++) {
            for (int j = 0; j < rings[i].amountOfSegments; j++) {
                positions.Add(new Vector4((int)rings[i].CachedPositions[j].X, (int)rings[i].CachedPositions[j].Y, (int)rings[i].CachedPositions[j].Z, rings[i].Scale));
            }
        }
        float intensity = 1f;

        if (Action == -100) {
            var focus = Main.item[(int)NPC.ai[1]].Center;
            intensity += NPC.localAI[3] / 60;
            Lighting.GlobalBrightness -= intensity * 0.2f;

            //ScreenFlash.Flash.Set(Main.item[(int)NPC.ai[1]].Center, Math.Min(Math.Max(intensity - 1f, 0f) * 0.04f, 0.8f));
            ViewHelper.LegacyScreenShake(intensity * 0.5f);
            ModContent.GetInstance<CameraFocus>().SetTarget("Omega Starite", focus, CameraPriority.VeryImportant, 0.5f, 60);
        }
        else if (Action == ACTION_DEAD) {
            intensity += NPC.ai[1] / 20;
            if (NPC.CountNPCS(Type) == 1) {
                ModContent.GetInstance<CameraFocus>().SetTarget("Omega Starite", NPC.Center, CameraPriority.BossDefeat, 12f, 60);
            }
            float val = MathHelper.Clamp(3f - intensity, 0f, 1f);
            if (val < 0.1f) {
                Music = MusicID.Night;
            }
            for (int i = 0; i < Main.musicFade.Length; i++) {
                Main.musicFade[i] = Math.Min(Main.musicFade[i], val);
            }

            ScreenFlash.Instance.Set(NPC.Center, Math.Min(Math.Max(intensity - 1f, 0f) * 0.2f, 0.8f));
            ViewHelper.LegacyScreenShake(intensity * 2.25f);

            int range = (int)intensity + 4;
            drawPos += new Vector2(Main.rand.Next(-range, range), Main.rand.Next(-range, range));
            for (int i = 0; i < positions.Count; i++) {
                positions[i] += new Vector4(Main.rand.Next(-range, range), Main.rand.Next(-range, range), Main.rand.Next(-range, range), 0f);
            }
        }
        else if (_hitShake > 0) {
            drawPos += new Vector2(Main.rand.Next(-_hitShake, _hitShake), Main.rand.Next(-_hitShake, _hitShake));
            _hitShake--;
        }
        positions.Sort((o, o2) => -o.Z.CompareTo(o2.Z));
        Main.instance.LoadProjectile(ModContent.ProjectileType<OmegaStariteProj>());
        var omegiteTexture = TextureAssets.Projectile[ModContent.ProjectileType<OmegaStariteProj>()].Value;
        var omegiteFrame = new Rectangle(0, 0, omegiteTexture.Width, omegiteTexture.Height);
        var omegiteOrigin = omegiteFrame.Size() / 2f;
        float xOff = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1f);
        var clr3 = new Color(50, 50, 50, 0) * intensity;
        float deathSpotlightScale = 0f;
        if (intensity > 3f)
            deathSpotlightScale = NPC.scale * (intensity - 2.1f) * ((float)Math.Sin(NPC.ai[1] * 0.1f) + 1f) / 2f;
        var spotlight = AequusTextures.Bloom;
        var spotlightOrig = spotlight.Size() / 2f;
        var spotlightColor = Main.tenthAnniversaryWorld ? Color.DeepPink with { A = 0 } * 0.5f : new Color(100, 100, 255, 0);
        var drawOmegite = new List<DrawDelegate>();
        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        if (Aequus.HighQualityEffects) {
            drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth) {
                spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * 1.33f, SpriteEffects.None, 0f);
            });
        }
        drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth) {
            spriteBatch.Draw(omegiteTexture, position, omegiteFrame, drawColor, rotation, origin1, scale, SpriteEffects.None, 0f);
        });
        float secretIntensity = 1f;
        if (Action == -100) {
            DrawDeathLightRays(intensity, Main.item[(int)NPC.ai[1]].Center - screenPos, spotlight, spotlightColor, spotlightOrig, deathSpotlightScale, NPC.localAI[3] * 0.05f);
            float decMult = Math.Clamp(2f - intensity * 0.4f, 0f, 1f);
            secretIntensity *= decMult;
            byte a = drawColor.A;
            drawColor *= secretIntensity;
            drawColor.A = a;
            a = clr3.A;
            clr3 *= secretIntensity;
            clr3.A = a;
        }
        if (intensity * secretIntensity >= 1f) {
            drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth) {
                for (int j = 0; j < intensity * layerDepth; j++) {
                    spriteBatch.Draw(omegiteTexture, position + new Vector2(2f + xOff * 2f * j, 0f),
                        omegiteFrame, clr3 * layerDepth, rotation, origin1, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(omegiteTexture, position + new Vector2(2f - xOff * 2f * j, 0f),
                        omegiteFrame, clr3 * layerDepth, rotation, origin1, scale, SpriteEffects.None, 0f);
                }
            });
        }
        if (intensity * secretIntensity > 3f) {
            float omegiteDeathDrawScale = deathSpotlightScale * secretIntensity * 0.5f;
            drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth) {
                spriteBatch.Draw(spotlight, position, null, drawColor, rotation, spotlightOrig, scale * omegiteDeathDrawScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * omegiteDeathDrawScale * 2, SpriteEffects.None, 0f);
            });
        }

        for (int i = 0; i < positions.Count; i++) {
            if (positions[i].Z > 0f) {
                var drawPosition = ViewHelper.GetViewPoint(new Vector2(positions[i].X, positions[i].Y), positions[i].Z * 0.00728f, viewPos) - screenPos;
                var drawScale = ViewHelper.GetViewScale(positions[i].W, positions[i].Z * 0.0314f);
                foreach (var draw in drawOmegite) {
                    draw.Invoke(
                        omegiteTexture,
                        drawPosition,
                        omegiteFrame,
                        drawColor * secretIntensity,
                        drawScale,
                        omegiteOrigin,
                        NPC.rotation,
                        SpriteEffects.None,
                        secretIntensity);
                }
                positions.RemoveAt(i);
                i--;
            }
        }
        var texture = TextureAssets.Npc[NPC.type].Value;
        var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
        Vector2 origin = NPC.frame.Size() / 2f;
        float mult = 1f / NPCSets.TrailCacheLength[NPC.type];
        var clr = drawColor * 0.25f;
        //for (int i = 0; i < intensity; i++)
        //{
        //    spriteBatch.Draw(spotlight, drawPos, null, spotlightColor, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + i, SpriteEffects.None, 0f);
        //}
        spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * secretIntensity, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + intensity, SpriteEffects.None, 0f);

        spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * (1f - intensity) * secretIntensity, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + (intensity + 1), SpriteEffects.None, 0f);

        if (!NPC.IsABestiaryIconDummy) {
            //Main.spriteBatch.End();
            //Main.spriteBatch.BeginWorld(shader: true);

            if ((NPC.position - NPC.oldPos[1]).Length() > 0.01f) {
                DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.Trail, NPC.oldPos, OldDrawHelper.GenerateRotationArr(NPC.oldPos),
                    (p) => GlimmerColors.Blue * (1f - p),
                    (p) => 20 * (1f - p),
                    -Main.screenPosition + NPC.Size / 2f);
            }
            else {
                NPC.oldPos[0] = new Vector2(0f, 0f);
            }

            //Main.spriteBatch.End();
            //Main.spriteBatch.BeginWorld(shader: false);
        }

        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        for (int j = 0; j < intensity * secretIntensity; j++) {
            spriteBatch.Draw(texture, drawPos + new Vector2(2f + xOff * 2f * j, 0f), NPC.frame, clr3 * secretIntensity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPos - new Vector2(2f + xOff * 2f * j, 0f), NPC.frame, clr3 * secretIntensity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }
        for (int i = 0; i < positions.Count; i++) {
            var drawPosition = ViewHelper.GetViewPoint(new Vector2(positions[i].X, positions[i].Y), positions[i].Z * 0.00728f, viewPos) - screenPos;
            var drawScale = ViewHelper.GetViewScale(positions[i].W, positions[i].Z * 0.0314f);
            foreach (var draw in drawOmegite) {
                draw.Invoke(
                    omegiteTexture,
                    drawPosition,
                    omegiteFrame,
                    drawColor * secretIntensity,
                    drawScale,
                    omegiteOrigin,
                    NPC.rotation,
                    SpriteEffects.None,
                    secretIntensity);
            }
        }
        if (Action != -100)
            DrawDeathLightRays(intensity, drawPos, spotlight, spotlightColor, spotlightOrig, deathSpotlightScale, NPC.ai[1]);
        return false;
    }
    public void DrawDeathLightRays(float intensity, Vector2 drawPos, Texture2D spotlight, Color spotlightColor, Vector2 spotlightOrig, float deathSpotlightScale, float deathTime) {
        if (intensity > 3f || Action == -100 && intensity > 2f) {
            float intensity2 = intensity - 2f;
            float raysScaler = intensity2;
            if (deathTime > DEATHTIME) {
                float scale = (deathTime - DEATHTIME) * 0.2f;
                scale *= scale;
                raysScaler += scale;
                Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, NPC.rotation, spotlightOrig, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, NPC.rotation, spotlightOrig, scale * 2.15f, SpriteEffects.None, 0f);
            }
            else {
                Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, NPC.rotation, spotlightOrig, deathSpotlightScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, NPC.rotation, spotlightOrig, deathSpotlightScale * 2f, SpriteEffects.None, 0f);
            }

            var shineColor = new Color(200, 40, 150, 0) * Math.Min(raysScaler, 1f) * NPC.Opacity;

            var lightRay = AequusTextures.LightRayFlat;
            var lightRayOrigin = lightRay.Size() / 2f;

            var r = new FastRandom((int)NPC.localAI[0]);

            int i = 0;
            for (int k = 0; k < 24; k++) {
                float f = k + Main.GlobalTimeWrappedHourly * 0.12f + NPC.localAI[0];
                var rayScale = new Vector2(Helper.Oscillate(r.NextFloat(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * r.NextFloat(1f, 5f) * 0.1f, 0.3f, 1f) * r.NextFloat(0.5f, 2.25f));
                rayScale.X *= 0.02f;
                rayScale.X *= (float)Math.Pow(raysScaler, Math.Min(rayScale.Y, 1f));
                Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * NPC.Opacity, f, lightRayOrigin, raysScaler * rayScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * NPC.Opacity, f, lightRayOrigin, raysScaler * rayScale * 2f, SpriteEffects.None, 0f);
                i++;
            }

            var bloom = AequusTextures.Bloom;
            var bloomOrigin = bloom.Size() / 2f;
            raysScaler *= 0.7f;
            Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * raysScaler * NPC.Opacity, 0f, bloomOrigin, raysScaler, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * 0.5f * raysScaler * NPC.Opacity, 0f, bloomOrigin, raysScaler * 1.4f, SpriteEffects.None, 0f);

            Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
            var shineOrigin = shine.Size() / 2f;
            Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * raysScaler, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * raysScaler, SpriteEffects.None, 0);
        }
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) {
        if (ProjectileMetadata.IsStar.Contains(projectile.type)) {
            modifiers.FinalDamage *= starDamageMultiplier;
        }
    }

    #region Loot
    public override void BossLoot(ref string name, ref int potionType) {
        potionType = ItemID.RestorationPotion;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        //    if (Main.rand.NextBool(3))
        //        Item.NewItem(rect, ModContent.ItemType<CosmicTelescope>());

        //int bossBag = ModContent.ItemType<OmegaStariteBag>();
        //npcLoot.Add(ItemDropRule.ByCondition(LootBuilder.GetCondition_OnFirstKill(() => AequusWorld.downedOmegaStarite), ModContent.ItemType<SupernovaFruit>()));
        //npcLoot.Add(AequusDropRules.Trophy<OmegaStariteTrophy>());
        //npcLoot.Add(ItemDropRule.BossBag(bossBag));
        //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<OmegaStariteRelic>()));
        //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<DragonBall>(), AequusDropRules.DroprateMasterPet));
        //npcLoot.Add<FlawlessCondition>(ItemDropRule.Common(ModContent.ItemType<OriginPainting>()));
        //npcLoot.AddExpertDrop<CelesteTorus>(bossBag);
        //npcLoot.Add(LootBuilder.GetDropRule_PerPlayerInstanced<CosmicEnergy>(min: 3, max: 3));
        //npcLoot.AddBossLoot(bossBag, ItemDropRule.Common(ModContent.ItemType<OmegaStariteMask>(), AequusDropRules.DroprateMask));
        //npcLoot.AddBossLoot(bossBag, ItemDropRule.OneFromOptions(1, ModContent.ItemType<UltimateSword>(), ModContent.ItemType<Raygun>(), ModContent.ItemType<Gamestar>(), ModContent.ItemType<ScribbleNotebook>()));
    }
    #endregion

    public override void OnKill() {
        GlimmerSystem.EndEventDelay = 240;

        NPC.SetEventFlagCleared(ref WorldState._downedTrueCosmicBoss, -1);
    }

    public override void SendExtraAI(BinaryWriter writer) {
        if (rings == null) {
            writer.Write(0);
            return;
        }
        writer.Write(rings.Count);
        for (int i = 0; i < rings.Count; i++) {
            rings[i].SendNetPackage(writer);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        rings ??= new();
        int amt = reader.ReadInt32();
        if (rings.Count != amt) {
            rings.Clear();
            Initalize_Rings();
        }
        for (int i = 0; i < rings.Count; i++) {
            if (rings.Count < i || rings[i] == null) {
                Initalize_Rings();
            }
            rings[i].RecieveNetPackage(reader);
        }
    }

    public bool IsUltimateRayActive() {
        return NPC.ai[0] == ACTION_LASER_ORBITAL_1 && NPC.ai[2] < 1200f;
    }

    private static bool CloseEnough(float comparison, float intendedValue, float closeEnoughMargin = 1f) {
        return Math.Abs(comparison - intendedValue) <= closeEnoughMargin;
    }
}