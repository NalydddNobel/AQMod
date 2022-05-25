using Aequus.NPCs.Boss;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Summons
{
    public class HypnoticPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.SlimeCrown];
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneBeach/* || player.Biomes().zoneCrabCrevice*/ && !NPC.AnyNPCs(ModContent.NPCType<Crabson>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnBoss((int)player.position.X, (int)player.position.Y + 1000, ModContent.NPCType<Crabson>(), player.whoAmI);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }
            return true;
        }
    }
}