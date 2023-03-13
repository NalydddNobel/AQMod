using Aequus.Items.Weapons.Summon;
using Aequus.Projectiles.Summon.Necro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Unused.DebugItems
{
    public class EnthrallingScepter : ScepterBase
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 0;
            Item.staff[Type] = true;

            Tooltip.SetDefault(
                """
                Can turn any enemy into a ghost minion
                Testing Item
                """);
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(2, 1f, 96);
            Item.shoot = ModContent.ProjectileType<EnthrallingBolt>();
            Item.shootSpeed = 20f;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}