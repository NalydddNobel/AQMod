using Aequus.Projectiles.Misc.AshGraves;
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

        public bool CustomTombstone(int coinsOwned, NetworkText deathText, int hitDirection)
        {
            var graves = new List<int>();
            if (Player.position.Y > (Main.maxTilesY - 200) * 16f)
            {
                graves.Add(ModContent.ProjectileType<AshTombstoneProj>());
                graves.Add(ModContent.ProjectileType<AshGraveMarkerProj>());
                graves.Add(ModContent.ProjectileType<AshCrossGraveMarkerProj>());
                graves.Add(ModContent.ProjectileType<AshHeadstoneProj>());
                graves.Add(ModContent.ProjectileType<AshGravestoneProj>());
                graves.Add(ModContent.ProjectileType<AshObeliskProj>());
            }
            if (graves.Count > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int t = Main.rand.Next(graves.Count);
                    int p = Projectile.NewProjectile(Player.GetSource_Death(), Player.Center, GetRandomTombstoneVelocity(hitDirection), graves[t], 0, 0f, Main.myPlayer);
                    Main.projectile[p].miscText = deathText.ToString();
                }
                return true;
            }
            return false;
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
    }
}