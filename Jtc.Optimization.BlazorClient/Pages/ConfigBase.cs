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
using System.Text.Json;
using Jtc.Optimization.BlazorClient.Shared;

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
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public HttpClient httpClient { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public BlazorClientState BlazorClientState { get; set; }

        [CascadingParameter]
        protected EditContext CurrentEditContext { get; set; }

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

            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.MainInterop.fetchConfig();
                var json = await (context as EvalContext).InvokeAsync<string>();

                if (!string.IsNullOrEmpty(json))
                {
                    Config = JsonSerializer.Deserialize<Models.OptimizerConfiguration>(json, _options);
                }
            }

            ToggleFitness();

            using (dynamic context = new EvalContext(JSRuntime))
            {
                (context as EvalContext).Expression = () => context.jQuery("body").css("overflow-y", "scroll");
            }

            await base.OnInitializedAsync();
        }

        protected void ValidSubmit()
        {
            Json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            StoreConfig(Config);
            ToastService.ShowSuccess("Config was stored.");
        }

        protected async Task Download()
        {
            ValidSubmit();
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
            Config.Genes = Config.Genes.Except(new[] { Config.Genes.Last() }).ToArray();
        }

        protected async Task UploadFile()
        {
            var data = await new EvalContext(JSRuntime).InvokeAsync<string>("MainInterop.getFileData()");
            try
            {
                Config = JsonSerializer.Deserialize<Models.OptimizerConfiguration>(data, _options);

                StoreConfig(Config);

                Config.FitnessTypeName = Config.FitnessTypeName.Split('.').LastOrDefault();
                Console.WriteLine(Config.FitnessTypeName);
            }
            catch (Exception)
            {
                ToastService.ShowError("The deserialization of config failed.");
                throw;
            }
            ToggleFitness();
            Json = data;
            StateHasChanged();
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
                var serialized = JsonSerializer.Serialize(value);
                (context as EvalContext).Expression = () => context.MainInterop.storeConfig(serialized);
            }

            BlazorClientState.NotifyStateHasChanged(typeof(SidebarBase));
        }


    }
}
