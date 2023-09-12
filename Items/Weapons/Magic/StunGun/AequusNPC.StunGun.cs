using Aequus.Common.Players.Attributes;
using Aequus.Items.Weapons.Magic.StunGun;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Aequus.Common.NPCs;

public partial class AequusNPC {
    [ResetEffects]
    public bool stunGun;

    private void Draw_StunGun(NPC npc, SpriteBatch spriteBatch, float waveTime) {
        var drawLocation = npc.Center + StunGun.GetVisualOffset(npc, waveTime);
        float scale = StunGun.GetVisualScale(npc);
        spriteBatch.Draw(AequusTextures.StunEffect, drawLocation - Main.screenPosition, null, Color.White with { A = 0 }, 0f, AequusTextures.StunEffect.Size() / 2f, (0.9f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f) * scale, SpriteEffects.None, 0f);
    }

    private void DrawBehindNPC_StunGun(NPC npc, SpriteBatch spriteBatch) {
        if (stunGun) {
            Draw_StunGun(npc, spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: false));
        }
    }

    private void DrawAboveNPC_StunGun(NPC npc, SpriteBatch spriteBatch) {
        if (stunGun) {
            Draw_StunGun(npc, spriteBatch, StunGun.GetVisualTime(StunGun.VisualTimer, front: true));
        }
    }
}