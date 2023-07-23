using Aequus.Content.ItemPrefixes.Necromancy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreBase : ModItem {
        private bool _procSoulAttack;

        public abstract Color GlowColor { get; }
        public abstract int DustSpawn { get; }

        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
#if !DEBUG
            Item.ResearchUnlockCount = 0;
#endif
        }

        public override void SetDefaults() {
            Item.DamageType = Aequus.NecromancyMagicClass;
            Item.UseSound = AequusSounds.normalproj;
            _procSoulAttack = false;
        }

        public override bool CanUseItem(Player player) {
            return player.ItemTimeIsZero;
        }

        public override bool AllowPrefix(int pre) {
            return PrefixLoader.GetPrefix(pre) is NecromancyPrefixBase;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame) {
            if (player.JustDroppedAnItem) {
                return;
            }

            if (player.itemAnimation > 0) {
                _procSoulAttack = true;
            }
        }

        public override void HoldItem(Player player) {
            if (_procSoulAttack && player.itemAnimation == 0) {
                if (Main.myPlayer == player.whoAmI) {
                    Projectile.NewProjectile(
                        player.GetSource_ItemUse(Item),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<SceptreProcProj>(),
                        player.GetWeaponDamage(Item),
                        player.GetWeaponKnockback(Item),
                        player.whoAmI
                    );
                }
                _procSoulAttack = false;
            }
        }
    }
}