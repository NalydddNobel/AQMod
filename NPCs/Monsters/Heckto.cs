using Aequus.Items.Misc;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters
{
    public class Heckto : ModNPC
    {
        public static HashSet<int> SpawnableIDs { get; private set; }
        public static SoundStyle ZombieMoanSound => new SoundStyle(SoundID.ZombieMoan.SoundPath, 53, 2);

        public override void Load()
        {
            SpawnableIDs = new HashSet<int>()
            {
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.DiabolistRed,
                NPCID.DiabolistWhite,
            };
        }

        public override void Unload()
        {
            SpawnableIDs?.Clear();
            SpawnableIDs = null;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData() { ImmuneToAllBuffsThatAreNotWhips = true, });
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 70;
            NPC.defense = 30;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit36;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.value = 510f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.Dungeon);
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Vector2 center = NPC.Center;
            var target = Main.player[NPC.target];
            Vector2 trueDifference = target.Center - center;
            Vector2 difference = trueDifference;
            float speedMult = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
            var tile = Framing.GetTileSafely(NPC.Center.ToTileCoordinates());
            float maxSpeed = 18f;
            bool inTiles = tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
            if (inTiles)
                maxSpeed = 5f;
            maxSpeed += 10 * (1f - NPC.life / (float)NPC.lifeMax);
            speedMult = maxSpeed / speedMult;
            difference *= speedMult;
            if (!inTiles && trueDifference.X.Abs() > 100f && trueDifference.Y.Abs() < 120f)
            {
                NPC.ai[0] = 1f;
                NPC.direction = difference.X < 0f ? -1 : 1;
                NPC.velocity.X += NPC.direction * 0.5f;
                if (NPC.velocity.X > maxSpeed)
                {
                    NPC.velocity.X = maxSpeed;
                }
                else if (NPC.velocity.X < -maxSpeed)
                {
                    NPC.velocity.X = -maxSpeed;
                }
                NPC.directionY = difference.Y < 0 ? -1 : 1;
                NPC.velocity.Y += NPC.directionY * 0.1f;
                if ((NPC.velocity.Y < 0f && NPC.directionY == 1) || (NPC.velocity.Y > 0f && NPC.directionY == -1))
                    NPC.velocity.Y += NPC.directionY * 0.1f;
            }
            else
            {
                NPC.ai[0] = 0f;
                NPC.velocity.X = (NPC.velocity.X * 100f + difference.X) / 101f;
                NPC.velocity.Y = (NPC.velocity.Y * 100f + difference.Y) / 101f;
            }
            NPC.rotation = (float)Math.Atan2(difference.Y, difference.X) - 1.57f;
            int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(240, 90, 100, 0));
            Main.dust[d].velocity *= 0.1f;
            Main.dust[d].scale = 1.3f;
            Main.dust[d].noGravity = true;
            if (Main.netMode == NetmodeID.Server)
                return;
            if (Main.rand.NextBool(400))
            {
                SoundEngine.PlaySound(ZombieMoanSound, NPC.Center);
            }
        }

        public override void FindFrame(int frameHeight)
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

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<MonoDust>(), NPC.velocity.X, NPC.velocity.Y, 0, new Color(240, 90, 100, 0));
                    Main.dust[d].velocity *= 2f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 1.4f;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<Hexoplasm>(chance: 1, stack: (1, 2));
        }

        //public override void NPCLoot()
        //{
        //    if (Main.rand.NextBool(10))
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<RedLicorice>());
        //    if (Main.rand.NextBool(15))
        //        Item.NewItem(NPC.getRect(), ModContent.ItemType<Dreadsoul>());
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.position - screenPos;
            var orig = new Vector2(texture.Width / 2, texture.Height / Main.npcFrameCount[NPC.type] / 2);
            var offset = -(new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f) + orig * NPC.scale + new Vector2(0f, NPC.gfxOffY + 2f);
            offset += new Vector2(NPC.width / 2f, NPC.height / 2f);
            var spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if ((int)NPC.ai[0] == 1 && !NPC.IsABestiaryIconDummy)
            {
                int trailLength = NPCID.Sets.TrailCacheLength[NPC.type];
                for (int i = 0; i < trailLength; i++)
                {
                    if (NPC.oldPos[i] == Vector2.Zero)
                        break;
                    float progress = 1f - 1f / trailLength * i;
                    progress += (float)Math.Sin(-i * 0.314f + Main.GlobalTimeWrappedHourly * 20) * 0.1f;
                    Main.spriteBatch.Draw(texture, NPC.oldPos[i] + offset - screenPos, NPC.frame, new Color(88, 38, 20, 2) * progress, NPC.rotation, orig, NPC.scale * progress, spriteEffects, 0f);
                }
            }
            else
            {
                NPC.oldPos[0] = Vector2.Zero;
            }
            drawPos += offset;
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(200, 200, 200, 0), NPC.rotation, orig, NPC.scale, spriteEffects, 0f);
            Main.spriteBatch.Draw(texture, drawPos, NPC.frame, new Color(120, 40, 40, 0), NPC.rotation, orig, NPC.scale * 1.1f, spriteEffects, 0f);
            return false;
        }
    }
}