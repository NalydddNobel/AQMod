using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class AequusTexturesGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        var pngFiles = ConvertToTextureList(context);
        context.AddSource("AequusTexturesGenerator.cs", SourceText.From(
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
    }

    private const string TextureAssetField =
        """
        /// <summary>
                /// Full Path: $Path
                /// </summary>
                public static readonly TextureAsset $Name = new TextureAsset("$Path");
        """;

    private record struct TextureFile(string Path, string Name);

    private static List<TextureFile> ConvertToTextureList(GeneratorExecutionContext context)
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
}