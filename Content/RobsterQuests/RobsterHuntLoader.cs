namespace AQMod.Content.RobsterQuests
{
    public class RobsterHuntLoader : ContentLoader<RobsterHunt>
    {
        public static class ID
        {
            public const byte None = 0;
            public const byte JeweledChalice = 1;
        }

        public override void Setup()
        {
            base.Setup();

            var mod = AQMod.Instance;
            AddContent(new HuntJeweledChalice(mod, "JeweledChalice"));
            AddContent(new HuntJeweledCandelabra(mod, "JeweledCandelabra"));
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