using Terraria;

namespace AQMod.Content.WorldEvents
{
    /// <summary>
    /// A replica of vanilla's windy day flags, also manages things with the wind.
    /// </summary>
    public static class ImitatedWindyDay
    {
		public static bool OverrideVanillaWindUpdates { get; set; }
        public static bool IsItAHappyWindyDay { get; private set; }
        public static bool IsItAHappyWindyDay_WindyEnough { get; private set; }

		// these are statics in vanilla so why not here lol?
		public static float MinWind;
		public static float MaxWind;
		public static float MaxWindSpeed;
		public static float MinWindSpeed;

		internal static void Reset(bool resetNonUpdatedStatics = false)
        {
			IsItAHappyWindyDay = false;
			if (resetNonUpdatedStatics)
            {
				MinWind = 0.34f;
				MaxWind = 0.4f;
				MaxWindSpeed = 1f;
				MinWindSpeed = -1f;
			}
        }

        internal static void UpdateWindyDayFlags()
        {
			if (Main.cloudAlpha == 0f)
			{
				float windSpeed = Main.windSpeedSet.Abs();
				if (windSpeed < MinWind)
				{
					IsItAHappyWindyDay_WindyEnough = false;
				}
				if (windSpeed >= MaxWind)
				{
					IsItAHappyWindyDay_WindyEnough = true;
				}
				if (Main.time < 10800.0d || Main.time > 43200.0d || !Main.dayTime)
				{
					IsItAHappyWindyDay = false;
				}
                else
                {
					if (IsItAHappyWindyDay_WindyEnough)
					{
						IsItAHappyWindyDay = true;
					}
				}
			}
			else
			{
				IsItAHappyWindyDay = false;
			}
		}
	}
}