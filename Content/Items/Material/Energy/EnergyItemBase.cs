using Aequus.Common.Items;
using Aequus.Common.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;

namespace Aequus.Content.Items.Material.Energy;

public abstract class EnergyItemBase<T> : ModItem where T : EnergyParticle<T>, new() {
    protected abstract Vector3 LightColor { get; }
    public abstract int Rarity { get; }

    public override void SetStaticDefaults() {
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Energies;
        ItemID.Sets.ItemNoGravity[Type] = true;
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 30;
        Item.rare = Rarity;
        Item.value = Item.sellPrice(silver: 10);
        Item.maxStack = Item.CommonMaxStack;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 255, 255, 255);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        var color = itemColor.A > 0 ? itemColor : drawColor;
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        return false;
    }

    // In PostDraw to fix an issue with that one mod which adds auras around dropped items
    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        var itemTexture = TextureAssets.Item[Type].Value;
        var frame = new Rectangle(0, 0, itemTexture.Width, itemTexture.Height);
        var origin = frame.Size() / 2f;
        float y = MathF.Sin(Main.GlobalTimeWrappedHourly * 2f);
        var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + origin.X + Item.width / 2 - origin.X, Item.position.Y - Main.screenPosition.Y + origin.Y + Item.height - frame.Height + 4f + y * 1.5f).Floor();

        spriteBatch.Draw(itemTexture, drawPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
    }

    public override void Update(ref float gravity, ref float maxFallSpeed) {
        if (Main.netMode != NetmodeID.Server) {
            if (Item.timeSinceItemSpawned % 24 == 0) {
                ParticleSystem.New<T>(ParticleLayer.AboveItems).Setup(Item.position + new Vector2(Main.rand.Next(Item.width), Main.rand.NextFloat(8f, Item.height * 0.66f) - 8f), -Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.22f), new Color(LightColor * 2f) with { A = 0 }, Main.rand.NextFloat(0.8f, 1.1f));
            }
            if (Item.timeSinceItemSpawned % 32 == 0) {
                ParticleSystem.New<T>(ParticleLayer.AboveNPCs).Setup(Item.position + new Vector2(Main.rand.Next(Item.width), Main.rand.NextFloat(8f, Item.height * 0.66f) - 8f), -Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.22f), new Color(LightColor * 0.75f) with { A = 0 }, Main.rand.NextFloat(0.8f, 1.1f));
            }
        }
        Lighting.AddLight(Item.Center, LightColor);
    }
}