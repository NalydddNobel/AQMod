using Terraria.ModLoader;

namespace Aequus.Content.Biomes.CrabCrevice.Background {
    public class CrabCreviceSurfaceBackground : ModSurfaceBackgroundStyle
    {
        private static string path;

        public override void Load()
        {
            path = this.GetPath();
            BackgroundTextureLoader.AddBackgroundTexture(Mod, $"{path}_0");
            BackgroundTextureLoader.AddBackgroundTexture(Mod, $"{path}_1");
            BackgroundTextureLoader.AddBackgroundTexture(Mod, $"{path}_2");
            path = string.Join("/", path.Split('/')[1..^0]);
        }

        public override void Unload()
        {
            path = null;
        }

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(Mod, $"{path}_0");
        }

        public override int ChooseMiddleTexture()
        {
            return -1;
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return -1;
        }
    }
}