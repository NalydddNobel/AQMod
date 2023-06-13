using Aequus.Buffs.Debuffs;
using Aequus.Content.Events.DemonSiege;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Heavy {
    public class Cauterizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DemonSiegeSystem.RegisterSacrifice(new(ItemID.BloodButcherer, Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<CauterizerProj>(38);
            Item.SetWeaponValues(49, 4.5f, 6);
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.autoReuse = true;
            Item.value = ItemDefaults.ValueDemonSiege;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(200);
        }

        public override bool? UseItem(Player player)
        {
            Item.FixSwing(player);
            return null;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuffs(240, 1, CrimsonHellfire.Debuffs);
        }
    }
}