using System;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System.IO.Abstractions;
using Jtc.Optimization.Objects;

namespace Jtc.Optimization.LeanOptimizer
{
    public class OptimizerLauncher
    {

        private readonly IFileSystem _file;
        private IOptimizerManager _manager;
        OptimizerConfiguration _config;

        public OptimizerLauncher(IFileSystem file, IOptimizerManager manager)
        {
            _file = file;
            _manager = manager;
        }

        public OptimizerLauncher()
        {
            _file = new FileSystem();
        }

        public void Launch(string[] args)
        {
            _config = LoadConfig(args);
            _file.File.Copy(_config.ConfigPath, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"), true);

            string path = _config.AlgorithmLocation;
            if (!string.IsNullOrEmpty(path) && (path.Contains('\\') || path.Contains('/')) && !path.EndsWith("py"))
            {
                _file.File.Copy(path, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileName(path)), true);
                string pdb = path.Replace(System.IO.Path.GetExtension(path), ".pdb");

                //due to locking issues, need to manually clean to update pdb
                if (!_file.File.Exists(pdb))
                {
                    _file.File.Copy(pdb, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileName(pdb)), true);
                }
            }

            var fitnessTypeName = _config.FitnessTypeName.Contains(".") ? _config.FitnessTypeName : "Jtc.Optimization.LeanOptimizer." 
                + _config.FitnessTypeName;

            OptimizerFitness fitness = (OptimizerFitness)Assembly.GetExecutingAssembly().CreateInstance(fitnessTypeName, false, 
                BindingFlags.Default, null, new object[] { _config, new FitnessFilter() }, null, null);

            if (_manager == null)
            {
                if (new[] { typeof(SharpeMaximizer), typeof(NFoldCrossReturnMaximizer), typeof(NestedCrossSharpeMaximizer), 
                    typeof(NFoldCrossSharpeMaximizer), typeof(WalkForwardSharpeMaximizer),
                    typeof(WalkForwardWeightedMetricSharpeMaximizer)}.Contains(fitness.GetType()))
                {
                    _manager = new MaximizerManager();
                    if (fitness.GetType() == typeof(OptimizerFitness))
                    {
                        LogProvider.ErrorLogger.Info("Fitness for shared app domain was switched to the default: SharpeMaximizer.");
                        fitness = new SharpeMaximizer(_config, new FitnessFilter());
                    }
                }
                else
                {
                    _manager = new GeneManager();
                }
            }

            //todo: some constraints for running python
            if (_config.AlgorithmLanguage == "Python")
            {
                _config.UseSharedAppDomain = true;
                _config.MaxThreads = 1;
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
