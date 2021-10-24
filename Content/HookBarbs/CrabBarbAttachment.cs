using Terraria;
using Terraria.ID;

namespace AQMod.Content.HookBarbs
{
    public class CrabBarbAttachment : DamageBarbAttachmentType
    {
        public CrabBarbAttachment(Item item) : base(item)
        {
        }

        protected override void OnHit(NPC npc, int damage, float knockback, bool crit, int hitDirection, Projectile projectile, HookBarbsProjectile barbProj, Player player, HookBarbPlayer barbPlayer)
        {
            npc.AddBuff(BuffID.Poisoned, 120);
        }
    }
}
