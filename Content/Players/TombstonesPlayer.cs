using AQMod.Projectiles.Tombstones.Hell;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.Players
{
    public sealed class TombstonesPlayer : ModPlayer
    {
        public static class Hooks
        {
            internal static void Player_DropTombstone(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, Terraria.Localization.NetworkText deathText, int hitDirection)
            {
                var tombstonesPlayer = self.GetModPlayer<TombstonesPlayer>();
                if (tombstonesPlayer.disableTombstones)
                {
                    return;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && tombstonesPlayer.CreateTombstone(coinsOwned, deathText, hitDirection))
                {
                    orig(self, coinsOwned, deathText, hitDirection);
                }
            }
        }

        public bool disableTombstones;
        public bool hellTombstones;

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["disableTombstones"] = disableTombstones,
            };
        }

        public override void Load(TagCompound tag)
        {
            disableTombstones = tag.GetBool("disableTombstones");
        }

        public Vector2 GetRandomTombstoneVelocity(int hitDirection)
        {
            float num;
            for (num = Main.rand.Next(-35, 36) * 0.1f; num < 2f && num > -2f; num += Main.rand.Next(-30, 31) * 0.1f)
            {
            }
            return new Vector2(Main.rand.Next(10, 30) * 0.1f * hitDirection + num,
                Main.rand.Next(-40, -20) * 0.1f);
        }

        public bool CreateTombstone(int coinsOwned, Terraria.Localization.NetworkText deathText, int hitDirection)
        {
            List<int> tombstoneChoices = new List<int>();
            if (hellTombstones || player.position.Y > (Main.maxTilesY - 200) * 16f)
            {
                tombstoneChoices.Add(ModContent.ProjectileType<HellTombstone>());
                tombstoneChoices.Add(ModContent.ProjectileType<HellGraveMarker>());
                tombstoneChoices.Add(ModContent.ProjectileType<HellCrossGraveMarker>());
                tombstoneChoices.Add(ModContent.ProjectileType<HellHeadstone>());
                tombstoneChoices.Add(ModContent.ProjectileType<HellGravestone>());
                tombstoneChoices.Add(ModContent.ProjectileType<HellObelisk>());
            }
            if (tombstoneChoices.Count > 0)
            {
                int t = Main.rand.Next(tombstoneChoices.Count);
                int p = Projectile.NewProjectile(player.Center, GetRandomTombstoneVelocity(hitDirection), tombstoneChoices[t], 0, 0f, Main.myPlayer);
                Main.projectile[p].miscText = deathText.ToString();
                return false;
            }
            return true;
        }
    }
}