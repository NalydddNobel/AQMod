using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Necromancer
{
    [AutoloadEquip(EquipType.Body, EquipType.Legs)]
    public class NecromancerRobe : ModItem
    {

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 6;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
            Item.legSlot = -1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.Aequus().ghostLifespan += 1800;
            player.Aequus().ghostSlotsMax++;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, "NecromancerRobe_Legs", EquipType.Legs);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GravetenderRobes>()
                .AddIngredient<DemonicEnergy>(1)
                .AddTile(TileID.Loom)
                .TryRegisterBefore((ItemID.GravediggerShovel));
        }
    }
}