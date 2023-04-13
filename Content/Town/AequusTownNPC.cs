using Aequus.Common.Personalities;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Profiles;

namespace Aequus.Content.Town {
    public class AequusTownNPC<T> : ModNPC, IModifyShoppingSettings where T : AequusTownNPC<T> {

        public static int ShimmerHeadIndex;
        public static StackedNPCProfile Profile;

        public override void Load() {
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults() {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;

            NPCID.Sets.ShimmerTownTransform[NPC.type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
                Velocity = 1f,
                Direction = -1,
            };
        }

        public override void SetDefaults() {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 50;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.1f;
            AnimationType = NPCID.Guide;
        }

        public virtual void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
        }

        public override ITownNPCProfile TownNPCProfile() {
            return Profile;
        }
    }
}