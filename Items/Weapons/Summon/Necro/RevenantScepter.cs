using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class RevenantScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;

            OnOpenBag.LockboxPool.Add(Type);

            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(25);
            Item.shoot = ModContent.ProjectileType<RevenantBolt>();
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.DungeonValue;
            Item.mana = 15;
            Item.UseSound = SoundID.Item8;
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.Aequus().revenantScepterZombie > 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int zombie = Main.LocalPlayer.Aequus().revenantScepterZombie;
                if (zombie > 0)
                {
                    Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<RevenantEnemySpawner>(), 0, 0f, player.whoAmI, zombie);
                    Main.LocalPlayer.Aequus().revenantScepterZombie = 0;
                    return false;
                }
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int zombie = Main.LocalPlayer.Aequus().revenantScepterZombie;
            if (zombie > 0)
            {
                tooltips.Insert(ItemTooltips.GetLineIndex(tooltips, "BuffTime"),
                    new TooltipLine(Mod, "RevenantScepterTooltip", AequusText.GetText("Tooltips.RevenantScepterGhost", Lang.GetNPCNameValue(zombie))));
            }
        }
    }
}