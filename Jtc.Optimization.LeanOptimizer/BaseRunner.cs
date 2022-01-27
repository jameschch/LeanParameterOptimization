using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.Objects.Interfaces;
using Newtonsoft.Json;
using QuantConnect.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Jtc.Optimization.LeanOptimizer
{
    public abstract class BaseRunner : IRunner
    {

        protected OptimizerResultHandler _resultsHandler { get; set; }
        protected IOptimizerConfiguration OptimizerConfig { get; set; }
        private object _resultsLocker = new object();

        public Dictionary<string, decimal> Run(Dictionary<string, object> items, IOptimizerConfiguration config)
        {

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            OptimizerConfig = config;

            var id = (items.ContainsKey("Id") ? items["Id"] : Guid.NewGuid().ToString("N")).ToString();

            if (OptimizerConfig.StartDate.HasValue && OptimizerConfig.EndDate.HasValue)
            {
                if (!items.ContainsKey("startDate")) { items.Add("startDate", OptimizerConfig.StartDate); }
                if (!items.ContainsKey("endDate")) { items.Add("endDate", OptimizerConfig.EndDate); }
            }

            string jsonKey = JsonConvert.SerializeObject(items.Where(i => i.Key != "Id"));

            if (!OptimizerConfig.EnableRunningDuplicateParameters && GetResults().ContainsKey(jsonKey))
            {
                return GetResults()[jsonKey];
            }

            ConfigMerger.Merge(OptimizerConfig, id, this.GetType());
            LogProvider.TraceLogger.Trace($"id: {id} started.");
            LaunchLean(items, id);
            LogProvider.TraceLogger.Trace($"id: {id} finished.");

            AddToResults(config, jsonKey);

            return _resultsHandler.FullResults;
        }

        private void AddToResults(IOptimizerConfiguration config, string jsonKey)
        {
            lock (_resultsLocker)
            {
                //for multiple runs, keep most recent only
                if (config.EnableRunningDuplicateParameters)
                {
                    if (GetResults().ContainsKey(jsonKey))
                    {
                        GetResults().Remove(jsonKey);
                    }

                    GetResults().Add(jsonKey, _resultsHandler.FullResults);
                }
                else
                {
                    if (!GetResults().ContainsKey(jsonKey))
                    {
                        GetResults().Add(jsonKey, _resultsHandler.FullResults);
                    }
                }
            }
        }

        private Dictionary<string, Dictionary<string, decimal>> GetResults()
        {
            return ResultMediator.GetData<Dictionary<string, Dictionary<string, decimal>>>(AppDomain.CurrentDomain, "Results");
        }

        protected void AddJobParameters(Dictionary<string, object> items, BacktestNodePacket job)
        {
            foreach (var pair in items.Where(i => i.Key != "Id"))
            {
                if (pair.Value is DateTime?)
                {
                    var cast = ((DateTime?)pair.Value);
                    if (cast.HasValue)
                    {
                        job.Parameters.Add(pair.Key, cast.Value.ToString("O"));
                    }
                }
                else
                {
                    job.Parameters.Add(pair.Key, pair.Value.ToString());
                }
            }
        }

        protected abstract void LaunchLean(Dictionary<string, object> items, string id);

    }
}
