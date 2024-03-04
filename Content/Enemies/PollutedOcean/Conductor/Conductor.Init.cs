using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.DataSets;
using Aequus.Core.Initialization;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

public partial class Conductor : IPostPopulateItemDropDatabase {
    public static int TravelingMerchantInfoItemChance { get; set; } = 30;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 18;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = -1f,
            Scale = 1f,
        };
        NPCSets.StatueSpawnedDropRarity[Type] = 0.05f;
        NPCMetadata.PushableByTypeId.Add(Type);
    }

    public override void SetDefaults() {
        NPC.width = 18;
        NPC.height = 40;
        NPC.damage = 20;
        NPC.defense = 16;
        NPC.lifeMax = 35;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.knockBackResist = 0.5f;
        NPC.value = Item.silver;
        NPC.aiStyle = -1;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
    }

    public virtual void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database) {
        ExtendLoot.InheritDropRules(NPCID.Skeleton, Type, database);
    }
}
