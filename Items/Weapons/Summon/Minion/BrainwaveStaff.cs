using Aequus.Biomes.DemonSiege;
using Aequus.Buffs.Minion;
using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Minion
{
    public class BrainwaveStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ModContent.ItemType<MindfungusStaff>(), Type, UpgradeProgressionType.PreHardmode));
        }

        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<BrainCauliflowerMinion>();
            Item.buffType = ModContent.BuffType<BrainCauliflowerBuff>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback);
            return false;
        }
    }
}