using Aequus.NPCs.OccultistNPC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class OccultistProj : ModProjectile
    {
        public override string Texture => $"{Aequus.VanillaTexture}Projectile_{ProjectileID.DemonScythe}";

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.npcProj = true;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item117.WithPitch(0.6f).WithVolumeScale(0.5f), Projectile.Center);
                for (int i = 0; i < 32; i++)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X, Projectile.velocity.Y, 100, Scale: Main.rand.NextFloat(0.7f, 1.5f));
                    d.noGravity = true;
                }
            }
            else if (Main.rand.NextBool())
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X, Projectile.velocity.Y, 100, Scale: Main.rand.NextFloat(0.6f, 1.1f));
                d.noGravity = true;
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = Projectile.velocity.Length() * 3f;
            }
            if (Projectile.alpha <= 50)
            {
                var target = Projectile.FindTargetWithinRange(700f);
                if (target != null)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * Math.Max(Projectile.velocity.Length(), 5f), 0.05f + Projectile.ai[0] * 0.01f);
                }
            }
            if (Projectile.ai[0] < 10f)
            {
                Projectile.ai[0] += 0.25f;
            }
            else
            {
                if (Projectile.velocity.Length() < Projectile.ai[1])
                {
                    Projectile.velocity *= 1.1f;
                }
                Projectile.ai[0]++;
            }
            Projectile.rotation += 0.7f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 32; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Projectile.velocity.X, Projectile.velocity.Y, 100, Scale: Main.rand.NextFloat(0.7f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = new Color(255, 222, 211, 200) * Projectile.Opacity;
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
            Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class OccultistProjSpawner : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public int NPCIndex { get => (int)Projectile.ai[0] - 1; set => Projectile.ai[0] = value + 1; }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.npcProj = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (AequusHelpers.HereditarySource(source, out var entity) && entity is NPC npc)
            {
                NPCIndex = npc.whoAmI;
            }
        }

        public override void AI()
        {
            if (NPCIndex == -1)
            {
                NPCIndex = NPC.FindFirstNPC(ModContent.NPCType<Occultist>());
                if (NPCIndex == -1)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else if (!Main.npc[NPCIndex].active || !Main.npc[NPCIndex].townNPC)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Main.npc[NPCIndex].Center;

            int timeBetweenShots = 6;
            if ((int)Projectile.ai[1] % timeBetweenShots == 0)
            {
                var v = Vector2.Normalize(Projectile.velocity).RotatedBy(Projectile.ai[1] / timeBetweenShots / 5f * MathHelper.TwoPi);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Main.npc[NPCIndex].GetSource_FromAI(), Projectile.Center + v * 48f, v * 5f, ModContent.ProjectileType<OccultistProj>(), Projectile.damage / 2, Projectile.knockBack / 2f, Projectile.owner);
                if (Projectile.ai[1] >= timeBetweenShots * 4f)
                    Projectile.Kill();
            }
            Projectile.ai[1]++;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}