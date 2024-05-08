using Aequus.Content.Configuration;
using Aequus.Content.Equipment.Informational.Calendar;
using Aequus.Content.Equipment.Informational.DebuffDPSMeter;
using System.Collections.Generic;
using Terraria.Localization;
using tModLoaderExtended.Recipes;

namespace Aequus.Content.VanillaChanges;

public class InfoAccessoryChanges {
    public class PDA : GlobalItem, IRecipeEditor {
        public override bool IsLoadingEnabled(Mod mod) {
            return GameplayConfig.Instance.PDAGetsAequusItems;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.PDA;
        }

        public override void SetDefaults(Item entity) {
            entity.StatsModifiedBy.Add(Mod);
        }

        public override void UpdateInfoAccessory(Item item, Player player) {
            UpdateAequusInfoAccs(player);
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.PDA) {
                Aequus.Log.Debug("Adding Aequus PDA ingredients...");
                recipe.AddIngredient(ModContent.ItemType<Calendar>());

                recipe.AddIngredient(ModContent.ItemType<GeigerCounter>());
            }
        }

        public static void UpdateAequusInfoAccs(Player player) {
            AequusPlayer info = player.GetModPlayer<AequusPlayer>();
            info.accInfoDayCalendar = true;
            info.accInfoDebuffDPS = true;
#if !DEBUG
            info.accInfoQuestFish = true;
#endif
        }
    }

    public class FishFinder : GlobalItem, IRecipeEditor {
        public override bool IsLoadingEnabled(Mod mod) {
            return GameplayConfig.Instance.PDAGetsAequusItems;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.FishFinder;
        }

        public override void SetDefaults(Item entity) {
            entity.StatsModifiedBy.Add(Mod);
        }

        public override void UpdateInfoAccessory(Item item, Player player) {
#if !DEBUG
            player.GetModPlayer<AequusPlayer>().accInfoQuestFish = true;
#endif
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            tooltips.AddTooltip(new TooltipLine(Mod, "AnglerBroadcaster", Language.GetTextValue("Mods.Aequus.Items.Vanilla.FishFinder.Tooltip")));
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.FishFinder) {
                Aequus.Log.Debug("Adding Aequus Fish Finder ingredients...");
#if !DEBUG
                recipe.AddIngredient(ModContent.ItemType<Old.Content.Equipment.Info.AnglerBroadcaster>());
#endif
            }
        }
    }
}
