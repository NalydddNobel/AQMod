using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.StunGun;

public class StunGun : ModItem, IManageProjectile {
    public static float VisualTimer => Main.GlobalTimeWrappedHourly * 5f;

    public override void SetDefaults() {
        Item.SetWeaponValues(20, 3f);
        Item.DamageType = DamageClass.Magic;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 60;
        Item.useTime = 60;
        Item.mana = 60;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<StunGunProj>();
        Item.UseSound = SoundID.DD2_LightningBugZap;
        Item.shootSpeed = 12f;
        Item.noMelee = true;
        Item.value = Item.buyPrice(gold: 15);
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-2f, 0f);
    }

    public static float GetVisualTime(float time, bool front) {
        return front ? time % MathHelper.Pi + MathHelper.Pi - MathHelper.PiOver2 : time % MathHelper.Pi - MathHelper.PiOver2;
    }

    public static Vector2 GetVisualOffset(NPC npc, float time) {
        return new Vector2(npc.width * 1.1f * MathF.Sin(time), MathF.Sin(Main.GlobalTimeWrappedHourly * 10.8f + npc.whoAmI));
    }

    public static float GetVisualScale(NPC npc) {
        return MathF.Max(npc.Size.Length() / 50f, 1f);
    }
}