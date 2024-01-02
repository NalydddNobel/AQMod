using System;
using Terraria.GameContent;

namespace Aequus.Content.Critters.HorseshoeCrab;

public abstract class HorseshoeCrab : ModNPC {
    public float tailRotation;

    public override void SetDefaults() {
        NPC.aiStyle = -1;
        NPC.lifeMax = 5;
        NPC.damage = 0;
        NPC.defense = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void FindFrame(int frameHeight) {
        tailRotation = Utils.AngleLerp(tailRotation, NPC.rotation, 0.1f);
    }

    public override void AI() {
        float detectionRange = 200f;
        int closestPlayer = -1;
        for (int i = 0; i < Main.maxPlayers; i++) {
            if (Main.player[i].active && !Main.player[i].DeadOrGhost && !Main.player[i].invis) {
                float distance = NPC.Distance(Main.player[i].Center);
                if (distance < detectionRange) {
                    detectionRange = distance;
                    closestPlayer = i;
                }
            }
        }

        if (NPC.ai[1] == 0) {

            NPC.ai[1] = Main.rand.Next(-52, -20);
            if (closestPlayer != -1) {
                NPC.velocity.X += Math.Sign(NPC.Center.X - Main.player[closestPlayer].Center.X);
                NPC.ai[1] /= 5f;
            }
            else {
                NPC.velocity.X += NPC.ai[2];
                NPC.ai[2] = Main.rand.NextFloat(0.5f, 1.2f) * (Main.rand.NextBool() ? -1 : 1);
            }
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -4f, 4f);
            NPC.spriteDirection = Math.Sign(NPC.velocity.X + 0.01f);

            NPC.netUpdate = true;
        }
        if (NPC.ai[1] < 0f) {
            NPC.ai[1]++;
            if (NPC.ai[1] >= 0) {
                if (closestPlayer != -1) {
                    NPC.ai[1] = 0f;
                }
                else {
                    NPC.ai[1] = Main.rand.Next(30, 180);
                    NPC.netUpdate = true;
                }
            }
        }
        else if (NPC.ai[1] > 0f) {
            NPC.velocity.X *= 0.9f;
            NPC.ai[1]--;
            if (closestPlayer != -1) {
                NPC.ai[1] *= 0.9f;
            }
            if (NPC.ai[1] < 0f) {
                NPC.ai[1] = 0f;
            }
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        var texture = TextureAssets.Npc[Type].Value;
        var bottom = NPC.Center + (NPC.rotation + MathHelper.PiOver2).ToRotationVector2() * NPC.Size * 0.5f - screenPos;
        bottom.Y += DrawOffsetY;
        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        bool onWall = false;
        var frame = texture.Frame(verticalFrames: 4, frameY: onWall ? 1 : 0);
        var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(texture, bottom + (NPC.rotation + MathHelper.PiOver2 + MathHelper.PiOver2 * NPC.spriteDirection).ToRotationVector2() * frame.Width, frame with { Y = frame.Y + frame.Height * 2 }, drawColor, tailRotation, frame.Size() / 2f, NPC.scale, effects, 0f);
        spriteBatch.Draw(texture, bottom, frame, drawColor, NPC.rotation, frame.Size() / 2f, NPC.scale, effects, 0f);
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
