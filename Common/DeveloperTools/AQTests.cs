using AQMod.Assets;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;

namespace AQMod.Common.DeveloperTools
{
    internal class AQTests
    {
        public static class TestFabrikMethod2D
        {
            public static Vector2 origin = new Vector2();
            public static Vector2 target = new Vector2();
            public static Vector2[] segments = new Vector2[3];

            public static float distanceBetweenSegments = 40f;

            public static void Apply()
            {
                if (Main.mouseRight)
                {
                    origin = new Vector2(Main.mouseX, Main.mouseY);
                }
                else if (Main.mouseLeft)
                {
                    target = new Vector2(Main.mouseX, Main.mouseY);
                    float distanceToTarget = (origin - target).Length();
                    if (distanceToTarget > segments.Length * distanceBetweenSegments)
                    {
                        target = origin + Vector2.Normalize(target - origin) * (segments.Length * distanceBetweenSegments);
                    }
                }

                int scroll = PlayerInput.ScrollWheelDelta / 120;
                if (scroll != 0)
                {
                    int count = segments.Length + scroll;
                    if (count < 2)
                    {
                        count = 2;
                    }
                    if (count > 10)
                    {
                        count = 10;
                    }
                    segments = new Vector2[count];
                    segments[0] = origin;
                    for (int i = 1; i < segments.Length; i++)
                    {
                        segments[i] = origin + new Vector2(0f, distanceBetweenSegments * i);
                    }
                }

                var texture = TextureCache.OmegaStariteOrb.GetValue();
                var textureOrigin = texture.Size() / 2f;
                Main.spriteBatch.Draw(texture, origin, null, Color.Black, 0f, textureOrigin, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, target, null, Color.Black, 0f, textureOrigin, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);

                segments = AQUtils.Fabrik2D(segments, distanceBetweenSegments, target);

                for (int i = 0; i < segments.Length; i++)
                {
                    Main.spriteBatch.Draw(texture, segments[i], null, AQUtils.MovingRainbow(i * 0.5f), 0f, textureOrigin, 1f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
                }
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public static class TestFabrikMethod3D
        {
            public static Vector3 origin = new Vector3();
            public static Vector3 target = new Vector3();
            public static Vector3[] segments = new Vector3[3];

            public static float distanceBetweenSegments = 40f;

            public static void Apply()
            {
                if (!Main.playerInventory)
                {
                    if (Main.mouseRight)
                    {
                        origin = new Vector3(Main.mouseX, Main.mouseY, origin.Z);
                        int scroll = PlayerInput.ScrollWheelDelta / 120;
                        origin.Z += scroll;
                        if (origin.Z > 60f)
                        {
                            origin.Z = 60f;
                        }
                        else if (origin.Z < -60f)
                        {
                            origin.Z = -60f;
                        }
                    }
                    else if (Main.mouseLeft)
                    {
                        target = new Vector3(Main.mouseX, Main.mouseY, target.Z);
                        float distanceToTarget = (origin - target).Length();
                        if (distanceToTarget > segments.Length * distanceBetweenSegments)
                        {
                            target = origin + Vector3.Normalize(target - origin) * (segments.Length * distanceBetweenSegments);
                        }
                        int scroll = PlayerInput.ScrollWheelDelta / 120;
                        target.Z += scroll;
                        if (target.Z > 60f)
                        {
                            target.Z = 60f;
                        }
                        else if (target.Z < -60f)
                        {
                            target.Z = -60f;
                        }
                    }
                    else
                    {
                        int scroll = PlayerInput.ScrollWheelDelta / 120;
                        if (scroll != 0)
                        {
                            int count = segments.Length + scroll;
                            if (count < 2)
                            {
                                count = 2;
                            }
                            if (count > 10)
                            {
                                count = 10;
                            }
                            segments = new Vector3[count];
                            segments[0] = origin;
                            for (int i = 1; i < segments.Length; i++)
                            {
                                segments[i] = origin + new Vector3(0f, distanceBetweenSegments * i, 0f);
                            }
                        }
                    }
                }

                var texture = TextureCache.OmegaStariteOrb.GetValue();
                var textureOrigin = texture.Size() / 2f;
                Main.spriteBatch.Draw(texture, Parralax.GetParralaxPosition(new Vector2(origin.X, origin.Y) + Main.screenPosition, origin.Z * 0.01f) - Main.screenPosition, null, Color.Black, 0f, textureOrigin, Parralax.GetParralaxScale(1f, origin.Z * 0.1f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, Parralax.GetParralaxPosition(new Vector2(target.X, target.Y) + Main.screenPosition, target.Z * 0.01f) - Main.screenPosition, null, Color.Black, 0f, textureOrigin, Parralax.GetParralaxScale(1f, target.Z * 0.1f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);

                segments = AQUtils.Fabrik3D(segments, distanceBetweenSegments, target);

                var positions = new List<Vector3>(segments);
                positions.Sort((v, v2) => v.Z.CompareTo(v2.Z));
                for (int i = 0; i < segments.Length; i++)
                {
                    Main.spriteBatch.Draw(texture, Parralax.GetParralaxPosition(new Vector2(positions[i].X, positions[i].Y) + Main.screenPosition, positions[i].Z * 0.1f) - Main.screenPosition, null, AQUtils.MovingRainbow(7f - i * 0.5f), 0f, textureOrigin, Parralax.GetParralaxScale(1f, positions[i].Z * 0.1f), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
                }
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}