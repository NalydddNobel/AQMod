using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Core.Initialization;
using System;
using System.Collections.Generic;

namespace Aequus.Content.Tools.AnglerLamp;

[AutoloadGlowMask]
public class WispLantern : ModItem {
    public static Int32 PotSightRange { get; set; } = 900;

    public static Vector3 LightColor { get; set; } = Vector3.Normalize(new Vector3(0.75f, 1.35f, 1.5f));
    public static Single LightBrightness { get; set; } = 3.6f;
    public static Single LightUseBrightness { get; set; } = 4.5f;

    public static Int32 MiscDebuffType { get; set; } = BuffID.Confused;
    public static Int32 MiscDebuffTime { get; set; } = 480;
    public static Int32[] FireDebuffTypes { get; set; } = new Int32[] { BuffID.OnFire3, BuffID.Frostburn2, BuffID.ShadowFlame };
    public static Int32 FireDebuffTime { get; set; } = 720;
    public static Int32 DebuffRange { get; set; } = 480;

    private readonly List<Dust> _dustEffects = new();

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 24;
        Item.rare = ItemCommons.Rarity.HardDungeonLoot;
        Item.value = Item.sellPrice(gold: 5, silver: 50);
        Item.holdStyle = ItemHoldStyleID.HoldLamp;
        Item.useStyle = ItemUseStyleID.RaiseLamp;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.autoReuse = true;
        Item.UseSound = AequusSounds.LanternConfuse with { Volume = 0.5f };
    }

    private Vector2 GetLampPosition(Player player) {
        return player.MountedCenter + new Vector2(player.direction * 19f - 0.5f, -2f * player.gravDir + player.gfxOffY);
    }

    public static void LanternHitEffect(Int32 npc, Boolean quiet = false) {
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            ModContent.GetInstance<WispLanternEffectPacket>().Send(npc);
            return;
        }

        if (Main.netMode != NetmodeID.Server) {
            Int32 count = Math.Clamp(Math.Max(Main.npc[npc].width, Main.npc[npc].height) / 10, 3, 8);
            foreach (var particle in ModContent.GetInstance<WispLanternParticles>().NewMultiple(count)) {
                particle.Location = Main.rand.NextVector2FromRectangle(Main.npc[npc].getRect());
                particle.Color = Color.Lerp(Color.Teal, Color.LightSkyBlue, Main.rand.NextFloat(0.15f, 0.85f));
                particle.Scale = Main.rand.NextFloat(0.5f, 0.9f);
                particle.NPCAnchor = npc;
            }
        }
    }

    public override Boolean? UseItem(Player player) {
        var lampPosition = GetLampPosition(player);
        _dustEffects.Clear();
        for (Int32 i = 0; i < 12; i++) {
            var d = Dust.NewDustPerfect(lampPosition, 156);
            d.velocity = (i / 13f * MathHelper.TwoPi).ToRotationVector2() * 2f;
            d.noGravity = true;
            d.fadeIn = d.scale + 0.3f;
            d.noLight = true;
            d.frame.Y = d.frame.Y / 30 * 30;
            _dustEffects.Add(d);
        }

        if (Main.myPlayer != player.whoAmI) {
            return true;
        }

        for (Int32 i = 0; i < Main.maxNPCs; i++) {
            var npc = Main.npc[i];
            if (npc.active && npc.damage > 0) {
                Single distance = player.Distance(npc.Center);
                if (distance >= DebuffRange) {
                    continue;
                }

                npc.AddBuff(MiscDebuffType, MiscDebuffTime);
                if (CanBurnNPC(npc)) {
                    for (Int32 j = 0; j < FireDebuffTypes.Length; j++) {
                        npc.AddBuff(FireDebuffTypes[j], FireDebuffTime);
                    }
                }
                if (npc.HasBuff(MiscDebuffType) || HasFireDebuff(npc)) {
                    LanternHitEffect(i);
                }
            }
        }

        Boolean HasFireDebuff(NPC npc) {
            for (Int32 i = 0; i < FireDebuffTypes.Length; i++) {
                if (npc.HasBuff(FireDebuffTypes[i])) {
                    return true;
                }
            }
            return false;
        }

        Boolean CanBurnNPC(NPC npc) {
            if (npc.dontTakeDamage || !player.CanNPCBeHitByPlayerOrPlayerProjectile(npc)) {
                return false;
            }
            var canHitOverride = CombinedHooks.CanPlayerHitNPCWithItem(player, Item, npc);
            if (canHitOverride.HasValue && canHitOverride == false || (!canHitOverride.HasValue || canHitOverride != true) && npc.friendly && (npc.type != NPCID.Guide || !player.killGuide) && (npc.type != NPCID.Clothier || !player.killClothier)) {
                return false;
            }
            return npc.noTileCollide || Collision.CanHitLine(player.position, player.width, player.height, npc.position, npc.width, npc.height);
        }
        return true;
    }

    public override void HoldItem(Player player) {
        Int32 animationEndTime = 8;
        if (player.itemTime > animationEndTime) {
            var lampPosition = GetLampPosition(player);
            Single animationProgress = (player.itemTime - animationEndTime) / (Single)(player.itemTimeMax - animationEndTime);
            Single outwards = MathF.Sin((1f - MathF.Pow(animationProgress, 2f)) * 2.2f) * 24f;
            Single positionLerp = 1f - MathF.Pow(1f - animationProgress, 3f);
            for (Int32 i = 0; i < _dustEffects.Count; i++) {
                if (!_dustEffects[i].active) {
                    _dustEffects.RemoveAt(i);
                    i--;
                }
                else {
                    _dustEffects[i].rotation = _dustEffects[i].rotation.AngleLerp(0f, animationProgress);
                    _dustEffects[i].scale = Math.Max(_dustEffects[i].scale, 0.33f);
                    _dustEffects[i].position = Vector2.Lerp(_dustEffects[i].position, lampPosition + (i / (Single)_dustEffects.Count * MathHelper.TwoPi).ToRotationVector2() * outwards, positionLerp);
                }
            }
        }
        else {
            _dustEffects.Clear();
        }

        player.aggro += 400;
        player.GetModPlayer<AequusPlayer>().potSightRange += PotSightRange;

        Single brightness = LightBrightness;
        if (player.itemTime > 0) {
            Single animationProgress = player.itemTime / (Single)player.itemTimeMax;
            brightness = MathHelper.Lerp(brightness, LightUseBrightness, animationProgress);
        }
        Lighting.AddLight(player.Center, LightColor * brightness);
    }

    public override void UpdateInventory(Player player) {
        Lighting.AddLight(player.Center, LightColor * LightBrightness * 0.5f);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.8f) with { A = 200 };
    }

    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, LightColor * LightBrightness / 2f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<AnglerLamp>()
            .AddIngredient(ItemID.Ectoplasm, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}