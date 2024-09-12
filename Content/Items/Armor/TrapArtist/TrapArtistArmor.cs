using Aequus.Common.ContentTemplates;
using Aequus.Common.ContentTemplates.Armor;
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Items;
using Aequus.Common.PlayerLayers;
using Aequus.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Items.Armor.TrapArtist;

[Gen.AequusPlayer_ResetField<Item>("equipDartTrapHat")]
public class TrapArtistArmor : UnifiedArmorSet {
    public ModItem DartTrapHat;
    public ModItem VenomHelmet;
    public ModItem SuperHelmet;

    public readonly float VenomHelmet_SummonDamage = 0.1f;
    public readonly int VenomHelmet_MinionSlots = 1;
    public readonly float SuperDartHelmet_SummonDamage = 0.4f;
    public readonly int SuperDartHelmet_MinionSlots = 1;

    public TrapArtistArmor() {
        DartTrapHat = this.AddContent(new InstancedDartTrapHat("DartTrapHat", [], new(
            Damage: 28,
            Defense: 1,
            ArmorPenetration: 10,
            KnockBack: 2f,
            AttackCooldown: 300,
            Speed: 10f,
            DebuffType: BuffID.Poisoned,
            DebuffDuration: 480,
            Rarity: ItemRarityID.White,
            Price: Item.sellPrice(silver: 20)
        )));

        VenomHelmet = this.AddContent(new InstancedDartTrapHat("VenomDartTrapHat",
            [ALanguage.Percent(VenomHelmet_SummonDamage), VenomHelmet_MinionSlots], new(
            Damage: 100,
            Defense: 10,
            ArmorPenetration: 15,
            KnockBack: 4f,
            AttackCooldown: 200,
            Speed: 15f,
            DebuffType: BuffID.Venom,
            DebuffDuration: 480,
            Rarity: ItemRarityID.LightRed,
            Price: Item.sellPrice(silver: 30),
            EquipEffects: UpdateVenomHat
        )));

        SuperHelmet = this.AddContent(new InstancedDartTrapHat("SuperDartTrapHat", [ALanguage.Percent(SuperDartHelmet_SummonDamage), SuperDartHelmet_MinionSlots], new(
            Damage: 200,
            Defense: 10,
            ArmorPenetration: 15,
            KnockBack: 5f,
            AttackCooldown: 150,
            Speed: 15f,
            DebuffType: BuffID.Venom,
            DebuffDuration: 480,
            Rarity: ItemRarityID.Yellow,
            Price: Item.sellPrice(silver: 30),
            EquipEffects: UpdateSuperHat
        )));
    }

    void UpdateVenomHat(Player player) {
        player.GetDamage(DamageClass.Summon) += VenomHelmet_SummonDamage;
        player.maxMinions += VenomHelmet_MinionSlots;
    }

    void UpdateSuperHat(Player player) {
        player.GetDamage(DamageClass.Summon) += SuperDartHelmet_SummonDamage;
        player.maxMinions += SuperDartHelmet_MinionSlots;
    }

