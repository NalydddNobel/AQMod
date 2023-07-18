using Aequus.Items.Equipment.Accessories.CrownOfBlood;
using Aequus.Projectiles.Misc.CrownOfBlood;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer : ModPlayer {
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