using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class HighSteaks : ModItem
    {
        public static int WaistSlot;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            WaistSlot = Item.waistSlot;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(16, 16);
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.ValueBloodMoon;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.bloodDiceDamage = Math.Max(aequus.bloodDiceDamage, 0.25f) + 0.25f;
            if (aequus.bloodDiceMoney > 0)
            {
                aequus.bloodDiceMoney = Math.Max(aequus.bloodDiceMoney / 2, 1);
            }
            else
            {
                aequus.bloodDiceMoney = Item.buyPrice(silver: 1);
            }
        }
    }

    public class HighSteaksMoneyConsumeEffect : PlayerDrawLayer
    {
        public static List<float> CoinAnimations { get; private set; }

        public override void Load()
        {
            CoinAnimations = new List<float>();
        }

        public override void Unload()
        {
            CoinAnimations?.Clear();
            CoinAnimations = new List<float>();
        }

        public override Position GetDefaultPosition()
        {
            return new BeforeParent(PlayerDrawLayers.WaistAcc);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!drawInfo.drawPlayer.active)
                return;

            //Main.NewText(drawInfo.drawPlayer.waist +"|"+ HighSteaks.WaistSlot);
            if (Main.gameMenu || drawInfo.drawPlayer.waist != HighSteaks.WaistSlot)
            {
                CoinAnimations.Clear();
                return;
            }

            for (int i = 0; i < CoinAnimations.Count; i++)
            {
                float animationTime = CoinAnimations[i] % 100f;
                float rotation = CoinAnimations[i] / 100f;
                ulong seed = (ulong)rotation;
                var drawLocation = drawInfo.Position + new Vector2(drawInfo.drawPlayer.width / 2f, drawInfo.drawPlayer.height / 2f) + (rotation).ToRotationVector2() * animationTime * Utils.RandomInt(ref seed, 25, 40) / 10f;
                float opacity = (float)Math.Pow(animationTime > 8 ? 1f - (animationTime - 8f) / 16f : 1f, 2f);
                var texture = TextureAssets.Coin[1];
                var frame = texture.Value.Frame(verticalFrames: 8, frameY: (int)((Main.GameUpdateCount / 10 + CoinAnimations[i] / 5) % 8));
                drawInfo.DrawDataCache.Add(
                    new DrawData(texture.Value, (drawLocation - Main.screenPosition).Floor(), frame, AequusHelpers.GetColor(drawLocation) * opacity, 0f, frame.Size() / 2f, 1f, drawInfo.drawPlayer.direction.ToSpriteEffect(), 0) { shader = 0, });
                CoinAnimations[i]++;
                if (animationTime > 24)
                {
                    CoinAnimations.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}