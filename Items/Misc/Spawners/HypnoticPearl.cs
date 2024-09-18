using Aequus.Common.Items;
using Aequus.Items.Materials.PearlShards;
using Aequus.NPCs.BossMonsters.Crabson.CrabsonOld;
using Terraria.Audio;

namespace Aequus.Items.Misc.Spawners;
public class HypnoticPearl : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.SortingPriorityBossSpawns[Item.type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.SlimeCrown];
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults() {
        Item.DefaultToHoldUpItem();
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 2, silver: 50);
    }

    public override bool CanUseItem(Player player) {
        bool inBiome = player.ZoneBeach;
#if !CRAB_CREVICE_DISABLE
        if (player.InModBiome<global::Aequus.Content.Biomes.CrabCrevice.CrabCreviceBiome>()) {
            inBiome = true;
        }
#endif
        return inBiome && !NPC.AnyNPCs(ModContent.NPCType<CrabsonOld>());
    }

    public override bool? UseItem(Player player) {
        if (Main.netMode == NetmodeID.SinglePlayer) {
            NPC.SpawnBoss((int)player.position.X, (int)player.position.Y + 1000, ModContent.NPCType<CrabsonOld>(), player.whoAmI);
        }
        else if (Main.myPlayer == player.whoAmI) {
            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<CrabsonOld>());
        }
        SoundEngine.PlaySound(SoundID.Roar, player.position);
        return true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<PearlShardWhite>(3)
            .AddIngredient<PearlShardBlack>(3)
            .AddIngredient<PearlShardPink>()
            .AddTile(TileID.DemonAltar)
            .TryRegisterBefore(ItemID.SuspiciousLookingEye)
            .DisableDecraft();
    }
}