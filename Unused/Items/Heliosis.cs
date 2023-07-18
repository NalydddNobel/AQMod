using Aequus.Common;
using Aequus.Items.Equipment.Accessories.Misc.Fishing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class Heliosis : ModItem {
        public static HashSet<int> ValidItemSlotContext { get; private set; }
        public bool devilsTongue;

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 3);
            devilsTongue = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Lerp(Main.DiscoColor, Color.White, 0.6f).UseA(200);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accDevilsTongue = devilsTongue;
            aequus.accRamishroom = Item;
            aequus.accNeonGenesis = Item;
            RegrowingBait.CheckRegrowingBait(player, Item);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            foreach (var t in tooltips) {
                if (t.Text == "{DevilsTongueTooltip}") {
                    t.Text = TextHelper.GetTextValue($"ItemTooltip.{nameof(Heliosis)}.DevilsTongue{(devilsTongue ? "Enabled" : "Disabled")}");
                }
            }
        }

        public override bool? CanConsumeBait(Player player) {
            return Item.consumable ? null : false;
        }

        public override void SaveData(TagCompound tag) {
            tag["DevilsTongue"] = devilsTongue;
        }

        public override void LoadData(TagCompound tag) {
            devilsTongue = tag.Get<bool>("DevilsTongue");
        }

        public override void NetSend(BinaryWriter writer) {
            writer.Write(devilsTongue);
        }

        public override void NetReceive(BinaryReader reader) {
            devilsTongue = reader.ReadBoolean();
        }

        public override bool CanRightClick() {
            return true;
        }

        public override void RightClick(Player player) {
            devilsTongue = !devilsTongue;
        }

        public override bool ConsumeItem(Player player) {
            return false;
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .AddIngredient<DevilsTongue>()
        //        .AddIngredient<Ramishroom>()
        //        .AddIngredient<RegrowingBait>()
        //        .AddIngredient<NeonGenesis>()
        //        .AddTile(TileID.TinkerersWorkbench)
        //        .Register();
        //}
    }
}