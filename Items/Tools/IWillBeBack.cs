using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class IWillBeBack : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = Constants.BossSpawnItemSortOrder.Abeemination;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                return true;
            return player.ZoneUnderworldHeight && !NPC.AnyNPCs(NPCID.WallofFlesh);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (item.lavaWet)
            {
                gravity = 0f;
                if (item.velocity.Y > 0f)
                    item.velocity.Y *= 0.9f;
                item.velocity.Y -= 0.2f;
                if (item.velocity.Y < 4f)
                    item.velocity.Y = 4f;
            }
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            if (item.lavaWet)
                grabRange *= 2;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                WorldDefeats.TownNPCLavaImmunity = !WorldDefeats.TownNPCLavaImmunity;
            else
            {
                NPC.SpawnWOF(player.Center);
            }
            return true;
        }
    }
}