using Aequus.Buffs;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public sealed class Reliefshroom : ModItem
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

            if (relief.EffectActive && Main.rand.NextBool(12))
            {
                var d = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<MonoDust>(), newColor: Color.Purple.UseA(0) * 0.8f);
                d.velocity *= 0.2f;
                d.velocity -= player.velocity * 0.2f;
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
    }

    /// <summary>
    /// Used by <see cref="Reliefshroom"/>
    /// </summary>
    public sealed class ReliefshroomPlayer : ModPlayer
    {
        public int regenerationToGive;
        public int increasedRegen;

        public bool EffectActive => Player.GetModPlayer<MendshroomPlayer>().idleTime == 0 && Player.velocity.X.Abs() > 3.8f; // abt walking speed

        public override void ResetEffects()
        {
            regenerationToGive = 0;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (MendshroomPlayer)clientClone;
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
            if (EffectActive)
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