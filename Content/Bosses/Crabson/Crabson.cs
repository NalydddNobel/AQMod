using Aequus.Common.Bosses;
using Aequus.Common.Items;
using Aequus.Common.NPCs;
using Aequus.Content.Items.Potions.Healing.Restoration;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Bosses.Crabson;

public class Crabson : AequusBoss {
    protected override bool CloneNewInstances => true;

    #region Initialization
    public const float BossChecklistProgression = 2.66f;

    public override void Load() {
        TreasureBagId = LoadItem(new BossBag(this, ItemCommons.Rarity.CrabsonLoot, preHardmode: true));
        MaskId = LoadItem(new BossMask(this));
        TrophyId = LoadItem(new BossTrophy(this, BossTrophiesTile.Crabson));
        RelicId = LoadItem(new BossRelic(this, BossRelicsTile.Crabson));
    }

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 4;

        NPCID.Sets.TrailingMode[Type] = 7;
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            PortraitPositionYOverride = 48f,
            Position = new(0f, 60f),
            Scale = 0.8f,
            PortraitScale = 1f,
            Velocity = 2f,
            Direction = -1,
        };

        ExpertItemId = ItemID.EoCShield;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.CavernsBiome)
            .AddSpawn(BestiaryBuilder.OceanBiome);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        AddMasterRelic(npcLoot);
        AddExpertItem(npcLoot);
        AddTrophy(npcLoot);
        AddBossDropItems(npcLoot,
            ItemDropRule.Common(MaskId, 7)
        );
    }

    public override void SetDefaults() {
        NPC.lifeMax = 2500;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(gold: 4);
        NPC.aiStyle = -1;
        NPC.lavaImmune = true;
        NPC.trapImmune = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.width = 90;
        NPC.height = 60;
        NPC.damage = 20;
        NPC.defense = 14;
        NPC.boss = true;
        NPC.behindTiles = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment) {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
    }
    #endregion

    public override void BossLoot(ref string name, ref int potionType) {
        potionType = ModContent.ItemType<LesserRestorationPotion>();
    }
}