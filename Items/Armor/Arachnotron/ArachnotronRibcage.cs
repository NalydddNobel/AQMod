using AQMod.Assets.PlayerLayers;
using AQMod.Common;
using AQMod.Items.Energies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.Arachnotron
{
    [AutoloadEquip(EquipType.Body)]
    public class ArachnotronRibcage : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 20;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 10);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<AQPlayer>().arachnotron = true;
            player.maxMinions += 1;
            player.meleeSpeed += 0.1f;
            player.endurance += 0.05f;
        }

        public override void UpdateVanity(Player player, EquipType type)
        {
            var drawingPlayer = player.GetModPlayer<GraphicsPlayer>();
            if (drawingPlayer.oldPosLength < 10)
                drawingPlayer.oldPosLength = 10;
            drawingPlayer.specialBody = SpecialBodyID.ArachnotronRibcage;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderBreastplate);
            r.AddIngredient(ItemID.BandofRegeneration);
            r.AddIngredient(ItemID.HallowedBar, 18);
            r.AddIngredient(ItemID.SoulofFright, 6);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}