using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerators;

[Generator]
public class LegacyTypeGenerator : ISourceGenerator {
    private readonly Dictionary<string, Action<TypeConstructor, QualifiedNameSyntax, AttributeSyntax>> _attributeActions = new() {
        ["Field"] = (type, name, attr) => type.AddField(attr.GetTextArgument(0), name.Right.GetGeneric(0)),
        ["ResetField"] = (type, name, attr) => {
            string fieldName = attr.GetTextArgument(0);
            type.AddField(fieldName, name.Right.GetGeneric(0));
            type.AddMethod("ResetEffectsInner()", $"SourceGeneratorTools.ResetObj(ref {fieldName});");
        },
        ["InfoField"] = (type, name, attr) => {
            string fieldName = attr.GetTextArgument(0);
            type.AddField(fieldName, "bool");
            type.AddMethod("ResetInfoAccessoriesInner()", $"SourceGeneratorTools.ResetObj(ref {fieldName});");
            type.AddMethod("MatchInfoAccessoriesInner(AequusPlayer other)", $"{fieldName} |= other.{fieldName};");
        },
        ["SavedField"] = (type, name, attr) => {
            string fieldName = attr.GetTextArgument(0);
            type.AddField(fieldName, name.Right.GetGeneric(0));
            type.AddMethod("SaveInner(TagCompound tag)", $"SourceGeneratorTools.SaveObj(tag, \"{fieldName}\", {fieldName});");
            type.AddMethod("LoadInner(TagCompound tag)", $"SourceGeneratorTools.LoadObj(tag, \"{fieldName}\", ref {fieldName});");
        },
        ["WorldField"] = (type, name, attr) => {
            string fieldName = attr.GetTextArgument(0);
            string upperFieldName = $"{char.ToUpper(fieldName[0])}{fieldName.Substring(1)}";
            string fieldType = name.Right.GetGeneric(0);

            // Add static field.
            type.AddField(fieldName, $"static {fieldType}");

            if (fieldType == "bool") {
                // Add condition.
                type.AddField($"Condition{upperFieldName} = new(\"Mods.Aequus.Condition.{upperFieldName}\", () => {fieldName})", "static readonly Condition");
                // Add reverse condition.
                type.AddField($"ConditionNot{upperFieldName} = new(\"Mods.Aequus.Condition.{upperFieldName}\", () => !{fieldName})", "static readonly Condition");
            }

            type.AddMethod("SaveInner(TagCompound tag)", $"SourceGeneratorTools.SaveObj(tag, \"{fieldName}\", {fieldName});");
            type.AddMethod("LoadInner(TagCompound tag)", $"SourceGeneratorTools.LoadObj(tag, \"{fieldName}\", ref {fieldName});");
            type.AddMethod("SendDataInner(BinaryWriter writer)", $"SourceGeneratorTools.SendObj(writer, {fieldName});");
            type.AddMethod("ReceiveDataInner(BinaryReader reader)", $"SourceGeneratorTools.ReceiveObj(reader, ref {fieldName});");
            type.AddMethod("ClearWorldInner()", $"SourceGeneratorTools.ResetObj(ref {fieldName});");
        },
    };

    private bool GetGenerationParams(QualifiedNameSyntax name, out string type, out string action) {
        string left = Utilities.ConvertNameSyntax(name.Left);
        if (left != "Gen") {
            goto FalseResult;
        }

        string resultName = Utilities.ConvertNameSyntax(name.Right);
        int underscore = resultName.IndexOf('_');
        if (underscore == -1 || underscore >= resultName.Length - 1) {
            goto FalseResult;
        }

        type = resultName.Substring(0, underscore);
        action = resultName.Substring(underscore + 1);
        return true;

    FalseResult:
        type = string.Empty;
        action = string.Empty;
        return false;
    }

    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        Dictionary<string, TypeConstructor> constructors = new() {
            ["AequusNPC"] = new TypeConstructor("AequusNPC", "Aequus",
             Usings: ["Terraria.ModLoader.IO", "Terraria.DataStructures", "Aequus.Common.Structures", "System.Collections.Generic"],
             MethodInputConversions: new Dictionary<string, string>() {
                 ["AequusNPC"] = "this",
             }),
            ["AequusPlayer"] = new TypeConstructor("AequusPlayer", "Aequus",
             Usings: ["Terraria.ModLoader.IO", "Aequus.Common.Structures", "System.Collections.Generic"],
             MethodInputConversions: new Dictionary<string, string>() {
                 ["AequusPlayer"] = "this",
                 ["Player"] = "Player",
             }),
            ["AequusItem"] = new TypeConstructor("AequusItem", "Aequus",
             Usings: ["Terraria.ModLoader.IO", "Aequus.Common.Structures", "System.Collections.Generic"],
             MethodInputConversions: new Dictionary<string, string>() {
                 ["AequusItem"] = "this",
             }),
            ["AequusSystem"] = new TypeConstructor("AequusSystem", "Aequus",
             Usings: ["Terraria.ModLoader.IO", "Aequus.Common.Structures", "System.IO", "System.Collections.Generic"],
             MethodInputConversions: new Dictionary<string, string>() {
                 ["AequusSystem"] = "this",
             }),
            ["AequusTile"] = new TypeConstructor("AequusTile", "Aequus",
             Usings: ["Terraria.ModLoader.IO", "Aequus.Common.Structures", "System.IO", "System.Collections.Generic"],
             MethodInputConversions: new Dictionary<string, string>() {
                 ["AequusTile"] = "this",
             }),
        };
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
                if (attr.Name is not QualifiedNameSyntax qualifiedNameSyntax || !GetGenerationParams(qualifiedNameSyntax, out string type, out string action)) {
                    continue;
                }

                if (constructors.TryGetValue(type, out TypeConstructor constructor) && _attributeActions.TryGetValue(action, out var func)) {
                    func(constructor, qualifiedNameSyntax, attr);
                }
            }

            // Methods
            foreach (MethodDeclarationSyntax method in definedClass.Members.OfType<MethodDeclarationSyntax>().Where(m => m.AttributeLists.Any())) {
                foreach (AttributeSyntax attr in method.AttributeLists.SelectMany(list => list.Attributes)) {
                    if (attr.Name is not QualifiedNameSyntax qualifiedNameSyntax || !GetGenerationParams(qualifiedNameSyntax, out string type, out string action)) {
                        continue;
                    }

                    if (!constructors.TryGetValue(type, out TypeConstructor constructor)) {
                        continue;
                    }

                    string returnType = method.ReturnType.GetText().ToString().Trim();

                    string existingCallArguments = "";
                    string generatedMethodArguments = "";
                    foreach ((string type, string identifier, string callName) arg in method.GetArguments()) {
                        if (!string.IsNullOrEmpty(existingCallArguments)) {
                            existingCallArguments += ", ";
                        }

                        if (constructor.GetArgOverride(arg.type, out string argOverride)) {
                            existingCallArguments += argOverride;
                        }
                        else {
                            if (!string.IsNullOrEmpty(generatedMethodArguments)) {
                                generatedMethodArguments += ", ";
                            }

                            generatedMethodArguments += $"{arg.type} {arg.identifier}";
                            existingCallArguments += arg.callName;
                        }
                    }
                    constructor.AddMethod($"{action}Inner({generatedMethodArguments})", $"{namespaceSyntax.GetNamespace()}.{definedClass.Identifier.Text}.{method.Identifier.Text}({existingCallArguments});");
                }
            }
        }

        foreach (TypeConstructor constructor in constructors.Values) {
            constructor.AddSource(in context);
        }
    }
}
