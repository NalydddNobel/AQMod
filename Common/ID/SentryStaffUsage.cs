using AQMod.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Common.ID
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
                if (Main.tile[x, y + j] == null)
                {
                    Main.tile[x, y + j] = new Tile();
                    return null;
                }
                if (Main.tile[x, y + j].active() && Main.tile[x, y + j].Solid())
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
                int shoot = AQUtils.ShootProj(player, item, resultPosition, Vector2.Zero, item.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), resultPosition);
                return shoot != -1;
            }
            else
            {
                var shootPosition = target.position;
                shootPosition.X += target.width / 2f;
                shootPosition.Y -= 200f;
                int shoot = AQUtils.ShootProj(player, item, shootPosition, Vector2.Zero, item.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item, item.knockBack), shootPosition);
                return shoot != -1;
            }
        }
    }
}