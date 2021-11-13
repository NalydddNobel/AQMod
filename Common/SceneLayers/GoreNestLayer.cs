using AQMod.Assets;
using AQMod.Assets.Graphics;
using AQMod.Assets.Textures;
using AQMod.Common.Config;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AQMod.Common.SceneLayers
{
    public sealed class GoreNestLayer : SceneLayer
    {
        private const int Length = 100;

        private static ushort[] x;
        private static ushort[] y;
        private static int index = -1;
        private static bool reset = false;

        private readonly bool _test;

        public GoreNestLayer(bool test = true)
        {
            x = new ushort[Length];
            y = new ushort[Length];
            index = 0;
            reset = false;
            _test = test;
        }

        public static void AddCorrds(int i, int j)
        {
            if (index == -1 || reset)
            {
                RefreshCoords();
                reset = false;
            }
            try
            {
                if (index < Length)
                {
                    x[index] = (ushort)i;
                    y[index] = (ushort)j;
                    index++;
                }
            }
            catch
            {
                index = -1;
            }
            //Main.NewText(index, Main.DiscoColor);
        }

        public static void RefreshCoords()
        {
            x = new ushort[Length];
            y = new ushort[Length];
            index = 0;
        }

        private static float portalSize() => 80f;

        private static Color portalColor()
        {
            if (AQMod.AprilFools)
                return Main.DiscoColor;
            return Color.Red;
        }

        protected override void Draw()
        {
            for (int k = 0; k < index; k++)
            {
                var portalPosition = new Vector2(x[k] * 16f + 24f, y[k] * 16f - 24f + (float)Math.Sin(Main.GlobalTime) * 4f);
                if (AQConfigClient.Instance.PortalShader)
                {
                    Main.spriteBatch.End();
                    BatcherMethods.StartShaderBatch_GeneralEntities(Main.spriteBatch);
                    SamplerDraw.DrawSampler("AQMod:GoreNestPortal", portalPosition - Main.screenPosition, 0f, new Vector2(portalSize()), portalColor());
                    Main.spriteBatch.End();
                    BatcherMethods.StartBatch_GeneralEntities(Main.spriteBatch);
                }
                else
                {
                    var texture = TextureCache.Lights[LightTex.Spotlight66x66Negative];
                    float size = portalSize() / texture.Size().Length();
                    Main.spriteBatch.Draw(texture, portalPosition - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, size, SpriteEffects.None, 0);
                }
                if (_test)
                {
                    var font = Main.fontMouseText;
                    var text = "time: " + DemonSiege.UpgradeTime;
                    var orig = font.MeasureString(text) / 2f;
                    ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, portalPosition + new Vector2(0f, -portalSize()) - Main.screenPosition, Color.White, 0f, orig, Vector2.One);
                }
                if (DemonSiege.IsActive && DemonSiege.BaseItem != null && DemonSiege.BaseItem.type > ItemID.None && DemonSiege.altarTopLeft() == new Point(x[k], y[k]))
                {
                    var texture = TextureCache.GetItem(DemonSiege.BaseItem.type);
                    var frame = new Rectangle(0, 0, texture.Width, texture.Height);
                    var origin = frame.Size() / 2f;
                    float scale = DemonSiege.BaseItem.scale;
                    float rotation = (float)Math.Sin(Main.GlobalTime) * 0.05f;
                    var drawPosition = new Vector2(portalPosition.X, portalPosition.Y);
                    float y2 = texture.Height / 2f;
                    if (y2 > 24f)
                        drawPosition.Y += 24f - y2;
                    if (ItemLoader.PreDrawInWorld(DemonSiege.BaseItem, Main.spriteBatch, Color.White, Color.White, ref rotation, ref scale, 666))
                        Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                    ItemLoader.PostDrawInWorld(DemonSiege.BaseItem, Main.spriteBatch, Color.White, Color.White, rotation, scale, 666);
                }
            }
            reset = true;
        }

        public override void Update()
        {
        }
    }
}