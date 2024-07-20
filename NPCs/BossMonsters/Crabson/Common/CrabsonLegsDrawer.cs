using ReLogic.Utilities;
using System;
using Terraria.Audio;

namespace Aequus.NPCs.BossMonsters.Crabson.Segments;

public struct CrabsonLegsDrawer {
    public const int MaxFrames = 8;

    public bool _floatingWalk;
    public bool _soundsEnabled;

    public int frame;
    public float frameCounter;
    public int soundDelay;
    public SlotId walkingSoundSlot;

    private void UpdateSounds(NPC npc, float magnitudeX, bool aboveAir) {
        if (magnitudeX > 0.5f && !aboveAir) {
            if (!walkingSoundSlot.IsValid || soundDelay <= 0) {
                walkingSoundSlot = SoundEngine.PlaySound(AequusSounds.walk with { Volume = 0.5f }, npc.Bottom + new Vector2(0f, -12f));
                soundDelay = 70;
            }
        }
        else if ((aboveAir || magnitudeX < 0.2f) && walkingSoundSlot.IsValid && SoundEngine.TryGetActiveSound(walkingSoundSlot, out var sound)) {
            sound.Stop();
        }
    }

    public void Update(NPC npc) {
        float magnitudeX = Math.Abs(npc.velocity.X);
        if (soundDelay > 0) {
            soundDelay--;
        }

        bool aboveAir = _floatingWalk ? false : Math.Abs(npc.velocity.Y) > 0.001f;
        if (aboveAir) {
            frame = 7;
            frameCounter = 0f;
        }
        else {
            frameCounter += magnitudeX;
            if (frameCounter > 10f) {
                frame = (frame + 1) % 7;
                frameCounter = 0f;
            }

            if (magnitudeX < 0.1f) {
                frame = 0;
            }
        }

        if (!npc.IsABestiaryIconDummy && Main.netMode != NetmodeID.Server) {
            if (_soundsEnabled) {
                UpdateSounds(npc, magnitudeX, aboveAir);
            }
        }
    }

    public void Draw(NPC npc, SpriteBatch spriteBatch, Vector2 drawPosition, Color bodyDrawColor) {
        var legFrame = AequusTextures.Crabson_Legs.Frame(verticalFrames: MaxFrames, frameY: frame);
        spriteBatch.Draw(
            AequusTextures.Crabson_Legs,
            drawPosition,
            legFrame,
            bodyDrawColor,
            npc.rotation,
            legFrame.Size() / 2f,
            npc.scale, npc.velocity.X < 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
    }
}