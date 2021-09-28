using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Content.HookBarbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.GrapplingHooks.Barbs
{
    public class CrabBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 15;
            item.knockBack = 0f;
            item.crit = 4;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 40);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var barbPlayer = player.GetModPlayer<HookBarbPlayer>();
            if (barbPlayer.BarbCount == 0)
            {
                barbPlayer.BarbCount++;
                barbPlayer.BarbPostAI += BarbPostAI;
            }
        }

        public void BarbPostAI(Projectile Projectile, HookBarbsProjectile hookBarbs, Player owner, HookBarbPlayer barbPlayer)
        {
            var rect = Projectile.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && rect.Intersects(Main.npc[i].getRect()) && AQUtils.CanNPCBeHitByProjectile(Main.npc[i], Projectile))
                {
                    if (Main.npc[i].immune[Projectile.owner] <= 0)
                    {
                        Main.npc[i].immune[Projectile.owner] = 3;
                        owner.ApplyDamageToNPC(Main.npc[i], Main.DamageVar(owner.GetWeaponDamage(item)), item.knockBack, Projectile.velocity.X < 0f ? -1 : 1, AQPlayer.PlayerCrit(item.crit, Main.rand));
                        Main.npc[i].AddBuff(BuffID.Poisoned, 120);
                    }
                }
            }
        }
    }
}