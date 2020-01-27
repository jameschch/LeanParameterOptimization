using Jtc.Optimization.Objects.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture()]
    public class OptimizerInitializerTests
    {
        [Test()]
        public void InitializeTest()
        {
            var file = new Mock<IFileSystem>();
            file.Setup(f => f.File.ReadAllText(It.IsAny<string>())).Returns("{}");
            var manager = new Mock<IOptimizerManager>();
            SetEntryAssembly(Assembly.GetCallingAssembly());

            var unit = new OptimizerInitializer(file.Object, manager.Object);

            unit.Initialize(new[] {""});

            manager.Verify(m => m.Initialize(It.IsAny<IOptimizerConfiguration>(), It.IsAny<OptimizerFitness>()));
            manager.Verify(m => m.Start());
        }

        // From http://frostwave.googlecode.com/svn-history/r75/trunk/F2DUnitTests/Code/AssemblyUtilities.cs
        public static void SetEntryAssembly(Assembly assembly)
        {
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }

    }
}