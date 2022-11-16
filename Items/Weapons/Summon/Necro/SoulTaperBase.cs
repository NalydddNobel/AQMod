using Aequus.Content.Necromancy;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public abstract class SoulTaperBase : LegacySoulGemWeaponBase
    {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        protected void DefaultToTaper(int summonDamage, int ammoTier)
        {
            Item.holdStyle = ItemHoldStyle;
            Item.DamageType = NecromancyDamageClass.Instance;

            OriginalTier = ammoTier;
            tier = ammoTier;
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
                ApplySupportEffects(player, Main.npc[chosenNPC], 0);
            }
            return true;
        }
        public virtual bool CanApplyTaper(NPC npc, Player player)
        {
            return npc.active && (npc.realLife == -1 || npc.realLife == npc.whoAmI) && !NPCID.Sets.ProjectileNPC[npc.type] && npc.TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie && n.zombieOwner == player.whoAmI;
        }

        public abstract void ApplySupportEffects(Player player, NPC npc, int soulGemDamage);

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