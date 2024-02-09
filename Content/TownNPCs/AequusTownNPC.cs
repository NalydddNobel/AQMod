using Aequus.Common.NPCs.Components;
using Terraria.GameContent;
using static Terraria.GameContent.Profiles;

namespace Aequus.Content.TownNPCs;

public abstract class AequusTownNPC : ModNPC, IModifyShoppingSettings, ITalkNPCUpdate {
    public bool ShowExclamation { get; protected set; }
    public byte CheckExclamationTimer;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[NPC.type] = 25;
        NPCSets.ExtraFramesCount[NPC.type] = 9;
        NPCSets.AttackFrameCount[NPC.type] = 4;
        NPCSets.DangerDetectRange[NPC.type] = 400;

        NPCSets.ShimmerTownTransform[Type] = true;

        NPCSets.NPCBestiaryDrawOffset[Type] = new() {
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

    protected virtual bool CheckExclamation() {
        return false;
    }

    public override void AI() {
        base.AI();

        if (CheckExclamationTimer > 0) {
            CheckExclamationTimer--;
        }
        else {
            CheckExclamationTimer = byte.MaxValue;
            ShowExclamation = CheckExclamation();
        }

        if (Main.netMode != NetmodeID.Server) {
            ModContent.GetInstance<TownNPCUI>().SetExclamation(NPC.whoAmI, ShowExclamation);
        }
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        CheckExclamationTimer = 0;
    }

    public virtual void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
    }

    public virtual void TalkNPCUpdate(Player player) {
    }
}

public abstract class AequusTownNPC<T> : AequusTownNPC where T : AequusTownNPC<T> {
    public static int ShimmerHeadIndex { get; protected set; }
    public static StackedNPCProfile Profile { get; protected set; }

    protected static string ShimmerTexture => $"{typeof(T).NamespaceFilePath()}/Shimmer/{typeof(T).Name}";

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
