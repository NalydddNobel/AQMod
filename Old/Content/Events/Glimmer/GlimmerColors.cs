namespace Aequus.Old.Content.Events.Glimmer;

public class GlimmerColors : ModSystem {
    public static Color Red { get; private set; }
    public static Color Pink { get; private set; }
    public static Color Yellow { get; private set; }
    public static Color Cyan { get; private set; }
    public static Color Blue { get; private set; }
    public static Color CosmicEnergy { get; private set; }

    private bool _needInit;

    private static void SetRegularColors() {
        Red = new Color(255, 107, 107, 0);
        Pink = new Color(200, 40, 150, 0);
        Yellow = new Color(255, 255, 25, 0);
        Cyan = new Color(35, 255, 255, 0);
        Blue = new Color(35, 85, 255, 0);
        CosmicEnergy = new Color(200, 10, 255, 0);
    }

    private static void SetAnniversaryColors() {
        Red = Color.BlueViolet with { A = 0 };
        Pink = Color.Yellow with { A = 0 };
        Yellow = Color.HotPink with { A = 0 };
        Cyan = Color.HotPink with { A = 0 };
        Blue = Color.Pink with { A = 0 };
        CosmicEnergy = Color.Blue with { A = 0 };
    }

    public override void PreUpdateEntities() {
        if (_needInit) {
            SetRegularColors();

            if (Main.tenthAnniversaryWorld) {
                SetAnniversaryColors();
            }
        }
    }

    public override void ClearWorld() {
        _needInit = true;
    }
}
