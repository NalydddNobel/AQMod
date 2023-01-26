using Aequus.Items.GlobalItems;
using Aequus.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee
{
    public class IronLotus : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            TooltipsGlobal.Dedicated[Type] = new TooltipsGlobal.ItemDedication(new Color(110, 30, 60, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<IronLotusProj>(24);
            Item.SetWeaponValues(240, 2f, 0);
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 10);
            Item.autoReuse = true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}