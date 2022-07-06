using Aequus.Graphics;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Common
{
    public class Experiments
    {
        public static void ScreenTargetSuckyThing()
        {
            if (!Main.drawToScreen/* && Main.GameUpdateCount % 4 == 0*/)
            {
                var t = Main.screenTarget;
                var c = new Color[t.Width * t.Height];
                t.GetData(c);
                int w = 12;
                int h = 12;
                int w2 = 2;
                int h2 = 2;
                if (Main.mouseRight)
                {
                    w = 150;
                    h = 150;
                }
                if (Main.mouseLeft || Main.mouseMiddle)
                {
                    w /= 2;
                    h /= 2;
                    w2 *= 2;
                    h2 *= 2;
                }
                float circular = (float)Math.Sqrt((w * w2) * (w * w2) + (h * h2) * (h * h2)) / 2f;
                for (int i = -w; i < w; i++)
                {
                    for (int j = -h; j < h; j++)
                    {
                        int i2 = i * w2;
                        int j2 = j * h2;
                        if (Math.Sqrt(i2 * i2 + j2 * j2) > circular || Main.rand.NextFloat() < 0.95f)
                            continue;
                        int index = Main.mouseX + i2 + (Main.mouseY + j2) * t.Width;
                        if (index >= 0 && index < c.Length)
                        {
                            var p = new BloomParticle(Main.MouseWorld + new Vector2(i2, j2), Vector2.Zero, c[index].UseA(20), c[index].UseA(0) * 0.05f, (w2 + h2) / 8f, 0.5f, 0f);
                            p.frame = new Rectangle(0, 0, 10, 10);
                            if (Main.mouseLeft)
                                p.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 2f);
                            if (Main.mouseMiddle)
                                p.Velocity = -(p.Position - Main.MouseWorld) / 6f;
                            AequusEffects.AbovePlayers.Add(p);
                        }
                    }
                }
            }
        }
    }
}