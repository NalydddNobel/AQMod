using Aequus.Core;
using Aequus.Core.ContentGeneration;
using Aequus.Core.UI;
using System;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Aequus.Old.Content.Items.Materials.Energies;

public class EnergyMaterial : ModSystem {
    public static ModItem Aquatic { get; private set; }
    public static ModItem Demonic { get; private set; }
    public static ModItem Cosmic { get; private set; }
    public static ModItem Atmospheric { get; private set; }
    public static ModItem Organic { get; private set; }
    public static ModItem Ultimate { get; private set; }

    public override void Load() {
        AddEnergy(nameof(Aquatic), Commons.Rare.BossSalamancer + 1, new Vector3(0.05f, 0.3f, 0.33f));
        AddEnergy(nameof(Demonic), Commons.Rare.BossWallOfFlesh + 1, new Vector3(0.5f, 0.1f, 0.1f));
        AddEnergy(nameof(Cosmic), Commons.Rare.BossOmegaStarite + 1, new Vector3(0.3f, 0.3f, 0.8f));
        AddEnergy(nameof(Atmospheric), Commons.Rare.BossDustDevil + 1, new Vector3(0.33f, 0.15f, 0.05f));
        AddEnergy(nameof(Organic), Commons.Rare.BossPlantera + 1, new Vector3(0.1f, 0.4f, 0.12f));
        AddEnergy(nameof(Ultimate), Commons.Rare.BossMoonLord, new Vector3(0.33f, 0.33f, 0.33f), ItemSets.SortingPriorityMaterials[ItemID.LunarBar]);

        void AddEnergy(string name, int rarity, Vector3 lightColor, int? sortingPriority = null) {
            int realSortingPrority = sortingPriority ?? ItemSets.SortingPriorityMaterials[ItemID.SoulofLight] - 1;

            InstancedEnergyItem item = new InstancedEnergyItem(name, rarity, lightColor, realSortingPrority);

            typeof(EnergyMaterial).GetProperty(name).SetValue(this, item);
            Mod.AddContent(item);
        }
    }

