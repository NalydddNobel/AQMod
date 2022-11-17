using Aequus.Content.Necromancy;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Necro
{
    [Obsolete("Soul gem weapon experiment ended, souls gems were cut.")]
    public abstract class SoulCandleBase : LegacySoulGemWeaponBase
    {
        public const int ItemHoldStyle = ItemHoldStyleID.HoldFront;

        public float OriginalNPCSpeed { get; protected set; }
        public int NPCToSummon { get; protected set; }

        public float npcSpeed;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        protected void DefaultToCandle(int summonDamage, int ammoTier, int npc, float speed = 0f)
        {
            Item.holdStyle = ItemHoldStyle;
            Item.DamageType = NecromancyDamageClass.Instance; // Invisible damage type which should hopefully trick the game into believing it's some sort of summoner related item

            OriginalTier = ammoTier;
            tier = ammoTier;
            OriginalNPCSpeed = speed;

            npcSpeed = speed;

            NPCToSummon = npc;
            Item.damage = summonDamage;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.mana = 20;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.SummonWeapon;
        }

        public override bool? UseItem(Player player)
        {
            var aequus = player.Aequus();
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
            SpawnGhost(player, 0);
            return true;
        }

        public virtual void SpawnGhost(Player player, int soulGemDamage)
        {
            var position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectileDirect(player.GetSource_ItemUse_WithPotentialAmmo(Item, 0), position, Vector2.Zero, ModContent.ProjectileType<GhostSpawner>(), Item.damage + soulGemDamage, 0f, player.whoAmI, NPCToSummon, npcSpeed);
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
            base.ModifyTooltips(tooltips);
        }
    }
}