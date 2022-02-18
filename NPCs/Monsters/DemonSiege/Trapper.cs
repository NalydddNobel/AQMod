using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.DemonSiege
{
    public class Trapper : ModNPC, IDecideFallThroughPlatforms
    {
        public const int FRAME_IDLE0 = 0;
        public const int FRAME_IDLE1 = 1;
        public const int FRAME_IDLE2 = 2;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 40;
            npc.aiStyle = -1;
            npc.damage = 50;
            npc.defense = 20;
            npc.lifeMax = 125;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.lavaImmune = true;
            npc.trapImmune = true;
            npc.value = 100f;
            npc.noGravity = true;
            npc.knockBackResist = 0.4f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.buffImmune[BuffID.Ichor] = false;
            npc.SetLiquidSpeed(lava: 1f);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.65f);
            if (AQMod.calamityMod.IsActive)
            {
                npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                npc.damage = (int)(npc.damage * 1.5f);
                npc.defense *= 2;
            }
        }

        public override void AI()
        {
            if ((int)npc.ai[0] == -1)
            {
                npc.velocity.X *= 0.98f;
                npc.velocity.Y -= 0.025f;
                return;
            }
            int npcOwner = (int)npc.ai[1] - 1;
            if (npcOwner == -1 || !Main.npc[npcOwner].active)
            {
                npc.life = -1;
                npc.HitEffect();
                npc.active = false;
                return;
            }
            npc.TargetClosest(faceTarget: false);
            if (npc.HasValidTarget)
            {
                int count = 0;
                int index = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == npc.whoAmI)
                    {
                        count++;
                        index = count;
                    }
                    else if (Main.npc[i].active && Main.npc[i].type == npc.type && (int)Main.npc[i].ai[1] == (int)npc.ai[1])
                    {
                        count++;
                    }
                }
                var center = npc.Center;
                float rotation = MathHelper.TwoPi / count;
                rotation *= index;
                npc.rotation = rotation;
                var gotoPosition = Main.npc[npcOwner].Center + new Vector2(0f, npc.height * -2.5f).RotatedBy(rotation);
                var difference = gotoPosition - npc.Center;
                npc.velocity = Vector2.Lerp(npc.velocity, Vector2.Normalize(difference) * 8.5f, 0.03f);
                if (npc.localAI[0] > 0f)
                    npc.localAI[0]--;
                if (Main.npc[npcOwner].ai[1] > 100f && Main.npc[npcOwner].ai[1] > 160f)
                {
                    npc.noTileCollide = true;
                }
                else if (npc.noTileCollide && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.noTileCollide = false;
                }
                if (Main.npc[npcOwner].ai[1] > 280f)
                {
                    if ((int)npc.ai[0] == 0 && Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.ai[0] = 1f;
                        npc.localAI[0] = 30f;
                        Main.PlaySound(SoundID.NPCDeath13, npc.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projectileSpeed = 10f;
                            int projectileType = ModContent.ProjectileType<Projectiles.Monster.Trapper>();
                            int damage = 20;
                            if (Main.expertMode)
                                damage = 15;
                            var normal = Vector2.Normalize(Main.player[npc.target].Center - center);
                            npc.velocity -= normal * (projectileSpeed / 3f);
                            Projectile.NewProjectile(center, normal * projectileSpeed, projectileType, damage, 1f, Main.myPlayer, 45f);
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
            }
            else
            {
                npc.noTileCollide = true;
                npc.ai[0] = -1f;
            }
            npc.rotation += npc.velocity.X * 0.0314f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.localAI[0] > 0f)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = 0;
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter > 6)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                        npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var drawPosition = new Vector2(npc.position.X + npc.width / 2f, npc.position.Y + npc.height / 2f);
            var screenPos = Main.screenPosition;

            var texture = Main.npcTexture[npc.type];
            var orig = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 2f);

            Main.spriteBatch.Draw(texture, drawPosition - screenPos, npc.frame, drawColor, npc.rotation, orig, npc.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int count = 1;
            if (npc.life <= 0)
                count = 20;
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
            }
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            return true;
        }
    }
}