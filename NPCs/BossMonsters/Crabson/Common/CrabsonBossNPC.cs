using Aequus.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.Crabson.Common;

public abstract class CrabsonBossNPC : AequusBoss {
    #region Constants
    public const int ACTION_CLAWSHOTS = 2;
    public const int PHASE_GROUNDBUBBLES = 3;
    public const int ACTION_COMEONANDSLAM = 4;
    public const int PHASE2_GROUNDBUBBLES_SPAMMY = 5;
    public const int ACTION_P2_CLAWSHOTS_SHRAPNEL = 6;
    public const int ACTION_WELCOMETOTHESLAMJAM = 8;
    public const int ACTION_CLAWRAIN = 9;
    public const int ACTION_NOATTACK = 10;
    #endregion

    public int npcHandLeft = -1;
    public int npcHandRight = -1;
    public int npcBody = -1;

    public bool contactDamage;

    public NPC HandLeft => Main.npc.IndexInRange(npcHandLeft) ? Main.npc[npcHandLeft] : null;
    public NPC HandRight => Main.npc.IndexInRange(npcHandRight) ? Main.npc[npcHandRight] : null;
    public NPC Body => Main.npc[npcBody];

    public int SharedAction => (int)Main.npc[npcBody].ai[0];
    public bool PhaseTwo => Body.life * (Main.expertMode ? 2f : 4f) <= Body.lifeMax;
    public float LifeRatio => Math.Clamp(Body.life / (float)Body.lifeMax, 0f, 1f);
    public float BattleProgress => 1f - LifeRatio;
    public static readonly ConfiguredMusicData ConfiguredMusic = new(MusicID.Boss3, MusicID.OtherworldlyBoss2);

    public override void SetDefaults() {
        npcHandLeft = -1;
        npcHandRight = -1;
        npcBody = -1;

        NPC.lifeMax = 2500;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(gold: 2);
        NPC.aiStyle = -1;
        NPC.lavaImmune = true;
        NPC.trapImmune = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;

        if (Main.netMode != NetmodeID.Server) {
            Music = ConfiguredMusic.GetID();
            SceneEffectPriority = SceneEffectPriority.BossLow;
        }
    }

    protected bool CheckClaws() {
        if (npcHandLeft != -1 && (!HandLeft.active || HandLeft.ModNPC is not CrabsonBossNPC)) {
            return false;
        }
        if (npcHandRight != -1 && (!HandRight.active || HandRight.ModNPC is not CrabsonBossNPC)) {
            return false;
        }
        return true;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
        return contactDamage;
    }

    protected void SharedAI() {
        if (!NPC.dontTakeDamage) {
            AequusPlayer.DashImmunityHack.Add(NPC);
        }
    }

    protected void DrawClaw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, float mouthAnimation) {
        var claw = AequusTextures.CrabsonClaw.Value;
        var origin = new Vector2(claw.Width / 2f + 20f, claw.Height / 8f);
        var drawCoords = npc.Center + new Vector2(npc.direction * 10f, -20f) - screenPos;
        if (NPC.ModNPC != null) {
            drawCoords.Y += NPC.ModNPC.DrawOffsetY;
        }
        SpriteEffects spriteEffects;
        bool flip;
        if (npc.rotation == 0f) {
            spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            flip = npc.direction == 1;
            if (!flip) {
                origin.X = claw.Width - origin.X;
            }
        }
        else {
            spriteEffects = Math.Abs(npc.rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally : SpriteEffects.FlipHorizontally;
            flip = spriteEffects.HasFlag(SpriteEffects.FlipVertically);
        }

        DrawClawManual(spriteBatch, claw, drawCoords, drawColor, origin, npc.rotation, flip ? -mouthAnimation : mouthAnimation, npc.scale, spriteEffects);
    }
    protected void DrawClawManual(SpriteBatch spriteBatch, Texture2D claw, Vector2 drawCoords, Color drawColor, Vector2 origin, float rotation, float mouthAnimation, float scale, SpriteEffects spriteEffects) {
        int frameHeight = claw.Height / 4;
        var clawFrame = new Rectangle(0, frameHeight, claw.Width, frameHeight - 2);
        spriteBatch.Draw(claw, drawCoords, clawFrame, drawColor, -mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = 0, }, drawColor, mouthAnimation + 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(claw, drawCoords, clawFrame with { Y = frameHeight * (Math.Abs(mouthAnimation) > 0.05f ? 3 : 2), }, drawColor, 0.1f * NPC.direction + rotation, origin, scale, spriteEffects, 0f);
    }

    protected void ResetActionTimers() {
        ActionTimer = 0;
        NPC.ai[2] = 0f;
        NPC.ai[3] = 0f;
        NPC.localAI[0] = 0f;
        HandLeft.ai[1] = 0f;
        HandRight.ai[1] = 0f;
    }
}