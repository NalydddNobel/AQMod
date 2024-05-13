using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerators;

public static class GeneratorCommons {
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