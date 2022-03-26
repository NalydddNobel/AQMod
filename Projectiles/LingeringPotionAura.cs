using AQMod.Dusts;
using AQMod.Effects;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public sealed class LingeringPotionAura : ModProjectile
    {
        public override string Texture => AQMod.TextureNone;

        private Color potionColor;

        public override void SetDefaults()
        {
            projectile.width = 450;
            projectile.height = 450;
            projectile.friendly = true;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            var center = projectile.Center;
            float distance = projectile.Size.Length() / 2f;
            int buff = (int)projectile.ai[0];
            if (buff <= 0)
            {
                projectile.ai[0] = BuffID.Regeneration;
                buff = BuffID.Regeneration;
                potionColor = BuffColorGenerator.GetColorFromItemID(ItemID.RegenerationPotion);
                CheckColor();
            }
            if (projectile.timeLeft < 30)
            {
                projectile.localAI[0] -= 1f / 30f;
            }
            else if (projectile.localAI[0] < 1f)
            {
                projectile.localAI[0] += 1f / 30f;
                if (projectile.localAI[0] > 1f)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead &&
                    Main.player[i].team == Main.player[projectile.owner].team &&
                    Main.player[i].Distance(center) < distance)
                {
                    int index = Main.player[i].FindBuffIndex(buff);
                    if (index == -1)
                    {
                        Main.player[i].AddBuff(buff, 300);
                    }
                    else if (Main.player[i].buffTime[index] < (int)projectile.ai[1])
                    {
                        Main.player[i].buffTime[index] += 10;
                    }
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                int dustCount = (int)Math.Max(distance / 10f * projectile.localAI[0], 1f);
                var color = potionColor;
                if (Main.player[Main.myPlayer].HasBuff(buff))
                {
                    color *= 0.9f;
                    dustCount = (int)(dustCount * 0.9f);
                }
                for (int i = 0; i < dustCount; i++)
                {
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, color, Main.rand.NextFloat(1f, 1.2f));
                    d.position = center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(distance * projectile.localAI[0]);
                }
            }
        }

        public void ApplyPotion(Item potion)
        {
            projectile.ai[0] = potion.buffType;
            projectile.ai[1] = potion.buffTime;
            potionColor = BuffColorGenerator.GetColorFromItemID(potion.type);
            CheckColor();
        }

        private void CheckColor()
        {
            potionColor.R = Math.Max(potionColor.R, (byte)10);
            potionColor.G = Math.Max(potionColor.G, (byte)10);
            potionColor.G = Math.Max(potionColor.G, (byte)10);
            potionColor.A = 50;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            CheckColor();
            writer.Write(potionColor.R);
            writer.Write(potionColor.G);
            writer.Write(potionColor.B);
            writer.Write(potionColor.A);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            potionColor.R = reader.ReadByte();
            potionColor.G = reader.ReadByte();
            potionColor.B = reader.ReadByte();
            potionColor.A = reader.ReadByte();
            CheckColor();
        }
    }
}