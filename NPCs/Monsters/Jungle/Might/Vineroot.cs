using Aequus.Biomes;
using Aequus.NPCs.AIs;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Jungle.Might
{
    public class Vineroot : LegacyAIManEater
    {
        public const int FramesX = 3;

        private bool _setupFrame;

        public override void SetStaticDefaults()
        {
            CoreOfMight.SpawnData.Add(new BaseCore.EnemySpawn_Any4Sides(Type));
            Main.npcFrameCount[Type] = 5;
            base.SetStaticDefaults();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .QuickUnlock();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add(ItemID.SoulofMight, chance: 1, stack: (1, 2));
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
            NPC.defense = 20;
            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            movementSpeed = 0.05f;
            range = 300f;

            this.SetBiome<OrganicEnergyBiome>();
        }

        private void SpawnBrother(int ai3)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            var n = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Type, NPC.whoAmI, ai0: NPC.ai[0], ai1: NPC.ai[1], ai3: ai3);
            n.TryGetGlobalNPC<AequusNPC>(out var aequus);
            NPC.TryGetGlobalNPC<AequusNPC>(out var myAequus);
            aequus.jungleCoreInvasion = myAequus.jungleCoreInvasion;
            aequus.jungleCoreInvasionIndex = myAequus.jungleCoreInvasionIndex;
        }

        public override Vector2 DetermineTargetPosition()
        {
            var pos = base.DetermineTargetPosition();
            switch ((int)NPC.ai[3]) 
            {
                case 2:
                    return pos + new Vector2(200f, 0f);
                case 3:
                    return pos + new Vector2(-200f, 0f);
            }
            return pos;
        }

        public override void AI()
        {
            NPC.CollideWithOthers();
            if ((int)NPC.ai[3] == 0)
            {
                NPC.ai[3] = 1f;
                FindVaildSpot();
                SpawnBrother(2);
                SpawnBrother(3);
            }
            base.AI();
            NPC.rotation += MathHelper.PiOver2 * NPC.spriteDirection;
            if (NPC.active)
            {
                AequusTile.Circles.Add(new AequusTile.IndestructibleCircle() { CenterPoint = new Point((int)NPC.ai[0], (int)NPC.ai[1]), tileRadius = 5f, });
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame)
            {
                NPC.frame.Width /= FramesX;
                _setupFrame = true;
            }
            NPC.frame.X = NPC.frame.Width * (int)(NPC.ai[3] - 1f);
            NPC.frameCounter++;
            if (NPC.frameCounter > 24.0)
            {
                NPC.frameCounter = 0.0;
            }
            NPC.frame.Y = frameHeight * (((int)NPC.frameCounter / 6 + (int)NPC.ai[3]) % 4);
        }

        public override bool PreDrawChain(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, ref Texture2D chainTexture, out Rectangle frame, ref Color _drawColor)
        {
            chainTexture = TextureAssets.Npc[Type].Value;
            frame = new Rectangle(NPC.frame.X + 34, NPC.frame.Height * 4 + 2, 18, 28);
            return true;
        }

        public override bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
            spriteBatch.Draw(t, NPC.Center - screenPos, frame, NPC.IsABestiaryIconDummy ?Color.White : NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
