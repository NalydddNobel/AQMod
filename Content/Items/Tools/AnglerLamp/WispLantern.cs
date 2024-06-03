using Aequus.Common;
using System;
using System.Collections.Generic;
using tModLoaderExtended.GlowMasks;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequus.Content.Items.Tools.AnglerLamp;

[AutoloadGlowMask]
[FilterOverride(FilterOverride.Tools)]
public class WispLantern : ModItem {
    public static int PotSightRange { get; set; } = 900;

    public static Vector3 LightColor { get; set; } = Vector3.Normalize(new Vector3(0.75f, 1.35f, 1.5f));
    public static float LightBrightness { get; set; } = 3.6f;
    public static float LightUseBrightness { get; set; } = 4.5f;

    public static int MiscDebuffType { get; set; } = BuffID.Confused;
    public static int MiscDebuffTime { get; set; } = 480;
    public static int[] FireDebuffTypes { get; set; } = new int[] { BuffID.OnFire3, BuffID.Frostburn2, BuffID.ShadowFlame };
    public static int FireDebuffTime { get; set; } = 720;
    public static int DebuffRange { get; set; } = 480;

    [CloneByReference]
    private readonly List<Dust> _dustEffects = new();

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 24;
        Item.rare = Commons.Rare.BiomeDungeonHM;
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

    public static void LanternHitEffect(int npc, bool quiet = false) {
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            ModContent.GetInstance<WispLanternEffectPacket>().Send(npc);
            return;
        }

        if (Main.netMode != NetmodeID.Server) {
            int count = Math.Clamp(Math.Max(Main.npc[npc].width, Main.npc[npc].height) / 10, 3, 8);
            foreach (var particle in WispLanternParticles.NewMultiple(count)) {
                particle.Location = Main.rand.NextVector2FromRectangle(Main.npc[npc].getRect());
                particle.Color = Color.Lerp(Color.Teal, Color.LightSkyBlue, Main.rand.NextFloat(0.15f, 0.85f));
                particle.Scale = Main.rand.NextFloat(0.5f, 0.9f);
                particle.NPCAnchor = npc;
            }
        }
    }

    public override bool? UseItem(Player player) {
        var lampPosition = GetLampPosition(player);
        _dustEffects.Clear();
        for (int i = 0; i < 12; i++) {
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

        for (int i = 0; i < Main.maxNPCs; i++) {
            var npc = Main.npc[i];
            if (npc.active && npc.damage > 0) {
                float distance = player.Distance(npc.Center);
                if (distance >= DebuffRange) {
                    continue;
                }

                npc.AddBuff(MiscDebuffType, MiscDebuffTime);
                if (CanBurnNPC(npc)) {
                    for (int j = 0; j < FireDebuffTypes.Length; j++) {
                        npc.AddBuff(FireDebuffTypes[j], FireDebuffTime);
                    }
                }
                if (npc.HasBuff(MiscDebuffType) || HasFireDebuff(npc)) {
                    LanternHitEffect(i);
                }
            }
        }

        bool HasFireDebuff(NPC npc) {
            for (int i = 0; i < FireDebuffTypes.Length; i++) {
                if (npc.HasBuff(FireDebuffTypes[i])) {
                    return true;
                }
            }
            return false;
        }

        bool CanBurnNPC(NPC npc) {
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
        int animationEndTime = 8;
        if (player.itemTime > animationEndTime) {
            var lampPosition = GetLampPosition(player);
            float animationProgress = (player.itemTime - animationEndTime) / (float)(player.itemTimeMax - animationEndTime);
            float outwards = MathF.Sin((1f - MathF.Pow(animationProgress, 2f)) * 2.2f) * 24f;
            float positionLerp = 1f - MathF.Pow(1f - animationProgress, 3f);
            for (int i = 0; i < _dustEffects.Count; i++) {
                if (!_dustEffects[i].active) {
                    _dustEffects.RemoveAt(i);
                    i--;
                }
                else {
                    _dustEffects[i].rotation = _dustEffects[i].rotation.AngleLerp(0f, animationProgress);
                    _dustEffects[i].scale = Math.Max(_dustEffects[i].scale, 0.33f);
                    _dustEffects[i].position = Vector2.Lerp(_dustEffects[i].position, lampPosition + (i / (float)_dustEffects.Count * MathHelper.TwoPi).ToRotationVector2() * outwards, positionLerp);
                }
            }
        }
        else {
            _dustEffects.Clear();
        }

        player.aggro += 400;
        player.GetModPlayer<AequusPlayer>().potSightRange += PotSightRange;

        float brightness = LightBrightness;
        if (player.itemTime > 0) {
            float animationProgress = player.itemTime / (float)player.itemTimeMax;
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