using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using Jtc.Optimization.Objects;
using Blazor.DynamicJavascriptRuntime.Evaluator;
using Blazored.Toast.Services;
using Jtc.Optimization.BlazorClient.Shared;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Jtc.Optimization.BlazorClient
{
    public class ConfigBase : ComponentBase
    {

        protected Models.OptimizerConfiguration Config { get; set; } = new Models.OptimizerConfiguration
        {
            Genes = new Models.GeneConfiguration[] { new Models.GeneConfiguration() },
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now,
            Fitness = new Models.FitnessConfiguration()
        };
        protected string Json { get; set; }
        protected IEnumerable<string> FitnessTypeNameOptions { get; set; } = Jtc.Optimization.Objects.FitnessOptions.Name;
        protected IEnumerable<string> ResultKeyOptions { get; set; } = StatisticsBinding.Binding.Select(s => s.Value).OrderBy(a => a);
        protected IEnumerable<string> OptimizerTypeNameOptions { get; set; } = Enum.GetNames(typeof(Enums.OptimizerTypeOptions)).OrderBy(o => o);
        protected IEnumerable<string> AlgorithmLanguageOptions { get; set; } = new[] { "CSharp", "Python" };
        protected string FitnessDisabled { get; set; }
        protected string OptimizerTypeNameDisabled { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public HttpClient httpClient { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public BlazorClientState BlazorClientState { get; set; }
        //[CascadingParameter]
        //protected EditContext CurrentEditContext { get; set; }

        protected async override Task OnInitializedAsync()
        {
            //todo: allow upload of fitness
            //FitnessTypeNameOptions = Jtc.Optimization.Objects.FitnessTypeNameOptions.Options; assembly.GetTypes().Where(w => w.GetInterfaces().Contains(typeof(IFitness))).Select(s => s.FullName).OrderBy(o => o);

            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.ClientStorage.fetchConfig();
                var json = await (context as EvalContext).InvokeAsync<string>();

                if (!string.IsNullOrEmpty(json))
                {
                    Config = JsonSerializer.Deserialize<Models.OptimizerConfiguration>(json);
                }
            }

            ToggleFitness();

            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.jQuery("body").css("overflow-y", "scroll");
            }

            await base.OnInitializedAsync();
        }

        protected async Task ValidSubmit()
        {
            Json = JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(Config));
            StoreConfig(Config);
            ToastService.ShowSuccess("Config was saved.");
        }

        protected async Task Download()
        {
            await ValidSubmit();
            await JSRuntime.InvokeAsync<string>("MainInterop.downloadConfig", Json);
        }

        public void FitnessTypeNameChange(ChangeEventArgs e)
        {
            Config.FitnessTypeName = e.Value.ToString();

            ToggleFitness();
        }

        private void ToggleFitness()
        {
            OptimizerTypeNameDisabled = FitnessOptions.Genetic.Contains(Config.FitnessTypeName.Split('.').LastOrDefault()) ? "disabled" : null;
            FitnessDisabled = FitnessOptions.Configurable.Contains(Config.FitnessTypeName.Split('.').LastOrDefault()) ? null : "disabled";
        }

        protected void AddGene()
        {
            Config.Genes = Config.Genes.Concat(new[] { new Models.GeneConfiguration() }).ToArray();
        }

        protected void RemoveGene()
        {
            if (FitnessOptions.Genetic.Contains(Config.FitnessTypeName) && Config.Genes.Length == 2)
            {
                ToastService.ShowError("Minimum of 2 genes for Genetic algorithms.");
                return;
            }

            Config.Genes = Config.Genes.Except(new[] { Config.Genes.Last() }).ToArray();
        }

        protected async Task UploadFile()
        {
            var data = await new EvalContext(JSRuntime).InvokeAsync<string>("MainInterop.getFileData()");
            try
            {
                Config = JsonSerializer.Deserialize<Models.OptimizerConfiguration>(data);

                StoreConfig(Config);

                Config.FitnessTypeName = Config.FitnessTypeName.Split('.').LastOrDefault();
            }
            catch (Exception)
            {
                ToastService.ShowError("The deserialization of config failed.");
                throw;
            }
            ToggleFitness();
            Json = data;
            ToastService.ShowInfo("Config was loaded");
            StateHasChanged();
        }

        protected async Task LoadSample()
        {
            Config = JsonSerializer.Deserialize<Models.OptimizerConfiguration>(Resource.OptimizationConfigSample);

            StoreConfig(Config);

            Config.FitnessTypeName = Config.FitnessTypeName.Split('.').LastOrDefault();
            ToggleFitness();
            Json = Resource.OptimizationConfigSample;
            ToastService.ShowInfo("Sample was loaded");
        }

        protected string IsSelected(string item)
        {
            return (Config.FitnessTypeName == item) ? "true" : "false";
        }

        private void StoreConfig(Models.OptimizerConfiguration value)
        {
            var settings = new EvalContextSettings();
            settings.SerializableTypes.Add(typeof(Models.OptimizerConfiguration));
            using (dynamic context = new EvalContext(JSRuntime, settings))
            {
                var serialized = JsonSerializer.ToJsonString(value);
                (context as EvalContext).Expression = () => context.ClientStorage.storeConfig(serialized);
            }

            BlazorClientState.NotifyStateHasChanged(typeof(SidebarBase));
        }


    }
}
