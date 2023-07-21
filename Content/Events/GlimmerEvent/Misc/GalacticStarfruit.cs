using Aequus.Common.Items;
using Aequus.NPCs.BossMonsters.OmegaStarite;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Events.GlimmerEvent.Misc {
    public class GalacticStarfruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.WormFood];
            Item.ResearchUnlockCount = 3;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 2);
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !GlimmerZone.EventTechnicallyActive && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.myPlayer == player.whoAmI)
            {
                GlimmerSystem.BeginEvent();
            }
            return GlimmerZone.EventTechnicallyActive;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.DemoniteBar, 1)
                .AddTile(TileID.DemonAltar)
                .TryRegisterAfter(ItemID.DeerThing)
                .Clone()
                .ReplaceItem(ItemID.DemoniteBar, ItemID.CrimtaneBar)
                .TryRegisterAfter(ItemID.DeerThing);
        }
    }
}