using Aequus.Common.Structures;
using Aequus.Common.Utilities;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Aequus.Content.Items.Accessories.DamageOnKillClaw;

[Gen.AequusPlayer_ResetField<int>("accDamageOnKill")]
[Gen.AequusPlayer_Field<int>("buffDamageOnKill")]
public sealed class DamageOnKillClaw : ModItem {
    public static readonly float DamageIncreasePerBuffStack = 0.02f;
    public static readonly int MaxStacks = 10;
    public static readonly int BuffDuration = 180;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Percent(DamageIncreasePerBuffStack));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        AequusPlayer aequus = player.GetModPlayer<AequusPlayer>();
        aequus.accDamageOnKill += MaxStacks;
        player.GetDamage(DamageClass.Generic) += aequus.buffDamageOnKill * DamageIncreasePerBuffStack;

        if (!hideVisual) {
            EmitParticles(player, aequus);
        }
    }

    void EmitParticles(Player player, AequusPlayer aequus) {
        if (aequus.buffDamageOnKill <= 0) {
            return;
        }

        int dustRate = Math.Max(80 / aequus.buffDamageOnKill, 3);
        if (Main.GameUpdateCount % dustRate == 0 || Main.rand.NextBool(dustRate)) {
            Dust d = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Firework_Red, Scale: 0.7f);
            d.velocity = (d.position - player.Center) * 0.1f;
            d.position += d.velocity * 2f;
            d.noGravity = true;
            d.fadeIn = d.scale + Main.rand.NextFloat(0.5f);
            d.noLight = true;
            d.noLightEmittence = true;
        }
    }

    [Gen.AequusPlayer_ResetEffects]
    public static void CheckBuffStacks(Player player, AequusPlayer aequus) {
        if (aequus.buffDamageOnKill <= 0) {
            return;
        }

        // Reset buff stack stat if player no longer has the buff.
        if (!player.HasBuff<DamageOnKillBuff>()) {
            aequus.buffDamageOnKill = 0;
        }
    }

    [Gen.AequusPlayer_OnHitNPC]
    public static void OnHitNPC(Player player, AequusPlayer aequus, NPC target, NPC.HitInfo hit) {
        // Only re-apply the buff if there is already a damage buff on the player.
        if (aequus.buffDamageOnKill <= 0) {
            return;
        }

        player.AddBuff(ModContent.BuffType<DamageOnKillBuff>(), BuffDuration, quiet: false);
    }

    [Gen.AequusPlayer_OnKillNPC]
    public static void OnKillNPC(Player player, AequusPlayer aequus, EnemyKillInfo info) {
        // Increment kill count and keep it within the cap.
        aequus.buffDamageOnKill = Math.Min(aequus.buffDamageOnKill + 1, aequus.accDamageOnKill);

        // Add buff.
        player.AddBuff(ModContent.BuffType<DamageOnKillBuff>(), BuffDuration, quiet: false);
    }

    [Gen.AequusNPC_ModifyNPCLoot]
    public static void AddNPCLoot(NPC npc, AequusNPC aequus, NPCLoot loot) {
        if (npc.type == NPCID.GiantFlyingFox) {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<DamageOnKillClaw>(), 60));
        }
    }
}

public sealed class DamageOnKillBuff : ModBuff {
    public override LocalizedText DisplayName => Instance<DamageOnKillClaw>().DisplayName;
    public override LocalizedText Description => Instance<DamageOnKillClaw>().GetLocalization("BuffDescription");

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare) {
        if (Main.LocalPlayer.TryGetModPlayer(out AequusPlayer aequus)) {
            tip = string.Format(tip, ALanguage.Percent(DamageOnKillClaw.DamageIncreasePerBuffStack * aequus.buffDamageOnKill));
        }
    }

    public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
        if (!Main.LocalPlayer.TryGetModPlayer(out AequusPlayer aequus)) {
            return;
        }

        string text = ALanguage.GetText("BuffStack").Format(aequus.buffDamageOnKill);
        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, text, drawParams.TextPosition, drawParams.DrawColor, 0f, Vector2.Zero, Vector2.One * 0.8f);
    }
}