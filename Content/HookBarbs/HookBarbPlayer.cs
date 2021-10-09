using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.HookBarbs
{
    public class HookBarbPlayer : ModPlayer
    {
        //public byte BarbCount { get; set; }
        //private List<BarbAttachment> attachments;
        private BarbAttachment attachment;

        public bool BarbPreAI(Projectile projectile, HookBarbsProjectile barbProj)
        {
            //bool value = true;
            //foreach (var b in attachments)
            //{
            //    if (!b.BarbPreAI(projectile, barbProj, player, this))
            //    {
            //        value = false;
            //    }
            //}
            if (attachment != null)
                return attachment.BarbPreAI(projectile, barbProj, player, this);
            return true;
        }

        public void BarbAI(Projectile projectile, HookBarbsProjectile barbProj)
        {
            if (attachment != null)
                attachment.BarbAI(projectile, barbProj, player, this);
            //foreach (var b in attachments)
            //{
            //    b.BarbAI(projectile, barbProj, player, this);
            //}
        }

        public void BarbPostAI(Projectile projectile, HookBarbsProjectile barbProj)
        {
            if (attachment != null)
                attachment.BarbPostAI(projectile, barbProj, player, this);
            //foreach (var b in attachments)
            //{
            //    b.BarbPostAI(projectile, barbProj, player, this);
            //}
        }

        public bool AddBarb(BarbAttachment attachment)
        {
            if (this.attachment == null)
            {
                this.attachment = attachment;
                //attachments.Add(attachment);
                return true;
            }
            return false;
        }

        private void ResetBarbAttachments()
        {
            attachment = null;
            //try
            //{
            //    attachments.Clear();
            //}
            //catch
            //{
            //    attachments = new List<BarbAttachment>();
            //}
        }

        public override void Initialize()
        {
            //BarbCount = byte.MaxValue;
            //attachments = new List<BarbAttachment>();
            ResetBarbAttachments();
        }

        public override void ResetEffects()
        {
            //BarbCount = 0;
            ResetBarbAttachments();
        }
    }
}