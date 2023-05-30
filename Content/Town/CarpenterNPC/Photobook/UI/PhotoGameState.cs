using Terraria;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Town.CarpenterNPC.Photobook.UI {
    public struct PhotoGameState : TagSerializable
    {
        public bool isDayTime;
        public ushort time;

        public static PhotoGameState Current()
        {
            var gameState = new PhotoGameState
            {
                time = (ushort)(int)Main.time,
                isDayTime = Main.dayTime
            };
            return gameState;
        }

        public TagCompound SerializeData()
        {
            return new TagCompound()
            {
                ["DayTime"] = isDayTime,
                ["Time"] = time,
            };
        }

        public static PhotoGameState DeserialzeData(TagCompound tag)
        {
            var gameState = new PhotoGameState
            {
                isDayTime = tag.GetOrDefault("DayTime", defaultValue: true)
            };
            gameState.time = tag.GetOrDefault("Time", defaultValue: (ushort)(gameState.isDayTime ? Main.dayLength / 2 : Main.nightLength / 2));
            return gameState;
        }
    }
}