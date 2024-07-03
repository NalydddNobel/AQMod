using Terraria.GameContent.UI;

namespace Aequu2.Content.Items.Potions.Food.TaintedSeafood;

public class FoodPoisonedPlayer : ModPlayer {
    public bool foodPoisoned;
    private bool foodPoisonedOld;

    public override void ResetEffects() {
        // Make the player starve in the Dont Starve seed after food poison is done
        if (Main.dontStarveWorld && !foodPoisoned && foodPoisonedOld) {
            Player.AddBuff(BuffID.Starving, 5);
            EmoteBubble.MakeLocalPlayerEmote(EmoteID.Starving);
        }

        foodPoisonedOld = foodPoisoned;
        foodPoisoned = false;
    }
}
