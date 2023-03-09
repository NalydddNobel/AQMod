using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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

                    $TextureFields
                }
            }
            """
            .Replace("$Count", pngFiles.Count().ToString())
            .Replace("$TextureFields", string.Join("\n        ", pngFiles.Select((t) => TextureAssetField.Replace("$Name", t.Name).Replace("$Path", t.Path)))),
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
                ))),
            //.Replace("$Errors", string.Join("\n", Errors)),
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

    private record struct TextureFile(string Path, string Name);
    private record struct SoundFile(string Path, string Name, int Amount);

    private void AddError(Exception ex, string assetName)
    {
        Errors.Add($"{assetName}: {ex.Message}\n{ex.StackTrace}");
    }

    private List<TextureFile> ConvertToTextureList(GeneratorExecutionContext context)
    {
        var l = new List<TextureFile>();
        var quickLookup = new HashSet<string>();
        foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(".png")))
        {
            var safePath = file.Path.Replace('\\', '/');
            safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".png", "");

            var name = safePath.Substring(safePath.LastIndexOf('/') + 1);

        CheckName:
            int minDepth = 2;
            if (quickLookup.Contains(name))
            {
                var split = safePath.Split('/');
                int depth = Math.Max(split.Length - minDepth, 0);
                name = $"{name}_{split[depth]}";

                minDepth++;

                goto CheckName;
            }

            l.Add(new TextureFile(safePath, name));
            quickLookup.Add(name);
        }
        return l;
    }

    private string GetMultiSoundName(AdditionalText file)
    {
        string path = file.Path;
        path = path.Substring(path.LastIndexOf('\\') + 1);

        if (path.Length < 6)
            return path;

        path = path.Substring(0, path.Length - 5);
        return path;
    }

    private void CheckMultiSounds(ImmutableArray<AdditionalText> files, List<SoundFile> l, string name, string safePath)
    {
        try
        {
            if (name[name.Length - 1] == '0')
            {
                name = name.Substring(0, name.Length - 1);

                var list = files.Where(f => GetMultiSoundName(f) == name);
                int count = list.Count();

                if (count > 0)
                {
                    if (name[name.Length - 1] == '_')
                    {
                        name = name.Substring(0, name.Length - 1);
                    }
                    l.Add(new SoundFile(safePath.Substring(0, safePath.LastIndexOf('/')) + "/" + name, name, count));
                }
            }
        }
        catch (Exception ex)
        {
            AddError(ex, name);
        }
    }

    private List<SoundFile> ConvertToSoundList(GeneratorExecutionContext context)
    {
        var l = new List<SoundFile>();
        var quickLookup = new HashSet<string>();
        var files = context.AdditionalFiles;
        foreach (var file in files.Where(f => f.Path.EndsWith(".ogg")))
        {
            var safePath = file.Path.Replace('\\', '/');
            safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".ogg", "");

            var name = safePath.Substring(safePath.LastIndexOf('/') + 1);

        CheckName:
            int minDepth = 2;
            if (quickLookup.Contains(name))
            {
                var split = safePath.Split('/');
                int depth = Math.Max(split.Length - minDepth, 0);
                name = $"{name}_{split[depth]}";

                minDepth++;

                goto CheckName;
            }

            try
            {
                CheckMultiSounds(files, l, name, safePath);

                l.Add(new SoundFile(safePath, name, 1));
                quickLookup.Add(name);
            }
            catch (Exception ex)
            {
                AddError(ex, name);
            }
        }
        return l;
    }
}