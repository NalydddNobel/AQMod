using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerators;

public static class Utilities {
    public const string TabForward = "    ";
        
    public static string GetTextArgument(this AttributeSyntax attr, int index) {
        return (attr.ArgumentList.Arguments[index].Expression as LiteralExpressionSyntax).Token.Text.Trim('"');
    }
    public static string GetArgument(this AttributeSyntax attr, int index) {
        return attr.ArgumentList.Arguments[index].Expression.ToString();
    }

    public static string ConvertNameSyntax(SyntaxNode node) {
        return node switch {
            NamespaceDeclarationSyntax namespaceDeclarationSyntax => ConvertNameSyntax(namespaceDeclarationSyntax.Name),
            _ => node.TryGetInferredMemberName() ?? "Unknown"
        };
    }

    public static string ConvertNameSyntax(NameSyntax name) {
        return name switch {
            IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.Text,
            QualifiedNameSyntax qualifiedNameSyntax => ResolveQualifiedName(qualifiedNameSyntax),
            _ => "Unresolved",
        };

        static string ResolveQualifiedName(QualifiedNameSyntax qualifiedNameSyntax) 
            => $"{ConvertNameSyntax(qualifiedNameSyntax.Left)}.{ConvertNameSyntax(qualifiedNameSyntax.Right)}";
    }

    public static List<T> GetFiles<T>(GeneratorExecutionContext context, string fileType, Func<string, T> initializer) where T : AssetFile {
        fileType = "." + fileType;
        var fileList = new List<T>();
        foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(fileType))) {
            var safePath = file.Path.Replace('\\', '/');
            int index = safePath.LastIndexOf("Aequus/") + 7;
            safePath = safePath.Substring(index, safePath.Length - fileType.Length - index);

            fileList.Add(initializer(safePath));
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