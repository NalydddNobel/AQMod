using Aequus;
using Aequus.Common.Audio;
using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Offense.Crit;
using Aequus.Items.Accessories.Offense.Necro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Crit
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
            aequus.highSteaksHide = hideVisual;
            aequus.highSteaksDamage = Math.Max(aequus.highSteaksDamage, 0.25f) + 0.25f;
            if (aequus.highSteaksCost > 0)
            {
                aequus.highSteaksCost = Math.Max(aequus.highSteaksCost / 2, 1);
            }
            else
            {
                aequus.highSteaksCost = Item.buyPrice(silver: 1);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            string colorText = TextHelper.ColorCommandStart(Colors.CoinSilver, alphaPulse: true);
            foreach (var t in tooltips) {
                if (t.Mod != "Terraria") {
                    continue;
                }

                t.Text = t.Text.Replace("[[", colorText);
                t.Text = t.Text.Replace("]]", "]");
            }
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<BloodiedBucket>());
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
            if (Main.gameMenu || drawInfo.drawPlayer.Aequus().highSteaksHide)
            {
                CoinAnimations.Clear();
                return;
            }

            for (int i = 0; i < CoinAnimations.Count; i++)
            {
                float animationTime = CoinAnimations[i] % 100f;
                float rotation = CoinAnimations[i] / 100f;
                ulong seed = (ulong)rotation;
                var drawLocation = drawInfo.Position + new Vector2(drawInfo.drawPlayer.width / 2f, drawInfo.drawPlayer.height / 2f) + rotation.ToRotationVector2() * animationTime * Utils.RandomInt(ref seed, 40, 64) / 10f;
                float opacity = (float)Math.Pow(animationTime > 8 ? 1f - (animationTime - 8f) / 16f : 1f, 2f);
                var texture = TextureAssets.Coin[1];
                var frame = texture.Value.Frame(verticalFrames: 8, frameY: (int)((Main.GameUpdateCount / 10 + CoinAnimations[i] / 5) % 8));
                drawInfo.DrawDataCache.Add(
                    new DrawData(texture.Value, (drawLocation - Main.screenPosition).Floor(), frame, Helper.GetColor(drawLocation) * opacity, 0f, frame.Size() / 2f, 1f, drawInfo.drawPlayer.direction.ToSpriteEffect(), 0) { shader = 0, });
                CoinAnimations[i]++;
                if (animationTime > 24)
                {
                    CoinAnimations.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public class HighSteaksCritCoinSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.coinCrit with { PitchVariance = 0.2f };
        }
    }

    public class HighSteaksCoinSound : NetSound
    {
        protected override SoundStyle InitDefaultSoundStyle()
        {
            return AequusSounds.coinHit with { Volume = 0.2f, PitchVariance = 0.2f };
        }
    }
}

namespace Aequus
{
    public partial class AequusPlayer
    {
        public float highSteaksDamage;
        public int highSteaksCost;
        public bool highSteaksHide;

        public void ResetEffects_HighSteaks()
        {
            highSteaksHide = false;
            highSteaksCost = 0;
            highSteaksDamage = 0f;
        }

        public bool UseHighSteaks(Entity target, ref int damage, bool crit)
        {
            if (highSteaksDamage > 0f)
            {
                if (Main.rand.NextBool(8))
                {
                    ModContent.GetInstance<HighSteaksCoinSound>().Play(target.Center);
                }

                if (!crit)
                {
                    return false;
                }

                if (highSteaksCost > 0)
                {
                    if (!Player.CanBuyItem(highSteaksCost))
                    {
                        return false;
                    }
                    ModContent.GetInstance<HighSteaksCritCoinSound>().Play(target.Center);
                    Player.BuyItem(highSteaksCost);
                    if (HighSteaksMoneyConsumeEffect.CoinAnimations != null)
                    {
                        HighSteaksMoneyConsumeEffect.CoinAnimations.Add(Main.rand.Next((int)(MathHelper.TwoPi * 100f)) * 100);
                    }
                }
                damage = (int)(damage * (1f + highSteaksDamage / 2f));
                return true;
            }
            return false;
        }
    }
}