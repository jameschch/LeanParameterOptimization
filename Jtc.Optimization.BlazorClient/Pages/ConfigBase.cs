using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Jtc.Optimization.Objects;

namespace Jtc.Optimization.BlazorClient
{
    public class ConfigBase : ComponentBase
    {
        protected Models.OptimizerConfiguration Config { get; set; }
        protected string Json { get; set; }
        protected IEnumerable<string> FitnessTypeNameOptions { get; set; } = Jtc.Optimization.Objects.FitnessOptions.Name;
        protected IEnumerable<string> ResultKeyOptions { get; set; } = StatisticsBinding.Binding.Select(s => s.Value).OrderBy(a => a);
        protected IEnumerable<string> OptimizerTypeNameOptions { get; set; } = Enum.GetNames(typeof(Enums.OptimizerTypeOptions)).OrderBy(o => o);
        protected string FitnessDisabled { get; set; }
        protected string OptimizerTypeNameDisabled { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public HttpClient httpClient { get; set; }

        [CascadingParameter] protected EditContext CurrentEditContext { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Config = new Models.OptimizerConfiguration
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now,
                Fitness = new Models.FitnessConfiguration(),
                Genes = new Models.GeneConfiguration[] { new Models.GeneConfiguration() }
            };

            //todo: allow upload of fitness
            //FitnessTypeNameOptions = Jtc.Optimization.Objects.FitnessTypeNameOptions.Options; assembly.GetTypes().Where(w => w.GetInterfaces().Contains(typeof(IFitness))).Select(s => s.FullName).OrderBy(o => o);

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

        public void FitnessTypeNameChange(ChangeEventArgs e)
        {
            Config.FitnessTypeName = e.Value.ToString();

            ToggleFitness();
        }

        private void ToggleFitness()
        {
            FitnessDisabled = FitnessTypeNameOptions.Contains(Config.FitnessTypeName.Split('.').LastOrDefault()) ? null : "disabled";
            OptimizerTypeNameDisabled = FitnessOptions.Genetic.Contains(Config.FitnessTypeName.Split('.').LastOrDefault()) ? "disabled" : null;
        }

        protected void AddGene()
        {
            Config.Genes = Config.Genes.Concat(new[] { new Models.GeneConfiguration() }).ToArray();
        }

        protected void RemoveGene()
        {
            Config.Genes = Config.Genes.Except(new[] { Config.Genes.Last() }).ToArray();
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
