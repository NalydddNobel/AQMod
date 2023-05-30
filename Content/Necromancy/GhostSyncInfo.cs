using Terraria;

namespace Aequus.Content.Necromancy {
    public struct GhostSyncInfo
    {
        public bool IsZombie;
        public int Player;
        public int TimeLeft;
        public int TimeLeftMax;
        public int RenderLayerID;
        public int Slots;
        public float Tier;

        public Entity BaseEntity;

        public GhostSyncInfo(Entity baseEntity, bool isZombie)
        {
            BaseEntity = baseEntity;
            IsZombie = isZombie;
            Player = Main.maxPlayers;
            TimeLeft = 2;
            TimeLeftMax = 2;
            RenderLayerID = 0;
            Slots = 0;
            Tier = 0f;
        }

        public static GhostSyncInfo GetInfo(NPC npc)
        {
            var info = new GhostSyncInfo(npc, npc.GetGlobalNPC<NecromancyNPC>().isZombie);
            if (info.IsZombie)
            {
                info.GetZombieNPCInfo(npc, npc.GetGlobalNPC<NecromancyNPC>());
            }
            return info;
        }

        public void GetZombieNPCInfo(NPC npc, NecromancyNPC n)
        {
            Player = n.zombieOwner;
            TimeLeft = n.zombieTimer;
            TimeLeftMax = n.zombieTimerMax;
            RenderLayerID = n.renderLayer;
            Slots = n.slotsConsumed;
            Tier = n.zombieDebuffTier;
        }

        public void SetZombieNPCInfo(NPC npc, NecromancyNPC n)
        {
            if (!IsZombie)
                return;

            npc.boss = false;
            npc.friendly = true;
            npc.SpawnedFromStatue = true;
            npc.extraValue = 0;
            npc.value = 0;
            n.isZombie = true;
            n.zombieOwner = Player;
            n.zombieTimer = TimeLeft;
            n.zombieTimerMax = TimeLeftMax;
            n.renderLayer = RenderLayerID;
            n.slotsConsumed = Slots;
            n.zombieDebuffTier = Tier;
        }
    }
}