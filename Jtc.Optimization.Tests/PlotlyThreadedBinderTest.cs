using BlazorWorker.BackgroundServiceFactory;
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;
using Jtc.Optimization.Objects;
using Jtc.Optimization.Transformation;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Jtc.Optimization.Tests
{
    public class PlotlyThreadedBinderTest : PlotlyBinderTest
    {

        private const int ExpectedLines = 1397;
        protected override IPlotlyBinder CreateUnit()
        {
            var lineSplitter = new PlotlyLineSplitter();
            var wrapper = new Mock<IPlotlyLineSplitterBackgroundWrapper>();
            wrapper.Setup(w => w.Split(It.IsAny<string[]>())).Returns<string[]>(c => Task.FromResult(lineSplitter.Split(c)));

            return new PlotlyThreadedBinder(wrapper.Object);
        }

    }
}
