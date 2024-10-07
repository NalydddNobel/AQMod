using Aequus.Common.CodeGeneration;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Aequus.Common.Structures;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusNPC {
    
    [CompilerGenerated]
    private void ModifyNPCLootInner(NPC npc, NPCLoot loot) {
        Content.Items.Accessories.AccessoryDamageEarring.AccessoryDamageEarring.AddNPCLoot(npc, this, loot);
        Content.Items.Accessories.Backpacks.PlanteraBackpack.ModifyNPCDrops(npc, loot);
        Content.Items.Accessories.Backpacks.WallOfFleshBackpack.ModifyNPCDrops(npc, loot);
        Content.Items.Accessories.DamageOnKillClaw.DamageOnKillClaw.AddNPCLoot(npc, this, loot);
        Content.Items.Materials.PossessedShard.PossessedShard.ModifyNPCLoot(npc, loot);
    }
    
    [CompilerGenerated]
    private void ModifyShopInner(NPCShop shop) {
        Content.Items.Consumable.ThrowPowders.MeadowPowder.ModifyShop(shop);
    }
    
    [CompilerGenerated]
    private void OnSpawnInner(NPC npc, IEntitySource source) {
        Content.Villagers.NPCSettleDownMessage.OnSpawn(npc, source);
    }
}