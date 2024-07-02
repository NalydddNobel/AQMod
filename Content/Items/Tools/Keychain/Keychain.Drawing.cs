using Aequus.Core.Graphics;
using Aequus.Core.Structures;
using Aequus.DataSets;
using System;

namespace Aequus.Content.Items.Tools.Keychain;

public partial class Keychain {
    internal static Paletter KeyTextures;
    internal static readonly ConversionPool<int, string> KeyTextureNames = new((i) => {
        if (!ItemID.Search.TryGetName(i, out var name)) {
            return "Unknown";
        }

        name = name.Replace("Aequus/", "");
        return $"Aequus/Assets/Textures/Keychain/{name}Palette";
    });

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        KeychainPlayer keychain = Main.LocalPlayer.GetModPlayer<KeychainPlayer>();
        if (keychain.sortedKeysForIcons == null || keychain.sortedKeysForIcons.Count == 0) {
            return;
        }

        DrawKeys(keychain, spriteBatch, position, drawColor, 0f, scale);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        KeychainPlayer keychain = Main.LocalPlayer.GetModPlayer<KeychainPlayer>();
        if (keychain.sortedKeysForIcons == null || keychain.sortedKeysForIcons.Count == 0) {
            return;
        }

        Main.GetItemDrawFrame(Type, out _, out Rectangle frame);
        Vector2 drawCoordinates = ExtendItem.WorldDrawPos(Item, frame);

        DrawKeys(keychain, spriteBatch, drawCoordinates, lightColor, rotation, scale);
    }

    protected void DrawKeys(KeychainPlayer keychain, SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, float rotation, float scale) {
        int count = Math.Min(keychain.sortedKeysForIcons.Count, KEYS_FRAME_COUNT);

        drawCoordinates.Y -= 2f * scale;
        int keyIndex = 0;
        int frameIndex = 0;
        int stackLeft = KEYS_FRAME_COUNT - count;
        int currentItemStack = 0;
        while (keyIndex < count) {
            Item key = keychain.sortedKeysForIcons[keyIndex];
            Rectangle frame = AequusTextures.KeychainKeysTemplate.Frame(verticalFrames: KEYS_FRAME_COUNT, frameY: frameIndex);
            //frame.Height -= 2;
            //if (ItemDataSet.KeychainData.TryGetValue(, out KeychainInfo value)) {
            //    keyColor = Utils.MultiplyRGBA(keyColor, value.Color);
            //}

            GetKeyDrawData(key.type, out Texture2D keyTexture, out Color keyColor);
            keyColor = Utils.MultiplyRGBA(keyColor, drawColor);
            spriteBatch.Draw(keyTexture, drawCoordinates, frame, keyColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);

            frameIndex++;
            if (stackLeft > 0 && key.stack > 1 && currentItemStack != 1) {
                if (currentItemStack == 0) {
                    currentItemStack = key.stack;
                }
                currentItemStack--;
                stackLeft--;
            }
            else {
                currentItemStack = 0;
                keyIndex++;
            }
        }
    }

    protected void GetKeyDrawData(int keyId, out Texture2D texture, out Color color) {
        color = Color.White;

        string name = KeyTextureNames.Get(keyId);
        if (ModContent.RequestIfExists<Texture2D>(name, out var keyPalette) && keyPalette.IsLoaded && KeyTextures.TryGetValueOrRequest(keyPalette.Value, out Texture2D value)) {
            texture = value;
            return;
        }

        if (ItemDataSet.KeychainData.TryGetValue(keyId, out KeychainInfo keychainInfo)) {
            color = keychainInfo.Color;
        }

        texture = AequusTextures.KeychainKeys;
    }
}
