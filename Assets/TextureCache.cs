using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.Exceptions;

namespace AQMod.Assets
{
    public sealed class TextureCache
    {
        public static TextureCache Cache { get; internal set; }
        private readonly Dictionary<string, TextureAsset> Textures;

        internal TextureCache()
        {
            Textures = new Dictionary<string, TextureAsset>();
            Setup();
        }

        public static TextureAsset GetAsset(string name)
        {
            if (Cache.Textures.ContainsKey(name))
                return Cache.Textures[name];
            return null;
        }
        internal static TextureAsset getAsset(string name)
        {
            return Cache.Textures[name];
        }

        public static string GetPath(string name)
        {
            if (Cache.Textures.ContainsKey(name))
                return Cache.Textures[name].Path();
            return null;
        }
        internal static string getPath(string name)
        {
            return Cache.Textures[name].Path();
        }

        public static Texture2D GetTexture(string name)
        {
            if (Cache.Textures.ContainsKey(name))
                return Cache.Textures[name].GetValue();
            return null;
        }
        internal static Texture2D getTexture(string name)
        {
            return Cache.Textures[name].GetValue();
        }

        private void Setup()
        {
            string path = "Assets/Textures.txt";
            string errorMessage = path;
            try
            {
                using (var stream = AQMod.Instance.GetFileStream(path))
                {
                    var reader = new StreamReader(stream);
                    while (true)
                    {
                        string text = reader.ReadLine();
                        errorMessage = text;
                        if (string.IsNullOrEmpty(text))
                            break;
                        if (text[0] == '#') // comment
                            continue;
                        var split = text.Split('=');
                        string key = split[0];
                        int i = key.Length - 1;
                        errorMessage = "removing whitespace from key: [" + key + "]";
                        while (true)
                        {
                            if (key[i] != ' ')
                            {
                                break;
                            }
                            i--;
                        }
                        errorMessage = "removing whitespace from key: [" + key + ", " + i + "]";
                        if (i != key.Length - 1)
                        {
                            string newKey = "";
                            for (int j = 0; j <= i; j++)
                            {
                                newKey += key[j];
                            }
                            key = newKey;
                        }
                        string texturePath = split[1];
                        i = 0;
                        errorMessage = "removing whitespace from path: [" + texturePath + "]";
                        while (true)
                        {
                            if (texturePath[i] != ' ')
                            {
                                break;
                            }
                            i++;
                        }
                        errorMessage = "removing whitespace from path: [" + texturePath + ", " + i + "]";
                        if (i != 0)
                        {
                            string newPath = "";
                            for (int j = i; j < texturePath.Length; j++)
                            {
                                newPath += texturePath[j];
                            }
                            texturePath = newPath;
                        }
                        errorMessage = "Final result error: [" + key + "." + texturePath + "]";
                        Textures.Add(key, new TextureAsset("AQMod/Assets/" + texturePath));
                        errorMessage = "Unknown Error";
                    }
                }
            }
            catch (Exception e)
            {
                throw new MissingResourceException("Error: " + errorMessage, e);
            }
            logTextures();
        }

        private void logTextures()
        {
            foreach (var pair in Textures)
            {
                AQMod.Instance.Logger.Debug("[" + pair.Key + "-" + pair.Value.Path() + "]");
            }
        }
    }
}