using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod.BossChecklist
{
    internal struct EventEntry : IBossChecklistEntryData
    {
        private readonly Func<bool> isDowned;
        private readonly float progression;
        private readonly List<int> enemies;
        private readonly string eventName;
        private readonly int summonItem;
        private readonly List<int> loot;
        private readonly List<int> collectibles;
        private readonly string summonDescription;
        private readonly string despawnMessage;
        private readonly string portraitTexture;
        private readonly AQMod _aQMod;

        public EventEntry(Func<bool> downed, float progression, List<int> enemies, string eventName, int summonItem = 0, List<int> loot = null, List<int> collectibles = null,
            string summonDescription = null, string texture = null, string despawnMessage = null)
        {
            isDowned = downed;
            this.progression = progression;
            this.enemies = enemies;
            this.eventName = eventName;
            this.summonItem = summonItem;
            this.loot = loot;
            this.collectibles = collectibles;
            this.summonDescription = summonDescription;
            portraitTexture = texture;
            this.despawnMessage = despawnMessage;
            _aQMod = AQMod.Instance;
        }

        public void AddEntry(Mod bossChecklist)
        {
            bossChecklist.Call("AddEvent",
            progression,
            enemies,
            _aQMod,
            eventName,
            isDowned,
            summonItem,
            collectibles,
            loot,
            summonDescription,
            despawnMessage,
            portraitTexture);
        }
    }
}