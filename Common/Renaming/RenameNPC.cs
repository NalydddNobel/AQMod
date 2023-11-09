using Aequus;
using Aequus.Common.UI;
using Aequus.Content.Items.Tools.NameTag;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Aequus.Common.Renaming;

[LegacyName("NPCNameTag")]
[LegacyName("NameTagGlobalNPC")]
public class RenameNPC : GlobalNPC {
    public float nameTagAnimation;

    public bool HasCustomName => CustomName != null;

    public string CustomName { get; set; } = string.Empty;

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return NameTag.CanRename(entity);
    }

    private void CheckMarker(NPC npc) {
        if (Main.netMode == NetmodeID.MultiplayerClient || Main.GameUpdateCount % 60 != 0 || NPCLoader.SavesAndLoads(npc)) {
            return;
        }
    }

    public override bool PreAI(NPC npc) {
        if (npc.realLife != -1 && Main.npc[npc.realLife].TryGetGlobalNPC<RenameNPC>(out var nameTag)) {
            CustomName = nameTag.CustomName;
        }
        if (HasCustomName) {
            npc.GivenName = CustomName;
            CheckMarker(npc);
        }
        return true;
    }

    public override void SaveData(NPC npc, TagCompound tag) {
        if (HasCustomName && npc.realLife == -1) {
            if (npc.netID < NPCID.Count) {
                tag["ID"] = npc.netID; // Vanilla entities don't load properly for some reason! So I am doing this to save their ID for reloading properly.
            }

            tag["NameTag"] = CustomName;
        }
    }

    public override void LoadData(NPC npc, TagCompound tag) {
        var position = npc.position;

        // Workaround for vanilla entities not saving and loading properly
        if (npc.netID == 0 && tag.TryGet("ID", out int netID)) {
            npc.netID = netID;
            npc.type = netID;
            npc.CloneDefaults(netID);
        }

        npc.position = position;
        npc.timeLeft = (int)(NPC.activeTime * 1.25f);
        npc.wet = Collision.WetCollision(npc.position, npc.width, npc.height);
        if (tag.TryGet("NameTag", out string savedNameTag)) {
            CustomName = savedNameTag;
        }

#if DEBUG
        if (HasCustomName) {
            Mod.Logger.Debug($"netID: {npc.netID}, {npc}");
            Mod.Logger.Debug(CustomName ?? "Null");
        }
#endif
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (nameTagAnimation <= 0f || CustomName == null || npc.IsABestiaryIconDummy) {
            return true;
        }

        string name = CustomName;
        var font = FontAssets.MouseText.Value;
        var textMeasurement = ChatManager.GetStringSize(font, name, Vector2.One);
        var backgroundScale = new Vector2(textMeasurement.X / 2f + 4f, textMeasurement.Y - 2f);
        var drawLocation = npc.Top + new Vector2(0f, npc.gfxOffY - textMeasurement.Y / 2f) - screenPos;
        float textOpacity = 1f;
        float backgroundOpacity = 0.5f;

        if (nameTagAnimation > 0.8f) {
            float animation = 1f - MathF.Pow((nameTagAnimation - 0.8f) / 0.2f, 4f);
            backgroundScale.X *= animation;
            textOpacity *= animation;
        }
        if (nameTagAnimation < 0.4f) {
            float animation = MathF.Pow(nameTagAnimation / 0.4f, 2f);
            textOpacity *= animation;
            backgroundOpacity *= animation;
        }

        if (backgroundScale != Vector2.Zero) {
            var texture = TextureAssets.Extra[ExtrasID.FairyQueenLance].Value;
            var textureOrigin = new Vector2(0f, texture.Height / 2f);
            var realScale = backgroundScale / texture.Size();
            var backgroundLocation = drawLocation - new Vector2(0f, 4f);
            for (int i = 0; i < 2; i++) {
                float rotation = MathHelper.Pi * i;

                MiscWorldInterfaceElements.Draw(texture, backgroundLocation + new Vector2(0f, backgroundScale.Y / -2f), null, Color.Black * backgroundOpacity, rotation, textureOrigin, realScale with { Y = 2f }, SpriteEffects.None, 0f);
                MiscWorldInterfaceElements.Draw(texture, backgroundLocation + new Vector2(0f, backgroundScale.Y / 2f), null, Color.Black * backgroundOpacity, rotation, textureOrigin, realScale with { Y = 2f }, SpriteEffects.None, 0f);
                MiscWorldInterfaceElements.Draw(texture, backgroundLocation, null, Color.Black * 0.5f * backgroundOpacity, rotation, textureOrigin, realScale, SpriteEffects.None, 0f);
            }
        }
        if (textOpacity > 0f) {
            MiscWorldInterfaceElements.DrawColorCodedString(font, name, drawLocation, Color.White * textOpacity, 0f, textMeasurement / 2f, Vector2.One);
        }

        npc.nameOver = 0f;
        nameTagAnimation -= 0.01f;
        return true;
    }
}