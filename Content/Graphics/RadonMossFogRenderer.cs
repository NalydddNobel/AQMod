using Aequus.Common.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Content.Graphics;

public class RadonMossFogRenderer : ScreenRenderer {
    public static RadonMossFogRenderer Instance { get; private set; }
    public static List<Point> Tiles { get; private set; }
    public static List<DrawInfo> DrawInfoCache { get; private set; }
    public readonly struct DrawInfo {
        public readonly Vector2 Position;
        public readonly float Intensity;
        public readonly Rectangle Frame;
        public readonly float Rotation;

        public DrawInfo(Vector2 pos, float intensity, Rectangle frame, float rotation) {
            Position = pos;
            Intensity = intensity;
            Frame = frame;
            Rotation = rotation;
        }
    }

    public override int FinalResultResolutionDiv => 2;

    public override void Load(Mod mod) {
        base.Load(mod);
        Tiles = new List<Point>();
        DrawInfoCache = new List<DrawInfo>();
        Instance = this;
        if (!Main.dedServ) {
        }
    }

    public override void Unload() {
        Instance = null;
        DrawInfoCache?.Clear();
        DrawInfoCache = null;
        Tiles?.Clear();
        Tiles = null;
    }

    protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
        if (Tiles.Count > 0) {
            DrawInfoCache.Clear();
            var me = Main.LocalPlayer;
            var myPosition = me.GetPlayerFocusPosition();

            foreach (var p in Tiles) {
                int i = p.X;
                int j = p.Y;
                FastRandom rand = Helper.RandomTileCoordinates(i, j);
                var lighting = DrawHelper.GetBrightestLight(new Point(i, j), 6);
                float intensity = 1f - (lighting.R + lighting.G + lighting.B) / 765f;
                intensity = MathHelper.Lerp(intensity, 1f, (float)MathHelper.Clamp(Vector2.Distance(new Vector2(i * 16f + 8f, j * 16f + 8f), myPosition) / 250f - Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly * rand.NextFloat(0.1f, 0.6f))), 0f, 1f));
                if (intensity <= 0f) {
                    continue;
                }

                var drawCoordinates = new Vector2(i * 16f, j * 16f);
                for (int k = 0; k < 10; k++) {
                    DrawInfoCache.Add(new DrawInfo(drawCoordinates + new Vector2(8 + k * 8f).RotatedBy(rand.NextFloat(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.NextFloat(0.03f, 0.2f)), intensity * 1f, AequusTextures.Fog.Frame(verticalFrames: 8, frameY: rand.Next(8)), rand.NextFloat(MathHelper.TwoPi)));
                    if (rand.Next(2) == 0) {
                        break;
                    }
                }
            }
            Tiles.Clear();
        }

        if (DrawInfoCache.Count > 0) {
            Main.spriteBatch.Begin_World(shader: false);

            var texture = AequusTextures.Fog;
            var origin = AequusTextures.Fog.GetCenteredFrameOrigin(verticalFrames: 8);
            foreach (var info in DrawInfoCache) {
                Main.spriteBatch.Draw(texture, info.Position - Main.screenPosition, info.Frame, Color.Black * info.Intensity, info.Rotation, origin, 2f, SpriteEffects.None, 0f);
            }

            PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv, RenderTargetUsage.PreserveContents);

            spriteBatch.End();

            //var shader = GameShaders.Armor.GetSecondaryShader(Main.LocalPlayer.dye[0].dye, Main.LocalPlayer);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            DrawData dd = new(helperTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            //if (shader != null) {
            //    shader.Apply(null, dd);
            //}
            dd.Draw(spriteBatch);

            spriteBatch.End();

            _wasPrepared = true;
        }
    }

    protected override bool PrepareTarget() {
        return Tiles.Count > 0 || DrawInfoCache.Count > 0;
    }

    public void DrawOntoScreen(SpriteBatch spriteBatch) {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

        var drawData = new DrawData(GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(255, 255, 255, 255));
        //Shader.ShaderData.Apply(drawData);

        drawData.Draw(spriteBatch);

        spriteBatch.End();
        _wasPrepared = false;
    }
}
