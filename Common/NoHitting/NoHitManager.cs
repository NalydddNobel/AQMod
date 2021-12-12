using AQMod.NPCs.Boss.Starite;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common.NoHitting
{
    public class NoHitManager : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool[] hitPlayer;
        public bool dontHitPlayers;
        public byte rewardOption;

        public NoHitManager()
        {
            hitPlayer = new bool[Main.maxPlayers];
        }

        public override GlobalNPC NewInstance(NPC npc)
        {
            return new NoHitManager();
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            CollapseNoHit(target.whoAmI);
        }

        private void CollapseNoHit(int player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                {
                    var manager = Main.npc[i].GetGlobalNPC<NoHitManager>();
                    if (!manager.dontHitPlayers)
                    {
                        manager.hitPlayer[player] = true;
                    }
                }
            }
        }

        private void ResetNoHit(int player)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active)
                    Main.npc[i].GetGlobalNPC<NoHitManager>().hitPlayer[player] = false;
            }
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (npc.life <= 0 && (int)damage < npc.lifeMax)
            {
                switch (npc.type)
                {
                    case NPCID.CultistBoss:
                    {
                        if (HasBeenNoHit(npc, this, Main.myPlayer))
                        {
                            PlayNoHitJingle(npc.Center);
                        }
                    }
                    break;
                }
            }
        }

        public static bool HasBeenNoHit(NPC npc, int player)
        {
            return HasBeenNoHit(npc, npc.GetGlobalNPC<NoHitManager>(), player);
        }

        public static bool HasBeenNoHit(NPC npc, NoHitManager noHitManager, int player)
        {
            return npc.playerInteraction[player] && !noHitManager.hitPlayer[player];
        }

        public static void PlayNoHitJingle(Vector2 position)
        {
            if (Vector2.Distance(position, Main.player[Main.myPlayer].Center) < 3000f)
                AQSoundPlayer.PlaySound(SoundType.NPCHit, "Sounds/NPCHit/ThisIsNotASecretSound_S3K", position);
        }

        public override void AI(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.CultistBoss:
                {
                    if (rewardOption != 1 && Main.eclipse && Main.dayTime)
                    {
                        rewardOption = 2;
                        if ((int)npc.ai[0] != 5f)
                        {
                            int neededMothronCount = 0;
                            if (npc.life * 2 < npc.lifeMax)
                                neededMothronCount++;
                            if (npc.life * 4 < npc.lifeMax)
                                neededMothronCount++;
                            neededMothronCount += NPC.CountNPCS(NPCID.CultistBossClone);
                            if (neededMothronCount > 0)
                            {
                                int mothronCount = NPC.CountNPCS(NPCID.Mothron);
                                int x = 100 * neededMothronCount / 2;
                                for (int i = mothronCount; i < neededMothronCount; i++)
                                {
                                    int spawnX = (int)npc.position.X + npc.width / 2 + x - 100 * i;
                                    int spawnY = (int)npc.position.Y + 1250;
                                    NPC.NewNPC(spawnX, spawnY, NPCID.Mothron);
                                }
                            }
                        }
                    }
                    else
                    {
                        rewardOption = 1;
                    }
                }
                break;
            }
        }

        public override void NPCLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.CultistBoss:
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (HasBeenNoHit(npc, this, i))
                        {
                            if (rewardOption == 2)
                            {
                                WorldDefeats.ObtainedMothmanMask = true;
                                AQItem.DropInstancedItem(i, npc.getRect(), ModContent.ItemType<Items.Dedicated.ContentCreators.MothmanMask>());
                            }
                            else
                            {
                                WorldDefeats.ObtainedCatalystPainting = true;
                                AQItem.DropInstancedItem(i, npc.getRect(), ModContent.ItemType<Items.Placeable.Furniture.RockFromAnAlternateUniverse>());
                            }
                        }
                    }
                }
                break;
            }
        }
    }
}