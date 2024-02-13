using System;
using Terraria.GameContent;

namespace Aequus.Core.Utilities;

public static class ExtendNPC {
    public static void Kill(this NPC npc) {
        npc.StrikeInstantKill();
    }

    public static void KillEffects(this NPC npc, bool quiet = false) {
        npc.life = Math.Min(npc.life, -1);
        npc.HitEffect();
        npc.active = false;
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, 9999 + npc.lifeMax * 2 + npc.defense * 2);
        }
    }

    /// <summary>Manually sends a <see cref="MessageID.SyncNPC"/> packet to sync this NPC.</summary>
    public static void SyncNPC(NPC npc) {
        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
    }

    /// <summary>Runs <see cref="NPC.TargetClosest(bool)"/>, and returns <see cref="NPC.HasValidTarget"/>.</summary>
    public static bool TryRetargeting(this NPC npc, bool faceTarget = true) {
        npc.TargetClosest(faceTarget: faceTarget);
        return npc.HasValidTarget;
    }

    /// <returns>Whether or not this is most likely a critter, dependant on multiple checks of their stats and values in data sets.</returns>
    public static bool IsProbablyACritter(this NPC npc) {
        return NPCSets.CountsAsCritter[npc.type] || (npc.lifeMax < 5 && npc.lifeMax != 1 && npc.damage <= 0);
    }

    #region Drawing
    /// <summary>Replicates the drawing of misc vanilla status effects. This is used to support these effects on NPCs with custom rendering.</summary>
    public static void DrawNPCStatusEffects(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos) {
        var halfSize = npc.frame.Size() / 2f;
        if (npc.confused) {
            spriteBatch.Draw(TextureAssets.Confuse.Value, new Vector2(npc.position.X - screenPos.X + npc.width / 2 - TextureAssets.Npc[npc.type].Width() * npc.scale / 2f + halfSize.X * npc.scale, npc.position.Y - screenPos.Y + npc.height - TextureAssets.Npc[npc.type].Height() * npc.scale / Main.npcFrameCount[npc.type] + 4f + halfSize.Y * npc.scale + Main.NPCAddHeight(npc) - TextureAssets.Confuse.Height() - 20f), (Rectangle?)new Rectangle(0, 0, TextureAssets.Confuse.Width(), TextureAssets.Confuse.Height()), npc.GetShimmerColor(new Color(250, 250, 250, 70)), npc.velocity.X * -0.05f, TextureAssets.Confuse.Size() / 2f, Main.essScale + 0.2f, SpriteEffects.None, 0f);
        }
    }
    #endregion

    #region Buffs
    /// <summary>Clears a buff of the specified ID from this NPC.</summary>
    /// <param name="npc"></param>
    /// <param name="buffId">The Buff Id to clear from this NPC.</param>
    public static bool ClearBuff(this NPC npc, int buffId) {
        int index = npc.FindBuffIndex(buffId);
        if (index != -1) {
            npc.DelBuff(buffId);
            return true;
        }
        return false;
    }
    #endregion
}