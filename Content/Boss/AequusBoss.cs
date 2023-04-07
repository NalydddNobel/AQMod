using Aequus.Common.Effects;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss
{
    public abstract class AequusBoss : ModNPC
    {
        public const int ACTION_GOODBYE = -1;
        public const int ACTION_INIT = 0;
        public const int ACTION_INTRO = 1;

        public int horizontalFrames = 1;

        public int Action { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public float ActionTimer { get => NPC.ai[1]; set => NPC.ai[1] = value; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        internal void SetFrame(int y, int x = 0)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            var t = TextureAssets.Npc[Type].Value;
            int w = t.Width / horizontalFrames;
            int h = t.Height / Main.npcFrameCount[Type];
            NPC.frame = new Rectangle(w * x, h * y, w - 2, h - 2);
        }

        internal int NewProjectile(int type, Vector2? where, Vector2 velocity, int damage, float ai0 = 0f, float ai1 = 0f) {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return Main.maxProjectiles;
            }

            return Projectile.NewProjectile(NPC.GetSource_FromAI(), where ?? NPC.Center, velocity, type, damage, 0f, Main.myPlayer, ai0, ai1);
        }
        internal int NewProjectile<T>(Vector2? where, Vector2 velocity, int damage, float ai0 = 0f, float ai1 = 0f) where T : ModProjectile {
            return NewProjectile(ModContent.ProjectileType<T>(), where, velocity, damage, ai0, ai1);
        }

        internal bool CanSee(Entity ent) {
            return Collision.CanHitLine(NPC.position, NPC.width, NPC.height, ent.position, ent.width, ent.height);
        }
        internal bool CanSeeTarget() {
            return CanSee(Main.player[NPC.target]);
        }

        internal int Mode(int normal, int expert, int ftw)
        {
            return Main.getGoodWorld ? ftw : Main.expertMode ? expert : normal;
        }
        internal float Mode(float normal, float expert, float ftw)
        {
            return Main.getGoodWorld ? ftw : Main.expertMode ? expert : normal;
        }

        internal void ClearAI()
        {
            NPC.ai[0] = 0f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
        }

        internal void ClearLocalAI()
        {
            NPC.localAI[0] = 0f;
            NPC.localAI[1] = 0f;
            NPC.localAI[2] = 0f;
            NPC.localAI[3] = 0f;
        }

        internal void SetCamera(string context = null, Vector2? position = null, CameraPriority priority = CameraPriority.BossDefeat, float speed = 12f, int hold = 60)
        {
            ModContent.GetInstance<CameraFocus>().SetTarget(context ?? Name, position ?? NPC.Center, priority, speed, hold);
        }
        internal void Flash(Vector2? position, float amt, float multiplier = 0.9f)
        {
            ScreenFlash.Flash.Set(position ?? NPC.Center, amt, multiplier);
        }
        internal void Shake(float amt, float multiplier = 0.9f)
        {
            ScreenShake.SetShake(amt, multiplier);
        }
    }
}