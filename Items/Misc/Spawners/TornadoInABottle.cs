﻿using Aequus.Common.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.GaleStreams;
using Aequus.NPCs.BossMonsters.DustDevil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Spawners {
    public class TornadoInABottle : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.QueenSlimeCrystal];
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override bool CanUseItem(Player player) {
            return player.ZoneSkyHeight && !NPC.AnyNPCs(ModContent.NPCType<DustDevil>());
        }

        public override bool? UseItem(Player player) {
            if (Main.netMode == NetmodeID.SinglePlayer) {
                NPC.SpawnBoss((int)player.position.X + player.width / 2, (int)player.position.Y - 1200, ModContent.NPCType<DustDevil>(), player.whoAmI);
            }
            else if (Main.myPlayer == player.whoAmI) {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<DustDevil>());
            }
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<Fluorescence>(10)
                .AddIngredient<FrozenTear>(10)
                .AddTile(TileID.DemonAltar)
                .TryRegisterAfter(ItemID.GoblinBattleStandard);
        }
    }
}