using Aequus.Core.CodeGeneration;
using System;
using Terraria.DataStructures;

namespace Aequus.Old.Content.Potions.NeutronYogurt;

[Gen.AequusPlayer_ResetField<float>("buffNeutronYogurt")]
public class NeutronYogurt : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 20;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemSets.DrinkParticleColors[Type] = [Color.HotPink, Color.Yellow];
    }

    public override void SetDefaults() {
        Item.width = 10;
        Item.height = 10;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 2);
        Item.buffType = ModContent.BuffType<NeutronYogurtBuff>();
        Item.buffTime = 28800;
    }

    [Gen.AequusPlayer_PostUpdateEquips]
    internal static void UpdateNeutronYogurt(Player Player, AequusPlayer aequusPlayer) {
        if (aequusPlayer.buffNeutronYogurt <= 0f || (Player.slowFall && !Player.controlDown)) {
            return;
        }

        float gravityMultiplier = aequusPlayer.buffNeutronYogurt + 1f;
        int velocityYDirection = Math.Sign(Player.velocity.Y);
        int gravityDirection = Math.Sign(Player.gravDir);
        if (velocityYDirection != gravityDirection) {
            return;
        }

        Player.gravity *= gravityMultiplier;
        if (Player.grappling[0] != -1 || (Player.mount.Active && (Player.mount.CanFly() || (Player.mount._data.swimSpeed > 0f && Player.wet)))) {
            return;
        }

        float fallSpeed = Math.Abs(Player.velocity.Y);
        if (fallSpeed < Player.maxFallSpeed - 0.01f) {
            float yOffset = gravityDirection == 1 ? Player.height : 0;
            float fallProgress = fallSpeed / Player.maxFallSpeed;
            float wavePattern = fallProgress * MathHelper.TwoPi;
            for (int i = 0; i < 2; i++) {
                float waveOffset = MathF.Sin(wavePattern + MathHelper.Pi * i);
                var d = Dust.NewDustPerfect(new Vector2(Player.position.X + Player.width / 2f + Player.width * waveOffset, Player.position.Y + yOffset), DustID.CrystalSerpent_Pink, Scale: fallProgress * 0.5f + 0.7f);
                d.noGravity = true;
                d.noLight = true;
                d.velocity *= 0.1f;
                d.velocity.X += Player.velocity.X;
            }
        }
    }
}