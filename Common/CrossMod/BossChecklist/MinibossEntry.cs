using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod.BossChecklist
{
    public struct MinibossEntry : IBossChecklistEntryData
    {
        private readonly Func<bool> isDowned;
        private readonly float progression;
        private readonly List<int> boss;
        private readonly string bossName;
        private readonly int summonItem;
        private readonly List<int> loot;
        private readonly List<int> collectibles;
        private readonly string summonDescription;
        private readonly string despawnMessage;
        private readonly string portraitTexture;
        private readonly string bossHeadTexture;
        private readonly Func<bool> available;
        private readonly AQMod _aQMod;

        public MinibossEntry(Func<bool> downed, float progression, int boss, string bossName, int summonItem = 0, List<int> loot = null, List<int> collectibles = null,
            string summonDescription = null, string texture = null, string bossHeadTexture = null, Func<bool> available = null, string despawnMessage = null)
        {
            isDowned = downed;
            this.progression = progression;
            this.boss = new List<int>() { boss, };
            this.bossName = bossName;
            this.summonItem = summonItem;
            this.loot = loot;
            this.collectibles = collectibles;
            this.summonDescription = summonDescription;
            portraitTexture = texture;
            this.despawnMessage = despawnMessage;
            this.bossHeadTexture = bossHeadTexture;
            this.available = available;
            _aQMod = AQMod.GetInstance();
        }

        public MinibossEntry(Func<bool> downed, float progression, List<int> boss, string eventName, int summonItem = 0, List<int> loot = null, List<int> collectibles = null,
            string summonDescription = null, string texture = null, string bossHeadTexture = null, Func<bool> available = null, string despawnMessage = null)
        {
            isDowned = downed;
            this.progression = progression;
            this.boss = boss;
            bossName = eventName;
            this.summonItem = summonItem;
            this.loot = loot;
            this.collectibles = collectibles;
            this.summonDescription = summonDescription;
            portraitTexture = texture;
            this.despawnMessage = despawnMessage;
            this.bossHeadTexture = bossHeadTexture;
            this.available = available;
            _aQMod = AQMod.GetInstance();
        }

        public void AddEntry(Mod bossChecklist)
        {
            bossChecklist.Call(
            "AddMiniBoss",
            progression,
            boss,
            _aQMod,
            bossName,
            isDowned,
            summonItem,
            collectibles,
            loot,
            summonDescription,
            despawnMessage,
            portraitTexture,
            bossHeadTexture,
            available);
        }
    }
}
