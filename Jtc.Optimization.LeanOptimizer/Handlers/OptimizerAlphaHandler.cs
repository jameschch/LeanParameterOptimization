using QuantConnect;
using QuantConnect.Interfaces;
using QuantConnect.Lean.Engine.Alpha;
using QuantConnect.Lean.Engine.TransactionHandlers;
using QuantConnect.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer
{
    class OptimizerAlphaHandler : IAlphaHandler
    {
        public bool IsActive => false;

        public AlphaRuntimeStatistics RuntimeStatistics => null;

        public void Exit()
        {
        }

        public void Initialize(AlgorithmNodePacket job, IAlgorithm algorithm, IMessagingHandler messagingHandler, IApi api,
            ITransactionHandler transactionHandler)
        {
            //HACK: needed to set id prior to initialize
            algorithm.SetAlgorithmId(((BacktestNodePacket)job).AlgorithmId);
        }

        public void OnAfterAlgorithmInitialized(IAlgorithm algorithm)
        {
        }

        public void ProcessSynchronousEvents()
        {
        }

        public void Run()
        {
        }
    }
}
