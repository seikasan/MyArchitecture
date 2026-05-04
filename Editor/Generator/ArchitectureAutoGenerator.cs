using UnityEditor;
using UnityEngine;

namespace MyArchitecture.Editor
{
    [InitializeOnLoad]
    internal static class ArchitectureAutoGenerator
    {
        static ArchitectureAutoGenerator()
        {
            EditorApplication.delayCall += GenerateWhenReady;
        }

        [MenuItem("Tools/MyArchitecture/Auto Generate")]
        public static void Generate()
        {
            var changed = false;
            changed |= ReadOnlyModelGenerator.Generate();
            changed |= ArchitectureAutoRegistrationGenerator.Generate();

            Debug.Log(changed
                ? "MyArchitecture generated files were updated."
                : "MyArchitecture generated files are already up to date.");
        }

        private static void GenerateWhenReady()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += GenerateWhenReady;
                return;
            }

            Generate();
        }
    }
}