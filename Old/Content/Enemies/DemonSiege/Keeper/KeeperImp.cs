using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.DataSets;
using Aequus.Core.ContentGeneration;
using Aequus.Core.DataSets;
using Aequus.Old.Content.Events.DemonSiege;
using Aequus.Old.Content.Materials.SoulGem;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Old.Content.Enemies.DemonSiege.Keeper;

[AutoloadBanner(legacyId: 9)]
[ModBiomes(typeof(DemonSiegeZone))]
public class KeeperImp : ModNPC {
    public const int TAIL_FRAME_COUNT = 15;
    public const int WING_FRAME_COUNT = 1;

    public const int CHAINED_SOUL_RITUAL_TIME = 120;

    public override void SetStaticDefaults() {
        ItemID.Sets.KillsToBanner[BannerItem] = 25;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
            Position = new Vector2(1f, 12f)
        });
        foreach (BuffEntry buff in BuffSets.DemonSiegeImmune) {
            NPCID.Sets.SpecificDebuffImmunity[Type][buff.Id] = true;
        }
        NPCSets.DealsHeatDamage.Add((NPCEntry)Type);
    }

    public override void SetDefaults() {
        NPC.width = 30;
        NPC.height = 50;
        NPC.aiStyle = -1;
        NPC.damage = 50;
        NPC.defense = 12;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.lavaImmune = true;
        NPC.trapImmune = true;
        NPC.value = 200f;
        NPC.noGravity = true;
        NPC.knockBackResist = 0.1f;
        NPC.lavaMovementSpeed = 1f;
        NPC.npcSlots = 2f;

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

    public override void AI() {
        if ((int)NPC.ai[0] == -1) {
            NPC.velocity.X *= 0.98f;
            NPC.velocity.Y -= 0.025f;
            return;
        }

        int maximumTrappers = GetMaximumTrappers();
        int trapperId = ModContent.NPCType<ChainedSoul>();
        int whoAmI = NPC.whoAmI + 1;

        if ((int)NPC.ai[0] == 0) {
            NPC.ai[0] = 1f;
            SpawnInitialTrappers(maximumTrappers, trapperId, whoAmI);
        }

        if (Main.expertMode && NPC.ai[1] > 30f && NPC.ai[1] < 240f) {
            TrapperSummonRitual(maximumTrappers, trapperId, whoAmI);
        }

        NPC.TargetClosest(faceTarget: false);

        if (NPC.HasValidTarget) {
            float speed = 7f;
            // Trapper Ritual
            if (NPC.ai[1] < 0f) {
                float progress = Math.Abs(NPC.ai[1]) / CHAINED_SOUL_RITUAL_TIME;
                if ((int)NPC.ai[1] == -90f) {
                    SoundEngine.PlaySound(AequusSounds.KeeperImpSummonTelegraph, NPC.Center);
                }
                speed /= 4f;

                int rate = (int)(4 * progress);
                if (rate <= 0 || Math.Abs((int)NPC.ai[1]) % rate == 0) {
                    Vector2 spawnPosition = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f) * NPC.width * (1f - (1f - progress) * 0.5f);
                    Vector2 spawnVelocity = NPC.Center - spawnPosition;
                    Dust d = Dust.NewDustPerfect(spawnPosition, DustID.Torch, spawnVelocity / 10f + NPC.velocity);
                    d.noGravity = true;
                    d.scale = 1.5f * (1f - progress);
                }
            }

            // Preparing to for trapper attack
            if (NPC.ai[1] > 240f) {
                speed /= 2f;
            }

            NPC.ai[1]++;

            // Summon new trapper
            if ((int)NPC.ai[1] == 0f) {
                SoundEngine.PlaySound(AequusSounds.KeeperImpSummon, NPC.Center);
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, trapperId, Start: NPC.whoAmI, ai1: whoAmI);
            }

            if (NPC.ai[1] > 320f) {
                NPC.ai[1] = 0f;
            }

            Vector2 gotoPosition = Main.player[NPC.target].Center + new Vector2(0f, NPC.height * -2.5f);
            Vector2 difference = gotoPosition - NPC.Center;
            Vector2 gotoVelocity = Vector2.Normalize(difference);
            if (!NPC.noTileCollide && NPC.ai[1] > 180f && NPC.ai[1] < 210f) {
                gotoVelocity = -gotoVelocity;
                NPC.noTileCollide = true;
            }
            else if (NPC.noTileCollide && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height)) {
                NPC.noTileCollide = false;
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, gotoVelocity * speed, 0.015f);
        }
        else {
            NPC.noTileCollide = true;
            NPC.ai[0] = -1f;
        }

        if (Main.zenithWorld) {
            NPC.noTileCollide = true;
        }

        NPC.rotation = NPC.velocity.X * 0.0314f;
    }

    private void SpawnInitialTrappers(int maximumTrappers, int trapperId, int whoAmI) {
        int count = Main.rand.Next(maximumTrappers) + 1;
        int spawnX = (int)NPC.position.X + NPC.width / 2;
        int spawnY = (int)NPC.position.Y + NPC.height / 2;
        for (int i = 0; i < count; i++) {
            NPC.NewNPC(NPC.GetSource_FromAI(), spawnX, spawnY, trapperId, Start: NPC.whoAmI, ai1: whoAmI);
        }
    }

    private void TrapperSummonRitual(int maximumTrappers, int trapperId, int whoAmI) {
        int totalTrappers = 0;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && !Main.npc[i].friendly && Main.npc[i].type == trapperId && (int)Main.npc[i].ai[1] == whoAmI) {
                totalTrappers++;
            }
        }

        if (totalTrappers < maximumTrappers) {
            if (Main.rand.NextBool(60 + 200 * totalTrappers)) {
                NPC.ai[1] = -CHAINED_SOUL_RITUAL_TIME;
                NPC.netUpdate = true;
            }
        }
    }

    private int GetMaximumTrappers() {
        return Main.getGoodWorld ? 12 : 4;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        int count = 1;
        if (NPC.life <= 0) {
            count = 28;

            for (int i = -1; i <= 1; i += 2) {
                NPC.NewGore(AequusTextures.KeeperGoreHorn, NPC.Center + new Vector2(12f * i, -20f), NPC.velocity);
                NPC.NewGore(AequusTextures.KeeperGoreEar, NPC.Center + new Vector2(12f * i, 0f), NPC.velocity);
                NPC.NewGore(AequusTextures.KeeperGoreEar, NPC.Center + new Vector2(12f * i, 20f), NPC.velocity);
                NPC.NewGore(AequusTextures.KeeperGoreWing, NPC.Center + new Vector2(12f * i, 0f), NPC.velocity);
            }

            NPC.NewGore(AequusTextures.KeeperGoreHead, NPC.position, NPC.velocity);
        }
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
        for (int i = 0; i < count * 2; i++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, newColor: Color.DarkRed);
        }
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulGem>(), 3));
        npcLoot.Add(ItemDropRule.Common(ItemID.ObsidianRose, chanceDenominator: 25));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Vector2 drawPosition = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height / 2f);
        drawPosition.Y -= 10.5f;

        Texture2D texture = TextureAssets.Npc[Type].Value;
        Texture2D glowTexture = AequusTextures.KeeperImp_Glow.Value;
        Vector2 orig = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);

        DrawWings(spriteBatch, drawPosition, screenPos, drawColor);
        DrawTail(spriteBatch, drawPosition, screenPos, drawColor);

        spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, drawColor, NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(glowTexture, drawPosition - screenPos, NPC.frame, new Color(200, 200, 200, 0), NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
        return false;
    }
    public void DrawTail(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, Color drawColor) {
        Texture2D tailTexture = AequusTextures.KeeperImpTail.Value;
        int frameTime = (int)(Main.GlobalTimeWrappedHourly * 15f);
        int animation = frameTime % (TAIL_FRAME_COUNT * 2);
        int frame;
        if (animation > TAIL_FRAME_COUNT) {
            frame = TAIL_FRAME_COUNT - animation % TAIL_FRAME_COUNT;
        }
        else {
            frame = animation % TAIL_FRAME_COUNT;
        }
        SpriteEffects effects = SpriteEffects.None;
        int frameHeight = tailTexture.Height / TAIL_FRAME_COUNT;
        Rectangle tailFrame = new Rectangle(0, frameHeight * frame, tailTexture.Width, frameHeight - 2);
        Vector2 tailOrig = new Vector2(tailFrame.Width / 2f, 4f);
        Vector2 offset = new Vector2(0f, 8f).RotatedBy(NPC.rotation);
        spriteBatch.Draw(tailTexture, drawPosition - screenPos + offset, tailFrame, drawColor, NPC.rotation, tailOrig, NPC.scale, effects, 0f);
    }
    public void DrawWings(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, Color drawColor) {
        drawPosition.Y -= 10f;
        Texture2D wingsTexture = AequusTextures.KeeperImpWings.Value;
        Rectangle wingFrame = new Rectangle(0, 0, wingsTexture.Width / 2 - 2, wingsTexture.Height);
        Vector2 wingOrig = new Vector2(wingFrame.Width, 4f);
        float wingRotation = NPC.rotation + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 25f) * 0.314f;
        Vector2 wingOffset = new Vector2(-8f, 0f).RotatedBy(NPC.rotation);
        spriteBatch.Draw(wingsTexture, drawPosition - screenPos + wingOffset + new Vector2(0f, 6f), wingFrame, drawColor, wingRotation, wingOrig, NPC.scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(wingsTexture, drawPosition - screenPos - wingOffset + new Vector2(0f, 6f), new Rectangle(wingFrame.Width + 2, wingFrame.Y, wingFrame.Width, wingFrame.Height), drawColor, -wingRotation, new Vector2(wingFrame.Width - wingOrig.X, wingOrig.Y), NPC.scale, SpriteEffects.None, 0f);
    }

    public override bool? CanFallThroughPlatforms() {
        return Main.player[NPC.target].dead
            ? true
            : Main.player[NPC.target].position.Y
                > NPC.position.Y + NPC.height;
    }
}