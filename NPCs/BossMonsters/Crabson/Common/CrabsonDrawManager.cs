using Aequus.NPCs.BossMonsters.Crabson.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Aequus.NPCs.BossMonsters.Crabson.Segments;

public class CrabsonDrawManager {
    public CrabsonDrawManager() {
        mood = new();
        eyes = new();
        legs = new();
        arms = new();
    }

    public CrabsonMood mood;
    public CrabsonEyeDrawer eyes;
    public CrabsonLegsDrawer legs;
    public CrabsonArmsDrawer arms;

    public void OnAIUpdate() {
        mood.OnAIUpdate();
    }

    public void OnHitPlayer() {
        mood.Laugh();
    }

    public void OnDamageRecieved(NPC.HitInfo hit) {
        mood.OnDamageRecieved(hit);
    }

    public void FindFrame(NPC npc, NPC leftArm, NPC rightArm, int frameHeight) {
        if (mood.laughAnimation > 0) {
            npc.frame.Y = frameHeight * (2 + mood.laughAnimation / 6 % 2);
            mood.laughAnimation--;
        }
        else {
            npc.frame.Y = 0;
        }
        eyes.Update(npc, mood);
        legs.Update(npc);
        if (Main.netMode == NetmodeID.Server || npc.IsABestiaryIconDummy) {
            return;
        }

        Vector2 chainOffset = new(44f, -14f);
        Vector2 chainEndOffset = new(20f, 0f);
        arms.Clear();
        arms.AddArm(npc, npc.Center + chainOffset with { X = -chainOffset.X } + npc.netOffset, leftArm.Center + chainEndOffset.RotatedBy(leftArm.rotation == 0f ? MathHelper.Pi : leftArm.rotation) + leftArm.netOffset);
        arms.AddArm(npc, npc.Center + chainOffset + npc.netOffset, rightArm.Center + chainEndOffset.RotatedBy(rightArm.rotation) + rightArm.netOffset);
    }
}