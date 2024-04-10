using Aequus.Content.Configuration;
using Aequus.Content.Equipment.Informational.Calendar;
using Aequus.Content.Equipment.Informational.DebuffDPSMeter;
using Aequus.Core.Initialization;
using Aequus.Old.Content.Equipment.Info;

namespace Aequus.Content.VanillaChanges;

public class InfoAccessoryChanges {
    public class PDA : GlobalItem, IRecipeEditor {
        public override bool IsLoadingEnabled(Mod mod) {
            return GameplayConfig.Instance.PDAGetsAequusItems;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            return entity.type == ItemID.PDA;
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
            info.accInfoDPSMeterDebuff = true;
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

        public override void UpdateInfoAccessory(Item item, Player player) {
#if !DEBUG
            player.GetModPlayer<AequusPlayer>().accInfoQuestFish = true;
#endif
        }

        public void EditRecipe(Recipe recipe) {
            if (recipe.createItem.type == ItemID.FishFinder) {
                Aequus.Log.Debug("Adding Aequus Fish Finder ingredients...");
#if !DEBUG
                recipe.AddIngredient(ModContent.ItemType<AnglerBroadcaster>());
#endif
            }
        }
    }
}
