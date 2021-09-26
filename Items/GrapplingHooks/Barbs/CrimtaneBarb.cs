using AQMod.Common;
using AQMod.Content.HookBarbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.GrapplingHooks.Barbs
{
    public class CrimtaneBarb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 30;
            item.knockBack = 1.1f;
            item.crit = 4;
            item.accessory = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(gold: 1, silver: 15);
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

        private void BarbPostAI(Projectile Projectile, HookBarbsProjectile hookBarbs, Player owner, HookBarbPlayer barbPlayer)
        {
            var rect = Projectile.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && AQUtils.CanNPCBeHitByProjectile(Main.npc[i], Projectile) && rect.Intersects(Main.npc[i].getRect()))
                {
                    if (Main.npc[i].immune[Projectile.owner] <= 0)
                    {
                        Main.npc[i].immune[Projectile.owner] = 5;
                        owner.ApplyDamageToNPC(Main.npc[i], Main.DamageVar(owner.GetWeaponDamage(item)), owner.GetWeaponKnockback(item, item.knockBack), Projectile.velocity.X < 0f ? -1 : 1, AQPlayer.PlayerCrit(item.crit, Main.rand));
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.CrimtaneBar, 8);
            r.AddIngredient(ItemID.Obsidian, 20);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}