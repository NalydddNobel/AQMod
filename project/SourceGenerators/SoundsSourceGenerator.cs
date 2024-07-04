using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class SoundsSourceGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        var files = Utilities.GetFiles<AssetFile>(context, "ogg", static s => new(s));
        context.AddSource("AequusSounds.cs", SourceText.From(
            $$"""
                using Terraria.Audio;
                using System.Runtime.CompilerServices;

                namespace AequusRemake;

                /// <summary>(Total Sounds: {{files.Count}})</summary>
                [CompilerGenerated]
                public partial class AequusSounds {            
                    {{string.Join("\n", files.Select((file) =>
                    $"""
                        /// <summary>Full Path: {file.Path}</summary>
                        public static readonly SoundStyle {file.Name} = new("{file.Path}");
                    """))}}
                }
                """, Encoding.UTF8)
        );
    }
}