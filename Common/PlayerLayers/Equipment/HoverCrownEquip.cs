using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Common.PlayerLayers.Equipment {
    public class HoverCrownEquip : EquipDraw {
        protected override string EquipTextureSuffix => "_Crown";

        private float _hoverAnimation;
        public Color CrownColor;

        public override void Clear() {
            base.Clear();
            CrownColor = default;
            _hoverAnimation += Math.Clamp(Main.LocalPlayer.velocity.Length() / 4f, 1f, 3f);
        }

        public override void Draw(ref PlayerDrawSet drawInfo, AequusPlayer aequus) {
            var drawLocation = drawInfo.Position + new Vector2(drawInfo.drawPlayer.width / 2f, -20f + Helper.Wave(_hoverAnimation / 30f, -2f, 2f) + Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.GetAnimationFrame()].Y);
            drawLocation.X -= drawInfo.drawPlayer.velocity.X;
            if (drawInfo.drawPlayer.velocity.Y < 0f) {
                drawLocation.Y -= Math.Min(drawInfo.drawPlayer.velocity.Y * 0.25f, 8f);
            }
            else {
                drawLocation.Y -= drawInfo.drawPlayer.velocity.Y;
            }

            var texture = GetEquipTexture();
            Color color;
            if (drawInfo.shadow != 0f && CrownColor != default) {
                color = CrownColor with { A = 0 } * 0.5f;
            }
            else {
                color = Helper.GetColor(drawLocation);
            }
            color *= 1f - drawInfo.shadow;
            var screenPosition = (drawLocation - Main.screenPosition).Floor();
            if (drawInfo.shadow == 0f && CrownColor != default) {
                drawInfo.DrawDataCache.Add(
                    new DrawData(
                        AequusTextures.Bloom0,
                        screenPosition,
                        null,
                        CrownColor with { A = 0 } * Helper.Wave(Main.GlobalTimeWrappedHourly * 0.1f, 0.1f, 0.2f) * (1f - drawInfo.shadow),
                        0f,
                        AequusTextures.Bloom0.Size() / 2f,
                        1f,
                        SpriteEffects.None,
                        0
                    )
                );
            }
            drawInfo.DrawDataCache.Add(
                new DrawData(
                    texture.Value,
                    screenPosition, 
                    null,
                    color, 
                    Math.Clamp(drawInfo.drawPlayer.velocity.X * 0.05f, -0.5f, 0.5f),
                    texture.Value.Size() / 2f,
                    1f, 
                    drawInfo.drawPlayer.direction.ToSpriteEffect(), 
                    0
                ) { shader = Dye, }
            );
        }
    }
}