    public override void AddRecipes() {
        DartTrapHat.CreateRecipe()
            .AddIngredient(ItemID.DartTrap)
            .AddIngredient(ItemID.Wire, 150)
            .AddIngredient(ItemID.Switch)
            .AddTile(TileID.Anvils)
            .Register();

        VenomHelmet.CreateRecipe()
            .AddIngredient(DartTrapHat)
            .AddIngredient(ItemID.VenomDartTrap)
            .AddTile(TileID.Anvils)
            .Register();

        SuperHelmet.CreateRecipe()
            .AddIngredient(VenomHelmet)
            .AddIngredient(ItemID.SuperDartTrap)
            .AddIngredient(ItemID.ChlorophyteBar, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public static void GetShootParams(Player player, out Vector2 ShootLocation, out Vector2 VelocityNormal) {
        ShootLocation = player.Center + new Vector2(8f * player.direction, player.height / 2f * -player.gravDir);
        VelocityNormal = new Vector2(player.direction, 0f);
    }
}

public record struct DartTrapHatSettings(int Damage, int Defense, int ArmorPenetration, float KnockBack, int AttackCooldown, float Speed, int DebuffType, int DebuffDuration, int Rarity, int Price, Action<Player>? EquipEffects = null);

[AutoloadEquip(EquipType.Head)]
internal class InstancedDartTrapHat(string Name, string Texture, object[] tooltipArgs, DartTrapHatSettings Settings) : InstancedModItem(Name, Texture), ItemHooks.IUpdateItemDye {
    internal InstancedDartTrapHat(string Name, object[] tooltipArgs, DartTrapHatSettings Settings) : this(Name, $"Aequus/Content/Items/Armor/TrapArtist/{Name}", tooltipArgs.Prepend(Settings.ArmorPenetration).ToArray(), Settings) { }

    [CloneByReference]
    private ModProjectile? ShootProjectile;
    public readonly DartTrapHatSettings Settings = Settings;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(tooltipArgs);

    public override string LocalizationCategory => "Items.Armor.TrapArtistArmor";

    public override void Load() {
        ShootProjectile = new InstancedTrapHatProj(this);
        Mod.AddContent(ShootProjectile);
    }

    public override void SetStaticDefaults() {
        StackingHatEffect.Blacklist.Add(Item.headSlot);
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.damage = Settings.Damage;
        Item.defense = Settings.Defense;
        Item.DamageType = DamageClass.Summon;
        Item.ArmorPenetration = Settings.ArmorPenetration;
        Item.knockBack = Settings.KnockBack;
        Item.rare = Settings.Rarity;
        Item.value = Settings.Price;
        Item.shoot = ShootProjectile!.Type;
        Item.shootSpeed = Settings.Speed;
    }

    public override void UpdateEquip(Player player) {
        var aequus = player.Aequus();
        aequus.wearingPassiveSummonHelmet = true;
        aequus.summonHelmetTimer--;
        aequus.equipDartTrapHat = Item;

        Settings.EquipEffects?.Invoke(player);

        if (aequus.summonHelmetTimer <= 0) {
            if (aequus.summonHelmetTimer != -1) {
                if (Main.myPlayer == player.whoAmI) {
                    IEntitySource source = player.GetSource_Accessory(Item, "Helmet");
                    int damage = player.GetWeaponDamage(Item);
                    float kb = player.GetWeaponKnockback(Item);

                    int p = Projectile.NewProjectile(source, player.Center, new Vector2(Item.shootSpeed, 0f), ShootProjectile!.Type, damage, kb, player.whoAmI);
                    Main.projectile[p].ArmorPenetration = Item.ArmorPenetration;
                }
            }
            aequus.summonHelmetTimer = Settings.AttackCooldown;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.RemoveKnockback();
        tooltips.RemoveCritChance();
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        if (player.Aequus().stackingHat == 0) {
            player.Aequus().stackingHat = Item.headSlot;
        }
    }
}

internal class InstancedTrapHatProj(InstancedDartTrapHat Parent) : InstancedProjectile(Parent.Name, $"{Parent.Texture}Proj") {
    [CloneByReference]
    public InstancedDartTrapHat Parent = Parent;

    public override LocalizedText DisplayName => Parent.DisplayName;

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.PoisonDart);
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 121;
        Projectile.trap = false;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.alpha = 0;
        AIType = ProjectileID.PoisonDart;
    }

    public override bool PreAI() {
        Player player = Main.player[Projectile.owner];
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 10;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        if (Projectile.timeLeft > 120) {
            TrapArtistArmor.GetShootParams(player, out Vector2 location, out Vector2 velocity);
            Projectile.velocity = velocity * Projectile.velocity.Length();
            Projectile.Center = location - Projectile.velocity * 0.5f * (1f - Projectile.Opacity);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.hide = true;

            return false;
        }

        Projectile.hide = false;
        return true;
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override bool? CanHitNPC(NPC target) {
        return NPCID.Sets.CountsAsCritter[target.type] ? false : null;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        target.AddBuff(Parent.Settings.DebuffType, Parent.Settings.DebuffDuration);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        target.AddBuff(Parent.Settings.DebuffType, Parent.Settings.DebuffDuration / 2);
    }

    public override void OnKill(int timeLeft) {
        for (int i = 0; i < 10; i++) {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
        }
    }
}

public class DartTrapHatPlayerLayer : PlayerDrawLayer {
    public override Position GetDefaultPosition() {
        return new AfterParent(PlayerDrawLayers.Head);
    }

    protected override void Draw(ref PlayerDrawSet drawInfo) {
        if (!drawInfo.drawPlayer.TryGetModPlayer(out AequusPlayer aequus) || aequus.equipDartTrapHat == null || aequus.summonHelmetTimer <= 0 || aequus.summonHelmetTimer > 60) {
            return;
        }

        float anim = 1f - aequus.summonHelmetTimer / 60f;

        // Get index of the helmet texture.
        Texture2D helmetTexture = TextureAssets.ArmorHead[aequus.equipDartTrapHat.headSlot].Value;
        int i = drawInfo.DrawDataCache.Count - 1;
        for (; i >= 0; i--) {
            if (drawInfo.DrawDataCache[i].texture == helmetTexture) {
                break;
            }
        }
        if (i == 0) {
            i = drawInfo.DrawDataCache.Count;
        }

        Item helm = aequus.equipDartTrapHat;

        Main.instance.LoadProjectile(helm.shoot);
        Texture2D projTexture = TextureAssets.Projectile[helm.shoot].Value;

        TrapArtistArmor.GetShootParams(drawInfo.drawPlayer, out Vector2 drawCoordinates, out Vector2 velocity);
        float projXOffset = 4f - 8f * (1f - MathF.Pow(anim, 2f));
        drawCoordinates = (drawCoordinates - Main.screenPosition + velocity * projXOffset).Floor();
        float rotation = velocity.ToRotation() + MathHelper.PiOver2;
        drawInfo.DrawDataCache.Insert(i, new DrawData(projTexture, drawCoordinates, null, drawInfo.colorArmorHead * anim, rotation, projTexture.Size() / 2f, 1f, SpriteEffects.None));
    }
}