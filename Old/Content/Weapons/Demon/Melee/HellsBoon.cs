using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Core.Initialization;

namespace Aequus.Old.Content.Weapons.Demon.Melee;

[AutoloadGlowMask]
public class HellsBoon : ModItem {
    private int slashTimer;

    public override void SetStaticDefaults() {
        AltarSacrifices.Register(ItemID.LightsBane, Type);
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.damage = 20;
        Item.useTime = 52;
        Item.useAnimation = 26;
        Item.autoReuse = true;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.knockBack = 3f;
        Item.shootSpeed = 35f;
        Item.shoot = ModContent.ProjectileType<HellsBoonSpawner>();
        Item.scale = 1f;
        Item.rare = ItemCommons.Rarity.DemonSiegeTier1Loot;
        Item.value = ItemCommons.Price.DemonSiegeLoot;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(50);
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
        target.AddBuff(ModContent.BuffType<CorruptionHellfire>(), 240);
    }

    public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) {
        target.AddBuff(ModContent.BuffType<CorruptionHellfire>(), 240);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame) {
        if (player.JustDroppedAnItem) {
            return;
        }

        if (slashTimer > 0) {
            slashTimer--;
            return;
        }

        slashTimer = player.itemTimeMax / 3;
        if (Main.myPlayer != player.whoAmI) {
            return;
        }

        int target = Helper.FindTargetWithLineOfSight(player.Center, maxRange: 196f);
        Vector2 spawnLocation;
        Vector2 velocity;
        if (target != -1) {
            spawnLocation = Main.npc[target].Center + Main.rand.NextVector2Square(-20f, 20f);
            velocity = player.DirectionTo(Main.npc[target].Center);
        }
        else {
            spawnLocation = player.Center + new Vector2(50f * player.direction, (12f + player.height / 2f) * -player.gravDir) * Item.scale;
            velocity = new Vector2(player.direction * Main.rand.NextFloat(1f), player.gravDir);
        }

        int damage = player.GetWeaponDamage(Item) / 2;
        float ai0 = 1f;
        if (player.RollCrit(Item)) {
            damage *= 2;
            ai0 = 2f;
        }

        Projectile.NewProjectile(player.GetSource_ItemUse(Item), spawnLocation, Vector2.Normalize(velocity).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 0.001f, ProjectileID.LightsBane, damage, player.GetWeaponKnockback(Item), player.whoAmI, ai0);
    }

    public override bool? UseItem(Player player) {
        return base.UseItem(player);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        damage = (int)(damage * 0.5f);
        position = Main.MouseWorld;
        player.LimitPointToPlayerReachableArea(ref position);
    }
}