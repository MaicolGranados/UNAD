using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using MercadoPago.Client.PaymentMethod;
using MercadoPago.Resource;
using MercadoPago.Resource.PaymentMethod;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;

namespace FUNSAR.HerramientasComunes
{
    public class ProcesaPago
    {

        public async Task mercadopago(string detalle, decimal monto, int cantidad)
        {
            try
            {
                MercadoPagoConfig.AccessToken = "TEST-4884608519740499-111713-c2a731ce8ce4029be18a98b2856e8419-272747853";

                var client = new PaymentClient();

                var identification = new IdentificationRequest()
                {
                    Type = "CC", // Ajusta según tu lógica
                    Number = "1000136197" // Ajusta según tu lógica
                };

                var payer = new PaymentPayerRequest()
                {
                    Type = "customer",
                    Email = "maicolest2000@gmail.com",
                    EntityType = "individual",
                    FirstName = "Test",
                    LastName = "User",
                    Identification = identification
                };

                var additionalInfo = new PaymentAdditionalInfoRequest()
                {
                    IpAddress = "127.0.0.1"
                };

                var transactionDetails = new PaymentTransactionDetailsRequest()
                {
                    FinancialInstitution = "1009"
                };

                var paymentCreateRequest = new PaymentCreateRequest()
                {
                    TransactionAmount = monto,
                    Description = detalle,
                    PaymentMethodId = "pse",
                    AdditionalInfo = additionalInfo,
                    TransactionDetails = transactionDetails,
                    CallbackUrl = "https://www.funsar.org.co/Cliente/Home/ProcesarPago",
                    Payer = payer
                };

                var payment = await client.CreateAsync(paymentCreateRequest);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            
        }

    }
}
