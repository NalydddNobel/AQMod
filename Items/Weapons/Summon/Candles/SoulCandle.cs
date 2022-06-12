using Aequus.Content.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Candles
{
    public abstract class SoulCandle : ModItem
    {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        public int soulLimit;
        public int useSouls;
        public int npcSummon;

        protected void DefaultToCandle(int limit, int souls, int npc)
        {
            Item.holdStyle = ItemHoldStyle;
            soulLimit = limit;
            useSouls = souls;
            npcSummon = npc;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
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
                var ghost = NecromancyDatabase.GetOrDefault(npcSummon);
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
                Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, 0), position, Vector2.Zero, ModContent.ProjectileType<NecromanticEnemySpawner>(), Item.damage, 0f, player.whoAmI, npcSummon);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int i = tooltips.GetIndex("UseMana");
            tooltips.Insert(i, new TooltipLine(Mod, "CarryingSouls", AequusText.GetText("Tooltips.CarryingSouls", Main.LocalPlayer.Aequus().candleSouls, soulLimit)));
            tooltips.Insert(i, new TooltipLine(Mod, "UseSouls", AequusText.GetText("Tooltips.UseSouls", useSouls)));
        }
    }
}