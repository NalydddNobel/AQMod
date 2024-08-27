using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace SourceGenerators;

[System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class ModCallAttribute(string Name = "") : System.Attribute {
    public readonly string Name = Name;

    public const string FullyQualifiedName = $"{Namespace}.{nameof(ModCallAttribute)}";

    internal const string Quine =
    $$"""
    namespace {{Namespace}};
        
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ModCallAttribute(string Name = "") : System.Attribute {
        public readonly string Name = Name;
    }
    """;

    internal static void GetAttributeParams(AttributeData attr, ref string Name) {
        if (!attr.ConstructorArguments.IsEmpty) {
            ImmutableArray<TypedConstant> args = attr.ConstructorArguments;

            foreach (TypedConstant arg in args) {
                if (arg.Kind == TypedConstantKind.Error) {
                    return;
                }
            }

            switch (args.Length) {
                case 1:
                    string result = (string)args[0].Value;
                    if (!string.IsNullOrWhiteSpace(result)) {
                        Name = result;
                    }
                    break;
            }
        }

        if (!attr.NamedArguments.IsEmpty) {
            foreach (var arg in attr.NamedArguments) {
                TypedConstant typedConstant = arg.Value;
                if (typedConstant.Kind == TypedConstantKind.Error) {
                    return;
                }

                switch (arg.Key) {
                    case "Name":
                        string result = (string)typedConstant.Value;
                        if (!string.IsNullOrWhiteSpace(result)) {
                            Name = result;
                        }
                        break;
                }
            }
        }
    }
}

[Generator]
internal class ModCallAttributeGenerator : IIncrementalGenerator {
    void IIncrementalGenerator.Initialize(IncrementalGeneratorInitializationContext context) {
        // Create marker attribute.
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{nameof(ModCallAttribute)}.g.cs",
            SourceText.From(ModCallAttribute.Quine, Encoding.UTF8)));
    }
}
