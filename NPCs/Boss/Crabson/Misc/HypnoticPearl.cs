using Aequus.Items;
using Aequus.Items.Materials.PearlShards;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.Crabson.Misc {
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
            Item.value = Item.buyPrice(gold: 5);
        }

        public override bool CanUseItem(Player player) {
            return (player.ZoneBeach || player.Aequus().ZoneCrabCrevice) && !NPC.AnyNPCs(ModContent.NPCType<Crabson>());
        }

        public override bool? UseItem(Player player) {
            if (Main.netMode == NetmodeID.SinglePlayer) {
                NPC.SpawnBoss((int)player.position.X, (int)player.position.Y + 1000, ModContent.NPCType<Crabson>(), player.whoAmI);
            }
            else if (Main.myPlayer == player.whoAmI) {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<Crabson>());
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
}