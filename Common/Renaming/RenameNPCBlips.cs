using Aequus.Common.UI;
using Aequus.Core;
using ReLogic.Graphics;
using System;
using System.Runtime.InteropServices;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.Renaming;

public class RenameNPCBlips : UILayer {
    public const int AnimationLength = 60;
    public const int AnimationIn = 20;
    public const int AnimationOut = 40;

    public static DynamicSpriteFont Font => FontAssets.MouseText.Value;

    public override string Layer => InterfaceLayers.EntityHealthBars_16;
    public override InterfaceScaleType ScaleType => InterfaceScaleType.Game;

    private class Animation {
        public string Name { get; private set; }
        public Vector2 TextMeasurement { get; private set; }

        public Vector2 backgroundScale = new Vector2(0f, 1f);

        public float time = AnimationLength;

        public float opacity = 0f;
        public float backgroundOpacity = 1f;
        public float textOpacity = 0f;

        public void ApplyName(string name, DynamicSpriteFont font) {
            Name = name;
            TextMeasurement = ChatManager.GetStringSize(font, name, Vector2.One);
        }
    }
    private static readonly TrimmableDictionary<int, Animation> _animations = new();

    public static void Add(int i, string customName) {
        var anim = CollectionsMarshal.GetValueRefOrAddDefault<int, Animation>(_animations, i, out _) ??= new();
        anim.ApplyName(customName, Font);
    }

    public override void OnPreUpdatePlayers() {
        foreach (var pair in _animations) {
            var anim = pair.Value;
            if (anim.time != 0) {
                anim.time--;
                if (anim.opacity != 1f) {
                    anim.opacity += 1f / AnimationIn;
                    if (anim.opacity > 1f) {
                        anim.opacity = 1f;
                    }

                    anim.textOpacity = MathF.Pow(anim.opacity, 4f);
                    anim.backgroundScale.X = anim.textOpacity;
                }
            }
            else {
                anim.opacity -= 1f / AnimationOut;
                if (anim.opacity <= 0f) {
                    _animations.RemoveEnqueue(pair.Key);
                }
                else {
                    anim.backgroundOpacity = anim.textOpacity = MathF.Pow(anim.opacity, 2f);
                }
            }
        }
        _animations.RemoveAllQueued();
    }

    public override bool Draw(SpriteBatch spriteBatch) {
        foreach (var pair in _animations) {
            var npc = Main.npc[pair.Key];
            if (!npc.active) {
                continue;
            }
            npc.nameOver = 0f;

            var anim = pair.Value;

            string name = anim.Name;
            var font = Font;
            var textMeasurement = anim.TextMeasurement;
            var backgroundScale = new Vector2(textMeasurement.X / 2f + 4f, textMeasurement.Y - 2f) * anim.backgroundScale;
            var drawLocation = npc.Top + new Vector2(0f, npc.gfxOffY - textMeasurement.Y / 2f) - Main.screenPosition;
            float textOpacity = anim.textOpacity;
            float backgroundOpacity = anim.backgroundOpacity;

            if (backgroundScale != Vector2.Zero && backgroundOpacity > 0f) {
                var texture = TextureAssets.Extra[ExtrasID.FairyQueenLance].Value;
                var textureOrigin = new Vector2(0f, texture.Height / 2f);
                var realScale = backgroundScale / texture.Size();
                var backgroundLocation = drawLocation - new Vector2(0f, 4f);
                for (int i = 0; i < 2; i++) {
                    float rotation = MathHelper.Pi * i;

                    spriteBatch.Draw(texture, backgroundLocation + new Vector2(0f, backgroundScale.Y / -2f), null, Color.Black * backgroundOpacity, rotation, textureOrigin, realScale with { Y = 2f }, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, backgroundLocation + new Vector2(0f, backgroundScale.Y / 2f), null, Color.Black * backgroundOpacity, rotation, textureOrigin, realScale with { Y = 2f }, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture, backgroundLocation, null, Color.Black * 0.5f * backgroundOpacity, rotation, textureOrigin, realScale, SpriteEffects.None, 0f);
                }
            }
            if (textOpacity > 0f) {
                spriteBatch.DrawText(font, name, drawLocation, Color.White * textOpacity, 0f, textMeasurement / 2f, Vector2.One);
            }
        }

        return true;
    }
}