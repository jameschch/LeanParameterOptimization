using Jtc.Optimization.LeanOptimizer.Base;
using Jtc.Optimization.Objects.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Legacy
{

    public class LegacyAppDomainManager : BaseAppDomainManager<LegacyAppDomainManager>
    {
        
        AppDomainSetup _setup;

        public LegacyAppDomainManager() : base()
        {
             CreateAppDomainSetup();
        }

        private void CreateAppDomainSetup()
        {
            _setup = new AppDomainSetup();
            _setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            _setup.DisallowBindingRedirects = false;
            _setup.DisallowCodeDownload = true;
            _setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        }

        protected override IRunner CreateRunnerInAppDomain(ref AppDomain appDomain)
        {
            var name = Guid.NewGuid().ToString("x");
            appDomain = AppDomain.CreateDomain(name, null, _setup);

            var rc = (IRunner)appDomain.CreateInstanceAndUnwrap("Jtc.Optimization.LeanOptimizer", "Jtc.Optimization.LeanOptimizer.Runner");

            return rc;
        }

    }

}
