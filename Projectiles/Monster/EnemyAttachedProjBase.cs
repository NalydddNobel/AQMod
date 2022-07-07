using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public abstract class EnemyAttachedProjBase : ModProjectile
    {
        public int AttachedNPC { get => (int)Projectile.ai[0] - 1; set => Projectile.ai[0] = value + 1; }

        public override void AI()
        {
            UpdateAttachment();
        }

        public void UpdateAttachment()
        {
            if (CheckAttached())
            {
                Projectile.timeLeft = 2;
                Projectile.Center = Main.npc[AttachedNPC].Center;
                AIAttached(Main.npc[AttachedNPC]);
            }
        }

        public bool CheckAttached()
        {
            int npc = AttachedNPC;
            if (npc == -1)
                return false;

            return Main.npc[npc].active && CheckAttachmentConditions(Main.npc[npc]);
        }
        protected virtual bool CheckAttachmentConditions(NPC npc)
        {
            return true;
        }
        protected virtual void AIAttached(NPC npc)
        {

        }
    }
}