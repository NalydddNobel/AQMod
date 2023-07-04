using Aequus.Common.Net;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Projectiles.Misc.CrownOfBlood;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood {
    public partial class CrownOfBloodItem {
        private void Load_ExpertEffects() {
            On_Player.SporeSac += Hook_NaniteBombs;
            On_Player.beeType += On_Player_beeType;
            On_Player.beeDamage += On_Player_beeDamage;
        }

        private int On_Player_beeDamage(On_Player.orig_beeDamage orig, Player self, int dmg) {
            if (self.TryGetModPlayer<AequusPlayer>(out var aequus) && aequus.crownOfBloodBees > 0) {
                return dmg + Main.rand.Next(8 * aequus.crownOfBloodBees, 12 * aequus.crownOfBloodBees);
            }
            return orig(self, dmg);
        }

        private int On_Player_beeType(On_Player.orig_beeType orig, Player self) {
            if (self.TryGetModPlayer<AequusPlayer>(out var aequus) && aequus.crownOfBloodBees > 0) {
                if (aequus.crownOfBloodCD <= 0) {
                    aequus.crownOfBloodCD = 30;
                    return ModContent.ProjectileType<HivePackMinion>();
                }
                return ProjectileID.GiantBee;
            }
            return orig(self);
        }

        private static void Hook_NaniteBombs(On_Player.orig_SporeSac orig, Player self, Item sourceItem) {
            if (sourceItem?.GetEquipEmpowerment()?.HasAbilityBoost == true) {
                SpawnNaniteBombs(self, sourceItem);
                return;
            }

            orig(self, sourceItem);
        }
        public static void SpawnNaniteBombs(Player player, Item sourceItem) {
            int damage = 70;
            float knockBack = 1.5f;
            if (!Main.rand.NextBool(15)) {
                return;
            }
            int num = 0;
            for (int i = 0; i < 1000; i++) {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<NaniteSpore>()) {
                    num++;
                }
            }
            if (Main.rand.Next(15) < num || num >= 10) {
                return;
            }
            int num2 = 50;
            int num3 = 24;
            int num4 = 90;
            int num5 = 0;
            Vector2 center;
            while (true) {
                if (num5 >= num2) {
                    return;
                }
                int num6 = Main.rand.Next(200 - num5 * 2, 400 + num5 * 2);
                center = player.Center;
                center.X += Main.rand.Next(-num6, num6 + 1);
                center.Y += Main.rand.Next(-num6, num6 + 1);
                if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3)) {
                    center.X += num3 / 2;
                    center.Y += num3 / 2;
                    if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1)) {
                        int num7 = (int)center.X / 16;
                        int num8 = (int)center.Y / 16;
                        bool flag = false;
                        if (Main.rand.NextBool(3) && Main.tile[num7, num8] != null && Main.tile[num7, num8].WallType > 0) {
                            flag = true;
                        }
                        else {
                            center.X -= num4 / 2;
                            center.Y -= num4 / 2;
                            if (Collision.SolidCollision(center, num4, num4)) {
                                center.X += num4 / 2;
                                center.Y += num4 / 2;
                                flag = true;
                            }
                            else if (Main.tile[num7, num8] != null && Main.tile[num7, num8].HasTile && Main.tile[num7, num8].TileType == 19) {
                                flag = true;
                            }
                        }
                        if (flag) {
                            for (int j = 0; j < 1000; j++) {
                                if (Main.projectile[j].active && Main.projectile[j].owner == player.whoAmI && Main.projectile[j].aiStyle == 105) {
                                    var val = center - Main.projectile[j].Center;
                                    if (val.Length() < 48f) {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            if (flag && Main.myPlayer == player.whoAmI) {
                                break;
                            }
                        }
                    }
                }
                num5++;
            }
            Projectile.NewProjectile(player.GetSource_Accessory(sourceItem), center.X, center.Y, 0f, 0f, ModContent.ProjectileType<NaniteSpore>(), damage, knockBack, player.whoAmI);
        }
    }

    public class WormScarfDodgePacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.WormScarfDodge;

        public void Send(Player player) {
            var p = GetPacket();
            p.Write((byte)player.whoAmI);
            p.Send();
        }

        public override void Receive(BinaryReader reader, int sender) {
            byte plr = reader.ReadByte();
            Main.player[plr].Aequus().ProcWormScarfDodge();
            if (Main.netMode == NetmodeID.Server) {
                Send(Main.player[plr]);
            }
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public int crownOfBloodWormScarfTarget;
        public int crownOfBloodWormScarfBulletCD;
        public int crownOfBloodBees;
        public int crownOfBloodDeerclops;
        public int crownOfBloodFriendlySlimes;

        public int crownOfBloodCD;

        private void PostUpdateEquips_WormScarfEmpowerment() {
            if (crownOfBloodWormScarfBulletCD > 0) {
                crownOfBloodWormScarfBulletCD--;
                return;
            }
            if (crownOfBloodWormScarfTarget <= -1 || accWormScarf == null || accWormScarf.GetEquipEmpowerment()?.HasAbilityBoost != true || Main.myPlayer != Player.whoAmI) {
                crownOfBloodWormScarfTarget = -1;
                return;
            }

            for (int i = crownOfBloodWormScarfTarget; i < Main.maxNPCs; i += 50) {
                var npc = Main.npc[i];
                if (npc.CanBeChasedBy(Player) && Player.Distance(npc.Center) < 800f) {
                    Projectile.NewProjectile(
                        Player.GetSource_Accessory(accWormScarf),
                        Player.Center,
                        Main.rand.NextVector2Unit() * 8f,
                        ModContent.ProjectileType<WormScarfLaser>(),
                        50,
                        1f,
                        Player.whoAmI,
                        ai0: i + 1
                    );
                    crownOfBloodWormScarfBulletCD = 2;
                }
            }

            crownOfBloodWormScarfTarget++;
            if (crownOfBloodWormScarfTarget >= 50) {
                crownOfBloodWormScarfTarget = -1;
                return;
            }
        }

        private void PostUpdateEquips_BoneHelmEmpowerment() {
            if (crownOfBloodDeerclops <= 0 || crownOfBloodCD > 0 || closestEnemy == -1 || Main.myPlayer != Player.whoAmI || accBoneHelm.GetEquipEmpowerment()?.HasAbilityBoost != true) {
                return;
            }

            if (!Main.npc[closestEnemy].CanBeChasedBy(Player) || Player.Distance(Main.npc[closestEnemy]) > 500f || !Player.CanHitLine(Main.npc[closestEnemy])) {
                return;
            }

            crownOfBloodCD = 1200;
            var spawnPosition = Main.npc[closestEnemy].Center + Main.rand.NextVector2Unit() * 750f;
            Projectile.NewProjectile(
                Player.GetSource_Accessory(accBoneHelm),
                spawnPosition,
                Vector2.Normalize(Main.npc[closestEnemy].Center - spawnPosition) * 4f,
                ModContent.ProjectileType<BoneHelmMinion>(),
                0,
                0f,
                Player.whoAmI
            );
        }

        private void PostUpdateEquips_RoyalGels() {
            if (crownOfBloodFriendlySlimes <= 0 || crownOfBloodCD > 0 || closestEnemy == -1 || Main.myPlayer != Player.whoAmI) {
                return;
            }

            int slime = -1;
            float minDistance = 300f;
            for (int i = 0; i < Main.maxNPCs; i++) {
                var npc = Main.npc[i];
                if (!npc.active || npc.friendly || !(npc.aiStyle == NPCAIStyleID.Slime || NPCID.Sets.CanConvertIntoCopperSlimeTownNPC[npc.type])) {
                    continue;
                }
                float distance = Player.Distance(npc);
                if (distance < minDistance) {
                    slime = i;
                    minDistance = distance;
                }
            }

            if (slime == -1) {
                return;
            }

            var slimeNPC = Main.npc[slime];
            var aequusNPC = slimeNPC.Aequus();
            aequusNPC.zombieInfo.IsZombie = true;
            aequusNPC.zombieInfo.PlayerOwner = Player.whoAmI;
            aequusNPC.zombieInfo.SetDamage = 50;
            aequusNPC.zombieInfo.DrawEffectID = 0;
        }

        public void ProcWormScarfDodge() {
            Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);
            crownOfBloodWormScarfTarget = 0;
            if (Player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<WormScarfDodgePacket>().Send(Player);
            }
        }

        private bool TryWormScarfDodge() {
            if (accWormScarf?.GetEquipEmpowerment()?.HasAbilityBoost != true) {
                return false;
            }

            crownOfBloodCD = 1200;
            ProcWormScarfDodge();
            return true;
        }

        private bool TryBoCDodge() {
            if (Player.brainOfConfusionItem?.GetEquipEmpowerment()?.HasAbilityBoost != true) {
                return false;
            }

            crownOfBloodCD = 1200;
            Player.BrainOfConfusionDodge();
            return true;
        }
    }
}