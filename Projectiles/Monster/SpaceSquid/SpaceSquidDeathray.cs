using Aequus.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster.SpaceSquid
{
    public class SpaceSquidDeathray : ModProjectile
    {
        public const int DEATHRAY_LENGTH = 2000;

        public TrailRenderer prim;
        public TrailRenderer smokePrim;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.manualDirectionChange = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (Projectile.direction == 0)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Projectile.direction = -1;
                }
                Projectile.netUpdate = true;
            }
            if (Main.expertMode)
            {
                if ((int)Projectile.ai[1] == 0)
                {
                    Projectile.width = (int)(Projectile.width * 1.5f);
                    Projectile.height = (int)(Projectile.height * 1.5f);
                    Projectile.ai[1]++;
                    Projectile.netUpdate = true;
                }
            }
            if ((int)(Projectile.ai[0] - 1) > -1)
            {
                if (!Main.npc[(int)(Projectile.ai[0] - 1)].active)
                {
                    Projectile.Kill();
                }
                if (Main.npc[(int)(Projectile.ai[0] - 1)].ai[1] > 300f)
                {
                    Projectile.height -= 2;
                    if (Main.expertMode)
                    {
                        Projectile.height -= 1;
                        Projectile.netUpdate = true;
                    }
                    if (Projectile.height < 2 || Main.npc[(int)(Projectile.ai[0] - 1)].ai[1] > 328f)
                    {
                        Projectile.Kill();
                    }
                }
                Projectile.Center = Main.npc[(int)(Projectile.ai[0] - 1)].ModNPC<NPCs.Monsters.Sky.SpaceSquid>().GetEyePos() + new Vector2(Projectile.direction * 10f, 0f);
            }
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.direction == -1)
            {
                projHitbox.X -= DEATHRAY_LENGTH + projHitbox.Width;
                projHitbox.Width = DEATHRAY_LENGTH;
                if (targetHitbox.Intersects(projHitbox))
                {
                    return true;
                }
            }
            else
            {
                projHitbox.Width += DEATHRAY_LENGTH;
                if (targetHitbox.Intersects(projHitbox))
                {
                    return true;
                }
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawPos = Projectile.Center - Main.screenPosition + new Vector2(Projectile.direction * 40f, 0f);
            var drawColor = new Color(10, 200, 80, 0);
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var arr = new Vector2[] {
                    drawPos,
                    drawPos + new Vector2(Main.screenWidth * Projectile.direction, 0f),
                    drawPos + new Vector2(Main.screenWidth * 2f * Projectile.direction, 0f), };
            if (prim == null)
            {
                prim = new TrailRenderer(TextureCache.Trail[1].Value, TrailRenderer.DefaultPass, (p) => new Vector2(Projectile.height * (1f - p * p)), (p) => drawColor * (1f - p), obeyReversedGravity: false, worldTrail: false);
            }
            if (smokePrim == null)
            {
                smokePrim = new TrailRenderer(TextureCache.Trail[3].Value, TrailRenderer.DefaultPass, (p) => new Vector2(Projectile.height), (p) => drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f) * (1f - p), obeyReversedGravity: false, worldTrail: false);
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(arr);
            }
            var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f);
            int amount = (int)(5 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
            var initialArr = new Vector2[amount];
            var center = Projectile.Center;

            initialArr[0] = arr[0];
            for (int i = 1; i < amount; i++)
            {
                initialArr[i] = drawPos + new Vector2(60f / amount * i * -Projectile.direction, 0f);
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(initialArr);
            }
            // funny prim shenanigans
            prim.Draw(initialArr);
            smokePrim.Draw(initialArr, -Main.GlobalTimeWrappedHourly * 2f, 4f);
            prim.Draw(arr);
            smokePrim.Draw(arr, -Main.GlobalTimeWrappedHourly, 2f);

            var spotlight = TextureCache.Bloom[2].Value;
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.4f, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * (Projectile.height / 32f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.5f * (Projectile.height / 32f), SpriteEffects.None, 0f);
            return false;
        }
    }
}