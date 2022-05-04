using Aequus.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public sealed class ItemsCatalogue : EmptyModType
    {
        public struct SentryStaffUsage
        {
            public static SentryStaffUsage Default => new SentryStaffUsage(isGrounded: false, range: 2000);

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

            public SentryStaffUsage(bool isGrounded = false, float range = 2000f) : this(null, isGrounded, range)
            {
            }

            public SentryStaffUsage(Func<Player, Item, NPC, bool> customAction, bool isGrounded = false, float range = 2000f)
            {
                IsGrounded = isGrounded;
                Range = range;
                this.customAction = customAction;
                DoNotUse = false;
            }

            public SentryStaffUsage(bool doNotUse)
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
                    if (Main.tile[x, y + j].HasTile && Main.tile[x, y + j].Solid())
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

        public static HashSet<int> SummonStaff { get; private set; }
        public static Dictionary<int, SentryStaffUsage> SentryUsage { get; private set; }

        public override void Load()
        {
            SummonStaff = new HashSet<int>();
            SentryUsage = new Dictionary<int, SentryStaffUsage>()
            {
                [ItemID.DD2BallistraTowerT1Popper] = new SentryStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2BallistraTowerT2Popper] = new SentryStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2BallistraTowerT3Popper] = new SentryStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2ExplosiveTrapT1Popper] = new SentryStaffUsage(isGrounded: true, range: 40f),
                [ItemID.DD2ExplosiveTrapT2Popper] = new SentryStaffUsage(isGrounded: true, range: 75f),
                [ItemID.DD2ExplosiveTrapT3Popper] = new SentryStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2FlameburstTowerT1Popper] = new SentryStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2FlameburstTowerT2Popper] = new SentryStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2FlameburstTowerT3Popper] = new SentryStaffUsage(isGrounded: true, range: 600f),
                [ItemID.DD2LightningAuraT1Popper] = new SentryStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2LightningAuraT2Popper] = new SentryStaffUsage(isGrounded: true, range: 100f),
                [ItemID.DD2LightningAuraT3Popper] = new SentryStaffUsage(isGrounded: true, range: 100f),
                [ItemID.QueenSpiderStaff] = new SentryStaffUsage(isGrounded: true, range: 300f),
                [ItemID.StaffoftheFrostHydra] = new SentryStaffUsage(isGrounded: true, range: 1000f),
                [ItemID.RainbowCrystalStaff] = new SentryStaffUsage(isGrounded: false, range: 500f),
                [ItemID.MoonlordTurretStaff] = new SentryStaffUsage(isGrounded: false, range: 1250f),
            };
        }

        public static void LoadAutomaticEntries()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = ContentSamples.ItemsByType[i];
                if (IsSummonStaff(item))
                {
                    SummonStaff.Add(i);
                }
            }
        }
        private static bool IsSummonStaff(Item item)
        {
            return item.damage > 0 && item.DamageType == DamageClass.Summon && item.shoot > ProjectileID.None && item.useStyle > 0 && (ContentSamples.ProjectilesByType[item.shoot].minionSlots > 0f || ContentSamples.ProjectilesByType[item.shoot].sentry);
        }
    }
}