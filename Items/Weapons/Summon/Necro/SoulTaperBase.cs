using Aequus.Content.Necromancy;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public abstract class SoulTaperBase : SoulWeaponBase
    {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        protected void DefaultToTaper(int summonDamage, int limit, int souls)
        {
            Item.holdStyle = ItemHoldStyle;
            Item.DamageType = NecromancyDamageClass.Instance;

            OriginalSoulLimit = limit;
            OriginalSoulCost = souls;

            soulLimit = limit;
            soulCost = souls;
            Item.damage = summonDamage;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.SummonWeapon;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer != player.whoAmI)
                return null;

            var aequus = player.Aequus();
            if (aequus.candleSouls >= soulCost)
            {
                int chosenNPC = -1;
                float distance = 64f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (CanApplyTaper(Main.npc[i], player))
                    {
                        float d = Main.npc[i].Distance(Main.MouseWorld);
                        if (d < distance)
                        {
                            chosenNPC = i;
                            distance = d;
                        }
                    }
                }
                if (chosenNPC != -1)
                {
                    aequus.candleSouls -= soulCost;
                    ApplySupportEffects(player, Main.npc[chosenNPC]);
                }
            }
            return true;
        }
        public virtual bool CanApplyTaper(NPC npc, Player player)
        {
            return npc.active && (npc.realLife == -1 || npc.realLife == npc.whoAmI) && !NPCID.Sets.ProjectileNPC[npc.type] && npc.TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie && n.zombieOwner == player.whoAmI;
        }

        public abstract void ApplySupportEffects(Player player, NPC npc);

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int j = 0; j < tooltips.Count; j++)
            {
                if (tooltips[j].Name == "Knockback")
                {
                    tooltips.RemoveAt(j);
                    j--;
                }
            }
            base.ModifyTooltips(tooltips);
        }

    }
}