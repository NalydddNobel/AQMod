using Aequus.Core;
using Aequus.Core.UI;
using System;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.Renaming;

public class NPCNameTagPopup : UILayer {
    private readonly TrimmableDictionary<Int32, Popup> NPCPopups = new();

    public void ShowRenamePopup(Int32 npcIndex) {
        Popup popup = NPCPopups[npcIndex] = new Popup();
        popup.Time = 1f;

        Activate();
    }

    public override Boolean OnUIUpdate(GameTime gameTime) {
        foreach (var popup in NPCPopups) {
            NPC npc = Main.npc[popup.Key];
            if (popup.Value.Time <= 0f || !Main.npc[popup.Key].active) {
                NPCPopups.RemoveEnqueue(popup.Key);
                continue;
            }

            npc.nameOver = 0f; // Disable the name over for gamepad

            if (popup.Value.Time > 0f) {
                popup.Value.Time -= 0.01f;
            }
        }

        NPCPopups.RemoveAllQueued();
        return NPCPopups.Count > 0;
    }

    protected override Boolean DrawSelf() {
        foreach (var pair in NPCPopups) {
            NPC npc = Main.npc[pair.Key];
            Popup popup = pair.Value;

            String name = npc.GivenName;
            var font = FontAssets.MouseText.Value;
            var textMeasurement = ChatManager.GetStringSize(font, name, Vector2.One);
            var backgroundScale = new Vector2(textMeasurement.X / 2f + 4f, textMeasurement.Y - 2f);
            var drawLocation = npc.Top + new Vector2(0f, npc.gfxOffY - textMeasurement.Y / 2f) - Main.screenPosition;
            Single textOpacity = 1f;
            Single backgroundOpacity = 0.5f;

            if (popup.Time > 0.8f) {
                Single animation = 1f - MathF.Pow((popup.Time - 0.8f) / 0.2f, 4f);
                backgroundScale.X *= animation;
                textOpacity *= animation;
            }
            if (popup.Time < 0.4f) {
                Single animation = MathF.Pow(popup.Time / 0.4f, 2f);
                textOpacity *= animation;
                backgroundOpacity *= animation;
            }

            if (backgroundScale != Vector2.Zero) {
                var texture = TextureAssets.Extra[ExtrasID.FairyQueenLance].Value;
                var textureOrigin = new Vector2(0f, texture.Height / 2f);
                var realScale = backgroundScale / texture.Size();
                var backgroundLocation = drawLocation - new Vector2(0f, 4f);
                for (Int32 i = 0; i < 2; i++) {
                    Single rotation = MathHelper.Pi * i;

                    Main.spriteBatch.Draw(texture, backgroundLocation + new Vector2(0f, backgroundScale.Y / -2f), null, Color.Black * backgroundOpacity, rotation, textureOrigin, realScale with { Y = 2f }, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, backgroundLocation + new Vector2(0f, backgroundScale.Y / 2f), null, Color.Black * backgroundOpacity, rotation, textureOrigin, realScale with { Y = 2f }, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(texture, backgroundLocation, null, Color.Black * 0.5f * backgroundOpacity, rotation, textureOrigin, realScale, SpriteEffects.None, 0f);
                }
            }
            if (textOpacity > 0f) {
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, name, drawLocation, Color.White * textOpacity, 0f, textMeasurement / 2f, Vector2.One);
            }

            npc.nameOver = 0f;
        }
        return true;
    }

    private class Popup {
        public Single Time;
    }

    public NPCNameTagPopup() : base("Name Tag Popup", InterfaceLayerNames.EntityHealthBars_16, InterfaceScaleType.Game, InsertOffset: -1) { }
}
