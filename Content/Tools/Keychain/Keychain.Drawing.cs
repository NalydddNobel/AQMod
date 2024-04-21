using Aequus.Core;
using Aequus.Core.Graphics;
using Aequus.DataSets;
using System;

namespace Aequus.Content.Tools.Keychain;

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
        if (_sortedKeyIcons == null || _sortedKeyIcons.Count == 0) {
            return;
        }

        //spriteBatch.End();
        //spriteBatch.BeginUI(immediate: true, useScissorRectangle: true);

        DrawKeys(spriteBatch, position, drawColor, 0f, scale);

        //spriteBatch.End();
        //spriteBatch.BeginUI(immediate: false, useScissorRectangle: true);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        if (_sortedKeyIcons == null || _sortedKeyIcons.Count == 0) {
            return;
        }

        Main.GetItemDrawFrame(Type, out _, out Rectangle frame);
        Vector2 drawCoordinates = ExtendItem.WorldDrawPos(Item, frame);

        //spriteBatch.End();
        //spriteBatch.BeginWorld(shader: true);

        DrawKeys(spriteBatch, drawCoordinates, lightColor, rotation, scale);

        //spriteBatch.End();
        //spriteBatch.BeginWorld(shader: false);
    }

    protected void DrawKeys(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, float rotation, float scale) {
        int count = Math.Min(_keys.Count, KEYS_FRAME_COUNT);

        drawCoordinates.Y -= 2f * scale;
        for (int i = 0; i < count; i++) {
            Item key = _sortedKeyIcons[i];
            Rectangle frame = AequusTextures.KeychainKeysTemplate.Frame(verticalFrames: KEYS_FRAME_COUNT, frameY: i);
            //frame.Height -= 2;
            //if (ItemDataSet.KeychainData.TryGetValue(, out KeychainInfo value)) {
            //    keyColor = Utils.MultiplyRGBA(keyColor, value.Color);
            //}

            GetKeyDrawData(key.type, out Texture2D keyTexture, out Color keyColor);
            keyColor = Utils.MultiplyRGBA(keyColor, drawColor);
            spriteBatch.Draw(keyTexture, drawCoordinates, frame, keyColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f); ;
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
