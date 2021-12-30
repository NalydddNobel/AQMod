using AQMod.Content.Quest.Lobster.HuntTypes;
using Terraria.ModLoader;

namespace AQMod.Content.Quest.Lobster
{
    public class RobsterHuntLoader : ContentLoader<RobsterHunt>
    {
        public static RobsterHuntLoader Instance { get => ModContent.GetInstance<RobsterHuntLoader>(); }

        public override void Load(AQMod mod)
        {
            InitializeContent(new HuntJeweledChalice(mod, "JeweledChalice"));
            InitializeContent(new HuntJeweledCandelabra(mod, "JeweledCandelabra"));
        }

        /// <summary>
        /// Runs the setup method for each hunt
        /// </summary>
        public void SetupHunts()
        {
            foreach (RobsterHunt h in _content)
            {
                h.Setup();
            }
        }
    }
}