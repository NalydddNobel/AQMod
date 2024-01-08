using Aequus.Common.NPCs;
using Aequus.Content.Items.Potions.Healing.Restoration;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.Bosses.Crabson;

[AutoloadBossHead]
[AutoloadBossMask]
public partial class Crabson : AequusBoss {
    #region AI
    public const int STATE_INIT = 0;
    public const int STATE_DESTROY = 1;
    public const int STATE_TEST = 2;
    public const int STATE_SUPERDUPERSLAP = 3;
    public const int STATE_GASCAN = 4;
    public const int STATE_PEARLCRUSH = 5;

    public override void AI() {
        UpdateMood();

        switch (State) {
            case STATE_INIT:
                State = 1;
                break;

            case STATE_DESTROY:
                break;
        }
    }
    #endregion

    #region Initialization
    public const float BossChecklistProgression = 2.66f;

    public override bool PreHardmode => true;

    public override int ItemRarity => ItemRarityID.Blue;

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
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.CavernsBiome)
            .AddSpawn(BestiaryBuilder.OceanBiome);
    }

    public override void ModifyNPCLoot() {

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