    public override void AddRecipes() {
        Ultimate.CreateRecipe()
            .AddIngredient(Aquatic)
            .AddIngredient(Demonic)
            .AddIngredient(Cosmic)
            .AddIngredient(Atmospheric)
            .AddIngredient(Organic)
            .AddIngredient(ItemID.LunarBar, 8)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void Unload() {
        Aquatic = null;
        Atmospheric = null;
        Cosmic = null;
        Demonic = null;
        Organic = null;
        Ultimate = null;
    }

    private class InstancedEnergyItem : InstancedModItem {
        private readonly Vector3 _lightXYZ;
        private readonly Color _lightRGB;
        private readonly int _rarity;
        private readonly int _sortingPriority;

        public InstancedEnergyItem(string name, int itemRarity, Vector3 lightColor, int sortingPriority) : base($"{name}Energy", $"{typeof(EnergyMaterial).NamespaceFilePath()}/{name}Energy") {
            _lightXYZ = lightColor;
            _lightRGB = new Color(_lightXYZ);
            _rarity = itemRarity;
            _sortingPriority = sortingPriority;
        }

        public override void SetStaticDefaults() {
            ItemSets.SortingPriorityMaterials[Type] = _sortingPriority;
            ItemSets.ItemNoGravity[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.rare = _rarity;
            Item.value = Item.sellPrice(gold: 1);
            Item.maxStack = Item.CommonMaxStack;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            DrawRays(spriteBatch, position, scale);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            float lensFlareScale = 1.2f;
            if (!Main.playerInventory || !InventoryUI.ContextsInv.Contains(CurrentSlot.Instance.Context)) {
                lensFlareScale = 0f;
            }
            DrawGodItem(spriteBatch, TextureAssets.Item[Type].Value, frame, origin, position, 0f, scale, lensFlareScale);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
            Vector2 origin = frame.Size() / 2f;
            Vector2 position = GetWorldDrawCoordinates(origin);

            DrawRays(spriteBatch, position, scale, randomizer: whoAmI);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
            Main.GetItemDrawFrame(Type, out Texture2D texture, out Rectangle frame);
            Vector2 origin = frame.Size() / 2f;
            Vector2 position = GetWorldDrawCoordinates(origin);

            DrawGodItem(spriteBatch, texture, frame, origin, position, rotation, scale, lensFlareScale: 1f);

            DrawHelper.DrawMagicLensFlare(spriteBatch, position, _lightRGB with { A = 0 } * 0.9f * GetPulseOscillation(), 0.7f);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed) {
            if (Item.timeSinceItemSpawned % 18 == 0) {
                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height - 4, ModContent.DustType<EnergyDust>(), 0f, 0f, 0, (_lightRGB * 2f).HueAdd(Main.rand.NextFloat(-0.05f, 0.05f)).SaturationMultiply(0.3f) with { A = 0 });
                d.velocity += Vector2.Normalize(d.position - Item.Center) * d.velocity.Length();
                d.alpha = Main.rand.Next(0, 35);
                d.scale = Main.rand.NextFloat(0.95f, 1.15f);
                if (d.scale > 1f)
                    d.noGravity = true;
                d.velocity *= 0.5f;
                d.velocity += Item.velocity * -0.2f;
            }
            Lighting.AddLight(Item.position, _lightXYZ);
        }

        public override void GrabRange(Player player, ref int grabRange) {
            grabRange += 96;
        }

        private void DrawGodItem(SpriteBatch spriteBatch, Texture2D texture, Rectangle frame, Vector2 origin, Vector2 position, float rotation, float scale, float lensFlareScale) {
            float pulse = GetPulseOscillation();

            spriteBatch.Draw(texture, position, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, position, frame, new Color(80, 80, 80, 0) * pulse, 0f, origin, scale * (1f + pulse * 0.2f), SpriteEffects.None, 0f);

            if (HighQualityEffects && lensFlareScale > 0f) {
                spriteBatch.Draw(AequusTextures.LensFlare, position - new Vector2(0f, 4f) * Main.inventoryScale, null, _lightRGB with { A = 0 } * pulse, -0.1f, AequusTextures.LensFlare.Size() / 2f, scale * lensFlareScale * (0.8f + pulse * 0.2f), SpriteEffects.None, 0f);
            }
        }

        private void DrawRays(SpriteBatch spriteBatch, Vector2 position, float scale, int randomizer = 0) {
            if (!HighQualityEffects) {
                return;
            }

            float pulse = GetPulseOscillation(0.5f, 1f);

            Texture2D shadow = AequusTextures.Bloom;
            spriteBatch.Draw(shadow, position, null, new Color(0, 0, 0, 200) * pulse, 0f, shadow.Size() / 2f, scale * 0.55f, SpriteEffects.None, 0f);

            FastRandom rand = new FastRandom(Type).WithModifier((ulong)randomizer);

            Texture2D rayTexture = AequusTextures.LightRayFlat;
            Vector2 rayOrigin = rayTexture.Size() / 2f;
            Color rayColor = _lightRGB with { A = 0 };
            int amt = 8;
            float rot = MathHelper.TwoPi / amt;
            for (int i = 0; i < amt; i++) {
                int direction = i % 2 == 0 ? 1 : -1;
                float rayOpacity = Helper.Oscillate(Main.GlobalTimeWrappedHourly * rand.NextFloat(2f, 4f), 0.1f, 0.6f) * MathF.Pow(pulse, 3f);
                float rayRotation = Main.GlobalTimeWrappedHourly * rand.NextFloat(0.01f, 0.05f) * direction;
                Vector2 rayScale = new Vector2(0.3f, 1.3f) * rand.NextFloat(0.3f, 0.5f) * scale;
                spriteBatch.Draw(rayTexture, position, null, rayColor * rayOpacity, rayRotation, rayOrigin, rayScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(rayTexture, position, null, Color.White * rayOpacity, rayRotation, rayOrigin, rayScale * 0.6f, SpriteEffects.None, 0f);
            }
        }

        private Vector2 GetWorldDrawCoordinates(Vector2 origin) {
            float y = GetYOscilliation();
            return new Vector2(Item.position.X - Main.screenPosition.X + origin.X + Item.width / 2 - origin.X, Item.position.Y - Main.screenPosition.Y + origin.Y + Item.height - origin.Y * 2f + 4f + y * 1.5f).Floor();
        }

        private float GetYOscilliation() {
            return MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + Type * 10f);
        }

        private float GetPulseOscillation(float min = 0f, float max = 1f) {
            return Helper.Oscillate(Main.GlobalTimeWrappedHourly * 1.25f + Type * 10, min, max);
        }
    }
}
