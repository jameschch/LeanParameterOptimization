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

            var unit = new OptimizerLauncher(file.Object, manager.Object);

            unit.Launch(new[] {""});

            manager.Verify(m => m.Initialize(It.IsAny<IOptimizerConfiguration>(), It.IsAny<OptimizerFitness>()));
            manager.Verify(m => m.Start());
        }

     

    }
}