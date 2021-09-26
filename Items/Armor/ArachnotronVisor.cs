using AQMod.Assets.PlayerLayers;
using AQMod.Common;
using AQMod.Items.Misc.Energies;
using AQMod.Localization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ArachnotronVisor : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 10;
            item.rare = ItemRarityID.LightPurple;
            item.value = Item.sellPrice(gold: 7, silver: 50);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ArachnotronRibcage>() && legs.type == ModContent.ItemType<ArachnotronRevvers>();
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 5;
            player.minionDamage += 0.1f;
            player.nightVision = true;
        }

        public override void UpdateVanity(Player player, EquipType type)
        {
            var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
            if (drawingPlayer.oldPosLength < 10)
                drawingPlayer.oldPosLength = 10;
            drawingPlayer.specialHead = SpecialHeadID.ArachnotronVisor;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = AQText.ArmorSetBonus("Arachnotron").Value;
            player.GetModPlayer<AQPlayer>().primeTime = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.SpiderMask);
            r.AddIngredient(ItemID.FlareGun);
            r.AddIngredient(ItemID.HallowedBar, 12);
            r.AddIngredient(ItemID.SoulofFright, 4);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}