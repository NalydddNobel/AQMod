namespace Aequus.Content.Items.Tools.PocketWormhole;

[Gen.AequusPlayer_ResetField<bool>("infiniteWormhole")]
public class PhaseMirror : ModItem, IPhaseMirror {
    // Note: These detours can be buggy due to code optimization ocassionally inlining them.
    public override void Load() {
        On_Player.HasUnityPotion += Player_HasUnityPotion;
        On_Player.TakeUnityPotion += Player_TakeUnityPotion;
    }

    private static void Player_TakeUnityPotion(On_Player.orig_TakeUnityPotion orig, Player self) {
        if (self.TryGetModPlayer(out AequusPlayer aequus) && aequus.infiniteWormhole) {
            return;
        }

        orig(self);
    }

    private static bool Player_HasUnityPotion(On_Player.orig_HasUnityPotion orig, Player self) {
        if (self.TryGetModPlayer(out AequusPlayer aequus) && aequus.infiniteWormhole) {
            return true;
        }

        return orig(self);
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<global::Aequus.Items.Equipment.PetsUtility.Drone.PersonalDronePack>();
        ItemID.Sets.WorksInVoidBag[Type] = true;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.IceMirror);
        Item.rare = ItemRarityID.Green;
        Item.useTime = 64;
        Item.useAnimation = 64;
    }

    public override void UpdateInfoAccessory(Player player) {
        player.GetModPlayer<AequusPlayer>().infiniteWormhole = true;
    }

    void IPhaseMirror.GetPhaseMirrorDust(Player player, out int dustType, out Color dustColor) {
        dustType = DustID.MagicMirror;
        dustColor = Color.White;
    }

    void IPhaseMirror.Teleport(Player player) {
        player.Spawn(PlayerSpawnContext.RecallFromItem);
    }
}