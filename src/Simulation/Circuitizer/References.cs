using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.Quantum.Intrinsic;

namespace Microsoft.Quantum.Simulation.Circuitizer
{
    public static class References
    {
        /// <summary>
        /// Calculates the paths for all the Assemblies and their dependencies.
        /// </summary>
        private static List<string> PathsInit(params Assembly[] assemblies)
        {
            var found = new List<string>();
            foreach (var a in assemblies)
            {
                AddReferencesPaths(found, a, a.Location);
            }
            return found;
        }

        private static void AddReferencesPaths(List<string> found, Assembly asm, string location)
        {
            if (string.IsNullOrEmpty(location)) return;

            if (found.Contains(location))
            {
                return;
            }

            found.Add(location);

            foreach (var a in asm.GetReferencedAssemblies())
            {
                try
                {
                    var assm = Assembly.Load(a);
                    AddReferencesPaths(found, assm, assm.Location);
                }
                catch (Exception)
                {
                    //Ignore assembly if it can't be loaded.
                }
            }
        }

        /// <summary>
        /// Calculates Roslyn's MetadataReference for all the Assemblies and their dependencies.
        /// </summary>
        public static IEnumerable<MetadataReference> Load()
        {
            var paths = PathsInit(typeof(X).Assembly);
            var mds = paths.Select(p => MetadataReference.CreateFromFile(p));
            return mds.Select(a => a as MetadataReference).ToArray();
        }
    }
}
