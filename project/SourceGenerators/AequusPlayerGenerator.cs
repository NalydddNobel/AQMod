using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace SourceGenerators;

[Generator]
public class AequusPlayerGenerator : ISourceGenerator {
    private readonly TypeConstructor _aequusPlayer = new("AequusPlayer", "Aequus", "Terraria.ModLoader.IO");

    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        _aequusPlayer.Clear();

        foreach (ClassDeclarationSyntax definedClass in context.Compilation.SyntaxTrees.SelectMany(tree => tree.GetRoot(context.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())) {

            //_aequusPlayer.AddField(definedClass.Parent?.GetType()?.Name ?? "Null", "parent");

            if (definedClass.Parent?.IsKind(SyntaxKind.NamespaceDeclaration) == true) {
                continue;
            }
            
            //string namespaceIdentifier = Utilities.ConvertNameSyntax(definedClass.Parent);

            foreach (var attr in definedClass.AttributeLists.SelectMany(list => list.Attributes)) {
                string attributeName = Utilities.ConvertNameSyntax(attr.Name);

                switch (attributeName) {
                    case "PlayerField":
                        _aequusPlayer.AddField(attr.GetTextArgument(0), attr.GetTextArgument(1));
                        break;
                    case "SavedPlayerField":
                        string name = attr.GetTextArgument(0);
                        _aequusPlayer.AddField(name, attr.GetTextArgument(1));
                        _aequusPlayer.AddMethod("SaveInner(TagCompound tag)", $"SaveObj(tag, \"{name}\", {name});");
                        _aequusPlayer.AddMethod("LoadInner(TagCompound tag)", $"LoadObj(tag, \"{name}\", ref {name});");
                        break;

                    //default:
                    //    _aequusPlayer.AddField(attributeName, "test");
                    //    break;
                }
            }
        }

        _aequusPlayer.AddSource(in context);
    }
}

