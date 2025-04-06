using FUNSAR.AccesoDatos.Data;
using FUNSAR.Data.Migrations;
using MercadoPago.Resource.Payment;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class ProcessValidationService : BackgroundService
{
    private readonly ILogger<ProcessValidationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProcessValidationService(
        ILogger<ProcessValidationService> logger,
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var pendingProcess = dbContext.Asistencia
                        .Where(a => a.EstadoAsistenciaId != 3)
                                       .GroupBy(a => a.VoluntarioId)
                                       .Select(g => new
                                       {
                                           Id = g.Key,
                                           Asistencias = g.Count()
                                       }).ToList();

                    var parametro1 = dbContext.Params.Where(p => p.id == 3)
                        .Select(a => new {Valor = a.Valor}).ToList(); //Obtiene valor para habilitar PFE
                    var parametro2 = dbContext.Params.Where(p => p.id == 2)
                        .Select(a => new { Valor = a.Valor }).ToList(); //Obtiene valor para habilitar pago
                    var parametro3 = dbContext.Params.Where(p => p.id == 5)
                        .Select(a => new { Valor = a.Valor }).ToList();
                    var parametro4 = dbContext.Params.Where(p => p.id == 6)
                        .Select(a => new { Valor = a.Valor }).ToList();
                    var parametro5 = dbContext.Params.Where(p => p.id == 4) //sabados proceso
                        .Select(a => new { Valor = a.Valor }).ToList();

                    int diasPfe = Convert.ToInt32(parametro1[0].Valor);
                    int diasPago = Convert.ToInt32(parametro2[0].Valor);
                    int asistenciaxSalida = Convert.ToInt32(parametro3[0].Valor);
                    int asistenciaxCampamento = Convert.ToInt32(parametro4[0].Valor);
                    int sabadosproceso = Convert.ToInt32(parametro5[0].Valor);

                    foreach (var voluntario in pendingProcess)
                    {
                        if (voluntario.Asistencias >= diasPago)
                        {
                            var planFamiliar = dbContext.PFE
                                .Where(a => a.VoluntarioId == voluntario.Id)
                                .Select(g => new
                                {
                                    Id = g.EstadoPFEId
                                }).ToList();

                            foreach (var item in planFamiliar)
                            {
                                if (item.Id == 1)
                                {
                                    await UpdateState(voluntario.Id); //Actualiza estado a 5 para habilitar pago
                                }
                            }
                        }else if (voluntario.Asistencias >= diasPfe)
                        {
                            await UpdateState(voluntario.Id); //Actualiza estado a 2 para habilitar la subida del PFE
                        }else if (voluntario.Asistencias < diasPago)
                        {
                            var asistenciaVoluntario = voluntario.Asistencias;

                            var salidas = dbContext.Pagos
                                .Where(p => p.ServicioId == 2) //id salidas
                                .Select(s => new
                                {
                                    Documento = s.DocumentoP,
                                    Servicio = s.ServicioId
                                }).ToList();

                            var campamentos = dbContext.Pagos
                                .Where(p => p.ServicioId == 3) //id campamentos
                                .Select(s => new
                                {
                                    Documento = s.DocumentoP,
                                    Servicio = s.ServicioId
                                }).ToList();

                            if (salidas.Count() > 0)
                            {
                                asistenciaVoluntario += asistenciaxSalida;
                            }

                            if (campamentos.Count() > 0)
                            {
                                asistenciaVoluntario += asistenciaxCampamento;
                            }

                            if (asistenciaVoluntario >= diasPago)
                            {
                                var planFamiliar = dbContext.PFE
                                .Where(a => a.VoluntarioId == voluntario.Id)
                                .Select(g => new
                                {
                                    Id = g.EstadoPFEId
                                }).ToList();

                                foreach (var item in planFamiliar)
                                {
                                    if (item.Id == 1)
                                    {
                                        await UpdateState(voluntario.Id); //Actualiza estado a 5 para habilitar pago
                                    }
                                }
                            }

                            else if (voluntario.Asistencias < sabadosproceso)
                            {
                                List<int> estadosIncompletos = new List<int> { 1, 2, 3, 4 };
                                var incompletos = dbContext.Voluntario.Where(v => estadosIncompletos.Contains(v.EstadoId));
                                foreach (var item in incompletos)
                                {
                                    await UpdateState(item.Id);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating process.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task UpdateState(int voluntarioId)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://www.funsar.org.co/Api/UpdateState?id={voluntarioId}");
        //var response = await client.GetAsync($"https://localhost:7029/Api/UpdateState?id={voluntarioId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Payment {voluntarioId} validated successfully.");
        }
        else
        {
            _logger.LogWarning($"Failed to validate payment {voluntarioId}. Status code: {response.StatusCode}");
        }
    }
}