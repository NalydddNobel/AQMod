using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class SentrySquid : ModItem, ItemHooks.IUpdateItemDye
    {
        public struct TurretStaffUsage
        {
            public static TurretStaffUsage Default => new TurretStaffUsage(isGrounded: false, range: 2000);

            public readonly bool IsGrounded;
            public readonly float Range;
            public readonly bool DoNotUse;
            /// <summary>
            /// Player is the player who is summoning the sentry
            /// <para>Item is the item used to summon the sentry</para>
            /// <para>NPC is the target</para>
            /// <para>.</para>
            /// <para>returns:</para> 
            ///     Whether it was successful at placing a sentry
            /// </summary>
            public Func<Player, Item, NPC, bool> customAction;

            public TurretStaffUsage(bool isGrounded = false, float range = 2000f) : this(null, isGrounded, range)
            {
            }

            public TurretStaffUsage(Func<Player, Item, NPC, bool> customAction, bool isGrounded = false, float range = 2000f)
            {
                IsGrounded = isGrounded;
                Range = range;
                this.customAction = customAction;
                DoNotUse = false;
            }

            public TurretStaffUsage(bool doNotUse)
            {
                IsGrounded = false;
                Range = -1f;
                customAction = null;
                DoNotUse = true;
            }

            private Vector2? FindGroundedSpot(int x, int y)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (Main.tile[x, y + j].HasTile && Main.tile[x, y + j].SolidType())
                    {
                        return new Vector2(x * 16f + 8f, (y + j) * 16f - 32f);
                    }
                }
                return null;
            }
            public bool TrySummoningThisSentry(Player player, Item item, NPC target)
            {
                if (DoNotUse)
                {
                    return false;
                }
                if (customAction?.Invoke(player, item, target) == true)
                {
                    return true;
                }

                var source = (EntitySource_ItemUse_WithAmmo)player.GetSource_ItemUse_WithPotentialAmmo(item, 0, "Aequus:AutoSentry");

                if (IsGrounded)
                {
                    List<Vector2> validLocations = new List<Vector2>();
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
                            d = Math.Min(d + Main.rand.Next(-150, 100), Range);
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
                        AequusPlayer.ShootProj(player, item, source, resultPosition, Vector2.Zero, item.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), resultPosition);
                    }
                }
                else
                {
                    var shootPosition = target.position;
                    shootPosition.X += target.width / 2f;
                    shootPosition.Y -= 200f;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        AequusPlayer.ShootProj(player, item, source, shootPosition, Vector2.Zero, item.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), shootPosition);
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
                [ItemID.DD2BallistraTowerT1Popper] = new TurretStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2BallistraTowerT2Popper] = new TurretStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2BallistraTowerT3Popper] = new TurretStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2ExplosiveTrapT1Popper] = new TurretStaffUsage(isGrounded: true, range: 40f),
                [ItemID.DD2ExplosiveTrapT2Popper] = new TurretStaffUsage(isGrounded: true, range: 75f),
                [ItemID.DD2ExplosiveTrapT3Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2FlameburstTowerT1Popper] = new TurretStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2FlameburstTowerT2Popper] = new TurretStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2FlameburstTowerT3Popper] = new TurretStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2LightningAuraT1Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2LightningAuraT2Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2LightningAuraT3Popper] = new TurretStaffUsage(isGrounded: true, range: 100f),
                [ItemID.QueenSpiderStaff] = new TurretStaffUsage(isGrounded: true, range: 300f),
                [ItemID.StaffoftheFrostHydra] = new TurretStaffUsage(isGrounded: true, range: 1000f),
                [ItemID.RainbowCrystalStaff] = new TurretStaffUsage(isGrounded: false, range: 500f),
                [ItemID.MoonlordTurretStaff] = new TurretStaffUsage(isGrounded: false, range: 1250f),
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
            player.Aequus().turretSquidItem = Item;
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedHat = Type;
                player.Aequus().cHat = dyeItem.dye;
            }
        }
    }
}