using AQMod.Common.Utilities;
using Terraria;

namespace AQMod.Content.HookBarbs
{
    public class DamageBarbAttachmentType : BarbAttachmentType
    {
        public DamageBarbAttachmentType(Item item) : base(item)
        {
        }

        public override void BarbPostAI(Projectile projectile, HookBarbsProjectile barbProj, Player player, HookBarbPlayer barbPlayer)
        {
            var rect = projectile.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].townNPC && rect.Intersects(Main.npc[i].getRect()) && CommonUtils.CanNPCBeHitByProjectile(Main.npc[i], projectile))
                {
                    if (Main.npc[i].immune[projectile.owner] <= 0)
                    {
                        Main.npc[i].immune[projectile.owner] = 12;
                        int damage = Main.DamageVar(player.GetWeaponDamage(_itemCache));
                        float knockback = player.GetWeaponKnockback(_itemCache, _itemCache.knockBack);
                        bool crit = AQPlayer.PlayerCrit(_itemCache.crit, Main.rand);
                        int direction = projectile.velocity.X < 0f ? -1 : 1;
                        player.ApplyDamageToNPC(Main.npc[i], damage, knockback, direction, crit);
                        OnHit(Main.npc[i], damage, knockback, crit, direction, projectile, barbProj, player, barbPlayer);
                    }
                }
            }
        }

        protected virtual void OnHit(NPC npc, int damage, float knockback, bool crit, int hitDirection, Projectile projectile, HookBarbsProjectile barbProj, Player player, HookBarbPlayer barbPlayer)
        {
        }
    }
}