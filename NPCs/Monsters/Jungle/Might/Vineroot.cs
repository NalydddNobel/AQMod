using Aequus.NPCs.AIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Aequus.NPCs.Monsters.Jungle.Might
{
    public class Vineroot : LegacyAIManEater
    {
        public const int FramesX = 3;

        private bool _setupFrame;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.damage = 50;
            NPC.aiStyle = -1;
            NPC.lifeMax = 2000;
            NPC.knockBackResist = 0.1f;
            movementSpeed = 0.05f;
            range = 300f;
        }

        public override void AI()
        {
            NPC.CollideWithOthers();
            if ((int)NPC.ai[3] == 0)
            {
                NPC.ai[3] = 1f;
                FindVaildSpot();
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Type, NPC.whoAmI, ai0: NPC.ai[0], ai1: NPC.ai[1], ai3: 2f);
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, Type, NPC.whoAmI, ai0: NPC.ai[0], ai1: NPC.ai[1], ai3: 3f);
            }
            base.AI();
            NPC.rotation += MathHelper.PiOver2 * NPC.spriteDirection;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame)
            {
                NPC.frame.Width /= FramesX;
                _setupFrame = true;
            }
            NPC.frame.X = NPC.frame.Width * (int)(NPC.ai[3] - 1f);
        }

        public override bool PreDrawChain(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, ref Texture2D chainTexture, out Rectangle frame, ref Color _drawColor)
        {
            chainTexture = TextureAssets.Npc[Type].Value;
            frame = new Rectangle(NPC.frame.X + 10, NPC.frame.Y + 2, 18, 28);
            return true;
        }
    }
}
