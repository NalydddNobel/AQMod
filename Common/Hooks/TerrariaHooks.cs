namespace Aequus.Common.Hooks;

public partial class TerrariaHooks : ILoadable {
    private Mod Mod { get; set; }

    public void Load(Mod mod) {
        Mod = mod;

        On_Player.FigureOutWhatToPlace += On_Player_FigureOutWhatToPlace; ;

        On_WorldGen.PlaceTile += On_WorldGen_PlaceTile;
    }

    public void Unload() { }
}
