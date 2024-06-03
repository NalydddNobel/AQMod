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
        var files = Utilities.GetFiles<AssetFile>(context, "png", static s => new(s));
        Utilities.FixFileNames(files, static f => f.Name = f.Name = f.Path.Replace("/", "_"));
        context.AddSource("AequusTextures.cs", SourceText.From(
            $$"""
                using Aequus.Core.Assets;
                using Microsoft.Xna.Framework.Graphics;
                using System.Runtime.CompilerServices;

                namespace Aequus;

                /// <summary>(Total Textures: {{files.Count}}) yo momma gai</summary>
                [CompilerGenerated]
                public partial class AequusTextures {            
                    {{string.Join("\n", files.Select((file) =>
                    $"""
                        /// <summary>Full Path: {file.Path}</summary>
                        public static readonly RequestCache<Texture2D> {file.Name} = new("Aequus/{file.Path}");
                    """))}}
                }
                """, Encoding.UTF8)
        );
    }
}