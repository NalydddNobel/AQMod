namespace Aequus.Content.Biomes.Meadows;

public class MeadowsBiome : ModBiome {
    private int? _music;
    public override int Music => _music ??= Aequus.GetMusicOrDefault("Extra_Meadows", -1);

    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

    public override bool IsBiomeActive(Player player) {
        return ModContent.GetInstance<MeadowsBiomeSystem>().TileCount > MeadowsBiomeSystem.TileCountNeeded;
    }
}
