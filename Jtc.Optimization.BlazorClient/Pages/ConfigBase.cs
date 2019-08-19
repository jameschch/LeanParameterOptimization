using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using GeneticSharp.Domain.Fitnesses;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace Jtc.Optimization.BlazorClient
{
    public class ConfigBase : ComponentBase
    {
        protected Models.OptimizerConfiguration Config { get; set; }
        protected string Json { get; set; }
        protected List<string> FitnessTypeNameOptions { get; set; }
        protected string[] ResultKeyOptions { get; set; }
        protected string[] OptimizerTypeNameOptions { get; set; }
        protected string FitnessDisabled { get; set; }
        protected string ConfiguredFitnessDisabled { get; set; }
        private string[] ConfigurableFitness = new[] { typeof(ConfiguredFitness).FullName, typeof(SharpeMaximizer).FullName, typeof(NFoldCrossReturnMaximizer).FullName,
            typeof(NFoldCrossSharpeMaximizer).FullName };

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient httpClient { get; set; }

        [CascadingParameter] protected EditContext CurrentEditContext { get; set; }

        protected async override Task OnInitAsync()
        {
            Config = new Models.OptimizerConfiguration
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now,
                Fitness = new Models.FitnessConfiguration(),
                Genes = new Models.GeneConfiguration[] { new Models.GeneConfiguration() }
            };

            var assembly = Assembly.GetAssembly(typeof(OptimizerFitness));

            FitnessTypeNameOptions = assembly.GetTypes().Where(w => w.GetInterfaces().Contains(typeof(IFitness))).Select(s => s.FullName).OrderBy(o => o).ToList();
            ResultKeyOptions = StatisticsAdapter.Binding.Select(s => s.Value).OrderBy(a => a).ToArray();
            OptimizerTypeNameOptions = Enum.GetNames(typeof(Enums.OptimizerTypeOptions)).OrderBy(o => o).ToArray();

            ToggleFitness();
        }


        protected void ValidSubmit()
        {
            Json = JsonConvert.SerializeObject(Config, new JsonSerializerSettings { Formatting = Formatting.Indented, DefaultValueHandling = DefaultValueHandling.Ignore });
        }

        protected async Task Save()
        {
            ValidSubmit();
            await JsRuntime.InvokeAsync<string>("JSInterop.DownloadConfig", Json);
        }

        public void FitnessTypeNameChange(UIChangeEventArgs e)
        {
            Config.FitnessTypeName = e.Value.ToString();

            ToggleFitness();
        }

        private void ToggleFitness()
        {
            if (!ConfigurableFitness.Contains(Config.FitnessTypeName))
            {
                FitnessDisabled = "disabled";
            }
            else
            {
                FitnessDisabled = null;
            }
            ConfiguredFitnessDisabled = Config.FitnessTypeName == typeof(ConfiguredFitness).FullName ? null : "disabled";
        }

        protected void AddGene()
        {
            Config.Genes = Config.Genes.Concat(new[] { new Models.GeneConfiguration() }).ToArray();
        }

        protected void RemoveGene()
        {
            Config.Genes = Config.Genes.Except(new[] { Config.Genes.Last() }).ToArray();
        }

        protected async override Task OnAfterRenderAsync()
        {

            base.OnAfterRender();

        }

        protected async Task UploadFile()
        {
            var data = await JsRuntime.InvokeAsync<string>("JSInterop.GetFileData");
            Config = JsonConvert.DeserializeObject<Models.OptimizerConfiguration>(data);
            ToggleFitness();
            Json = data;
            StateHasChanged();
        }

        protected string IsSelected(string item)
        {
            return (Config.FitnessTypeName == item) ? "true" : "false";
        }

    }
}
