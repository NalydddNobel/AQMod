namespace Aequus.Common.Utilities.Debug;

#if DEBUG
internal class PublishPrevention : LoadedType {
    protected override void Load() {
        On_Main.DoDraw += On_Main_DoDraw;
    }

    private static void On_Main_DoDraw(On_Main.orig_DoDraw orig, Main self, GameTime gameTime) {
        if (Main.gameMenu && Main.menuMode == 888 && Main.MenuUI?.CurrentState is Terraria.GameContent.UI.States.WorkshopPublishInfoStateForMods) {
            System.Windows.Forms.MessageBox.Show("You are in debug mode.", "Stop now.");
            Main.menuMode = 0;
            Main.MenuUI.SetState(null);
        }

        orig(self, gameTime);
    }
}
#endif