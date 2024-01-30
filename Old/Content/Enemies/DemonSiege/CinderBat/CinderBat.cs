using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.DataSets;
using Aequus.Content.Tiles.Banners;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Equipment.Accessories.WarHorn;
using Aequus.Old.Content.Events.DemonSiege;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Old.Content.Enemies.DemonSiege.CinderBat;

[LegacyName("Cindera")]
[AutoloadBanner(legacyId: 7)]
[ModBiomes(typeof(DemonSiegeZone))]
public class CinderBat : LegacyAIBat {
    public const int FRAME_FLY_0 = 0;
    public const int FRAME_FLY_1 = 1;
    public const int FRAME_FLY_2 = 2;
    public const int FRAME_FLY_3 = 3;

    public const int FRAME_OPEN_MOUTH_0 = 4;
    public const int FRAME_OPEN_MOUTH_1 = 5;
    public const int FRAME_OPEN_MOUTH_2 = 6;
    public const int FRAME_OPEN_MOUTH_3 = 7;

    public const int FRAME_CHOMP_PRE = 8;
    public const int FRAME_CHOMP_0 = 9;
    public const int FRAME_CHOMP_1 = 10;
    public const int FRAME_CHOMP_2 = 11;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 12;
        NPCID.Sets.TrailCacheLength[NPC.type] = 4;
        NPCID.Sets.TrailingMode[NPC.type] = 7;
        ItemID.Sets.KillsToBanner[BannerItem] = 25;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
            Position = new Vector2(4f, 12f)
        });

        foreach (BuffEntry buff in BuffSets.DemonSiegeImmune) {
            NPCID.Sets.SpecificDebuffImmunity[Type][buff.Id] = true;
        }
        NPCSets.DealsHeatDamage.Add((NPCEntry)Type);
    }

    public override void SetDefaults() {
        NPC.width = 24;
        NPC.height = 24;
        NPC.aiStyle = -1;
        NPC.damage = 30;
        NPC.defense = 2;
        NPC.lifeMax = 125;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
        NPC.alpha = 50;
        NPC.lavaImmune = true;
        NPC.trapImmune = true;
        NPC.value = 250f;
        NPC.knockBackResist = 0.4f;
        NPC.gfxOffY = -6f;
        NPC.lavaMovementSpeed = 1f;

        //this.SetBiome<DemonSiegeZone>();

        if (Main.zenithWorld) {
            NPC.scale = 0.5f;
        }
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        NPC.lifeMax = (int)(NPC.lifeMax * (1f + 0.1f * numPlayers));
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        int count = 1;
        if (NPC.life <= 0)
            count = 35;
        for (int i = 0; i < count; i++) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
            d.velocity = (d.position - NPC.Center) / 8f;
            if (Main.rand.NextBool(3)) {
                d.velocity *= 2f;
                d.scale *= 1.75f;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                d.noGravity = true;
            }
        }
    }

    protected override float MaxSpeedX => 6f;
    protected override float SpeedYMax => 2f;
    protected override float SpeedY => 0.125f;

    protected override bool PreCheckCollisions() {
        NPC.TargetClosest();
        return true;
    }

    protected override bool ShouldApplyWaterEffects() {
        return NPC.HasValidTarget && NPC.position.Y > Main.player[NPC.target].position.Y + Main.player[NPC.target].height;
    }

    protected override void InWater() {
        NPC.TargetClosest();
    }

    public override void AI() {
        bool canHitPlayer = false;
        if (NPC.HasValidTarget) {
            var target = Main.player[NPC.target];
            canHitPlayer = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);
        }

        if (!canHitPlayer) {
            if (NPC.ai[3] < 300f) {
                NPC.ai[3]++;
            }
            else {
                NPC.ai[0] = 10f;
                NPC.ai[3] = 301f;
                NPC.noTileCollide = true;
            }
        }
        else {
            if ((int)NPC.ai[3] == 301) {
                NPC.ai[3] = 0f;
                NPC.noTileCollide = false;
            }
            else if (NPC.ai[3] > 0f) {
                NPC.ai[3]--;
            }
        }

        if ((int)NPC.ai[0] <= 120f) {
            NPC.knockBackResist = 0.2f;
            base.AI();
            var target = Main.player[NPC.target];
            var differenceY = target.position.Y + target.height / 2f - (NPC.position.Y + NPC.height / 2f);
            float differenceYAbs = Math.Abs(differenceY);
            if (differenceYAbs < target.height + NPC.height) {
                NPC.ai[0]++;
            }
            else if (differenceYAbs > target.height * 6 + NPC.height) {
                if (NPC.ai[0] > 0) {
                    NPC.ai[0]--;
                }

                if (NPC.ai[0] < 60f && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height)) {
                    NPC.velocity.Y += 0.1f * NPC.directionY;
                }
            }
            NPC.rotation = NPC.velocity.X * 0.0628f;
            NPC.spriteDirection = NPC.velocity.X < 0f ? -1 : 1;
        }
        else if (NPC.ai[0] < 300f) {
            NPC.velocity *= 0.94f;
            NPC.rotation = NPC.velocity.X * 0.0628f;
            NPC.ai[0]++;
            NPC.knockBackResist = 0.05f;
            if (NPC.velocity.Length() < 1f || NPC.ai[0] > 300f) {
                NPC.knockBackResist = 0f;
                NPC.spriteDirection = NPC.direction;
                NPC.velocity.X = NPC.direction * (Main.expertMode ? 12f : 6f);
                NPC.velocity.Y = 0f;
                NPC.rotation = 0f;
                NPC.ai[0] = 301f;
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                for (int i = 0; i < 5; i++) {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                }
                for (int i = 0; i < 2; i++) {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Smoke);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                }
            }
        }
        else if (NPC.ai[0] < 800f) {
            NPC.knockBackResist = 0f;
            if (NPC.collideX) {
                NPC.ai[0] = 802f;
                NPC.direction = -NPC.direction;
                NPC.velocity.X = -NPC.oldVelocity.X * 0.5f;
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                for (int i = 0; i < 30; i++) {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                }
                for (int i = 0; i < 10; i++) {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Smoke);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                }
                for (int i = 0; i < 2; i++) {
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity, NPC.velocity * 0.2f, 61 + Main.rand.Next(3));
                }
            }
            var target = Main.player[NPC.target];
            var differenceX = target.position.X + target.width / 2f - (NPC.position.X + NPC.width / 2f);
            var differenceY = target.position.Y + target.height / 2f - (NPC.position.Y + NPC.height / 2f);
            NPC.ai[0]++;
            if (Math.Abs(differenceY) > target.height * 2 + NPC.height)
                NPC.ai[0] += 3f;
            if (NPC.ai[0] > 800f)
                NPC.ai[0] = 802f;
            if (NPC.direction == -1) {
                if (differenceX > target.width + NPC.width * 4) {
                    NPC.ai[0] = 801f;
                    NPC.netUpdate = true;
                }
            }
            else {
                if (differenceX < -(target.width + NPC.width * 4)) {
                    NPC.ai[0] = 801f;
                    NPC.netUpdate = true;
                }
            }
            if (NPC.ai[0] == 801f) {
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                NPC.ai[0] = 0f;
                for (int i = 0; i < 10; i++) {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                }
                for (int i = 0; i < 5; i++) {
                    int d = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Smoke);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                }
                Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity, NPC.velocity * 0.1f, 61 + Main.rand.Next(3));
            }
            int d2 = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
            Main.dust[d2].noGravity = true;
            Main.dust[d2].scale = Main.rand.NextFloat(0.9f, 2f);
        }
        else {
            if ((int)NPC.ai[0] == 802) {
                NPC.velocity *= 0.98f;
                if (Math.Abs(NPC.velocity.X) < MaxSpeedX / 4f) {
                    NPC.ai[0] = -50f;
                }
            }
            else {
                NPC.velocity *= 0.9f;
                if (Math.Abs(NPC.velocity.X) < MaxSpeedX / 2f) {
                    NPC.ai[0] = 0f;
                }
            }
        }

        if (Main.zenithWorld) {
            NPC.noTileCollide = true;
        }

        if (Main.rand.NextBool(3)) {
            int d3 = Dust.NewDust(NPC.position + new Vector2(0f, -8f), NPC.width, NPC.height, DustID.Torch);
            Main.dust[d3].noGravity = Main.rand.NextBool();
        }
    }

    private static byte GetIntensity() {
        return (byte)(150 + (int)((Math.Sin(Main.GlobalTimeWrappedHourly * 5f) + 1.0) / 2.0 * 80.0));
    }

    public override Color? GetAlpha(Color drawColor) {
        byte i = GetIntensity();
        return new Color(i * 2, i * 2, i * 2, i);
    }

    public override void FindFrame(int frameHeight) {
        if (NPC.ai[0] <= 120f) {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5.0) {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * FRAME_FLY_3) {
                    NPC.frame.Y = 0;
                }
            }
        }
        else if (NPC.ai[0] < 300f) {
            NPC.frameCounter = 0.0;
            if (NPC.velocity.X > 2f) {
                NPC.frame.Y = frameHeight * FRAME_OPEN_MOUTH_0;
            }
            else {
                NPC.frame.Y = frameHeight * FRAME_OPEN_MOUTH_1;
            }
        }
        else if (NPC.ai[0] < 800f) {
            var difference = NPC.Center - Main.player[NPC.target].Center;
            float length = difference.Length();
            if ((int)NPC.frameCounter > 0 || length < NPC.width * 1.5f) {
                if ((int)NPC.frameCounter == 0) {
                    NPC.frame.Y = frameHeight * FRAME_CHOMP_0;
                }
                else {
                    if ((int)NPC.frameCounter / 4 % 2 == 0) {
                        NPC.frame.Y = frameHeight * FRAME_CHOMP_1;
                    }
                    else {
                        NPC.frame.Y = frameHeight * FRAME_CHOMP_2;
                    }
                }
                NPC.frameCounter++;
            }
            else if (length < NPC.width * 3f) {
                NPC.frame.Y = frameHeight * FRAME_OPEN_MOUTH_3;
            }
            else if (length < NPC.width * 6f) {
                NPC.frame.Y = frameHeight * FRAME_OPEN_MOUTH_2;
            }
        }
        else {
            if (NPC.frameCounter > 5.0) {
                if (NPC.frameCounter > 9.0) {
                    NPC.frameCounter = 9.0;
                }

                NPC.frameCounter--;
                NPC.frame.Y = frameHeight * FRAME_OPEN_MOUTH_2;
            }
            else {
                NPC.frameCounter += MathHelper.Clamp(Math.Abs(NPC.velocity.X) / 12f, 0.25f, 1f);
                if (NPC.frameCounter > 5.0) {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > frameHeight * FRAME_FLY_3) {
                        NPC.frame.Y = 0;
                    }
                }
            }
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.OneFromOptions(
            chanceDenominator: 6,
            ModContent.ItemType<WarHorn>(),
            ItemID.MagmaStone
        ));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DontStarveIsNotUp(), ItemID.BatBat, chanceDenominator: 250));
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.DontStarveIsUp(), ItemID.BatBat, chanceDenominator: 100));

        //this.CreateLoot(npcLoot)
        //    .Add<DemonicEnergy>(chance: 10, stack: 1)
        //    .AddOptions(chance: 6, ItemID.MagmaStone, ModContent.ItemType<WarHorn>())
        //    .Add<AncientHellBeamDye>(chance: 16, stack: 1)
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var origin = NPC.frame.Size() / 2f;
        var offset = new Vector2(NPC.width / 2f, NPC.height / 2f + NPC.gfxOffY);

        drawColor = NPC.GetAlpha(drawColor);

        byte intensity = GetIntensity();
        if ((int)NPC.ai[0] > 300 && (int)NPC.ai[0] < 800) {
            intensity = 0;
        }

        var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        if (intensity > 150) {
            float value = (intensity - 150) / 33f;
            var c = drawColor * 0.08f * value;
            if (Aequus.HighQualityEffects) {
                var spotlight = AequusTextures.BloomStrong;
                var spotlightOrigin = spotlight.Size() / 2f;
                Main.spriteBatch.Draw(spotlight, NPC.position + offset - screenPos, null, new Color(255, 150, 10, 0) * value, NPC.rotation, spotlightOrigin, NPC.scale * value * 0.15f, effects, 0f);
                Main.spriteBatch.Draw(spotlight, NPC.position + offset - screenPos, null, new Color(255, 150, 10, 0) * value * 0.1f, NPC.rotation, spotlightOrigin, NPC.scale * value * 0.3f, effects, 0f);
            }
            Main.spriteBatch.Draw(texture, NPC.position + offset - screenPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            Main.spriteBatch.Draw(texture, NPC.position + offset + new Vector2(value, 0f) - screenPos, NPC.frame, c, NPC.rotation, origin, NPC.scale, effects, 0f);
            Main.spriteBatch.Draw(texture, NPC.position + offset + new Vector2(-value, 0f) - screenPos, NPC.frame, c, NPC.rotation, origin, NPC.scale, effects, 0f);
            Main.spriteBatch.Draw(texture, NPC.position + offset + new Vector2(0f, value) - screenPos, NPC.frame, c, NPC.rotation, origin, NPC.scale, effects, 0f);
            Main.spriteBatch.Draw(texture, NPC.position + offset + new Vector2(0f, -value) - screenPos, NPC.frame, c, NPC.rotation, origin, NPC.scale, effects, 0f);
        }
        else {
            Main.spriteBatch.Draw(texture, NPC.position + offset - screenPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
        }
        return false;
    }

    public override bool? CanFallThroughPlatforms() {
        return Main.player[NPC.target].dead ? true : Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height;
    }
}