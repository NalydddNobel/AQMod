using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class SentrySquid : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SentrySquidPlayer>().autoSentry = true;
        }
    }

    public class SentrySquidPlayer : ModPlayer
    {
        public bool autoSentry;
        public ushort autoSentryCooldown;

        public override void Initialize()
        {
            autoSentryCooldown = 120;
        }

        public override void UpdateDead()
        {
            autoSentry = false;
            autoSentryCooldown = 120;
        }

        public override void ResetEffects()
        {
            autoSentry = false;
            if (!Player.Aequus().InDanger)
            {
                autoSentryCooldown = Math.Min(autoSentryCooldown, (ushort)240);
            }
            AequusHelpers.TickDown(ref autoSentryCooldown);
        }

        public override void PostUpdate()
        {
            if (autoSentry && autoSentryCooldown == 0)
            {
                UpdateAutoSentry(Player.Aequus().closestEnemy);
            }
        }

        /// <summary>
        /// Attempts to place a sentry down near the <see cref="NPC"/> at <see cref="closestEnemy"/>'s index. Doesn't do anything if the index is -1, the enemy is not active, or the player has no turret slots. Runs after <see cref="CheckDanger"/>
        /// </summary>
        public void UpdateAutoSentry(int closestEnemy)
        {
            if (closestEnemy == -1 || !Main.npc[closestEnemy].active || Player.maxTurrets <= 0)
            {
                autoSentryCooldown = 30;
                return;
            }

            var item = AutoSentry_GetUsableSentryStaff();
            if (item == null)
            {
                autoSentryCooldown = 30;
                return;
            }

            if (Player.Aequus().turretSlotCount >= Player.maxTurrets)
            {
                int oldestSentry = -1;
                int time = int.MaxValue;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Player.whoAmI && Main.projectile[i].WipableTurret)
                    {
                        if (Main.projectile[i].timeLeft < time)
                        {
                            oldestSentry = i;
                            time = Main.projectile[i].timeLeft;
                        }
                    }
                }
                if (oldestSentry != -1)
                {
                    Main.projectile[oldestSentry].timeLeft = Math.Min(Main.projectile[oldestSentry].timeLeft, 30);
                }
                autoSentryCooldown = 30;
                return;
            }

            if (!ItemsCatalogue.SentryUsage.TryGetValue(item.type, out var sentryUsage))
            {
                sentryUsage = ItemsCatalogue.SentryStaffUsage.Default;
            }
            if (sentryUsage.TrySummoningThisSentry(Player, item, Main.npc[closestEnemy]))
            {
                Player.UpdateMaxTurrets();
                if (Player.maxTurrets > 1)
                {
                    autoSentryCooldown = 240;
                }
                else
                {
                    autoSentryCooldown = 3000;
                }
                if (Main.netMode != NetmodeID.Server && item.UseSound != null)
                {
                    SoundEngine.PlaySound(item.UseSound.Value, Main.npc[closestEnemy].Center);
                }
            }
            else
            {
                autoSentryCooldown = 30;
            }
        }

        /// <summary>
        /// Determines an item to use as a Sentry Staff for <see cref="UpdateAutoSentry"/>
        /// </summary>
        /// <returns></returns>
        public Item AutoSentry_GetUsableSentryStaff()
        {
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                // A very small check which doesn't care about checking damage and such, so this could be easily manipulated.
                if (!Player.inventory[i].IsAir && Player.inventory[i].sentry && Player.inventory[i].shoot > ProjectileID.None && (!Player.inventory[i].DD2Summon || !DD2Event.Ongoing)
                    && ItemLoader.CanUseItem(Player.inventory[i], Player))
                {
                    return Player.inventory[i];
                }
            }
            return null;
        }
    }
}