using Aequus.Content.Necromancy;
using Aequus.Content.Prefixes.SoulCandles;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons.Summon.Candles
{
    public abstract class SoulCandle : ModItem
    {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        public int soulLimit;
        public int defSoulLimit { get; protected set; }
        public int useSouls;
        public int defUseSouls { get; protected set; }
        public int npcSummon;

        protected void DefaultToCandle(int limit, int souls, int npc)
        {
            Item.holdStyle = ItemHoldStyle;
            Item.DamageType = NecromancyDamageClass.Instance; // Invisible damage type which should hopefully trick the game into believing it's some sort of summoner related item
            soulLimit = limit;
            useSouls = souls;
            defSoulLimit = limit;
            defUseSouls = souls;
            npcSummon = npc;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
        }
        public void ClearPrefix()
        {
            soulLimit = defSoulLimit;
            useSouls = defUseSouls;
        }

        public override void HoldItem(Player player)
        {
            player.Aequus().soulCandleLimit = soulLimit;
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().candleSouls >= useSouls;
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.Aequus();
            if (aequus.candleSouls >= useSouls)
            {
                var ghost = NecromancyDatabase.TryGet(npcSummon, out var g) ? g : default(GhostInfo);
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
                aequus.candleSouls -= useSouls;
                SpawnGhost(player);
            }
            return true;
        }

        public void SpawnGhost(Player player)
        {
            var position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, 0), position, Vector2.Zero, ModContent.ProjectileType<GhostSpawner>(), Item.damage, 0f, player.whoAmI, npcSummon);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int i = tooltips.GetIndex("UseMana");
            tooltips.Insert(i, new TooltipLine(Mod, "CarryingSouls", AequusText.GetText("Tooltips.CarryingSouls", Main.LocalPlayer.Aequus().candleSouls, soulLimit)));
            tooltips.Insert(i, new TooltipLine(Mod, "UseSouls", AequusText.GetText("Tooltips.UseSouls", useSouls)));

            AequusTooltips.PercentageModifier(useSouls, defUseSouls, "PrefixSoulCost", tooltips, lowerIsGood: false);
            AequusTooltips.PercentageModifier(soulLimit, defSoulLimit, "PrefixSoulLimit", tooltips, lowerIsGood: true);
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return SoulCandlePrefix.ChoosePrefix(Item, rand).Type;
        }
    }
}