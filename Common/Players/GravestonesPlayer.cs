using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Players
{
    public class GravestonesPlayer : ModPlayer
    {
        public bool disableTombstones;

        public override void Load()
        {
            On.Terraria.Player.DropTombstone += ModifyTombstoneDropped;
        }

        internal static void ModifyTombstoneDropped(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinsOwned, NetworkText deathText, int hitDirection)
        {
            var tombstonesPlayer = self.GetModPlayer<GravestonesPlayer>();
            if (tombstonesPlayer.disableTombstones)
            {
                return;
            }
            if (tombstonesPlayer.CustomTombstone(coinsOwned, deathText, hitDirection))
            {
                return;
            }
            orig(self, coinsOwned, deathText, hitDirection);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["DisableGravestones"] = disableTombstones;
        }

        public override void LoadData(TagCompound tag)
        {
            disableTombstones = tag.GetBool("DisableGravestones");
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

        public bool CustomTombstone(int coinsOwned, NetworkText deathText, int hitDirection)
        {
            List<int> tombstoneChoices = new List<int>();
            //if (hellTombstones || player.position.Y > (Main.maxTilesY - 200) * 16f)
            //{
            //    tombstoneChoices.Add(ModContent.ProjectileType<HellTombstone>());
            //    tombstoneChoices.Add(ModContent.ProjectileType<HellGraveMarker>());
            //    tombstoneChoices.Add(ModContent.ProjectileType<HellCrossGraveMarker>());
            //    tombstoneChoices.Add(ModContent.ProjectileType<HellHeadstone>());
            //    tombstoneChoices.Add(ModContent.ProjectileType<HellGravestone>());
            //    tombstoneChoices.Add(ModContent.ProjectileType<HellObelisk>());
            //}
            if (tombstoneChoices.Count > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int t = Main.rand.Next(tombstoneChoices.Count);
                    int p = Projectile.NewProjectile(Player.GetSource_Death(), Player.Center, GetRandomTombstoneVelocity(hitDirection), tombstoneChoices[t], 0, 0f, Main.myPlayer);
                    Main.projectile[p].miscText = deathText.ToString();
                }
                return true;
            }
            return false;
        }
    }
}