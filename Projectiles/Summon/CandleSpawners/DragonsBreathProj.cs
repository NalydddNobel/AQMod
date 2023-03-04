using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Summon.CandleSpawners
{
    public class DragonsBreathProj : BaseGhostSpawner
    {
        public override int NPCType()
        {
            return NPCID.CultistDragonHead;
        }

        public override int AuraColor => ColorTargetID.DungeonDarkBlue;

        public override void AI()
        {
            Projectile.rotation += 0.045f + (1f - Projectile.Opacity) * 0.045f;
            base.AI();
        }

        protected override void OnSpawnZombie(NPC npc, NecromancyNPC zombie)
        {
            base.OnSpawnZombie(npc, zombie);
            if (npc.whoAmI == 0)
            {
                npc.active = false;
                PacketSystem.SyncNPC(npc);
                return;
            }
            npc.ai[3] = npc.whoAmI;
            npc.realLife = npc.whoAmI;
            int lastSegment = npc.whoAmI;
            for (int i = 0; i < 7; i++)
            {
                int npcID = NPCID.CultistDragonHead + i + 1;
                if (i > 1)
                {
                    npcID -= 2;
                }
                int currentSegment = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + npc.width / 2), (int)(npc.position.Y + npc.height), npcID, npc.whoAmI);
                Main.npc[currentSegment].ai[3] = npc.whoAmI;
                Main.npc[currentSegment].realLife = npc.whoAmI;
                Main.npc[currentSegment].ai[1] = lastSegment;
                Main.npc[currentSegment].CopyInteractions(npc);
                Main.npc[lastSegment].ai[0] = currentSegment;
                Main.npc[currentSegment].GetGlobalNPC<NecromancyNPC>().slotsConsumed = 0;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, currentSegment);
                lastSegment = currentSegment;
            }
        }

        protected override void SpawnEffects()
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawCoords = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(AequusTextures.Bloom3, drawCoords, null, Color.Black * Projectile.Opacity,
                0f, AequusTextures.Bloom3.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(AequusTextures.Bloom0, drawCoords, null, Color.Black * Projectile.Opacity,
                0f, AequusTextures.Bloom0.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            QuickDrawAura(drawCoords, Color.Cyan.UseA(0), Color.Blue * 0.6f);
            Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.CultistRitual].Value, Projectile.Center - Main.screenPosition, null, Color.White.UseA(0) * Projectile.Opacity,
                -Projectile.rotation, TextureAssets.Extra[ExtrasID.CultistRitual].Value.Size() / 2f, Projectile.scale * Projectile.Opacity * 0.33f, SpriteEffects.None, 0);
            return false;
        }
    }
}