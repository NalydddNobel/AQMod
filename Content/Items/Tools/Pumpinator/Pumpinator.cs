using Aequus.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.Pumpinator;

[AutoloadGlowMask]
public class Pumpinator : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.knockBack = 10f;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useTime = 9;
        Item.useAnimation = 9;
        Item.UseSound = SoundID.Item39;
        Item.rare = ItemRarityID.Pink;
        Item.shoot = ModContent.ProjectileType<PumpinatorProj>();
        Item.shootSpeed = 9f;
        Item.autoReuse = true;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        try {
            tooltips.Insert(tooltips.GetIndex("Knockback"), new TooltipLine(Mod, "Knockback", TextHelper.GetKnockbackText(Item.knockBack)));
            tooltips.Insert(tooltips.GetIndex("Speed"), new TooltipLine(Mod, "Speed", TextHelper.GetUseAnimationText(Item.useAnimation)));
        }
        catch {
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        if (!CreativePowerManager.Instance.GetPower<CreativePowers.FreezeWindDirectionAndStrength>().Enabled && !LanternNight.LanternsUp) {
            float windChange = 0.02f * (1f - Math.Abs(velocity.Y) / Item.shootSpeed) * Math.Sign(velocity.X);
            if (Math.Abs(Main.windSpeedTarget + windChange) > 0.8f) {
                windChange = -(Main.windSpeedTarget - 0.8f * Math.Sign(Main.windSpeedTarget));
            }
            Main.windSpeedTarget += windChange;
            if (Math.Abs(Main.windSpeedCurrent - Main.windSpeedTarget) < 0.019f) {
                Main.windSpeedCurrent = MathHelper.Lerp(Main.windSpeedCurrent, Main.windSpeedTarget, 0.5f);
            }
            Main.windCounter = Math.Min(Main.windCounter, 480);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                //var p = Aequus.GetPacket(PacketType.PumpinatorWindSpeed);
                //p.Write(Main.windSpeedTarget);
                //p.Write(Main.windSpeedCurrent);
                //p.Write(Main.windCounter);
                //p.Send();
            }
            else if (Main.windSpeedCurrent > 0.6f) {
                Sandstorm.StartSandstorm();
            }
        }
        return true;
    }
}