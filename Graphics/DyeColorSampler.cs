using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class DyeColorSampler : ModSystem
    {
        private readonly struct RequestInfo
        {
            public readonly Vector3 Lighting;
            public readonly int player;

            public RequestInfo(int player, Vector3 vector)
            {
                this.player = player;
                Lighting = vector;
            }

            public RequestInfo(Vector3 vector) : this(Main.myPlayer, vector)
            {
            }
        }

        private static Vector3?[] dyeToColor;
        private static Dictionary<int, byte> dyeErrors;
        private static List<(int, RequestInfo)> dyeRequests;

        private static bool drawingDye;
        private static bool acceptingDyeRequests;
        public static bool AcceptingDyeRequests => acceptingDyeRequests || drawingDye;

        private static RenderTarget2D Target;
        private static RenderTarget2D SwapTarget;

        public static HashSet<int> DyeIgnore { get; private set; }

        public const int SAMPLESIZE = 25;

        public override void Load()
        {
            dyeRequests = new List<(int, RequestInfo)>();
            dyeErrors = new Dictionary<int, byte>();
            dyeToColor = Array.Empty<Vector3?>();
            DyeIgnore = new HashSet<int>();
            Main.OnPreDraw += OnPreDraw;
        }

        private static void OnPreDraw(GameTime obj)
        {
            acceptingDyeRequests = true;
            if (dyeRequests.Count > 0)
            {
                int dyeID = dyeRequests[0].Item1;
                try
                {
                    RunDyeRequest(dyeID, dyeRequests[0].Item2);
                }
                catch
                {
                    if (!dyeErrors.ContainsKey(dyeID))
                        dyeErrors.Add(dyeID, 1);
                    else
                        dyeErrors[dyeID]++;

                    if (dyeErrors[dyeID] > 5)
                        DyeIgnore.Add(dyeID);

                    acceptingDyeRequests = false;
                }

                dyeRequests.RemoveAt(0);
            }
        }
        private static void RunDyeRequest(int dyeID, RequestInfo request)
        {
            if (dyeToColor.Length < dyeID)
            {
                Array.Resize(ref dyeToColor, dyeID + 1);
            }
            drawingDye = true;
            var g = Main.instance.GraphicsDevice;
            if (g == null || !Main.IsGraphicsDeviceAvailable)
            {
                return;
            }

            if (Target == null)
            {
                Target = new RenderTarget2D(g, SAMPLESIZE, SAMPLESIZE);
            }
            if (SwapTarget == null)
            {
                SwapTarget = new RenderTarget2D(g, SAMPLESIZE, SAMPLESIZE);
            }

            var a = GameShaders.Armor.GetSecondaryShader(dyeID, request.player == -1 ? Main.LocalPlayer : Main.player[request.player]);
            var targets = g.GetRenderTargets();

            dyeToColor[dyeID] = SampleDye(Main.spriteBatch, g, a, dyeID, new Color(Vector3.Normalize(request.Lighting)));

            g.SetRenderTargets(targets);

            drawingDye = false;
        }
        private static Vector3 SampleDye(SpriteBatch sb, GraphicsDevice g, ArmorShaderData a, int dyeID, Color lightingColor)
        {
            var sample = Textures.DyeSample;
            if (!sample.IsLoaded)
                return new Vector3(1f, 1f, 1f);

            g.SetRenderTarget(SwapTarget);
            g.Clear(Color.Black);
            sb.Begin(SpriteSortMode.Immediate, null);
            var d = new DrawData(sample.Value, new Rectangle(0, 0, SAMPLESIZE, SAMPLESIZE), lightingColor);

            d.Draw(sb);

            sb.End();
            g.SetRenderTarget(Target);
            g.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, null);
            a.Apply(null, d);
            sb.Draw(SwapTarget, new Rectangle(0, 0, SAMPLESIZE, SAMPLESIZE), Color.White);
            sb.End();

            var colors = new Color[Target.Width * Target.Height];
            Target.GetData(colors);
            var color = new Vector3(0f, 0f, 0f);
            int colorAmt = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                var c = colors[i];
                if (c.A < 10)
                    continue;
                colorAmt++;
                color += c.ToVector3();
            }

            var resultColor = color / colorAmt;

            resultColor.X = Math.Min(resultColor.X, 1f);
            resultColor.Y = Math.Min(resultColor.Y, 1f);
            resultColor.Z = Math.Min(resultColor.Z, 1f);
            return resultColor;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= OnPreDraw;
            dyeRequests?.Clear();
            dyeRequests = null;
            dyeErrors?.Clear();
            dyeErrors = null;
            DyeIgnore?.Clear();
            DyeIgnore = null;
            dyeToColor = null;
        }

        public static void RequestDye(int dye, Vector3 color, int player = -1)
        {
            if (!AcceptingDyeRequests || (DyeIgnore.Count > 0 && DyeIgnore.Contains(dye)))
                return;

            if (dyeRequests.FindIndex((pair) => pair.Item1 == dye) == -1)
                dyeRequests.Add((dye, new RequestInfo(player == -1 ? Main.myPlayer : player, color)));
        }

        public static Color GetColor(Color cColor, int dye, int plr = -1)
        {
            RequestDye(dye, cColor.ToVector3(), plr);
            if (dyeToColor.Length > dye && dyeToColor[dye] != null)
            {
                var hsl = Main.rgbToHsl(cColor);
                hsl.X = Main.rgbToHsl(new Color(dyeToColor[dye].Value)).X;
                return Main.hslToRgb(hsl);
            }
            return cColor;
        }

        public static Color GetColor(int dye, int plr = -1)
        {
            var color = Color.White;
            if (dyeToColor.Length > dye && dyeToColor[dye] != null)
            {
                color = new Color(dyeToColor[dye].Value);
            }
            return GetColor(color, dye, plr);
        }
    }
}
