using Aequus.Projectiles.Summon;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class ZombieScepter : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;

            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(30);
            Item.SetWeaponValues(1, 1f, 0);
            Item.shoot = ModContent.ProjectileType<NecromancerBolt>();
            Item.shootSpeed = 6f;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 20;
            Item.UseSound = SoundID.Item8;
        }

        public override bool AllowPrefix(int pre)
        {
            return !AequusItem.CritOnlyModifier.Contains(pre);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                tooltips.RemoveCritChanceModifier();
            }
            catch
            {
            }
        }
    }
}