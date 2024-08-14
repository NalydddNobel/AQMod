using Aequus.NPCs.Town;
using Terraria.GameContent;
using static Terraria.GameContent.Profiles;

namespace Aequus.Common.ContentTemplates;

public abstract class UnifiedTownNPC : AequusTownNPC {
    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 25;
        NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
        NPCID.Sets.AttackFrameCount[NPC.type] = 4;
        NPCID.Sets.DangerDetectRange[NPC.type] = 400;

        NPCID.Sets.ShimmerTownTransform[Type] = true;

        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Velocity = 1f,
            Direction = -1,
            Scale = 1f,
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
}

public abstract class UnifiedTownNPC<T> : UnifiedTownNPC where T : UnifiedTownNPC<T> {
    public static int ShimmerHeadIndex { get; protected set; }
    public static StackedNPCProfile Profile { get; protected set; }

    protected static string ShimmerTexture => $"{Helper.NamespacePath<T>()}/Shimmer/{typeof(T).Name}_Shimmer";

    public override void Load() {
        ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, ShimmerTexture + "_Head");
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Profile = new StackedNPCProfile(
            new DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
            new DefaultNPCProfile(ShimmerTexture, ShimmerHeadIndex)
        );
    }

    public override ITownNPCProfile TownNPCProfile() {
        return Profile;
    }
}
