using Terraria;

namespace AQMod.Content.HookBarbs
{
    public abstract class BarbAttachment
    {
        protected readonly Item _itemCache;
        public BarbAttachment(Item item)
        {
            _itemCache = item;
        }

        public virtual bool BarbPreAI(Projectile projectile, HookBarbsProjectile barbProj, Player player, HookBarbPlayer barbPlayer)
        {
            return true;
        }
        public virtual void BarbAI(Projectile projectile, HookBarbsProjectile barbProj, Player player, HookBarbPlayer barbPlayer)
        {
        }
        public virtual void BarbPostAI(Projectile projectile, HookBarbsProjectile barbProj, Player player, HookBarbPlayer barbPlayer)
        {
        }
    }
}