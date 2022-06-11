using Aequus.Buffs;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public sealed class Reliefshroom : ModItem, Hooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var relief = player.GetModPlayer<ReliefshroomPlayer>();
            relief.Add(regen: 12);

            if (relief.EffectActive)
            {
                Lighting.AddLight(player.Center, Color.Violet.ToVector3() * 0.5f);
                if (Main.rand.NextBool(12))
                {
                    var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    var d = Dust.NewDustPerfect(relief.Player.Center + v * Main.rand.NextFloat(player.width * 0.8f, player.width * 2f), ModContent.DustType<ReliefshroomDustSpore>(), -v * Main.rand.NextFloat(0.1f, 1f), 255, Scale: Main.rand.NextFloat(0.6f, 0.7f));
                    if (relief.cReliefshroom != 0)
                    {
                        d.shader = GameShaders.Armor.GetSecondaryShader(relief.cReliefshroom, player);
                    }
                }
                if (Main.GameUpdateCount % 60 == 0)
                {
                    foreach (var v in AequusHelpers.CircularVector(10, Main.rand.NextFloat(MathHelper.TwoPi)))
                    {
                        var d = Dust.NewDustPerfect(relief.Player.Center + v * Main.rand.NextFloat(player.width * 2.4f, player.width * 2.6f), ModContent.DustType<ReliefshroomDustSpore>(), -v * Main.rand.NextFloat(0.9f, 1.1f), 255);
                        d.customData = player;
                        if (relief.cReliefshroom != 0)
                        {
                            d.shader = GameShaders.Armor.GetSecondaryShader(relief.cReliefshroom, player);
                        }
                    }
                }
            }
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return CheckMendshroom(equippedItem) && CheckMendshroom(incomingItem);
        }
        public bool CheckMendshroom(Item item)
        {
            return item.type != ModContent.ItemType<Mendshroom>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Mendshroom>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.GetModPlayer<ReliefshroomPlayer>().cReliefshroom = dyeItem.dye;
        }
    }

    /// <summary>
    /// Used by <see cref="Reliefshroom"/>
    /// </summary>
    public sealed class ReliefshroomPlayer : ModPlayer
    {
        public int regenerationToGive;
        public int increasedRegen;

        public int cReliefshroom;

        public bool EffectActive => Player.GetModPlayer<MendshroomPlayer>().idleTime <= 5 && Player.velocity.X.Abs() > 2f; // abt walking speed

        public override void ResetEffects()
        {
            regenerationToGive = 0;
            cReliefshroom = 0;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (ReliefshroomPlayer)clientClone;
            clone.regenerationToGive = regenerationToGive;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
        }

        public override void UpdateDead()
        {
            regenerationToGive = 0;
            increasedRegen = 0;
        }

        public override void UpdateLifeRegen()
        {
            Player.AddLifeRegen(increasedRegen);
            increasedRegen = 0;
        }

        public override void PostUpdateEquips()
        {
            if (regenerationToGive > 0 && EffectActive)
            {
                HealPlayer(Player.whoAmI);
            }
        }

        public void Add(int regen)
        {
            regenerationToGive = Math.Max(regenerationToGive, regen);
        }

        public void HealPlayer(int i)
        {
            var wungus = Main.player[i].GetModPlayer<ReliefshroomPlayer>();
            if (wungus.increasedRegen < regenerationToGive)
            {
                wungus.increasedRegen = regenerationToGive;
                Main.player[i].AddBuff(ModContent.BuffType<ReliefshroomBuff>(), 2);
            }
        }
    }
}