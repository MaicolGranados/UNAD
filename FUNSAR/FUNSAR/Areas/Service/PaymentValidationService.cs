using FUNSAR.AccesoDatos.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class PaymentValidationService : BackgroundService
{
    private readonly ILogger<PaymentValidationService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentValidationService(
        ILogger<PaymentValidationService> logger,
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
                    var pendingPayments = dbContext.Pagos
                        .Where(p => p.Estado == "pending")
                        .ToList();

                    foreach (var payment in pendingPayments)
                    {
                        await ValidatePayment(Convert.ToInt64(payment.Secuencia));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating payments.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ValidatePayment(long paymentId)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://www.funsar.org.co/Api/validaPago?payment_id={paymentId}");
        //var response = await client.GetAsync($"https://localhost:7029/Api/validaPago?payment_id={paymentId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Payment {paymentId} validated successfully.");
        }
        else
        {
            _logger.LogWarning($"Failed to validate payment {paymentId}. Status code: {response.StatusCode}");
        }
    }
}