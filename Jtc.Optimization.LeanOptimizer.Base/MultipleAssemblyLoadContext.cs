using Jtc.Optimization.Objects.Interfaces;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Jtc.Optimization.LeanOptimizer.Base
{
    public class MultipleAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly IOptimizerConfiguration _config;

        public MultipleAssemblyLoadContext(string name, IOptimizerConfiguration config, bool isCollectible = false)
            : base(name, isCollectible)
        {
            this.Resolving += MultipleAssemblyLoadContext_Resolving;
            _config = config;
        }

        private Assembly MultipleAssemblyLoadContext_Resolving(AssemblyLoadContext content, AssemblyName asssemblyName)
        {
            return Load(asssemblyName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            //todo: fix isolation for console logging
            //if (assemblyName.Name == "NLog")
            //{
            //    return Default.Assemblies.First(x => x.FullName == assemblyName.FullName);
            //}

            if (File.Exists(Path.Combine(_config.LauncherBuildPath, assemblyName.Name + ".dll")))
            {
                return LoadFromAssemblyPath(Path.GetFullPath(Path.Combine(_config.LauncherBuildPath, assemblyName.Name + ".dll")));
            }

            if (assemblyName.FullName != "Jtc.Optimization.LeanOptimizer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                && !assemblyName.Name.StartsWith("QuantConnect") && !assemblyName.Name.StartsWith("Lib.Harmony"))
            {
                var defaultAssembly = Default.Assemblies.FirstOrDefault(x => x.FullName == assemblyName.FullName)
                    ?? Default.LoadFromAssemblyName(assemblyName);

                if (defaultAssembly != null)
                {
                    return defaultAssembly;
                }
            }

            return LoadFromAssemblyPath(Path.GetFullPath(assemblyName.Name + ".dll"));

        }

        public Assembly LoadHarmony()
        {
            return LoadFromAssemblyPath(System.IO.Path.GetFullPath("0Harmony.dll"));
        }


    }
}
