using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SourceGenerators;

[Generator]
public class AequusPlayerGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        TypeConstructor player = new("AequusPlayer", "Aequus", "Terraria.ModLoader.IO", "Aequus.Core.Structures");

        foreach (ClassDeclarationSyntax definedClass in context.Compilation.SyntaxTrees.SelectMany(tree => tree.GetRoot(context.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())) {
            if (context.CancellationToken.IsCancellationRequested) {
                return;
            }

            if (definedClass.Parent is not BaseNamespaceDeclarationSyntax namespaceSyntax) {
                continue;
            }
                        
            // Attributes
            foreach (AttributeSyntax attr in definedClass.AttributeLists.SelectMany(list => list.Attributes)) {
                if (attr.Name is not QualifiedNameSyntax qualifiedNameSyntax) {
                    continue;
                }

                string left = Utilities.ConvertNameSyntax(qualifiedNameSyntax.Left);
                string right = Utilities.ConvertNameSyntax(qualifiedNameSyntax.Right);

                player.AddDebuggerText(left + "." + right);
                if (qualifiedNameSyntax.Right is GenericNameSyntax __) {
                    player.AddDebuggerText(__.GetGeneric(0));
                }

                switch (left) {
                    case "PlayerGen": {
                            switch (right) {
                                // Add a field.
                                case "Field": {
                                        player.AddField(attr.GetTextArgument(0), qualifiedNameSyntax.Right.GetGeneric(0));
                                    }
                                    break;

                                // Add a field, and code to Save/Load
                                case "SavedField": {
                                        string name = attr.GetTextArgument(0);
                                        player.AddField(name, qualifiedNameSyntax.Right.GetGeneric(0));
                                        player.AddMethod("SaveInner(TagCompound tag)", $"SaveObj(tag, \"{name}\", {name});");
                                        player.AddMethod("LoadInner(TagCompound tag)", $"LoadObj(tag, \"{name}\", ref {name});");
                                    }
                                    break;

                                // Add a field, and code to ResetEffects
                                case "ResetField": {
                                        string name = attr.GetTextArgument(0);
                                        player.AddField(name, qualifiedNameSyntax.Right.GetGeneric(0));
                                        player.AddMethod("ResetEffectsInner()", $"ResetObj(ref {name});");
                                    }
                                    break;

                                // Add a field, and code to ResetInfoAccessories
                                case "InfoField": {
                                        string name = attr.GetTextArgument(0);
                                        player.AddField(name, "bool");
                                        player.AddMethod("ResetInfoAccessoriesInner()", $"ResetObj(ref {name});");
                                        player.AddMethod("MatchInfoAccessoriesInner(AequusPlayer other)", $"{name} |= other.{name};");
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }

            // Methods
            foreach (MethodDeclarationSyntax method in definedClass.Members.OfType<MethodDeclarationSyntax>().Where(m => m.AttributeLists.Any())) {
                foreach (AttributeSyntax attr in method.AttributeLists.SelectMany(list => list.Attributes)) {
                    if (attr.Name is not QualifiedNameSyntax qualifiedNameSyntax) {
                        continue;
                    }

                    string left = Utilities.ConvertNameSyntax(qualifiedNameSyntax.Left);
                    string right = Utilities.ConvertNameSyntax(qualifiedNameSyntax.Right);

                    /*
 *     [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal class OnRespawn : Attribute { }
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal class SetControls : Attribute { }
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal class PostUpdateEquips : Attribute { }

 */

                    string returnType = method.ReturnType.GetText().ToString().Trim();

                    switch (left) {
                        case "PlayerGen": {
                                string resultArguments = "";
                                foreach ((string type, string name) arg in method.GetArguments()) {
                                    if (!string.IsNullOrEmpty(resultArguments)) {
                                        resultArguments += ", ";
                                    }

                                    resultArguments += arg.type switch {
                                        "Player" => "Player",
                                        "AequusPlayer" => "this",
                                        _ => arg.name
                                    };
                                }
                                player.AddMethod($"{right}Inner()", $"{namespaceSyntax.GetNamespace()}.{definedClass.Identifier.Text}.{method.Identifier.Text}({resultArguments});");
                            }
                            break;
                    }
                }
            }
        }

        player.AddSource(in context);
    }
}

