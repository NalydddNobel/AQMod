using AQMod.Content.HookBarbs;
using AQMod.Items.Materials.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class NeutralityAmulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.damage = 65;
            item.knockBack = 1.5f;
            item.crit = 4;
            item.accessory = true;
            item.defense = 2;
            item.rare = ItemRarityID.LightRed;
            item.value = Item.sellPrice(gold: 3, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AQPlayer>().lightAmulet = true;
            player.GetModPlayer<AQPlayer>().darkAmulet = true;
            player.armorPenetration += 5;
            player.GetModPlayer<HookBarbPlayer>().AddBarb(new DamageBarbAttachmentType(item));
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod); 
            r.AddIngredient(ModContent.ItemType<DarkAmulet>());
            r.AddIngredient(ModContent.ItemType<LightAmulet>());
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}