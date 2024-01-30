using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Core.Initialization;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.Tools.AnglerLamp;

[AutoloadGlowMask]
public class AnglerLamp : ModItem {
    public static Int32 PotSightRange { get; set; } = 300;

    public static Vector3 LightColor { get; set; } = new Vector3(1f, 0.76f, 0.24f);
    public static Single LightBrightness { get; set; } = CommonLight.ShadowOrbBrightness;
    public static Single LightUseBrightness { get; set; } = CommonLight.ShadowOrbBrightness * 2.5f;

    public static Int32 MiscDebuffType { get; set; } = BuffID.Confused;
    public static Int32 MiscDebuffTime { get; set; } = 240;
    public static Int32 FireDebuffType { get; set; } = BuffID.OnFire3;
    public static Int32 FireDebuffTime { get; set; } = 180;
    public static Int32 DebuffRange { get; set; } = 240;

    private readonly List<Dust> _dustEffects = new();

    public Single animation;

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 24;
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.useAmmo = AmmoID.Gel;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;
        Item.useStyle = ItemUseStyleID.RaiseLamp;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.autoReuse = true;
        Item.UseSound = AequusSounds.LanternConfuse with { Volume = 0.5f };
    }

    public override Boolean CanUseItem(Player player) {
        return !player.wet;
    }

    private Vector2 GetLampPosition(Player player) {
        return player.MountedCenter + new Vector2(player.direction * 16f - 0.5f, -2f * player.gravDir + player.gfxOffY);
    }

    public static void LanternHitEffect(Int32 npc, Boolean quiet = false) {
        if (Main.netMode != NetmodeID.SinglePlayer && !quiet) {
            ModContent.GetInstance<AnglerLampEffectPacket>().Send(npc);
            return;
        }

        if (Main.netMode != NetmodeID.Server) {
            Int32 count = Math.Clamp(Math.Max(Main.npc[npc].width, Main.npc[npc].height) / 10, 3, 8);
            foreach (var particle in ModContent.GetInstance<AnglerLampParticles>().NewMultiple(count)) {
                particle.Location = Main.rand.NextVector2FromRectangle(Main.npc[npc].getRect());
                particle.Color = Color.Lerp(Color.Red, Color.Yellow, Main.rand.NextFloat(0.15f, 0.85f));
                particle.Scale = Main.rand.NextFloat(0.4f, 0.76f);
                particle.NPCAnchor = npc;
            }
        }
    }

    public override Boolean? UseItem(Player player) {
        player.ConsumeItem(ItemID.Gel);
        var lampPosition = GetLampPosition(player);
        _dustEffects.Clear();
        for (Int32 i = 0; i < 12; i++) {
            var d = Dust.NewDustPerfect(lampPosition, DustID.Torch, Scale: 1.5f);
            d.velocity = (i / 13f * MathHelper.TwoPi).ToRotationVector2() * 2f;
            d.noGravity = true;
            d.fadeIn = d.scale + 0.3f;
            d.noLight = true;
            _dustEffects.Add(d);
        }

        if (Main.myPlayer != player.whoAmI) {
            return true;
        }

        Int32 closestNPC = -1;
        Single closestNPCDistance = Single.MaxValue;
        for (Int32 i = 0; i < Main.maxNPCs; i++) {
            var npc = Main.npc[i];
            if (npc.active && npc.damage > 0) {
                Single distance = player.Distance(npc.Center);
                if (distance >= DebuffRange) {
                    continue;
                }

                npc.AddBuff(MiscDebuffType, MiscDebuffTime);
                if (npc.HasBuff(FireDebuffType)) {
                    npc.AddBuff(FireDebuffType, FireDebuffTime);
                }
                else if (distance < closestNPCDistance && CanBurnNPC(npc)) {
                    closestNPC = i;
                    closestNPCDistance = distance;
                }
                if (npc.HasBuff(MiscDebuffType) || npc.HasBuff(FireDebuffType)) {
                    LanternHitEffect(i);
                }
            }
        }

        if (closestNPC >= 0) {
            Main.npc[closestNPC].AddBuff(FireDebuffType, FireDebuffTime);
            if (!Main.npc[closestNPC].HasBuff(MiscDebuffType)) {
                LanternHitEffect(closestNPC);
            }
        }

        Boolean CanBurnNPC(NPC npc) {
            if (npc.buffImmune[FireDebuffType] || npc.dontTakeDamage || !player.CanNPCBeHitByPlayerOrPlayerProjectile(npc)) {
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

        if (player.wet) {
            Item.holdStyle = ItemHoldStyleID.None;
            return;
        }

        player.aggro += 200;
        player.GetModPlayer<AequusPlayer>().potSightRange += PotSightRange;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;

        Single brightness = LightBrightness;
        if (player.itemTime > 0) {
            Single animationProgress = player.itemTime / (Single)player.itemTimeMax;
            brightness = MathHelper.Lerp(brightness, LightUseBrightness, animationProgress);
        }
        Lighting.AddLight(player.Center, LightColor * brightness);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, LightColor * LightBrightness / 2f);
    }

    public override Boolean PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, Single scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, MathF.Sin(animation * MathHelper.TwoPi * 2f) * animation * 0.3f, origin, scale, SpriteEffects.None, 0f);
        if (animation > 0f) {
            animation -= 0.033f;
            if (animation < 0f) {
                animation = 0f;
            }
        }
        return false;
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, Single rotation, Single scale, Int32 whoAmI) {
        animation = 0f;
    }
}