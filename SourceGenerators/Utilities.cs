using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceGenerators;

public static class Utilities {
    public const string Parent = "Aequus";
    public const string ParentS = $"{Parent}/";
    public const string ParentD = $"{Parent}.";
    public const string TabForward = "    ";

    public static string GetNamespace(this BaseNamespaceDeclarationSyntax namespaceSyntax) {
        string result = namespaceSyntax.Name.ToString();
        if (result.StartsWith(ParentD)) {
            result = result.Substring(ParentD.Length);
        }
        return result;
    }

    public static IEnumerable<(string, string)> GetArguments(this MethodDeclarationSyntax method) {
        return method.ParameterList.Parameters.Select(parameter => {
            (string type, string name) = (parameter.Type.GetText().ToString().Trim(), parameter.Identifier.ToString().Trim());

            SyntaxTokenList modifiers = parameter.Modifiers;
            if (modifiers.Any(SyntaxKind.RefKeyword)) {
                type = $"ref {type}";
                name = $"ref {name}";
            }
            else if (modifiers.Any(SyntaxKind.OutKeyword)) {
                type = $"out {type}";
                name = $"out {name}";
            }
            else if (modifiers.Any(SyntaxKind.InKeyword)) {
                type = $"in {type}";
                name = $"in {name}";
            }

            return (type, name);
        });
    }
    public static IEnumerable<string> GetArgumentTypes(this MethodDeclarationSyntax method) {
        return GetArguments(method).Select((ss) => ss.Item1);
    }
    public static IEnumerable<string> GetArgumentNames(this MethodDeclarationSyntax method) {
        return GetArguments(method).Select((ss) => ss.Item2);
    }

    public static string GetGeneric(this NameSyntax generic, int index) {
        return generic switch {
            GenericNameSyntax genericNameSyntax => ConvertNameSyntax(genericNameSyntax.TypeArgumentList.Arguments[index]),
            _ => "Unresolved"
        };
    }

    public static string GetTextArgument(this AttributeSyntax attr, int index) {
        return (attr.ArgumentList.Arguments[index].Expression as LiteralExpressionSyntax).Token.Text.Trim('"');
    }

    public static string GetArgument(this AttributeSyntax attr, int index) {
        return attr.ArgumentList.Arguments[index].Expression.ToString();
    }

    public static string ConvertNameSyntax(SyntaxNode node) {
        return node switch {
            NamespaceDeclarationSyntax namespaceDeclarationSyntax => ConvertNameSyntax(namespaceDeclarationSyntax.Name),
            _ => node.ToString() ?? "Unknown"
        };
    }

    public static string ConvertNameSyntax(NameSyntax name) {
        return name switch {
            IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.Text,
            GenericNameSyntax genericNameSyntax => genericNameSyntax.Identifier.Text,
            QualifiedNameSyntax qualifiedNameSyntax => ResolveQualifiedName(qualifiedNameSyntax),
            _ => "Unresolved",
        };

        static string ResolveQualifiedName(QualifiedNameSyntax qualifiedNameSyntax)
            => $"{ConvertNameSyntax(qualifiedNameSyntax.Left)}.{ConvertNameSyntax(qualifiedNameSyntax.Right)}";
    }

    public static List<T> GetFiles<T>(GeneratorExecutionContext context, string fileType, Func<string, string, T> initializer) where T : AssetFile {
        fileType = "." + fileType;
        var fileList = new List<T>();
        foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(fileType))) {
            string name = Path.GetFileNameWithoutExtension(file.Path);
            var safePath = file.Path.Replace('\\', '/');
            int index = safePath.LastIndexOf(ParentS) + ParentS.Length;
            safePath = safePath.Substring(index, safePath.Length - fileType.Length - index);

            T resultFile = initializer(name, safePath);
            if (resultFile != null) {
                fileList.Add(resultFile);
            }
        }
        return fileList;
    }

    public static void FixFileNames<T>(List<T> files, Action<T> renameFile) where T : AssetFile {
        foreach (var file in files) {
            bool renamed = false;
            foreach (var cloneFile in files.Where(f => f.Name.Equals(file.Name) && !f.Path.Equals(file.Path))) {
                renamed = true;
                renameFile(cloneFile);
            }
            if (renamed) {
                renameFile(file);
            }
        }
    }
}