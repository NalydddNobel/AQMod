using Aequus.Common.GlobalItems;
using Aequus.Content.Necromancy;
using Aequus.Items.Prefixes.SoulCandles;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Summon.Necro.Candles
{
    public abstract class BaseSoulCandle : ModItem, ItemHooks.IUpdateVoidBag
    {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        public int OriginalSoulLimit { get; protected set; }
        public int OriginalSoulCost { get; protected set; }
        public float OriginalNPCSpeed { get; protected set; }
        public int NPCToSummon { get; protected set; }

        public int soulLimit;
        public int soulCost;
        public float npcSpeed;

        protected void DefaultToCandle(int summonDamage, int limit, int souls, int npc, float speed = 0f)
        {
            Item.holdStyle = ItemHoldStyle;
            Item.DamageType = NecromancyDamageClass.Instance; // Invisible damage type which should hopefully trick the game into believing it's some sort of summoner related item

            OriginalSoulLimit = limit;
            OriginalSoulCost = souls;
            OriginalNPCSpeed = speed;

            soulLimit = limit;
            soulCost = souls;
            npcSpeed = speed;
            
            NPCToSummon = npc;
            Item.damage = summonDamage;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
        }
        public void ClearPrefix()
        {
            soulLimit = OriginalSoulLimit;
            soulCost = OriginalSoulCost;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.SummonWeapon;
        }

        public void UpdatePlayerSoulLimit(Player player)
        {
            var aequus = player.Aequus();
            aequus.heldSoulCandle = Math.Max(aequus.heldSoulCandle, soulLimit);
        }

        public override void HoldItem(Player player)
        {
            UpdatePlayerSoulLimit(player);
        }

        public override void UpdateInventory(Player player)
        {
            UpdatePlayerSoulLimit(player);
        }

        void ItemHooks.IUpdateVoidBag.UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            UpdatePlayerSoulLimit(player);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().candleSouls >= soulCost;
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.Aequus();
            if (aequus.candleSouls >= soulCost)
            {
                var ghost = NecromancyDatabase.TryGet(NPCToSummon, out var g) ? g : default(GhostInfo);
                int slots = ghost.SlotsUsed;
                if (aequus.ghostSlots + slots > aequus.ghostSlotsMax)
                {
                    int priority = ghost.despawnPriority;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].TryGetGlobalNPC<NecromancyNPC>(out var n) && n.isZombie && n.zombieOwner == player.whoAmI)
                        {
                            if (priority <= n.DespawnPriority(Main.npc[i]))
                            {
                                return false;
                            }
                        }
                    }
                }
                aequus.candleSouls -= soulCost;
                SpawnGhost(player);
            }
            return true;
        }

        public virtual void SpawnGhost(Player player)
        {
            var position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, 0), position, Vector2.Zero, ModContent.ProjectileType<GhostSpawner>(), Item.damage, 0f, player.whoAmI, NPCToSummon, npcSpeed);
            }
        }

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

            int i = tooltips.GetIndex("UseMana");
            tooltips.Insert(i, new TooltipLine(Mod, "CarryingSouls", AequusText.GetText("ItemTooltip.Common.CarryingSouls", Main.LocalPlayer.Aequus().candleSouls, soulLimit)));
            tooltips.Insert(i, new TooltipLine(Mod, "UseSouls", AequusText.GetText("ItemTooltip.Common.UseSouls", soulCost)));

            TooltipsGlobalItem.PercentageModifier(soulCost, OriginalSoulCost, "PrefixSoulCost", tooltips, higherIsGood: false);
            TooltipsGlobalItem.PercentageModifier(soulLimit, OriginalSoulLimit, "PrefixSoulLimit", tooltips, higherIsGood: true);
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return SoulCandlePrefix.ChoosePrefix(Item, rand).Type;
        }
    }
}