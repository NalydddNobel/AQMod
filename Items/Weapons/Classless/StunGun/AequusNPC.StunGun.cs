using Aequus.Common.Players.Attributes;
using Aequus.Items.Weapons.Classless.StunGun;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Aequus.Common.NPCs;

public partial class AequusNPC {
    [ResetEffects]
    public bool stunGun;
    [ResetEffects]
    public bool stunGunVisual;
    public bool stunGunOld;
    public bool stunned_NoTileCollide;
    public bool stunned_NoGravity;

    private void Draw_StunGun(NPC npc, SpriteBatch spriteBatch, float waveTime) {
        var drawLocation = npc.Center + StunGun.GetVisualOffset(npc, waveTime);
        float scale = StunGun.GetVisualScale(npc);
        spriteBatch.Draw(AequusTextures.StunEffect, drawLocation - Main.screenPosition, null, Color.White with { A = 0 }, 0f, AequusTextures.StunEffect.Size() / 2f, (0.9f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f) * scale, SpriteEffects.None, 0f);
    }

    private void DrawBehindNPC_StunGun(NPC npc, SpriteBatch spriteBatch, ref Vector2 drawOffset) {
        if (stunGunVisual) {
            drawOffset += Main.rand.NextVector2Square(-2f, 2f);
            Draw_StunGun(npc, spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: false));
        }
    }

    private void DrawAboveNPC_StunGun(NPC npc, SpriteBatch spriteBatch) {
        if (stunGunVisual) {
            Draw_StunGun(npc, spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: true));
        }
    }

    private bool AI_StunGun(NPC npc) {
        bool updateFields = stunGunOld != stunGun;
        stunGunOld = stunGun;
        if (stunGun) {
            if (updateFields) {
                stunned_NoTileCollide = npc.noTileCollide;
                stunned_NoGravity = npc.noGravity;
                npc.noTileCollide = false;
                npc.noGravity = false;
            }
            npc.velocity.X *= 0.8f;
            if (npc.velocity.Y < 0f) {
                npc.velocity.Y *= 0.8f;
            }
            return true;
        }

        if (updateFields) {
            npc.noTileCollide = stunned_NoTileCollide;
            npc.noGravity = stunned_NoGravity;
        }
        return false;
    }
}