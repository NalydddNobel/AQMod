using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.Items.Materials;
using Aequus.Content.Items.Tools.Keys;
using Aequus.DataSets;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Content.Enemies.PollutedOcean.Conductor;

public partial class Conductor : IPostPopulateItemDropDatabase {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = FRAME_COUNT;
        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = -1f,
            Scale = 1f,
        };
        NPCSets.StatueSpawnedDropRarity[Type] = 0.05f;
        NPCDataSet.PushableByTypeId.Add(Type);
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
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CompressedTrash>(), minimumDropped: 1, maximumDropped: 4));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CopperKey>(), chanceDenominator: CopperKey.DropRate));
    }

    public virtual void PostPopulateItemDropDatabase(Mod mod, ItemDropDatabase database) {
        //ExtendLoot.InheritDropRules(NPCID.Skeleton, Type, database);
    }
}
