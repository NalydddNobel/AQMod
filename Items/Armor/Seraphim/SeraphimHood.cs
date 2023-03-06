using Aequus.Buffs.Minion;
using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Materials;
using Aequus.Projectiles.Summon.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Seraphim
{
    [AutoloadEquip(EquipType.Head)]
    public class SeraphimHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 9;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<GravetenderWisp>();
            Item.buffType = ModContent.BuffType<GravetenderMinionBuff>();
            Item.value = Item.sellPrice(gold: 1);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SeraphimRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.Seraphim");
            player.Aequus().setSeraphim = Item;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GravetenderHood>()
                .AddIngredient<Hexoplasm>(8)
                .AddTile(TileID.Loom)
                .TryRegisterBefore((ItemID.GravediggerShovel));
        }
    }
}