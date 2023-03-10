using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Sentry
{
    public class SentrySquid : ModItem, ItemHooks.IUpdateItemDye
    {
        public struct TurretStaffUsage
        {
            public static TurretStaffUsage Default => new TurretStaffUsage(isGrounded: false, range: 500f);

            public readonly bool IsGrounded;
            public readonly float Range;
            /// <summary>
            /// Player is the player who is summoning the sentry
            /// <para>Item is the item used to summon the sentry</para>
            /// <para>NPC is the target</para>
            /// <para>.</para>
            /// <para>returns:</para> 
            ///     Whether it was successful at placing a sentry
            /// </summary>
            public Func<Player, Item, NPC, bool> customAction;

            public TurretStaffUsage(bool isGrounded = false, float range = 800f) : this(null, isGrounded, range)
            {
            }

            public TurretStaffUsage(Func<Player, Item, NPC, bool> customAction, bool isGrounded = false, float range = 2000f)
            {
                IsGrounded = isGrounded;
                Range = range;
                this.customAction = customAction;
            }

            public TurretStaffUsage(bool doNotUse)
            {
                IsGrounded = false;
                Range = -1f;
                customAction = null;
            }

            private static Vector2? FindGroundedSpot(int x, int y)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (Main.tile[x, y + j].HasTile && Main.tile[x, y + j].SolidType())
                    {
                        return new Vector2(x * 16f + 8f, (y + j) * 16f - 64f);
                    }
                }
                return null;
            }
            public bool TrySummoningThisSentry(Player player, Item item, NPC target)
            {
                if (customAction?.Invoke(player, item, target) == true)
                {
                    return true;
                }

                var source = (EntitySource_ItemUse_WithAmmo)player.GetSource_ItemUse_WithPotentialAmmo(item, 0, "Aequus:AutoSentry");

                if (IsGrounded)
                {
                    var validLocations = new List<Vector2>();
                    int x = ((int)target.position.X + target.width / 2) / 16;
                    int y = (int)target.position.Y / 16;
                    for (int i = -5; i <= 5; i++)
                    {
                        var v = FindGroundedSpot(x, y);
                        if (v.HasValue)
                        {
                            validLocations.Add(v.Value);
                        }
                    }
                    if (validLocations.Count == 0)
                    {
                        return false;
                    }
                    float closest = Range;
                    var resultPosition = Vector2.Zero;
                    var targetCenter = target.Center;
                    foreach (var v in validLocations)
                    {
                        float d = target.Distance(v);
                        if (!Collision.CanHitLine(v, 2, 2, target.position, target.width, target.height))
                        {
                            d *= 2f;
                        }
                        if (d < Range)
                        {
                            d = Math.Min(d + Main.rand.Next(-(int)Range / 3, (int)Range / 3), Range);
                        }
                        if (d < closest)
                        {
                            resultPosition = v;
                            closest = d;
                        }
                    }
                    if (resultPosition == Vector2.Zero)
                    {
                        return false;
                    }
                    if (Main.myPlayer == player.whoAmI)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            var shootPosition = resultPosition;
                            shootPosition.X += target.width / 2f;
                            shootPosition.Y += Main.rand.NextFloat(-64f, 120f);
                            if (Main.myPlayer == player.whoAmI && Collision.CanHitLine(shootPosition, 2, 2, target.position, target.width, target.height))
                            {
                                AequusPlayer.ShootProj(player, item, source, shootPosition, Vector2.Zero, item.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), shootPosition);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            var shootPosition = target.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(Range);
                            if (Collision.CanHitLine(shootPosition, 2, 2, target.position, target.width, target.height))
                            {
                                AequusPlayer.ShootProj(player, item, source, shootPosition, Vector2.Zero, item.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), shootPosition);
                                break;
                            }
                        }
                    }
                }
                return true;
            }
        }

        public static Dictionary<int, TurretStaffUsage> TurretStaffs { get; private set; }

        public override void Load()
        {
            TurretStaffs = new Dictionary<int, TurretStaffUsage>()
            {
                [ItemID.HoundiusShootius] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2BallistraTowerT1Popper] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2BallistraTowerT2Popper] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2BallistraTowerT3Popper] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2ExplosiveTrapT1Popper] = new TurretStaffUsage(isGrounded: true, range: 40f),
                [ItemID.DD2ExplosiveTrapT2Popper] = new TurretStaffUsage(isGrounded: true, range: 40f),
                [ItemID.DD2ExplosiveTrapT3Popper] = new TurretStaffUsage(isGrounded: true, range: 40f),
                [ItemID.DD2FlameburstTowerT1Popper] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2FlameburstTowerT2Popper] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2FlameburstTowerT3Popper] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.DD2LightningAuraT1Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2LightningAuraT2Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2LightningAuraT3Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.QueenSpiderStaff] = new TurretStaffUsage(isGrounded: true, range: 300f),
                [ItemID.StaffoftheFrostHydra] = new TurretStaffUsage(isGrounded: true, range: 900f),
                [ItemID.RainbowCrystalStaff] = new TurretStaffUsage(isGrounded: false, range: 400f),
                [ItemID.MoonlordTurretStaff] = new TurretStaffUsage(isGrounded: false, range: 900f),
            };
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accSentrySquid = Item;
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedHat = Type;
                player.Aequus().cHat = dyeItem.dye;
            }
        }

        public static void UseEquip(Player player, AequusPlayer aequus)
        {
            if (aequus.closestEnemy == -1 || !Main.npc[aequus.closestEnemy].active || player.maxTurrets <= 0)
            {
                aequus.sentrySquidTimer = 30;
                return;
            }

            var item = FindUsableStaff(player);
            if (item == null)
            {
                aequus.sentrySquidTimer = 30;
                return;
            }

            if (aequus.turretSlotCount >= player.maxTurrets)
            {
                int oldestSentry = -1;
                int time = int.MaxValue;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].WipableTurret)
                    {
                        if (Collision.CanHitLine(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, Main.npc[aequus.closestEnemy].position, Main.npc[aequus.closestEnemy].width, Main.npc[aequus.closestEnemy].height))
                        {
                            continue;
                        }
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
                aequus.sentrySquidTimer = 10;
                return;
            }

            if (!TurretStaffs.TryGetValue(item.type, out var sentryUsage))
            {
                sentryUsage = TurretStaffUsage.Default;
            }
            if (sentryUsage.TrySummoningThisSentry(player, item, Main.npc[aequus.closestEnemy]))
            {
                player.UpdateMaxTurrets();
                if (player.maxTurrets > 1)
                {
                    aequus.sentrySquidTimer = player.Aequus().turretSlotCount >= player.maxTurrets - 1 ? 180 : 60;
                }
                else
                {
                    aequus.sentrySquidTimer = 3000;
                }
                if (Main.netMode != NetmodeID.Server && item.UseSound != null)
                {
                    SoundEngine.PlaySound(item.UseSound.Value, Main.npc[aequus.closestEnemy].Center);
                }
            }
            else
            {
                aequus.sentrySquidTimer = 30;
            }
        }

        public static Item FindUsableStaff(Player player)
        {
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                if (!player.inventory[i].IsAir && player.inventory[i].sentry && player.inventory[i].damage > 0 && player.inventory[i].shoot > ProjectileID.None)
                {
                    if (player.inventory[i].DD2Summon && (DD2Event.Ongoing || !player.downedDD2EventAnyDifficulty))
                    {
                        continue;
                    }
                    if (ItemLoader.CanUseItem(player.inventory[i], player))
                        return player.inventory[i];
                }
            }
            return null;
        }
    }
}