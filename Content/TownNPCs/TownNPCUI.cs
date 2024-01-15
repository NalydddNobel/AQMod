using Aequus.Common.UI;
using Aequus.Core;
using Aequus.Core.UI;
using System;
using Terraria.UI;

namespace Aequus.Content.TownNPCs;

public class TownNPCUI : UILayer {
    private readonly TrimmableDictionary<int, Exclamation> NPCExclamations = new();

    public void SetExclamation(int npcIndex, bool active) {
        if (NPCExclamations.TryGetValue(npcIndex, out Exclamation existingExclamation)) {
            existingExclamation.Active = active;
            return;
        }

        if (active && Cull2D.Rectangle(Main.npc[npcIndex].getRect())) {
            Exclamation popup = NPCExclamations[npcIndex] = new Exclamation();
            popup.Opacity = 0f;

            Activate();
        }
    }

    public override bool OnUIUpdate(GameTime gameTime) {
        foreach (var popup in NPCExclamations) {
            NPC npc = Main.npc[popup.Key];
            if (popup.Value.Opacity <= 0f || !Main.npc[popup.Key].active) {
                NPCExclamations.RemoveEnqueue(popup.Key);
                continue;
            }

            if (popup.Value.Active) {
                if (popup.Value.Opacity < 1f) {
                    popup.Value.Opacity += 0.01f;
                    if (popup.Value.Opacity > 1f) {
                        popup.Value.Opacity = 1f;
                    }
                }
            }
            else {
                if (popup.Value.Opacity > 0f) {
                    popup.Value.Opacity -= 0.01f;
                }
            }
        }

        NPCExclamations.RemoveAllQueued();
        return NPCExclamations.Count > 0;
    }

    protected override bool DrawSelf() {
        foreach (var pair in NPCExclamations) {
            NPC npc = Main.npc[pair.Key];
            Exclamation exclamation = pair.Value;

            Texture2D texture = AequusTextures.TownNPCExclamation;
            float opacity = exclamation.Opacity;
            float scale = Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.9f, 1.1f) * opacity;
            Vector2 drawPosition = (npc.Top + new Vector2(0f, -6f - 20f * MathF.Pow(opacity, 3f)) - Main.screenPosition).Floor();
            Vector2 origin = texture.Size() / 2f;
            Color color = new Color(150, 150, 255, 222) * opacity;
            Main.spriteBatch.Draw(AequusTextures.BloomStrong, drawPosition, null, Color.Black * opacity * 0.2f, 0f, AequusTextures.BloomStrong.Size() / 2f, 0.5f, SpriteEffects.None, 0f);

            if (scale > 1f) {
                float auraOpacity = (scale - 1f) / 0.1f;
                var spinningPoint = new Vector2(2f, 0f);
                for (int i = 0; i < 4; i++) {
                    Main.spriteBatch.Draw(texture, drawPosition + spinningPoint.RotatedBy(i * MathHelper.PiOver2), null, color with { A = 0 } * auraOpacity, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }
        return true;
    }

    private class Exclamation {
        public float Opacity;
        public bool Active;
    }

    public TownNPCUI() : base("Town NPC UI", InterfaceLayers.EntityHealthBars_16, InterfaceScaleType.Game) { }
}