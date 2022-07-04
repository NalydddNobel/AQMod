using Aequus.Common.Networking;
using Aequus.Content.Necromancy;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class GhostSpawner : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 2;
            Projectile.hide = true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int x = (int)Projectile.position.X + Projectile.width / 2;
                int y = (int)Projectile.position.Y + Projectile.height / 2;
                int type = DetermineNPCType((int)Projectile.ai[0]);
                int n = NPC.NewNPC(Projectile.GetSource_Death("Aequus:NecromancySpawn"), x, y, type);
                OnSpawnZombie(Main.npc[n], Main.npc[n].GetGlobalNPC<NecromancyNPC>());
                PacketHandler.SyncNPC(Main.npc[n]);
            }
        }

        protected virtual void OnSpawnZombie(NPC npc, NecromancyNPC zombie)
        {
            zombie.SpawnZombie_SetZombieStats(npc, Projectile.Center, Projectile.velocity, 0, 0, out bool _);
            zombie.zombieTimerMax *= 5;
            zombie.zombieTimer *= 5;
            zombie.renderLayer = GhostOutlineRenderer.IDs.BloodRed;
        }

        public int DetermineNPCType(int type)
        {
            if (type == NPCID.PigronCorruption)
            {
                type = Main.rand.NextFromList(NPCID.PigronCorruption, NPCID.PigronCrimson, NPCID.PigronHallow);
            }
            else if (type == NPCID.Zombie)
            {
                var list = AequusHelpers.AllWhichShareBanner(NPCID.Zombie, vanillaOnly: true);
                list.Remove(NPCID.MaggotZombie);
                if (!Main.halloween && !Main.pumpkinMoon)
                {
                    list.Remove(NPCID.ZombieDoctor);
                    list.Remove(NPCID.ZombiePixie);
                    list.Remove(NPCID.ZombieSuperman);
                }
                if (!Main.xMas && !Main.snowMoon)
                {
                    list.Remove(NPCID.ZombieSweater);
                    list.Remove(NPCID.ZombieXmas);
                }
                type = Main.rand.NextFromCollection(list);
            }
            else if (type == NPCID.Skeleton)
            {
                var list = AequusHelpers.AllWhichShareBanner(NPCID.Skeleton, vanillaOnly: true);
                if (!Main.halloween && !Main.pumpkinMoon)
                {
                    list.Remove(NPCID.SkeletonAlien);
                    list.Remove(NPCID.SkeletonAstonaut);
                    list.Remove(NPCID.SkeletonTopHat);
                }
                if (!Main.expertMode)
                {
                    list.Remove(NPCID.BoneThrowingSkeleton);
                    list.Remove(NPCID.BoneThrowingSkeleton2);
                    list.Remove(NPCID.BoneThrowingSkeleton3);
                    list.Remove(NPCID.BoneThrowingSkeleton4);
                }
                type = Main.rand.NextFromCollection(list);
            }
            else if (type == NPCID.AngryBones || type == NPCID.BlueArmoredBones || type == NPCID.HellArmoredBones || type == NPCID.RustyArmoredBonesAxe)
            {
                var list = AequusHelpers.AllWhichShareBanner(type, vanillaOnly: true);
                type = Main.rand.NextFromCollection(list);
            }
            return type;
        }
    }
}