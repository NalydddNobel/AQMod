using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts
{
    public sealed class SilverFlameClone : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            int a = lightColor.A;
            lightColor = Color.Lerp(lightColor, Color.White, 0.8f);
            return new Color(lightColor.R, lightColor.G, lightColor.B, a) * MathHelper.Min(dust.scale, 1f);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.X *= 0.99f;
            dust.rotation += dust.velocity.X * 0.5f;
            if (!dust.noGravity)
            {
                dust.velocity.Y += 0.05f;
            }
            if (dust.customData != null)
            {
                if (dust.customData is NPC)
                {
                    var npc = (NPC)dust.customData;
                    dust.position += npc.position - npc.oldPos[1];
                }
                else if (dust.customData is Player)
                {
                    var player = (Player)dust.customData;
                    dust.position += player.position - player.oldPosition;
                }
                else if (dust.customData is Vector2)
                {
                    var vector = (Vector2)dust.customData - dust.position;
                    if (vector != Vector2.Zero)
                    {
                        vector.Normalize();
                    }
                    dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
                }
            }
            if (dust.fadeIn > 0f && dust.fadeIn < 100f)
            {
                if (dust.type == 235)
                {
                    dust.scale += 0.007f;
                    int num101 = (int)dust.fadeIn - 1;
                    if (num101 >= 0 && num101 <= 255)
                    {
                        Vector2 vector6 = dust.position - Main.player[num101].Center;
                        float num102 = vector6.Length();
                        num102 = 100f - num102;
                        if (num102 > 0f)
                        {
                            dust.scale -= num102 * 0.0015f;
                        }
                        vector6.Normalize();
                        float num103 = (1f - dust.scale) * 20f;
                        vector6 *= 0f - num103;
                        dust.velocity = (dust.velocity * 4f + vector6) / 5f;
                    }
                }
                else
                {
                    dust.scale += 0.03f;
                }
                if (dust.scale > dust.fadeIn)
                {
                    dust.fadeIn = 0f;
                }
            }
            dust.velocity *= 0.92f;
            if (dust.fadeIn == 0f)
            {
                dust.scale -= 0.04f;
            }
            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }
            else
            {
                dust.position += dust.velocity;
            }
            return false;
        }
    }
}