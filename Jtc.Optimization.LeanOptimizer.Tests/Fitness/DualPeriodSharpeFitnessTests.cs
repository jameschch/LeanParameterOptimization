using Jtc.Optimization.Objects.Interfaces;
using Moq;
using NUnit.Framework;

namespace Jtc.Optimization.LeanOptimizer.Tests
{
    [TestFixture]
    public class DualPeriodSharpeFitnessTests
    {
        private MockRepository mockRepository;

        private Mock<IOptimizerConfiguration> mockOptimizerConfiguration;
        private Mock<IFitnessFilter> mockFitnessFilter;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockOptimizerConfiguration = this.mockRepository.Create<IOptimizerConfiguration>();
            mockFitnessFilter = this.mockRepository.Create<IFitnessFilter>();
        }

        [TearDown]
        public void TearDown()
        {
            this.mockRepository.VerifyAll();
        }

        [Test]
        public void TestMethod1()
        {
            //todo:
            DualPeriodSharpeFitness unit = this.CreateDualPeriodSharpeFitness();


        }

        private DualPeriodSharpeFitness CreateDualPeriodSharpeFitness()
        {
            return new DualPeriodSharpeFitness(
                this.mockOptimizerConfiguration.Object,
                this.mockFitnessFilter.Object);
        }


    }
}
