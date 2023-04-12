using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class Generator : ISourceGenerator
{
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

            ManageFiles(textureFiles);
            return textureFiles;
        }

        private const string TextureAssetField =
                """
                /// <summary>
                /// Full Path: $Path
                /// </summary>
                public static readonly TextureAsset $Name = new TextureAsset("$Path");
                """;

        private const string SkeletonClass =
                """
                using System.Runtime.CompilerServices;
                using Terraria.ModLoader;
                using Aequus.Common;

                namespace Aequus
                {
                    /// <summary>
                    /// (Amt Textures: $Count)
                    /// </summary>
                    [CompilerGenerated]
                    public class AequusTextures : ILoadable
                    {
                        public void Load(Mod mod)
                        {
                        }

                        public void Unload()
                        {
                            foreach (var f in GetType().GetFields())
                            {
                                ((TextureAsset)f.GetValue(this))?.Unload();
                            }
                        }
            
                        $TextureFields
                    }
                }
                """;

        public void Generate(GeneratorExecutionContext context) {
            var pngFiles = ConvertToTextureList(context);
            context.AddSource("AequusTextures.cs", SourceText.From(
                SkeletonClass
                .Replace("$Count", pngFiles.Count().ToString())
                .Replace("$TextureFields", string.Join("\n        ", pngFiles.Select((t) => TextureAssetField.Replace("$Name", t.Name).Replace("$Path", t.Path))))
                ,
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

                ManageFiles(soundFiles);
            }
            catch (Exception ex) {
                AddError(ex, "Fatal Error!");
            }

            return soundFiles;
        }

        private const string SoundAssetField =
                """
                /// <summary>
                /// Full Path: $Path
                /// <para>Num Variants: $SoundCount</para>
                /// </summary>
                public static readonly SoundAsset $Name = new SoundAsset("$Path", $SoundCount);
                """;

        private const string SkeletonClass =
                """
                using System.Runtime.CompilerServices;
                using Terraria.ModLoader;
                using Aequus.Common;

                namespace Aequus
                {
                    /// <summary>
                    /// (Amt Sounds: $Count)
                    /// </summary>
                    [CompilerGenerated]
                    public class AequusSounds : ILoadable
                    {                    
                        public void Load(Mod mod)
                        {
                        }

                        public void Unload()
                        {
                            foreach (var f in GetType().GetFields())
                            {
                                ((SoundAsset)f.GetValue(this))?.Unload();
                            }
                        }

                        $SoundFields
                    }
                }
                """;

        public void Generate(GeneratorExecutionContext context) {
            var soundFiles = ConvertToSoundList(context);
            context.AddSource("AequusSounds.cs", SourceText.From(
                SkeletonClass
                .Replace("$Count", soundFiles.Count().ToString())
                .Replace("$SoundFields", string.Join("\n        ",
                    soundFiles.Select((t) => SoundAssetField
                    .Replace("$Name", t.Name)
                    .Replace("$Path", t.Path)
                    .Replace("$SoundCount", t.Amount.ToString())
                    )))
                //.Replace("$Errors", string.Join("\n", Errors))
                ,
                Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
        Errors = new List<string>();
        new TextureMachine().Generate(context);
        Errors.Clear();
        new SoundMachine().Generate(context);
        Errors = null;
    }

    private static void AddError(Exception ex, string assetName) {
        Errors.Add($"{assetName}: {ex.Message}\n{ex.StackTrace}");
    }
    private static void ManageFiles<T>(List<T> files) where T : IFile {
        files.Sort((f, f2) => f.Name.CompareTo(f2.Name));

        // Fix names
        List<int> others = new();
        for (int i = 0; i < files.Count; i++) {
            others.Clear();

            var soundFile = files[i];
            for (int j = i; j < files.Count; j++) {
                if (files[j].Name == soundFile.Name) {
                    others.Add(j);
                }
            }

            if (others.Count > 1) {
                i--;
                foreach (var j in others) {
                    var otherFile = files[j];
                    string path = otherFile.Path.Substring(0, otherFile.Path.LastIndexOf('/'));
                    otherFile.Name = otherFile.Name + "_" + path.Substring(path.LastIndexOf('/') + 1);
                }
            }
        }
    }
}