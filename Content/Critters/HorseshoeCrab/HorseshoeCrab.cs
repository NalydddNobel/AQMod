using Aequus.Common.NPCs;
using Aequus.Content.Biomes.PollutedOcean;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace Aequus.Content.Critters.HorseshoeCrab;

[ModBiomes(typeof(PollutedOceanBiome))]
public abstract class HorseshoeCrab : ModNPC {
    public float tailRotation;
    public int closestPlayerOld;
    public int wallTime;

    public override LocalizedText DisplayName => this.GetCategoryText("HorseshoeCrab.DisplayName");

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;
    }

    public override void SetDefaults() {
        NPC.lifeMax = 5;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;

        // Why re-invent the wheel? (or in this case, AI which walks along the edges of tiles)
        NPC.aiStyle = NPCAIStyleID.Snail;
        AIType = NPCID.Snail;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void FindFrame(int frameHeight) {
    }

    public override void AI() {
        const int GetUpTime = -30;

        if (wallTime < 0) {
            NPC.aiStyle = -1;
            NPC.directionY = 1;
            NPC.noGravity = false;
            if (NPC.collideY || wallTime > -20) {
                NPC.velocity.X *= 0.9f;
                wallTime++;
            }

            if (wallTime == GetUpTime) {
                NPC.velocity.Y = -3f;
                NPC.direction = -NPC.direction;
                NPC.spriteDirection = NPC.direction;
                wallTime++;
            }
            if (wallTime > GetUpTime) {
                NPC.rotation = Utils.AngleTowards(NPC.rotation, 0f, 0.1f);
                tailRotation = Utils.AngleLerp(tailRotation, NPC.rotation, 0.1f);
                DrawOffsetY = 0f;
            }
            else {
                NPC.rotation = Utils.AngleTowards(NPC.rotation, MathHelper.Pi + MathF.Sin(wallTime / 5f) * 0.2f, 0.1f);
                tailRotation = Utils.AngleTowards(tailRotation, MathHelper.PiOver2 * 1.5f * -NPC.spriteDirection, 0.05f);
                DrawOffsetY = 4f;
            }

            return;
        }

        NPC.aiStyle = NPCAIStyleID.Snail;
        tailRotation = Utils.AngleLerp(tailRotation, NPC.rotation, 0.1f);
        DrawOffsetY = 0f;

        float detectionRange = closestPlayerOld != -1 ? 250f : 100f;
        int closestPlayer = -1;
        NPC.TargetClosest(faceTarget: NPC.direction == 0);
        if (NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) < detectionRange) {
            closestPlayer = NPC.target;
        }

        if (closestPlayerOld != closestPlayer) {
            closestPlayerOld = closestPlayer;

            // Swap Directions
            if (closestPlayer != -1) {
                float directionX = NPC.Center.X + NPC.width / 2 * NPC.direction;
                float directionY = NPC.Center.Y + NPC.height / 2 * NPC.directionY;
                if (NPC.direction != 0 && !Framing.GetTileSafely((int)(directionX / 16f), (int)(NPC.Center.Y / 16f)).IsFullySolid()) {
                    NPC.direction = Math.Sign(NPC.Center.X - Main.player[closestPlayer].Center.X);
                }
                if (!NPC.collideY && NPC.directionY != 0 && !Framing.GetTileSafely((int)(NPC.Center.Y / 16f), (int)(directionY / 16f)).IsFullySolid()) {
                    NPC.directionY = Math.Sign(NPC.Center.Y - Main.player[closestPlayer].Center.Y);
                }
            }
        }

        if (!NPC.TryGetGlobalNPC<AequusNPC>(out var aequusNPC)) {
            return;
        }

        aequusNPC.statSpeedX += 1f;
        aequusNPC.statSpeedY += 1f;
        if (closestPlayer != -1) {
            aequusNPC.statSpeedX += 4f;
            aequusNPC.statSpeedY += 4f;
        }

        // Snail "On Wall" state
        if ((int)NPC.ai[1] == 1) {
            wallTime += (int)aequusNPC.statSpeedY;
            if (NPC.directionY == -1) {
                aequusNPC.statSpeedY *= Math.Clamp(1f - wallTime / 350f, -0.2f, 1f);
            }

            if (wallTime > 400 && Main.netMode != NetmodeID.MultiplayerClient) {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.netUpdate = true;

                NPC.position.X -= NPC.direction;
                //NPC.velocity.X -= NPC.direction * Main.rand.NextFloat(4f, 8f);
                NPC.velocity.Y = 0f;

                wallTime = GetUpTime - 30; 
                if (Main.rand.NextBool()) {
                    wallTime = -480;
                }
            }
        }
        else {
            wallTime = 0;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        bool onWall = false;
        var texture = TextureAssets.Npc[Type].Value;
        var frame = texture.Frame(verticalFrames: Main.npcFrameCount[Type], frameY: onWall ? 1 : 0);
        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Vector2 down = (NPC.rotation + MathHelper.PiOver2).ToRotationVector2();
        Vector2 offset = new Vector2(0f, NPC.gfxOffY + DrawOffsetY);
        Vector2 bodyDrawCoordinates = NPC.Center + down * (NPC.height - frame.Height + 8f) / 2f + offset - screenPos;
        Vector2 tailCoordinates = NPC.Center + down * (NPC.height - 2f) / 2f + offset - screenPos + down.RotatedBy(MathHelper.PiOver2 * NPC.spriteDirection) * frame.Size()/2f;

        spriteBatch.Draw(texture, tailCoordinates, frame with { Y = frame.Y + frame.Height * 2 }, drawColor, tailRotation, new Vector2(NPC.spriteDirection == 1 ? (frame.Width-4) : 4, frame.Height - 5f), NPC.scale, effects, 0f);
        spriteBatch.Draw(texture, bodyDrawCoordinates, frame, drawColor, NPC.rotation, frame.Size() / 2f, NPC.scale, effects, 0f);
        
        //spriteBatch.Draw(AequusTextures.BaseParticleTexture, tailCoordinates, new(0, 0, 10, 10), Color.Red, NPC.rotation, new(5f, 5f), NPC.scale, effects, 0f);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        // Horseshoe crabs have blue blood
        int dustId = DustID.BlueMoss;

        if (NPC.life <= 0) {
            for (int i = 0; i < 10; i++) {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dustId, hit.HitDirection * 2f, -2f);
            }

            var source = NPC.GetSource_Death();
            if (Mod.TryFind<ModGore>(Name + "GoreHead", out var headGore)) {
                Gore.NewGore(source, NPC.position, NPC.velocity, headGore.Type);
            }
            if (Mod.TryFind<ModGore>(Name + "GoreTail", out var tailGore)) {
                Gore.NewGore(source, NPC.position, NPC.velocity, tailGore.Type);
            }
            return;
        }
        for (int i = 0; i < hit.Damage / NPC.lifeMax * 20; i++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, dustId, hit.HitDirection, -1f);
        }
    }
}
