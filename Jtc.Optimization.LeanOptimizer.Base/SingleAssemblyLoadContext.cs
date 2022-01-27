using Jtc.Optimization.Objects.Interfaces;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Jtc.Optimization.LeanOptimizer.Base
{

    /// <summary>
    /// Default load context is enough. We use this to pull binaries from launcher
    /// </summary>
    public class SingleAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly IOptimizerConfiguration _config;

        public SingleAssemblyLoadContext(string name, IOptimizerConfiguration config, bool isCollectible = false)
            : base(name, isCollectible)
        {
            this.Resolving += SingleAssemblyLoadContext_Resolving;
            _config = config;
        }

        private Assembly SingleAssemblyLoadContext_Resolving(AssemblyLoadContext content, AssemblyName asssemblyName)
        {
            return Load(asssemblyName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {

            if (File.Exists(Path.Combine(_config.LauncherBuildPath, assemblyName.Name + ".dll")))
            {
                return Default.LoadFromAssemblyPath(Path.GetFullPath(Path.Combine(_config.LauncherBuildPath, assemblyName.Name + ".dll")));
            }

            var defaultAssembly = Default.Assemblies.FirstOrDefault(x => x.FullName == assemblyName.FullName)
                ?? Default.LoadFromAssemblyName(assemblyName);

            return defaultAssembly;

        }

    }
}
