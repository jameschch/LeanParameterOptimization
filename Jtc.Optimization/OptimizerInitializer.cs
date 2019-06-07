using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Fitnesses;
using Newtonsoft.Json;
using System.Reflection;
using System.IO.Abstractions;

namespace Optimization
{
    public class OptimizerInitializer
    {

        private readonly IFileSystem _file;
        private IOptimizerManager _manager;
        OptimizerConfiguration _config;

        public OptimizerInitializer(IFileSystem file, IOptimizerManager manager)
        {
            _file = file;
            _manager = manager;
        }

        public OptimizerInitializer()
        {
            _file = new FileSystem();
        }

        public void Initialize(string[] args)
        {
            _config = LoadConfig(args);
            _file.File.Copy(_config.ConfigPath, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), true);

            string path = _config.AlgorithmLocation;
            if (!string.IsNullOrEmpty(path) && (path.Contains('\\') || path.Contains('/')))
            {
                _file.File.Copy(path, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileName(path)), true);
                string pdb = path.Replace(System.IO.Path.GetExtension(path), ".pdb");

                //due to locking issues, need to manually clean to update pdb
                if (!_file.File.Exists(pdb))
                {
                    _file.File.Copy(pdb, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileName(pdb)), true);
                }
            }

            if (_config.UseSharedAppDomain)
            {
                SingleAppDomainManager.Initialize();
            }
            else
            {
                OptimizerAppDomainManager.Initialize();
            }

            OptimizerFitness fitness = (OptimizerFitness)Assembly.GetExecutingAssembly().CreateInstance(_config.FitnessTypeName, false, BindingFlags.Default, null,
                new object[] { _config, new FitnessFilter() }, null, null);

            if (_manager == null)
            {
                if (new[] { typeof(SharpeMaximizer), typeof(NFoldCrossReturnMaximizer), typeof(NestedCrossSharpeMaximizer), typeof(NestedCrossSharpeMaximizer) }.Contains(fitness.GetType()))
                {
                    _manager = new MaximizerManager();
                }
                else
                {
                    _manager = new GeneManager();
                }
            }

            _manager.Initialize(_config, fitness);
            _manager.Start();

        }

        private OptimizerConfiguration LoadConfig(string[] args)
        {
            string path = "optimization.json";
            if (args != null && args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                path = args[0];
            }

            return JsonConvert.DeserializeObject<OptimizerConfiguration>(_file.File.ReadAllText(path));
        }

    }

}
