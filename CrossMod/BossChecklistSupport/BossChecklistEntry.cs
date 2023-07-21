using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Aequus.CrossMod.BossChecklistSupport;

internal record BossChecklistEntry(string InternalName, LogEntryType EntryType, List<int> NPCIds, float Progression, Func<bool> Downed) {
    private LocalizedText CustomSpawnInfo;
    private Action<SpriteBatch, Rectangle, Color> CustomPortrait;

    public BossChecklistEntry UseCustomSpawnInfo(LocalizedText localizedText) {
        CustomSpawnInfo = localizedText;
        return this;
    }

    public BossChecklistEntry UseCustomPortrait(Asset<Texture2D> texture) {
        CustomPortrait = (spriteBatch, rect, color) => {
            var sourceRect = texture.Value.Bounds;
            float scale = Math.Min(1f, (float)rect.Width / sourceRect.Width);
            spriteBatch.Draw(texture.Value, rect.Center.ToVector2(), sourceRect, color, 0f, sourceRect.Size() / 2, scale, SpriteEffects.None, 0);
        };
        return this;
    }

    public void Register() {
        var extras = new Dictionary<string, object>();
        if (CustomSpawnInfo != null) {
            extras["spawnInfo"] = CustomSpawnInfo;
        }
        if (CustomPortrait != null) {
            extras["customPortrait"] = CustomPortrait;
        }

        BossChecklist.Instance.Call(
            $"Log{EntryType}",
            Aequus.Instance,
            InternalName,
            Progression,
            Downed,
            NPCIds,
            extras
        );
    }
}