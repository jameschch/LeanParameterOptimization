using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantConnect;
using QuantConnect.Api;
using QuantConnect.API;
using QuantConnect.Data;
using QuantConnect.Data.Market;
using QuantConnect.Interfaces;
using QuantConnect.Securities;

namespace Optimization
{
    class EmptyApiHandler : IApi
    {
        public void Dispose()
        {
        }

        public void Initialize(int userId, string token, string dataFolder)
        {
        }

        public ProjectResponse CreateProject(string name, Language language)
        {
            throw new NotImplementedException();
        }

        public ProjectResponse ReadProject(int projectId)
        {
            throw new NotImplementedException();
        }

        public ProjectFilesResponse AddProjectFile(int projectId, string name, string content)
        {
            throw new NotImplementedException();
        }

        public RestResponse UpdateProjectFileName(int projectId, string oldFileName, string newFileName)
        {
            throw new NotImplementedException();
        }

        public RestResponse UpdateProjectFileContent(int projectId, string fileName, string newFileContents)
        {
            throw new NotImplementedException();
        }

        public ProjectFilesResponse ReadProjectFile(int projectId, string fileName)
        {
            throw new NotImplementedException();
        }

        public ProjectFilesResponse ReadProjectFiles(int projectId)
        {
            throw new NotImplementedException();
        }

        public RestResponse DeleteProjectFile(int projectId, string name)
        {
            throw new NotImplementedException();
        }

        public RestResponse DeleteProject(int projectId)
        {
            throw new NotImplementedException();
        }

        public ProjectResponse ListProjects()
        {
            throw new NotImplementedException();
        }

        public Compile CreateCompile(int projectId)
        {
            throw new NotImplementedException();
        }

        public Compile ReadCompile(int projectId, string compileId)
        {
            throw new NotImplementedException();
        }

        public Backtest CreateBacktest(int projectId, string compileId, string backtestName)
        {
            throw new NotImplementedException();
        }

        public Backtest ReadBacktest(int projectId, string backtestId)
        {
            throw new NotImplementedException();
        }

        public RestResponse UpdateBacktest(int projectId, string backtestId, string backtestName = "", string backtestNote = "")
        {
            throw new NotImplementedException();
        }

        public RestResponse DeleteBacktest(int projectId, string backtestId)
        {
            throw new NotImplementedException();
        }

        public BacktestList ListBacktests(int projectId)
        {
            throw new NotImplementedException();
        }

        public LiveLog ReadLiveLogs(int projectId, string algorithmId, DateTime? startTime = null, DateTime? endTime = null)
        {
            throw new NotImplementedException();
        }

        public Link ReadDataLink(Symbol symbol, Resolution resolution, DateTime date)
        {
            throw new NotImplementedException();
        }

        public bool DownloadData(Symbol symbol, Resolution resolution, DateTime date)
        {
            throw new NotImplementedException();
        }

        public LiveAlgorithm CreateLiveAlgorithm(int projectId, string compileId, string serverType, BaseLiveAlgorithmSettings baseLiveAlgorithmSettings, string versionId = "-1")
        {
            throw new NotImplementedException();
        }

        public LiveList ListLiveAlgorithms(AlgorithmStatus? status = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            throw new NotImplementedException();
        }

        public LiveAlgorithmResults ReadLiveAlgorithm(int projectId, string deployId)
        {
            throw new NotImplementedException();
        }

        public RestResponse LiquidateLiveAlgorithm(int projectId)
        {
            throw new NotImplementedException();
        }

        public RestResponse StopLiveAlgorithm(int projectId)
        {
            throw new NotImplementedException();
        }

        public AlgorithmControl GetAlgorithmStatus(string algorithmId)
        {
            throw new NotImplementedException();
        }

        public void SetAlgorithmStatus(string algorithmId, AlgorithmStatus status, string message = "")
        {
        }

        public void SendStatistics(string algorithmId, decimal unrealized, decimal fees, decimal netProfit, decimal holdings, decimal equity, decimal netReturn, decimal volume, int trades, double sharpe)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MarketHoursSegment> MarketToday(DateTime time, Symbol symbol)
        {
            throw new NotImplementedException();
        }

        public void SendUserEmail(string algorithmId, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public void LiveSubscribe(IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }

        public void LiveUnsubscribe(IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BaseData> GetLiveData()
        {
            throw new NotImplementedException();
        }

        public PricesList ReadPrices(IEnumerable<Symbol> symbols)
        {
            throw new NotImplementedException();
        }

        public List<QuantConnect.Data.Market.Split> GetSplits(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public List<QuantConnect.Data.Market.Dividend> GetDividends(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public string Download(string address, IEnumerable<KeyValuePair<string, string>> headers, string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
