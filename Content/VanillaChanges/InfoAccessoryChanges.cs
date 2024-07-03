using Aequu2.Content.Configuration;
using Aequu2.Content.Items.Accessories.Informational.Calendar;
using Aequu2.Content.Items.Accessories.Informational.DebuffDPSMeter;
using System.Collections.Generic;
using Terraria.Localization;
using tModLoaderExtended.Recipes;

namespace Aequu2.Content.VanillaChanges;

public class InfoAccessoryChanges {
    public class PDA : GlobalItem, IRecipeEditor {
        public override bool IsLoadingEnabled(Mod mod) {
            return GameplayConfig.Instance.PDAGetsAequu2Items;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.PDA;
        }

        public override void SetDefaults(Item entity) {
            entity.StatsModifiedBy.Add(Mod);
        }

        public override void UpdateInfoAccessory(Item item, Player player) {
            UpdateAequu2InfoAccs(player);
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.PDA) {
                Aequu2.Log.Debug("Adding Aequu2 PDA ingredients...");
                recipe.AddIngredient(ModContent.ItemType<Calendar>());

                recipe.AddIngredient(ModContent.ItemType<GeigerCounter>());
            }
        }

        public static void UpdateAequu2InfoAccs(Player player) {
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
            return GameplayConfig.Instance.PDAGetsAequu2Items;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.FishFinder;
        }

        public override void SetDefaults(Item entity) {
            entity.StatsModifiedBy.Add(Mod);
        }

        public override void UpdateInfoAccessory(Item item, Player player) {
#if !DEBUG
            player.GetModPlayer<Aequu2Player>().accInfoQuestFish = true;
#endif
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            tooltips.AddTooltip(new TooltipLine(Mod, "AnglerBroadcaster", Language.GetTextValue("Mods.Aequu2.Items.Vanilla.FishFinder.Tooltip")));
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.FishFinder) {
                Aequu2.Log.Debug("Adding Aequu2 Fish Finder ingredients...");
#if !DEBUG
                recipe.AddIngredient(ModContent.ItemType<Old.Content.Items.Accessories.Info.AnglerBroadcaster>());
#endif
            }
        }
    }
}
