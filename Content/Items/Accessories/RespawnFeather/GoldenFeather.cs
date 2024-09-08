using Aequus.Common.Items;
using Aequus.Common.Utilities;
using System;
using Terraria.Localization;

namespace Aequus.Content.Items.Accessories.RespawnFeather;

[Gen.AequusPlayer_ResetField<Item>("accGoldenFeather")]
[Gen.AequusPlayer_ResetField<int>("accGoldenFeatherRespawnTimeModifier")]
[Gen.AequusPlayer_ResetField<byte>("accGoldenFeatherTeammate")]
public class GoldenFeather : ModItem {
    public static readonly int RespawnTimeAmount = -300;
    public static readonly int MaxDistance = 2000;
    public static readonly int MinimumRespawnTime = 300;

    protected LocalizedText BaseTooltip => base.Tooltip;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Seconds(-RespawnTimeAmount));

    public virtual int BuffType => ModContent.BuffType<GoldenFeatherBuff>();

    public override void Load() {
        On_Player.GetRespawnTime += On_Player_GetRespawnTime;
    }

    private static int On_Player_GetRespawnTime(On_Player.orig_GetRespawnTime orig, Player player, bool pvp) {
        int time = orig(player, pvp);
        if (time <= MinimumRespawnTime || !player.TryGetModPlayer(out AequusPlayer aequus)) {
            return time;
        }

        return Math.Max(time + aequus.accGoldenFeatherRespawnTimeModifier, MinimumRespawnTime);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = ItemDefaults.NPCSkyMerchant;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accGoldenFeather = Item;
    }

    [Gen.AequusPlayer_PostUpdateEquips]
    public static void UpdateGoldenFeather(Player player, AequusPlayer aequus) {
        if (aequus.accGoldenFeather == null) {
            return;
        }

        for (int i = 0; i < Main.maxPlayers; i++) {
            Player team = Main.player[i];
            if (team.active && team.team == player.team && team.TryGetModPlayer<AequusPlayer>(out var aequusTeam) && player.Distance(team.Center) < MaxDistance) {
                team.AddBuff(aequus.accGoldenFeather.ModItemOrDefault<GoldenFeather>().BuffType, 16, quiet: true);
                aequusTeam.accGoldenFeatherTeammate = (byte)player.whoAmI;
            }
        }
    }
}

public class GoldenFeatherBuff : ModBuff {
    public override LocalizedText DisplayName => ModContent.GetInstance<GoldenFeather>().DisplayName;
    public override LocalizedText Description => ModContent.GetInstance<GoldenFeather>().GetLocalization("BuffDescription").WithFormatArgs(ALanguage.Seconds(-GoldenFeather.RespawnTimeAmount));
    protected LocalizedText BuffedBy { get; private set; } = LocalizedText.Empty;

    public override void SetStaticDefaults() {
        BuffedBy = ALanguage.GetText("Items.CommonTooltips.BuffedBy");
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.persistentBuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        if (!player.TryGetModPlayer<AequusPlayer>(out var aequus)) {
            return;
        }

        aequus.accGoldenFeatherRespawnTimeModifier = Math.Min(aequus.accGoldenFeatherRespawnTimeModifier, GoldenFeather.RespawnTimeAmount);
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
        if (!Main.LocalPlayer.TryGetModPlayer(out AequusPlayer aequus) || Main.myPlayer == aequus.accGoldenFeatherTeammate || !Main.player.IndexInRange(aequus.accGoldenFeatherTeammate)) {
            return;
        }

        string teammateName = Main.player[aequus.accGoldenFeatherTeammate].name;
        if (string.IsNullOrEmpty(teammateName)) {
            return;
        }

        tip += BuffedBy.Format(teammateName);
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}