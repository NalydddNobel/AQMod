using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.HookBarbs
{
    public class HookBarbPlayer : ModPlayer
    {
        private BarbAttachmentType attachment;

        public bool BarbPreAI(Projectile projectile, HookBarbsProjectile barbProj)
        {
            if (attachment != null)
                return attachment.BarbPreAI(projectile, barbProj, player, this);
            return true;
        }

        public void BarbAI(Projectile projectile, HookBarbsProjectile barbProj)
        {
            if (attachment != null)
                attachment.BarbAI(projectile, barbProj, player, this);
        }

        public void BarbPostAI(Projectile projectile, HookBarbsProjectile barbProj)
        {
            if (attachment != null)
                attachment.BarbPostAI(projectile, barbProj, player, this);
        }

        public bool AddBarb(BarbAttachmentType attachment)
        {
            if (this.attachment == null)
            {
                this.attachment = attachment;
                return true;
            }
            return false;
        }

        private void ResetBarbAttachments()
        {
            attachment = null;
        }

        public override void Initialize()
        {
            ResetBarbAttachments();
        }

        public override void ResetEffects()
        {
            ResetBarbAttachments();
        }
    }
}