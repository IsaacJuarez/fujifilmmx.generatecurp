using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using fujifilmmx.generatecurp.Models;
using Microsoft.Extensions.DependencyInjection;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;

namespace LoadTesting
{
    public class LoadTesting
    {
        private static readonly HttpClient _httpClient;

        static LoadTesting()
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider();

            _httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
        }

        public static void Main(string[] args)
        {
            var scenario = Scenario.Create("AddOrSelectPerson_Scenario", async context =>
            {
                var paciente = new Paciente
                {
                    idHIS = "123",
                    Nombre = "Juan",
                    ApellidoPaterno = "Perez",
                    ApellidoMaterno = "Lopez",
                    Genero = "M",
                    EstadoNacimiento = 1,
                    FechaNacimiento = new DateOnly(1990, 1, 1),
                    origen = "Test",    
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(paciente), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5000/api/CurpID", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Response.Ok();
                }
                else
                {
                    return Response.Fail();
                }
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
                Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)),
                Simulation.Inject(rate: 30, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName($"LoadTestReport_{DateTime.Now.ToString("yyyyMMddHHmmss")}")
                .WithReportFolder("C://BORRA//Reportes")
                .WithReportFormats(ReportFormat.Txt, ReportFormat.Html)
                .WithTestName("AddOrSelectPerson_Test")
                .Run();
        }
    }
}