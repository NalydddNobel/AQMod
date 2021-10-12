using AQMod.Localization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.RobsterQuests
{
    public abstract class RobsterHunt : ContentItem
    {
        public RobsterHunt(Mod mod, string name) : base(mod, name)
        {
        }

        protected RobsterHunt(string mod, string name) : base(mod, name)
        {
        }

        public HuntData Hunt { get; private set; }

        public abstract void Setup();
        public abstract int GetQuestItem();
        public abstract string QuestChat();

        /// <summary>
        /// Checks if the player has completed the Hunt. DO NOT DO ANYTHING IN HERE OTHER THAN CHECKS SINCE THIS IS USED TO CHECK IF THE TEXT SHOULD SAY "Complete Hunt" WHICH RUNS EVERY FRAME
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool IsHuntComplete(Player player)
        {
            return player.HasItem(GetQuestItem());
        }

        /// <summary>
        /// Can be used to check if the player is null, since the player can be nulled in certain circumstances
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanStart(Player player)
        {
            return true;
        }

        /// <summary>
        /// Ran when the hunt starts. The player instance can be null.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnStart(Player player)
        {
        }

        /// <summary>
        /// Ran when the player manually quits the hunt. This doesn't run if you fail or the npc target dies.
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnQuit(Player player)
        {
        }

        /// <summary>
        /// A method that removes all normal quest tiles. This should be ran in <see cref="RemoveHunt"/>
        /// </summary>
        protected void RemoveQuestTiles()
        {
            var chaliceTile = ModContent.TileType<Tiles.QuestTiles.JeweledChalice>();
            var candelabraTile = ModContent.TileType<Tiles.QuestTiles.JeweledCandelabra>();
            for (int i = 5; i < Main.maxTilesX - 5; i++)
            {
                for (int j = 5; j < Main.maxTilesY - 5; j++)
                {
                    var tile = Main.tile[i, j];
                    if (tile.type == candelabraTile || tile.type == chaliceTile)
                        tile.active(active: false);
                }
            }
        }

        /// <summary>
        /// Called when a hunt ends in any way. Should be used to remove quest tiles and such.
        /// </summary>
        public virtual void RemoveHunt()
        {
        }

        public void RegularQuestCompleteChat()
        {
            Main.npcChatText = AQText.ModText("Common.RobsterRandomHuntComplete" + Main.rand.Next(6)).Value;
        }

        public void RegularQuestRewards(Player player)
        {
            player.QuickSpawnItem(ItemID.WoodenCrate);
            if (Main.rand.NextBool(3))
                player.QuickSpawnItem(ItemID.IronCrate);
            if (Main.rand.NextBool(10))
                player.QuickSpawnItem(ItemID.GoldenCrate);
        }

        /// <summary>
        /// Ran when the player completes the quest. Defaults to spawning in regular loot and changing the current npc text Runs <see cref="RemoveHunt"/> and then <see cref="HuntSystem.RandomizeHunt(Player)"/> right after this method
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnComplete(Player player)
        {
            RegularQuestCompleteChat();
            RegularQuestRewards(player);
        }

        /// <summary>
        /// Returns a tag used for saving. Do NOT add a value to "Hunt" or "TargetNPC" since those keys gets overidden
        /// </summary>
        /// <returns></returns>
        public virtual TagCompound Save()
        {
            return null;
        }

        public virtual void Load(TagCompound tag)
        {

        }
    }
}