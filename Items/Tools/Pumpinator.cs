﻿using Aequus.Common.Items;
using Aequus.Projectiles.Misc;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;

namespace Aequus.Items.Tools;
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
            int index = tooltips.GetIndex("Knockback");
            tooltips.Insert(index, new TooltipLine(Mod, "Knockback", TextHelper.KnockbackLine(Item.knockBack)));
            index = tooltips.GetIndex("Speed");
            tooltips.Insert(index, new TooltipLine(Mod, "Speed", TextHelper.UseAnimationLine(Item.useAnimation)));
            if (!Main.hardMode || AequusWorld.downedEventAtmosphere) {
                return;
            }
            index = tooltips.GetIndex("Material");
            tooltips.Insert(index, new TooltipLine(Mod, "StartsGaleStreams", TextHelper.GetTextValue("GaleStreamsHint")) { OverrideColor = AequusItem.HintColor, });
        }
        catch {
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        if (!CreativePowerManager.Instance.GetPower<CreativePowers.FreezeWindDirectionAndStrength>().Enabled/* && !LanternNight.LanternsUp*/) {
            float windChange = 0.02f * (1f - velocity.Y.Abs() / Item.shootSpeed) * Math.Sign(velocity.X);
            if ((Main.windSpeedTarget + windChange).Abs() > 0.8f) {
                windChange = -(Main.windSpeedTarget - 0.8f * Math.Sign(Main.windSpeedTarget));
            }
            Main.windSpeedTarget += windChange;
            if (!Main.windSpeedCurrent.CloseEnough(Main.windSpeedTarget, 0.019f)) {
                Main.windSpeedCurrent = MathHelper.Lerp(Main.windSpeedCurrent, Main.windSpeedTarget, 0.5f);
            }
            Main.windCounter = Math.Min(Main.windCounter, 480);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                var p = Aequus.GetPacket(PacketType.PumpinatorWindSpeed);
                p.Write(Main.windSpeedTarget);
                p.Write(Main.windSpeedCurrent);
                p.Write(Main.windCounter);
                p.Send();
            }
            else if (Main.windSpeedCurrent > 0.6f) {
                Sandstorm.StartSandstorm();
            }
        }
        return true;
    }
}