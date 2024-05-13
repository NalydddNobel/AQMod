using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace SourceGenerators;

[Generator]
public class AequusPlayerGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        TypeConstructor player = new("AequusPlayer", "Aequus", "Terraria.ModLoader.IO");

        foreach (ClassDeclarationSyntax definedClass in context.Compilation.SyntaxTrees.SelectMany(tree => tree.GetRoot(context.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())) {
            if (context.CancellationToken.IsCancellationRequested) {
                return;
            }

            //_aequusPlayer.AddField(definedClass.Parent?.GetType()?.Name ?? "Null", "parent");

            if (definedClass.Parent?.IsKind(SyntaxKind.NamespaceDeclaration) == true) {
                continue;
            }
            
            //string namespaceIdentifier = Utilities.ConvertNameSyntax(definedClass.Parent);

            foreach (var attr in definedClass.AttributeLists.SelectMany(list => list.Attributes)) {
                string attributeName = Utilities.ConvertNameSyntax(attr.Name);

                switch (attributeName) {
                    // Add a field.
                    case "PlayerField": {
                            player.AddField(attr.GetTextArgument(0), attr.GetTextArgument(1));
                        }
                        break;

                    // Add a field, and code to Save/Load
                    case "SavedPlayerField": {
                            string name = attr.GetTextArgument(0);
                            player.AddField(name, attr.GetTextArgument(1));
                            player.AddMethod("SaveInner(TagCompound tag)", $"SaveObj(tag, \"{name}\", {name});");
                            player.AddMethod("LoadInner(TagCompound tag)", $"LoadObj(tag, \"{name}\", ref {name});");
                        }
                        break;

                    // Add a field, and code to ResetEffects
                    case "ResetPlayerField": {
                            string name = attr.GetTextArgument(0);
                            player.AddField(name, attr.GetTextArgument(1));
                            player.AddMethod("ResetEffectsInner()", $"ResetObj(ref {name});");
                        }
                        break;

                    // Add a field, and code to ResetInfoAccessories
                    case "InfoPlayerField": {
                            string name = attr.GetTextArgument(0);
                            player.AddField(name, "bool");
                            player.AddMethod("ResetInfoAccessoriesInner()", $"ResetObj(ref {name});");
                            player.AddMethod("MatchInfoAccessoriesInner(AequusPlayer other)", $"{name} |= other.{name};");
                        }
                        break;
                }
            }
        }

        player.AddSource(in context);
    }
}

