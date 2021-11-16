using AQMod.NPCs.Boss.Starite;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.NoHitting
{
    public class NoHitManager : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        public bool[] hitPlayer;

        public static INoHitReward[] NoHitEffects { get; private set; }

        internal static void Setup()
        {
            NoHitEffects = new INoHitReward[NPCLoader.NPCCount];

            //NoHitEffects[NPCID.EyeofCthulhu] = new NoHitRewardLoot(ItemID.TwilightDye); testing
            NoHitEffects[ModContent.NPCType<OmegaStarite>()] = new NoHitOmegaStarite(ItemID.TwilightDye);
        }

        internal static void Unload()
        {
            NoHitEffects = null;
        }

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
                    Main.npc[i].GetGlobalNPC<NoHitManager>().hitPlayer[player] = true;
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
            if (npc.life <= 0 && (int)damage < npc.lifeMax && !hitPlayer[Main.myPlayer] &&
                NoHitEffects[npc.type] != null)
            {
                if (NoHitEffects[npc.type].OnEffect(npc, hitDirection, damage, this))
                {
                    var sound = new LegacySoundStyle((int)Terraria.ModLoader.SoundType.Custom, SoundLoader.GetSoundSlot(Terraria.ModLoader.SoundType.Custom, "AQMod/Sounds/Custom/ThisIsNotASecretSound_S3K"));
                    var center = npc.Center;
                    Main.PlaySound(sound.SoundId, (int)center.X, (int)center.Y, sound.Style, 0.3f);
                }
            }
        }

        public override void NPCLoot(NPC npc)
        {
            if (!hitPlayer[Main.myPlayer] && NoHitEffects[npc.type] != null)
            {
                NoHitEffects[npc.type].NPCLoot(npc, this);
            }
        }
    }
}