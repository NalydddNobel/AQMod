using Aequus.Buffs;
using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Underworld
{
    public class Trapper : ModNPC
    {
        public Asset<Texture2D> TrapperChainTexture => ModContent.Request<Texture2D>(Texture + "_Chain", AssetRequestMode.ImmediateLoad);

        public Vector2 chainsOrigin;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = AequusBuff.DemonSiegeEnemyImmunity.ToArray(),
            });

            this.HideBestiaryEntry();
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.defense = 6;
            NPC.lifeMax = 65;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.lavaImmune = true;
            NPC.trapImmune = true;
            NPC.value = 100f;
            NPC.noGravity = true;
            NPC.knockBackResist = 0.4f;
            NPC.npcSlots = 0.6f;

            NPC.SetLiquidSpeeds(lava: 1f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            int count = 1;
            if (NPC.life <= 0)
            {
                count = 14;

                for (int i = -1; i <= 1; i++)
                {
                    if (i != 0)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            NPC.DeathGore("Trapper_0", new Vector2(10f * i, 4f * j - 10f));
                        }
                        NPC.DeathGore("Trapper_1", new Vector2(16f * i, -10f));
                        NPC.DeathGore("Trapper_2", new Vector2(16f * i, 8f));
                    }
                }

                var trapperChain = TrapperChainTexture;
                if (chainsOrigin != Vector2.Zero && trapperChain.IsLoaded)
                {
                    var difference = chainsOrigin - NPC.Center;

                    int length = (int)(difference.Length() / trapperChain.Value.Height);
                    var chainSegment = difference / length;
                    for (int i = length; i > 0; i--)
                    {
                        Gore.NewGoreDirect(NPC.GetSource_Death(),
                            NPC.Center + chainSegment * i, Main.rand.NextVector2Unit(), GoreHelper.GoreType("Trapper_" + (Main.rand.Next(2) + 3)));
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch);
                d.velocity = (d.position - NPC.Center) / 8f;
                if (Main.rand.NextBool(3))
                {
                    d.velocity *= 2f;
                    d.scale *= 1.75f;
                    d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 0.75f);
                    d.noGravity = true;
                }
            }
            for (int i = 0; i < count * 2; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, newColor: Color.DarkRed);
            }
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == -1)
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y -= 0.025f;
                return;
            }
            int npcOwner = (int)NPC.ai[1] - 1;
            if (npcOwner == -1 || !Main.npc[npcOwner].active)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            chainsOrigin = Main.npc[npcOwner].Center;
            NPC.TargetClosest(faceTarget: false);
            if (NPC.HasValidTarget)
            {
                int count = 0;
                int index = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == NPC.whoAmI)
                    {
                        count++;
                        index = count;
                    }
                    else if (Main.npc[i].active && Main.npc[i].type == NPC.type && (int)Main.npc[i].ai[1] == (int)NPC.ai[1])
                    {
                        count++;
                    }
                }
                var center = NPC.Center;
                float rotation = MathHelper.TwoPi / count;
                rotation *= index;
                NPC.rotation = rotation;
                var gotoPosition = Main.npc[npcOwner].Center + new Vector2(0f, NPC.height * -2.5f).RotatedBy(rotation);
                var difference = gotoPosition - NPC.Center;
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(difference) * 8.5f, 0.03f);
                if (NPC.localAI[0] > 0f)
                    NPC.localAI[0]--;
                if (Main.npc[npcOwner].ai[1] > 100f && Main.npc[npcOwner].ai[1] > 160f)
                {
                    NPC.noTileCollide = true;
                }
                else if (NPC.noTileCollide && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.noTileCollide = false;
                }
                if (Main.npc[npcOwner].ai[1] > 280f)
                {
                    if ((int)NPC.ai[0] == 0 && Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                    {
                        NPC.ai[0] = 1f;
                        NPC.localAI[0] = 30f;
                        SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projectileSpeed = 10f;
                            int projectileType = ModContent.ProjectileType<Projectiles.Monster.TrapperBullet>();
                            int damage = 20;
                            if (Main.expertMode)
                                damage = 15;
                            var normal = Vector2.Normalize(Main.player[NPC.target].Center - center);
                            NPC.velocity -= normal * (projectileSpeed / 3f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), center, normal * projectileSpeed, projectileType, damage, 1f, Main.myPlayer, 45f);
                        }
                    }
                }
                else
                {
                    NPC.ai[0] = 0f;
                }
            }
            else
            {
                NPC.noTileCollide = true;
                NPC.ai[0] = -1f;
            }
            NPC.rotation += NPC.velocity.X * 0.0314f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[0] > 0f)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = 0;
            }
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 6)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                        NPC.frame.Y = 0;
                }
            }
        }

        public override void DrawBehind(int index)
        {
            EffectsSystem.NPCsBehindAllNPCs.Add(NPC);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy && EffectsSystem.NPCsBehindAllNPCs.Render)
            {
                var chainTexture = TrapperChainTexture;
                if (!chainTexture.IsLoaded)
                {
                    return false;
                }
                int npcOwner = (int)NPC.ai[1] - 1;
                int height = chainTexture.Value.Height - 2;
                var npcCenter = NPC.Center;
                var trapImpCenter = Main.npc[npcOwner].Center;
                var velocity = npcCenter - trapImpCenter;
                int length = (int)(velocity.Length() / height);
                velocity.Normalize();
                velocity *= height;
                float rotation = velocity.ToRotation() + MathHelper.PiOver2;
                var origin = new Vector2(chainTexture.Value.Width / 2f, chainTexture.Value.Height / 2f);
                for (int j = 1; j < length; j++)
                {
                    var position = trapImpCenter + velocity * j;
                    var color = Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f));
                    if (j < 6)
                        color *= 1f / 6f * j;
                    spriteBatch.Draw(chainTexture.Value, position - screenPos, null, color, rotation, origin, 1f, SpriteEffects.None, 0f);
                }
                if (!LegacyDrawList.ForceRender)
                    return false;
            }

            var drawPosition = new Vector2(NPC.position.X + NPC.width / 2f, NPC.position.Y + NPC.height / 2f);

            var texture = TextureAssets.Npc[Type].Value;
            var orig = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);

            foreach (var v in AequusHelpers.CircularVector(4, NPC.rotation))
            {
                spriteBatch.Draw(texture, drawPosition - screenPos + v * 2f, NPC.frame, Color.Orange.UseA(0) * 0.7f, NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, drawPosition - screenPos, NPC.frame, drawColor, NPC.rotation, orig, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
    }
}