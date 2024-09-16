using Terraria.GameContent;

namespace Aequus.Common.Utilities.Helpers;

public class NPCTools {
    /// <summary>Replicates the drawing of misc vanilla status effects. This is used to support these effects on NPCs with custom rendering.</summary>
    public static void ManuallyDrawNPCStatusEffects(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos) {
        var halfSize = npc.frame.Size() / 2f;
        if (npc.confused) {
            spriteBatch.Draw(TextureAssets.Confuse.Value, new Vector2(npc.position.X - screenPos.X + npc.width / 2 - TextureAssets.Npc[npc.type].Width() * npc.scale / 2f + halfSize.X * npc.scale, npc.position.Y - screenPos.Y + npc.height - TextureAssets.Npc[npc.type].Height() * npc.scale / Main.npcFrameCount[npc.type] + 4f + halfSize.Y * npc.scale + Main.NPCAddHeight(npc) - TextureAssets.Confuse.Height() - 20f), (Rectangle?)new Rectangle(0, 0, TextureAssets.Confuse.Width(), TextureAssets.Confuse.Height()), npc.GetShimmerColor(new Color(250, 250, 250, 70)), npc.velocity.X * -0.05f, TextureAssets.Confuse.Size() / 2f, Main.essScale + 0.2f, SpriteEffects.None, 0f);
        }
    }
}
