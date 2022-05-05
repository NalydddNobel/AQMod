using Aequus.Common.Configuration;
using Aequus.Graphics.Prims;
using Aequus.NPCs.Monsters.Sky;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class SpaceSquidDeathray : ModProjectile
    {
        private PrimRenderer prim;
        private PrimRenderer smokePrim;

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

            //Projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(-40);
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
                Projectile.Center = Main.npc[(int)(Projectile.ai[0] - 1)].ModNPC<SpaceSquid>().GetEyePos() + new Vector2(Projectile.direction * 10f, 0f);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.direction);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.direction = reader.ReadInt32();
        }

        public const int LaserLength = 2000;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.direction == -1)
            {
                projHitbox.X -= LaserLength + projHitbox.Width;
                projHitbox.Width = LaserLength;
                if (targetHitbox.Intersects(projHitbox))
                {
                    return true;
                }
            }
            else
            {
                projHitbox.Width += LaserLength;
                if (targetHitbox.Intersects(projHitbox))
                {
                    return true;
                }
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var orig = texture.Size() / 2f;
            var drawPos = Projectile.Center - Main.screenPosition;
            var drawColor = new Color(10, 200, 80, 0);
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var arr = new Vector2[] {
                    Projectile.Center - Main.screenPosition,
                    Projectile.Center + new Vector2(Main.screenWidth * Projectile.direction, 0f) - Main.screenPosition,
                    Projectile.Center + new Vector2(Main.screenWidth * 2f * Projectile.direction, 0f) - Main.screenPosition, };
            if (prim == null)
            {
                prim = new PrimRenderer(Images.Trail[1].Value, PrimRenderer.DefaultPass, (p) => new Vector2(Projectile.height * (1f - p) * (1f - p)), (p) => drawColor * (1f - p), obeyReversedGravity: false, worldTrail: false);
            }
            if (smokePrim == null)
            {
                smokePrim = new PrimRenderer(Images.Trail[3].Value, PrimRenderer.DefaultPass, (p) => new Vector2(Projectile.height * (1f - p) * (1f - p)), (p) => drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f) * (1f - p), obeyReversedGravity: false, worldTrail: false);
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(arr);
            }
            var smokeLineColor = drawColor * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 12f) + 2f);
            int amount = (int)(30 * (ClientConfig.Instance.HighQuality ? 1f : 0.5f));
            var initialArr = new Vector2[amount];
            var center = Projectile.Center;
            initialArr[0] = center - Main.screenPosition;
            for (int i = 1; i < amount; i++)
            {
                initialArr[i] = center + new Vector2(200f / amount * i * Projectile.direction, 0f) - Main.screenPosition;
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(initialArr);
            }

            initialArr[0] = center - Main.screenPosition;
            for (int i = 1; i < amount; i++)
            {
                initialArr[i] = center + new Vector2(20f / amount * i * -Projectile.direction, 0f) - Main.screenPosition;
            }
            if (Main.LocalPlayer.gravDir == -1)
            {
                AequusHelpers.ScreenFlip(initialArr);
            }
            // funny prim shenanigans
            prim.Draw(initialArr);
            smokePrim.Draw(initialArr, Main.GlobalTimeWrappedHourly * 20f, 20f);
            prim.Draw(arr);
            smokePrim.Draw(arr, Main.GlobalTimeWrappedHourly * 0.5f, 4f);

            var spotlight = Images.Bloom[2].Value;
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor * 0.4f, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * (Projectile.height / 32f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(spotlight, drawPos, null, drawColor, Projectile.rotation, spotlight.Size() / 2f, Projectile.scale * 0.5f * (Projectile.height / 32f), SpriteEffects.None, 0f);
            return false;
        }
    }
}