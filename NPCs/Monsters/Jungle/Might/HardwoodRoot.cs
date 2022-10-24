using Aequus.Biomes;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Jungle.Might
{
    public class HardwoodRoot : ModNPC
    {
        public List<Vector2> RootPoints;

        public int AttatchedNPC { get => (int)NPC.ai[3]; set => NPC.ai[3] = value; }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Hide = true, };
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 72;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.damage = 50;
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.defense = 50;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.behindTiles = true;
            NPC.scale = 1.2f;

            this.SetBiome<OrganicEnergyBiome>();
            RootPoints = new List<Vector2>();
        }

        public override void AI()
        {
            NPC.frame.Y = NPC.frame.Height;
            NPC.TargetClosest();
            if (RootPoints.Count == 0)
                RootPoints.Add(NPC.Center);
            if (RootPoints.Count > 25 || (int)NPC.ai[1] == 1)
            {
                if ((int)NPC.ai[1] != 1)
                {
                    NPC.velocity = -NPC.velocity;
                }
                NPC.ai[1] = 1f;
                
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Normalize(RootPoints[^1] - NPC.Center) * 4f, 0.1f);
                if (NPC.Distance(RootPoints[^1]) < 14f)
                    RootPoints.RemoveAt(RootPoints.Count - 1);
                if (RootPoints.Count == 1)
                {
                    NPC.ai[1] = 0f;
                    NPC.Center = RootPoints[0];
                    NPC.velocity *= -0.1f;
                    NPC.netUpdate = true;
                }
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                var gotoLocation = NPC.HasValidTarget ? Main.player[NPC.target].Center :
                    Main.npc[AttatchedNPC].Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(200f, 400f);
                float progress = RootPoints.Count / 25f;
                NPC.velocity = Vector2.Normalize(Vector2.Lerp(NPC.velocity, Vector2.Normalize(gotoLocation - NPC.Center), 0.001f + progress * 0.02f)).UnNaN() * (0.5f + progress * 0.5f);
                var lastPoint = RootPoints[^1];
                if (Vector2.Distance(lastPoint, NPC.Center) > 38f * NPC.scale)
                {
                    RootPoints.Add(NPC.Center);
                }
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
            }
            if (!Main.npc[AttatchedNPC].active)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            NPC.position += Main.npc[AttatchedNPC].velocity;
            for (int i = 0; i < RootPoints.Count; i++)
            {
                RootPoints[i] += Main.npc[AttatchedNPC].velocity;
                AequusTile.Circles.Add(new AequusTile.IndestructibleCircle() { CenterPoint = RootPoints[i].ToTileCoordinates(), tileRadius = 10f, });
            }
            AequusTile.Circles.Add(new AequusTile.IndestructibleCircle() { CenterPoint = NPC.Center.ToTileCoordinates(), tileRadius = 20f, });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var rootFrame = NPC.frame;
            rootFrame.Y = 0;
            rootFrame.Height /= 2;
            for (int i = RootPoints.Count - 1; i >= 0; i--)
            {
                float rotation = i < (RootPoints.Count - 1) ? (RootPoints[i + 1] - RootPoints[i]).ToRotation() : (NPC.Center - RootPoints[i]).ToRotation();
                Main.spriteBatch.Draw(TextureAssets.Npc[Type].Value, RootPoints[i] - Main.screenPosition, rootFrame, AequusHelpers.GetLightingSection(RootPoints[i].ToTileCoordinates(), 4),
                    rotation - MathHelper.PiOver2, rootFrame.Size() / 2f, NPC.scale * 0.9f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - Main.screenPosition, NPC.frame, AequusHelpers.GetLightingSection(NPC.Center.ToTileCoordinates(), 4), 
                NPC.rotation, new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 4f), NPC.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
