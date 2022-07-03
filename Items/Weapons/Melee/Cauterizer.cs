using Aequus.Biomes;
using Aequus.Buffs.Debuffs;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Melee
{
    public class Cauterizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DemonSiegeInvasion.RegisterSacrifice(DemonSiegeInvasion.PHM(ItemID.BloodButcherer, Type));
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<CauterizerProj>(40);
            Item.SetWeaponValues(75, 6.5f);
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1.35f;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.autoReuse = true;
            Item.value = ItemDefaults.DemonSiegeValue;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor.MaxRGBA(200);
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            CrimsonHellfire.AddStack(target, 240, 1);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = (int)(damage * 0.75f);
            position += Vector2.Normalize(velocity) * 32f;
        }
    }
}