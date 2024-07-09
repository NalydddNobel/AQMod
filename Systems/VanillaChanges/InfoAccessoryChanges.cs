using AequusRemake.Content.Configuration;
using AequusRemake.Content.Items.Accessories.Informational.Calendar;
using AequusRemake.Content.Items.Accessories.Informational.DebuffDPSMeter;
using System.Collections.Generic;
using Terraria.Localization;
using tModLoaderExtended.Recipes;

namespace AequusRemake.Systems.VanillaChanges;

public class InfoAccessoryChanges {
    public class PDA : GlobalItem, IRecipeEditor {
        public override bool IsLoadingEnabled(Mod mod) {
            return GameplayConfig.Instance.PDAGetsAequusRemakeItems;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.PDA;
        }

        public override void SetDefaults(Item entity) {
            entity.StatsModifiedBy.Add(Mod);
        }

        public override void UpdateInfoAccessory(Item item, Player player) {
            UpdateAequusRemakeInfoAccs(player);
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.PDA) {
                Log.Debug("Adding AequusRemake PDA ingredients...");
                recipe.AddIngredient(ModContent.ItemType<Calendar>());

                recipe.AddIngredient(ModContent.ItemType<GeigerCounter>());
            }
        }

        public static void UpdateAequusRemakeInfoAccs(Player player) {
            AequusPlayer info = player.GetModPlayer<AequusPlayer>();
            info.accInfoDayCalendar = true;
            info.accInfoDebuffDPS = true;
        }
    }

    public class FishFinder : GlobalItem, IRecipeEditor {
        public override bool IsLoadingEnabled(Mod mod) {
            return GameplayConfig.Instance.PDAGetsAequusRemakeItems;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.FishFinder;
        }

        public override void SetDefaults(Item entity) {
            entity.StatsModifiedBy.Add(Mod);
        }

        public override void UpdateInfoAccessory(Item item, Player player) {
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            tooltips.AddTooltip(new TooltipLine(Mod, "AnglerBroadcaster", Language.GetTextValue("Mods.AequusRemake.Items.Vanilla.FishFinder.Tooltip")));
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.FishFinder) {
                Log.Debug("Adding AequusRemake Fish Finder ingredients...");
            }
        }
    }
}
