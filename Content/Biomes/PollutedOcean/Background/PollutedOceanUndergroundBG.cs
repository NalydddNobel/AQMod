using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Background; 

public class PollutedOceanUndergroundBG : ModUndergroundBackgroundStyle {
    public override void FillTextureArray(int[] textureSlots) {
        textureSlots[0] = 290;
        textureSlots[1] = 291;
        textureSlots[2] = 291;
        textureSlots[3] = 297;
    }
}