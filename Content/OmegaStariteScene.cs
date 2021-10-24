using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Content
{
    public static class OmegaStariteScene
    {
        /// <summary>
        /// A cache of the most recently spawned Omega Starite npc. Use this only for visual effects please. Defaults to -1.
        /// </summary>
        public static short OmegaStariteIndexCache = -1;
        /// <summary>
        /// <list type="number">Default value, nothing important is going on with the Ultimate Sword.
        /// <item>This is when Omega Starite is swooping down to take the Ultimate Sword</item>
        /// <item>This is when Omega Starite has grabbed the Ultimate Sword</item>
        /// <item>This is when Omega Starite has died, and the sword shouldn't appear still even though the Glimmer Event is still technically going on for a bit longer</item>
        /// </list>
        /// </summary>
        public static byte SceneType;
        /// <summary>
        /// The brightness value for the bg sky
        /// </summary>
        public static float SkyLight;

        public static void Initialize()
        {
            OmegaStariteIndexCache = -1;
            SceneType = 0;
        }

        public static Vector2 SwordDrawPosition()
        {
            return new Vector2(AQMod.glimmerEvent.tileX * 16f, AQMod.glimmerEvent.tileY - 120f);
        }

        public static void InteractWithSword(Player player)
        {

        }
    }
}