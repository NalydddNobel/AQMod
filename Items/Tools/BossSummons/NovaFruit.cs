using AQMod.Assets.ItemOverlays;
using AQMod.Common.Config;
using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Items.Materials.Energies;
using AQMod.Localization;
using AQMod.NPCs.Boss.Starite;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.BossSummons
{
    public class NovaFruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = (int)BossSpawnItemSortOrder.Abeemination;
            if (!Main.dedServ && AQConfigClient.Instance.ScrollShader)
                AQMod.ItemOverlays.Register(new EnchantmentOverlay(), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Pink;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !AQMod.CosmicEvent.IsActive && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                GlimmerEventSky._glimmerLight = 1f;
            NPC.NewNPC((int)player.position.X, (int)player.position.Y - 1600, ModContent.NPCType<OmegaStarite>(), 0, OmegaStarite.PHASE_NOVA, 0f, 0f, 0f, player.whoAmI);
            AQMod.BroadcastMessage(AQText.Key + "Common.AwakenedOmegaStarite", AQMod.BossMessage);
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<UltimateEnergy>());
            r.AddIngredient(ItemID.HellstoneBar, 15);
            r.AddIngredient(ItemID.CrystalShard, 8);
            r.AddTile(TileID.DemonAltar);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}