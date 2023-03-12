using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class Generator : ISourceGenerator
{
    public List<string> Errors { get; private set; }

    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        Errors = new List<string>();
        var pngFiles = ConvertToTextureList(context);
        context.AddSource("AequusTextures.cs", SourceText.From(
            """
            using System.Runtime.CompilerServices;
            using Terraria.ModLoader;
            using Aequus.Common;

            namespace Aequus
            {
                /// <summary>
                /// (Amt Textures: $Count)
                /// </summary>
                public class AequusTextures : ILoadable
                {
                    public void Load(Mod mod)
                    {
                    }

                    public void Unload()
                    {
                        foreach (var f in GetType().GetFields())
                        {
                            ((TextureAsset)f.GetValue(this))?.Unload();
                        }
                    }

                    $Errors
            
                    $TextureFields
                }
            }
            """
            .Replace("$Count", pngFiles.Count().ToString())
            .Replace("$TextureFields", string.Join("\n        ", pngFiles.Select((t) => TextureAssetField.Replace("$Name", t.Name).Replace("$Path", t.Path))))
            .Replace("$Errors", string.Join("\n", Errors))
            ,
            Encoding.UTF8));

        Errors.Clear();
        var soundFiles = ConvertToSoundList(context);
        context.AddSource("AequusSounds.cs", SourceText.From(
            """
            using System.Runtime.CompilerServices;
            using Terraria.ModLoader;
            using Aequus.Common;

            namespace Aequus
            {
                /// <summary>
                /// (Amt Sounds: $Count)
                /// </summary>
                public class AequusSounds : ILoadable
                {                    
                    public void Load(Mod mod)
                    {
                    }

                    public void Unload()
                    {
                        foreach (var f in GetType().GetFields())
                        {
                            ((SoundAsset)f.GetValue(this))?.Unload();
                        }
                    }

                    $SoundFields
                }
            }
            """
            .Replace("$Count", soundFiles.Count().ToString())
            .Replace("$SoundFields", string.Join("\n        ",
                soundFiles.Select((t) => SoundAssetField
                .Replace("$Name", t.Name)
                .Replace("$Path", t.Path)
                .Replace("$SoundCount", t.Amount.ToString())
                )))
            //.Replace("$Errors", string.Join("\n", Errors))
            ,
            Encoding.UTF8));
    }

    private const string TextureAssetField =
        """
        /// <summary>
                /// Full Path: $Path
                /// </summary>
                public static readonly TextureAsset $Name = new TextureAsset("$Path");
        """;
    private const string SoundAssetField =
        """
        /// <summary>
                /// Full Path: $Path
                /// <para>Num Variants: $SoundCount</para>
                /// </summary>
                public static readonly SoundAsset $Name = new SoundAsset("$Path", $SoundCount);
        """;

    private record struct TextureFile
    {
        public readonly int uniqueIdentifier;
        private static int UniqueIdentifier;
        public readonly string Path;
        public readonly string Name;

        public TextureFile(string Path, string Name)
        {
            this.Path = Path;
            this.Name = Name;
            uniqueIdentifier = UniqueIdentifier++;
        }

        public override int GetHashCode()
        {
            return uniqueIdentifier;
        }
    }
    private record struct SoundFile(string Path, string Name, int Amount);

    private void AddError(Exception ex, string assetName)
    {
        Errors.Add($"{assetName}: {ex.Message}\n{ex.StackTrace}");
    }

    private List<TextureFile> ConvertToTextureList(GeneratorExecutionContext context)
    {
        var textureFiles = new List<TextureFile>();
        var quickLookup = new HashSet<string>();
        foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(".png")))
        {
            var safePath = file.Path.Replace('\\', '/');
            safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".png", "");

            var name = safePath.Substring(safePath.LastIndexOf('/') + 1);

            TextureFile textureFile = new TextureFile(safePath, name);
            textureFiles.Add(textureFile);
            //Errors.Add($"{textureFile.Name}: {textureFile.uniqueIdentifier}");
        }

        // Fix names
        List<int> others = new();
        for (int i = 0; i < textureFiles.Count; i++)
        {
            var textureFile = textureFiles[i];
            for (int j = 0; j < textureFiles.Count; j++)
            {
                if (textureFiles[j].Name == textureFile.Name)
                {
                    others.Add(j);
                }
            }
            if (others.Count > 1)
            {
                foreach (var j in others)
                {
                    var otherTextureFile = textureFiles[j];
                    // Has to use indexes, since List<T>.Remove bugged out!
                    textureFiles.RemoveAt(j);
                    string path = otherTextureFile.Path.Substring(0, otherTextureFile.Path.LastIndexOf('/'));
                    textureFiles.Add(new(otherTextureFile.Path, otherTextureFile.Name + "_" + path.Substring(path.LastIndexOf('/') + 1)));
                }
            }
            others.Clear();
        }

        return textureFiles;
    }

    private List<SoundFile> ConvertToSoundList(GeneratorExecutionContext context)
    {
        var soundFiles = new List<SoundFile>();
        var files = context.AdditionalFiles;
        var audioFiles = files.Where(f => f.Path.EndsWith(".ogg"));

        try
        {
            // Generate basic list
            foreach (var file in audioFiles)
            {
                var safePath = file.Path.Replace('\\', '/');
                safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".ogg", "");

                var name = safePath.Substring(safePath.LastIndexOf('/') + 1);
                soundFiles.Add(new SoundFile(safePath, name, 1));
            }

            // Handle multi-sounds
            int amount = soundFiles.Count;
            for (int i = 0; i < amount; i++)
            {
                var soundFile = soundFiles[i];
                if (soundFile.Path[soundFile.Path.Length - 1] == '0')
                {
                    string subPath = soundFile.Path.Substring(0, soundFile.Path.Length - 1);
                    int count = soundFiles.Where(f => f.Path.Substring(0, f.Path.Length - 1) == subPath).Count();

                    if (count > 1)
                    {
                        string rename = subPath.Substring(subPath.LastIndexOf('/') + 1);
                        int underscore = soundFile.Name.IndexOf('_');
                        if (underscore > 0)
                        {
                            rename += soundFile.Name.Substring(underscore);
                        }
                        soundFiles.Add(new SoundFile(subPath, rename, count));
                    }
                }
            }

            // Fix names
            List<int> others = new();
            for (int i = 0; i < soundFiles.Count; i++)
            {
                var soundFile = soundFiles[i];
                for (int j = 0; j < soundFiles.Count; j++)
                {
                    if (soundFiles[j].Name == soundFile.Name)
                    {
                        others.Add(j);
                    }
                }
                if (others.Count > 1)
                {
                    foreach (var j in others)
                    {
                        var otherSoundFile = soundFiles[j];
                        // Has to use indexes, since List<T>.Remove bugged out!
                        soundFiles.RemoveAt(j);
                        string path = otherSoundFile.Path.Substring(0, otherSoundFile.Path.LastIndexOf('/'));
                        soundFiles.Add(new(otherSoundFile.Path, otherSoundFile.Name + "_" + path.Substring(path.LastIndexOf('/') + 1), otherSoundFile.Amount));
                    }
                }
                others.Clear();
            }
        }
        catch (Exception ex)
        {
            AddError(ex, "Fatal Error!");
        }

        return soundFiles;
    }
}