using Aequus;
using Aequus.Common.Entities.Players;
using Aequus.Common.Items;
using System;
using static Aequus.Common.Items.ItemHooks;

namespace Aequus.Content.Items.Accessories.FallSpeedHorseshoe;

[Gen.AequusPlayer_ResetField<Item>("accWeightedHorseshoe")]
[Gen.AequusPlayer_ResetField<bool>("showHorseshoeAnvilRope")]
[Gen.AequusPlayer_ResetField<int>("cHorseshoeAnvil")]
public class WeightedHorseshoe : ModItem, IUpdateItemDye {
    public static readonly float MaxFallSpeedMultiplier = 2f;
    public static readonly float DamagingFallSpeedThreshold = 11f;
    public static readonly float EnemyFallDamage = 75f;
    public static readonly float EnemyFallKnockback = 10f;
    public static readonly float SlimeMountFallDamageMultiplier = 2f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Blue;
        Item.value = ItemDefaults.NPCSkyMerchant;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        ApplyEffects(Item, player, MaxFallSpeedMultiplier, DamagingFallSpeedThreshold);
    }

    public static void ApplyEffects(Item item, Player player, float maxFallSpeedMultiplier, float damagingFallSpeedThreshold) {
        if (player.grappling[0] != -1) {
            return;
        }

        var aequus = player.GetModPlayer<AequusPlayer>();
        player.maxFallSpeed *= maxFallSpeedMultiplier;
        aequus.accWeightedHorseshoe = item;

        int gravityDirection = Math.Sign(player.gravDir);
        if (Math.Sign(player.velocity.Y) != gravityDirection) {
            return;
        }

        float fallSpeed = Math.Abs(player.velocity.Y);
        if (fallSpeed > damagingFallSpeedThreshold) {
            player.GetModPlayer<VisualFlags>().drawShadow = true;
        }
    }

    static void UpdateFloorEffects(Player player, AequusPlayer aequus) {
        int gravDir = Math.Sign(player.gravDir);
        float playerHeight = gravDir == -1 ? -2f : player.height + 2f;
        var floorCoordinates = new Vector2(player.position.X + player.width / 2f, player.position.Y + playerHeight);
        var floorTileCoordinates = floorCoordinates.ToTileCoordinates();
        if (!TileHelper.ScanDown(floorTileCoordinates, 3, out floorTileCoordinates)) {
            return;
        }

        Vector2 transVelocity = player.GetModPlayer<VelocityTransition>().Value;
        var floorTile = Framing.GetTileSafely(floorTileCoordinates);
        if (floorTile.HasUnactuatedTile && (Main.tileSolid[floorTile.TileType] || Main.tileSolidTop[floorTile.TileType]) && Helper.IsFalling(transVelocity, player.gravDir)) {
            float oldFallSpeed = Math.Abs(transVelocity.Y);
            if (oldFallSpeed > DamagingFallSpeedThreshold && player.velocity.Y < 1f) {
                float intensity = Math.Min((oldFallSpeed - DamagingFallSpeedThreshold) / 10f, 1f);
                int particleAmount = (int)Math.Max(60f * intensity, 5f);

                for (int i = 0; i < particleAmount; i++) {
                    var d = Main.dust[WorldGen.KillTile_MakeTileDust(floorTileCoordinates.X, floorTileCoordinates.Y, floorTile)];
                    d.position = floorCoordinates;
                    d.position.X += Main.rand.NextFloat(-player.width / 2f, player.width / 2f);
                    d.position.Y += Main.rand.NextFloat(-2f, 2f);
                    d.noGravity = true;
                    d.velocity *= 0.5f;
                    d.velocity.X += Main.rand.NextFloat(-i / 10f, i / 10f);
                    d.velocity /= 0.5f;
                    d.scale *= Main.rand.NextFloat(0.9f, 1.1f + (particleAmount - i) / (float)particleAmount);
                    if (Math.Sign(d.velocity.Y) == gravDir) {
                        d.velocity.Y *= 0.1f;
                    }
                }
                //Main.NewText($"emit a big gigashit volcano of intensity: {intensity}");
            }
        }
    }

    void IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        if (isSetToHidden) {
            return;
        }

        var aequus = player.GetModPlayer<AequusPlayer>();
        aequus.showHorseshoeAnvilRope = true;
        aequus.cHorseshoeAnvil = dyeItem.dye;

        UpdateFloorEffects(player, aequus);
    }

    static void AdjustDamage(Player player, ref MiscHitInfo hitInfo) {
        if (player.mount.IsConsideredASlimeMount) {
            if (player.velocity.Y >= DamagingFallSpeedThreshold) {
                hitInfo.Damage *= SlimeMountFallDamageMultiplier;
            }
            hitInfo.Hurtbox.Inflate(6, 0);
            //hitInfo.DamagingHitbox.Height *= 2;
            hitInfo.DamageClass = DamageClass.Summon;
        }
    }

    static void OnHitNPCWithHorseshoe(Player player, int amountHit, MiscHitInfo hitInfo) {
        if (amountHit == 0) {
            return;
        }
        if (player.mount.IsConsideredASlimeMount) {
            player.velocity.Y = -10f;
        }
    }

    [Gen.AequusPlayer_PostUpdateEquips]
    internal static void OnPostUpdateEquips(Player player, AequusPlayer aequus) {
        int visualProj = ModContent.ProjectileType<WeightedHorseshoeVisual>();
        if (aequus.showHorseshoeAnvilRope && Main.myPlayer == player.whoAmI && player.ownedProjectileCounts[visualProj] < 1) {
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, player.velocity * 0.5f, visualProj, 0, 0f, player.whoAmI);
        }

        if (aequus.accWeightedHorseshoe == null || player.velocity.Y < DamagingFallSpeedThreshold) {
            return;
        }

        MiscHitInfo hitInfo = new MiscHitInfo() {
            Hurtbox = Utils.CenteredRectangle(player.Bottom, new(player.width + 12, player.velocity.Y * 2f)),
            Damage = EnemyFallDamage,
            DamageClass = DamageClass.Melee,
            Knockback = EnemyFallKnockback
        };

        AdjustDamage(player, ref hitInfo);

        int amountDamaged = player.CollideWithNPCs(hitInfo.Hurtbox, player.GetTotalDamage(hitInfo.DamageClass).ApplyTo((float)hitInfo.Damage), player.GetTotalKnockback(hitInfo.DamageClass).ApplyTo(hitInfo.Knockback), 10, 4, hitInfo.DamageClass);
        OnHitNPCWithHorseshoe(player, amountDamaged, hitInfo);
    }
}