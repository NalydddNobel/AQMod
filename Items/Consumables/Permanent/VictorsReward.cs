using Aequus.Common.ItemDrops;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Permanent
{
    public class VictorsReward : ModItem
    {
        public static HashSet<int> InvalidNPCIDs { get; private set; }

        public class DropCondition : FlawlessCondition
        {
            public override bool CanDrop(DropAttemptInfo info)
            {
                return info.npc.boss && !InvalidNPCIDs.Contains(info.npc.type) && base.CanDrop(info);
            }
        }

        public override void Load()
        {
            InvalidNPCIDs = new HashSet<int>()
            {
                NPCID.MartianSaucerCore,
            };
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void Unload()
        {
            InvalidNPCIDs?.Clear();
            InvalidNPCIDs = null;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 1);
            Item.maxStack = 9999;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.Aequus().maxLifeRespawnReward = false;
                return true;
            }
            if (!player.Aequus().maxLifeRespawnReward)
            {
                player.Aequus().maxLifeRespawnReward = true;
                return true;
            }

            return false;
        }

        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }
    }
}