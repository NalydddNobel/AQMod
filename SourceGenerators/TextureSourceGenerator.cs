using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class TextureSourceGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        var files = Utilities.GetFiles(context, "png", GetAssetFile);
        Utilities.FixFileNames(files, static f => f.Name = f.Name = f.Path.Replace("/", "_"));
        context.AddSource("AequusTextures.cs", SourceText.From(
            $$"""
                using Aequus.Common.Assets;
                using Microsoft.Xna.Framework.Graphics;
                using System.Runtime.CompilerServices;

                namespace Aequus;

                /// <summary>(Total Textures: {{files.Count}})</summary>
                [CompilerGenerated]
                public partial class AequusTextures {            
                    {{string.Join("\n", files.Select((file) =>
                    $"""
                        /// <summary>Full Path: {file.Path}</summary>
                        public static readonly RequestCache<Texture2D> {file.Name} = new("{file.Path}");
                    """))}}
                }
                """, Encoding.UTF8)
        );
    }

    private static AssetFile GetAssetFile(string name, string path) {
        if (name.Contains('.')) {
            return null;
        }

        return new AssetFile(name, path);
    }
}