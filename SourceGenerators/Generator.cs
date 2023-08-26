using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class Generator : ISourceGenerator {
    private static List<string> Errors { get; set; }

    private interface IFile {
        string Name { get; set; }
        string Path { get; set; }
    }
    private interface IGeneratorMachine {
        void Generate(GeneratorExecutionContext context);
    }

    private class TextureMachine : IGeneratorMachine {
        private class TextureFile : IFile {
            public string Path { get; set; }
            public string Name { get; set; }

            public TextureFile(string Path, string Name) {
                this.Path = Path;
                this.Name = Name;
            }
        }

        private List<TextureFile> ConvertToTextureList(GeneratorExecutionContext context) {
            var textureFiles = new List<TextureFile>();
            foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(".png"))) {
                var safePath = file.Path.Replace('\\', '/');
                safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".png", "");

                var name = safePath.Substring(safePath.LastIndexOf('/') + 1);

                TextureFile textureFile = new TextureFile(safePath, name);
                textureFiles.Add(textureFile);
            }

            ManageFiles(textureFiles, "{0}_{1}");
            return textureFiles;
        }

        private const string TextureAssetField =
                """
                /// <summary>Full Path: $Path</summary>
                public static readonly TextureAsset $Name = new("$Path");
                """;

        private const string SkeletonClass =
                """
                using System.Runtime.CompilerServices;
                using Terraria.ModLoader;
                using Aequus.Common;

                namespace Aequus {
                    /// <summary>(Amt Textures: $Count)</summary>
                    [CompilerGenerated]
                    public partial class AequusTextures : ILoadable {
                        public void Load(Mod mod) {
                        }

                        public void Unload() {
                            foreach (var f in GetType().GetFields()) {
                                ((TextureAsset)f.GetValue(this))?.Unload();
                            }
                        }
            
                        $TextureFields
                    }
                }
                """;

        public void Generate(GeneratorExecutionContext context) {
            var pngFiles = ConvertToTextureList(context);
            string spacing = "\n        ";
            context.AddSource("AequusTextures.cs", SourceText.From(
                SkeletonClass
                .Replace("$Count", pngFiles.Count().ToString())
                .Replace("$TextureFields", string.Join(spacing, pngFiles.Select((t) => TextureAssetField.Replace("$Name", t.Name).Replace("$Path", t.Path).Replace("\n", spacing)))),
                Encoding.UTF8));
        }
    }
    private class SoundMachine : IGeneratorMachine {
        private class SoundFile : IFile {
            public string Path { get; set; }
            public string Name { get; set; }
            public int Amount;

            public SoundFile(string Path, string Name, int Amount) {
                this.Path = Path;
                this.Name = Name;
                this.Amount = Amount;
            }
        }

        private List<SoundFile> ConvertToSoundList(GeneratorExecutionContext context) {
            var soundFiles = new List<SoundFile>();

            // Generate basic list
            foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(".ogg"))) {
                var safePath = file.Path.Replace('\\', '/');
                safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".ogg", "");

                var name = safePath.Substring(safePath.LastIndexOf('/') + 1);
                soundFiles.Add(new SoundFile(safePath, name, 1));
            }

            try {

                // Handle multi-sounds
                int amount = soundFiles.Count;
                for (int i = 0; i < amount; i++) {
                    var soundFile = soundFiles[i];
                    if (soundFile.Path[soundFile.Path.Length - 1] == '0') {
                        string subPath = soundFile.Path.Substring(0, soundFile.Path.Length - 1);
                        int count = soundFiles.Where(f => f.Path.Substring(0, f.Path.Length - 1) == subPath).Count();

                        if (count > 1) {
                            string rename = subPath.Substring(subPath.LastIndexOf('/') + 1);
                            int underscore = soundFile.Name.IndexOf('_');
                            if (underscore > 0) {
                                rename += soundFile.Name.Substring(underscore);
                            }
                            soundFiles.Add(new SoundFile(subPath, rename, count));
                        }
                    }
                }

                ManageFiles(soundFiles, "{0}_{1}");
            }
            catch (Exception ex) {
                AddError(ex, "Fatal Error!");
            }

            return soundFiles;
        }

        private const string SoundAssetField =
                """
                /// <summary>Full Path: $Path<para>Num Variants: $SoundCount</para></summary>
                public static readonly SoundAsset $Name = new SoundAsset("$Path", $SoundCount);
                """;

        private const string SkeletonClass =
                """
                using System.Runtime.CompilerServices;
                using Terraria.ModLoader;
                using Aequus.Common;

                namespace Aequus {
                    /// <summary>(Amt Sounds: $Count)</summary>
                    [CompilerGenerated]
                    public class AequusSounds : ILoadable {                    
                        public void Load(Mod mod) {
                        }

                        public void Unload() {
                            foreach (var f in GetType().GetFields()) {
                                ((SoundAsset)f.GetValue(this))?.Unload();
                            }
                        }

                        $SoundFields
                    }
                }
                """;

        public void Generate(GeneratorExecutionContext context) {
            var soundFiles = ConvertToSoundList(context);
            string spacing = "\n        ";
            context.AddSource("AequusSounds.cs", SourceText.From(
                SkeletonClass
                .Replace("$Count", soundFiles.Count().ToString())
                .Replace("$SoundFields", string.Join(spacing,
                    soundFiles.Select((t) => SoundAssetField
                    .Replace("$Name", t.Name)
                    .Replace("$Path", t.Path)
                    .Replace("$SoundCount", t.Amount.ToString())
                    .Replace("\n", spacing)
                ))),
                Encoding.UTF8));
        }
    }
    private class LangMachine : IGeneratorMachine {
        private class LangEntry : IFile {
            public string Name { get; set; }
            public string Path { get; set; }

            public LangEntry(string Key, string Name) {
                this.Path = Key;
                this.Name = Name;
            }
        }

        private void GotoNextLineBreak(string text, ref int index) {
            while (text[index] != '\n') {
                index++;
            }
        }
        private void NextLine(string text, ref int index) {
            bool multiText = false;
            char multiTextChar = '\'';
        MultiTextLoopLabel:
            GotoNextLineBreak(text, ref index);
            while (char.IsWhiteSpace(text[index])) {
                index++;
            }
            if (text[index] == multiTextChar && text[index + 1] == multiTextChar && text[index + 2] == multiTextChar) {
                index += 3;
                if (multiText) {
                    index--;
                    return;
                }
                multiText = true;
                goto MultiTextLoopLabel;
            }
            if (multiText) {
                goto MultiTextLoopLabel;
            }
            index--;
        }

        private List<LangEntry> GetEntries(GeneratorExecutionContext context) {
            var files = new List<LangEntry>();

            string[] keys = new string[20];
            // Generate basic list
            foreach (var file in context.AdditionalFiles.Where(f => f.Path.EndsWith(".hjson") && f.Path.Contains("en-US"))) {
                var safePath = file.Path.Replace('\\', '/');
                safePath = safePath.Substring(safePath.IndexOf("Aequus/")).Replace(".hjson", "");

                var name = safePath.Substring(safePath.LastIndexOf('/') + 1);
                var text = file.GetText().ToString();
                int keysIndex = 0;
                string keysCompleteText = "";
                string currentKey = "";
                for (int i = 0; i < text.Length; i++) {
                    try {
                        if (text[i] == ':') {
                            i += 2;
                            if (text[i] == '{') {
                                keysCompleteText += currentKey + ".";
                                keys[keysIndex++] = currentKey;
                            }
                            else {
                                if (currentKey == "$parentVal") {
                                    files.Add(new(keysCompleteText, keysCompleteText.Split().Last()));
                                }
                                else if (int.TryParse(currentKey, out var _)) {
                                    files.Add(new(keysCompleteText + currentKey, "Option_" + currentKey));
                                }
                                else {
                                    files.Add(new(keysCompleteText + currentKey, currentKey));
                                }
                            }
                            currentKey = "";
                            NextLine(text, ref i);
                            continue;
                        }

                        if (text[i] == '}') {
                            if (keysIndex > 0) {
                                keysCompleteText = keysCompleteText.Substring(0, keysCompleteText.Length - keys[keysIndex - 1].Length - 1);
                                keysIndex--;
                            }
                            continue;
                        }

                        if (text[i] == '#' || text[i] == '/' || text[i] == '"') {
                            NextLine(text, ref i);
                            continue;
                        }

                        if (char.IsWhiteSpace(text[i]) || text[i] == '\n') {
                            continue;
                        }
                        currentKey += text[i];
                    }
                    catch (Exception ex) {
                        AddError(ex,
                            $"""
                            {nameof(LangMachine)}
                            File: [{file.Path}]
                            Text Index: [{i}]
                            Current Key: [{keysCompleteText + currentKey}]
                            """);
                    }
                }
            }

            try {
                ManageFiles(files, "{1}_{0}", '.');
            }
            catch {

            }
            return files;
        }

        private const string Field =
                """
                /// <summary>
                /// Key: $Key
                /// </summary>
                public static readonly TextEntry $Name = new("$Key");
                """;

        private const string SkeletonClass =
                """
                using System.Runtime.CompilerServices;
                using Terraria.ModLoader;
                using Aequus.Common;

                namespace Aequus
                {
                    /// <summary>
                    /// (Amt Text: $Count)
                    /// </summary>
                    [CompilerGenerated]
                    public class AequusLocalization
                    {                    
                        $Fields
                    }
                }
                """;

        public void Generate(GeneratorExecutionContext context) {
            var soundFiles = GetEntries(context);
            context.AddSource("AequusLocalization.cs", SourceText.From(
                SkeletonClass
                .Replace("$Count", soundFiles.Count().ToString())
                .Replace("$Fields", string.Join("\n        ",
                    soundFiles.Select((t) => Field
                    .Replace("$Name", t.Name)
                    .Replace("$Key", t.Path)
                    ))),
                Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context) {
        Errors = new List<string>();
        try {
            new TextureMachine().Generate(context);
            new SoundMachine().Generate(context);
            //new LangMachine().Generate(context);
            //throw new Exception("Ballsy");
        }
        catch (Exception ex) {
            AddError(ex, "???");
        }
        if (Errors.Count > 0) {
            context.AddSource("Errors.cs", SourceText.From(
                """
                using System.Runtime.CompilerServices;

                [CompilerGenerated]
                public class AequusGeneratorErrors
                {
                    /*
                    $Errors
                    */
                }
                """
                .Replace("$Errors", string.Join("\n#---------------------------------------#\n    ", Errors)),
                Encoding.UTF8));
        }
        Errors = null;
    }

    private static void AddError(Exception ex, string assetName) {
        Errors.Add($"{assetName}: {ex.Message}\n{ex.StackTrace}");
    }
    private static void ManageFiles<T>(List<T> files, string nameFormat, char pathSeparator = '/') where T : IFile {
        // Fix names
        for (int i = 0; i < files.Count; i++) {
            files[i].Name = files[i].Name.Replace('-', '_').Replace('.', '_').Trim();
        }

        List<int> matchingNames = new();
        int depth = 2;
        int emergency;
        bool anyNameMatches;
        do {
            files.Sort((f, f2) => f.Name.CompareTo(f2.Name));
            anyNameMatches = false;
            for (int i = 0; i < files.Count; i++) {
                emergency = 0;
                matchingNames.Clear();
                var file = files[i];
                for (; i < files.Count; i++) {
                    if (files[i].Name != file.Name) {
                        i--;
                        break;
                    }
                    matchingNames.Add(i);
                }

                if (matchingNames.Count <= 1) {
                    continue;
                }
                anyNameMatches = true;
                foreach (var j in matchingNames) {

                    var matchingNamedFile = files[j];
                    var pathSegments = matchingNamedFile.Path.Split(pathSeparator);
                    if (depth < pathSegments.Length) {
                        matchingNamedFile.Name = string.Format(nameFormat, matchingNamedFile.Name, pathSegments[pathSegments.Length - depth]);
                    }
                    else {
                        matchingNamedFile.Name = matchingNamedFile.Name + "_" + emergency;
                        emergency++;
                    }
                }
            }
            depth++;
        }
        while (anyNameMatches);
    }
}