using Aequus.Biomes.Glimmer;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class CosmicMonolithScene : ModSceneEffect
    {
        public const string Key = "Aequus:CosmicMonolith";

        public static int Active { get; set; }

        public override bool IsSceneEffectActive(Player player)
        {
            return Active > 0;
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SkyManager.Instance[Key] = new GlimmerSky() { checkDistance = false, };
            }
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                if (!SkyManager.Instance[Key].IsActive())
                {
                    SkyManager.Instance.Activate(Key);
                }
            }
            else
            {
                if (SkyManager.Instance[Key].IsActive())
                {
                    SkyManager.Instance.Deactivate(Key);
                }
            }
        }
    }
}