#if POLLUTED_OCEAN
using Aequus.Common.NPCs;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Monsters.PollutedOcean.Conductor;

public partial class Conductor {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = FRAME_COUNT;
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = -1f,
            Scale = 1f,
        };
        NPCID.Sets.StatueSpawnedDropRarity[Type] = 0.05f;
        LegacyPushableEntities.NPCIDs.Add(Type);
    }

    public override void SetDefaults() {
        NPC.width = 18;
        NPC.height = 40;
        NPC.damage = 20;
        NPC.defense = 16;
        NPC.lifeMax = 35;
        NPC.HitSound = SoundID.NPCHit2;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.knockBackResist = 0.3f;
        NPC.value = Item.silver;
        NPC.aiStyle = -1;
        NPC.npcSlots = 2f;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.CompressedTrash.CompressedTrash>(), minimumDropped: 1, maximumDropped: 4));
#if COPPER_KEY
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CopperKey>(), chanceDenominator: CopperKey.DropRate));
#endif
    }
}
#endif