using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Client.PaymentMethod;
using MercadoPago.Resource;
using MercadoPago.Resource.PaymentMethod;
using FUNSAR.Models;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models.ViewModels;
using FUNSAR.Models.Request;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using FUNSAR.Utilidades;

namespace FUNSAR.Areas.API
{
    [Route("Api")]
    [ApiController]
    public class ApiController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public ApiController(IContenedorTrabajo contenedorTrabajo) {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpPost("pagoServicios")]
        public async Task<ActionResult> pagoServicios([FromBody] servicioPago servicio)
        {
            try
            {

                HerramientasComunes.Log log = new HerramientasComunes.Log();
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfiguration configuration = builder.Build();
                string token = configuration["AccessToken"];
                var tdoc = _contenedorTrabajo.TDocumento.GetFirstOrDefault(filter: t => t.tDocumento == servicio.tipoDocumento);
                var detalle = _contenedorTrabajo.Servicio.GetFirstOrDefault(filter: s => s.Id == servicio.id);
                var valorServicio = detalle.Valor;
                if (detalle.Id == 1)
                {
                    var voluntario = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: v => v.Id == Convert.ToInt32(servicio.documentoP));
                    string anio = voluntario.FechaRegistro.Value.Year.ToString();
                    string mes = voluntario.FechaRegistro.Value.Month.ToString();
                    string date = string.Empty;
                    if (Convert.ToInt32(mes) >= 1 && Convert.ToInt32(mes) >= 6)
                    {
                        date = string.Format("{0}-{1}", anio, "1");
                    }
                    else
                    {
                        date = string.Format("{0}-{1}", anio, "2");
                    }
                    var vigencia = _contenedorTrabajo.VigenciaServicio.GetFirstOrDefault(filter: v => v.ServicioId == detalle.Id && v.Vigencia == date);
                    valorServicio = vigencia.Valor;
                }
                
                var tperson = servicio.tipoPersona;
                var documentoP = _contenedorTrabajo.Voluntario.GetFirstOrDefault(v => v.Id == Convert.ToInt32(servicio.documentoP));
                int idPersona = 0;
                if (documentoP == null)
                {
                    var asistenSalida = _contenedorTrabajo.AsistenteSalida.GetFirstOrDefault(a => a.Id == Convert.ToInt32(servicio.documentoP));
                    documentoP = new Voluntario();
                    documentoP.Documento = asistenSalida.Documento;
                    documentoP.Id = asistenSalida.Id;
                    idPersona = asistenSalida.Id;
                }
                idPersona = documentoP.Id;
                servicio.documentoP = documentoP.Documento;
                switch (tperson)
                {
                    case "natural":
                        tperson = "individual";
                        break;
                    case "juridica":
                        tperson = "association";
                        break;
                }
                MercadoPagoConfig.AccessToken = token;
                var client = new PaymentClient();
                var identification = new IdentificationRequest()
                {
                    Type = tdoc.tDocumento,
                    Number = servicio.documentoR
                };
                var phone = new PaymentPayerPhoneRequest()
                {
                    AreaCode = "57",
                    Number = servicio.celularR,
                };
                var payer = new PaymentPayerRequest()
                {
                    Email = servicio.correoR.Trim(),
                    EntityType = tperson,
                    Identification = identification,
                    Phone = phone,
                    LastName = servicio.apellidoR,
                    FirstName = servicio.nombreR
                };
                var paymentItemRequest = new PaymentItemRequest()
                {
                    CategoryId = "1",
                    Description = "Servicio FUNSAR",
                    Id = "1",
                    Quantity = 1,
                    Title = detalle.Detalle,
                    UnitPrice = Convert.ToDecimal(valorServicio)
                };

                var listItemRequest = new List<PaymentItemRequest>();

                listItemRequest.Add(paymentItemRequest);

                var additionalInfo = new PaymentAdditionalInfoRequest()
                {
                    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                    Items = listItemRequest
                };

                var selectedFinancialInstitutionId = servicio.banco;

                var transactionDetails = new PaymentTransactionDetailsRequest()
                {
                    FinancialInstitution = selectedFinancialInstitutionId
                };


                var paymentCreateRequest = new PaymentCreateRequest()
                {
                    TransactionAmount = Convert.ToDecimal(valorServicio),
                    Description = detalle.Detalle,
                    PaymentMethodId = "pse",
                    AdditionalInfo = additionalInfo,
                    TransactionDetails = transactionDetails,
                    CallbackUrl = "https://www.funsar.org.co/Cliente/Home/validaPago",
                    Payer = payer,
                    ExternalReference = servicio.documentoP,
                    StatementDescriptor = "Pago FUNSAR"
                };

                var payment = await client.CreateAsync(paymentCreateRequest);
                var jsonResponse = payment.ApiResponse.Content;
                dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                if (responseObject != null && responseObject.transaction_details.external_resource_url != null)
                {
                    Pagos pagos = new Pagos();
                    pagos.Estado = responseObject.status;
                    pagos.Banco = transactionDetails.FinancialInstitution;
                    pagos.Secuencia = responseObject.id;
                    pagos.Fechapago = responseObject.date_created;
                    pagos.TipoPersona = payer.EntityType;
                    pagos.ApellidoR = payer.LastName;
                    pagos.NombreR = payer.FirstName;
                    pagos.CorreoR = payer.Email;
                    pagos.CelularR = payer.Phone.Number;
                    pagos.DocumentoIdR = tdoc.Id;
                    pagos.DocumentoR = identification.Number;
                    pagos.DocumentoP = servicio.documentoP;
                    pagos.ServicioId = servicio.id;
                    pagos.Valor = Convert.ToInt64(detalle.Valor);
                    pagos.DireccionR = servicio.direccionR;

                    _contenedorTrabajo.Pagos.Add(pagos);
                    _contenedorTrabajo.save();

                    if (detalle.Id != 1)
                    {
                        AsistenteSalida asistente = new AsistenteSalida();
                        asistente = _contenedorTrabajo.AsistenteSalida.GetFirstOrDefault(filter: a => a.Id == idPersona);
                        if (asistente == null)
                        {
                            var voluntario = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: v => v.Id == idPersona);
                            asistente = new AsistenteSalida();
                            asistente.Nombre = voluntario.Nombre;
                            asistente.Apellido = voluntario.Apellido;
                            asistente.Documento = voluntario.Documento;
                            asistente.DocumentoId = voluntario.DocumentoId;
                            asistente.telefono = voluntario.telefono;
                            asistente.correo = voluntario.correo;
                            asistente.ServicioId = detalle.Id;
                            asistente.EstadoPago = responseObject.status;
                            asistente.ReferenciaPago = responseObject.id;
                            asistente.FechaRegistro = DateTime.Now;
                            _contenedorTrabajo.AsistenteSalida.Add(asistente);
                            _contenedorTrabajo.save();
                        }
                        else
                        {
                            asistente.EstadoPago = responseObject.status;
                            asistente.ReferenciaPago = responseObject.id;
                            _contenedorTrabajo.AsistenteSalida.Update(asistente);
                            _contenedorTrabajo.save();
                        }
                        
                    }

                    return Json(jsonResponse);
                }
                else
                {
                    return Json(null);
                }

            }
            catch (Exception ex)
            {
                string err = string.Format("Documento participante: {0}," +
                    "Documento responsable: {1}," +
                    "Correo responsable: {2}",
                servicio.documentoP,
                servicio.documentoR,
                servicio.correoR,
                ex.ToString());
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", err, "ApiController.pagoServicio()");
                return Json(null);
            }
        }

        [HttpGet("validaPago")]
        public async Task<ActionResult> validaPago(long payment_id)
        {
            Pagos pagos = new Pagos();
            GetPago getPago = new GetPago();
            try
            {
                var validate = _contenedorTrabajo.Pagos.GetFirstOrDefault(filter: i => i.Secuencia == Convert.ToString(payment_id));
                if (validate != null)
                {
                    switch (validate.Estado)
                    {
                        case "approved":
                            getPago.validate = "existente";
                            break;
                        case "cancelled":
                            getPago.validate = "existente";
                            break;
                        case "rejected":
                            getPago.validate = "existente";
                            break;
                        case "pending":
                            getPago.payer = new payer();
                            getPago.payer.identification = new identification();
                            getPago.payer.phone = new phone();
                            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                            IConfiguration configuration = builder.Build();
                            string token = configuration["AccessToken"];
                            MercadoPagoConfig.AccessToken = token;

                            var getPayment = new PaymentClient();
                            var response = await getPayment.GetAsync(payment_id);
                            var jsonResponse = response.ApiResponse.Content;
                            dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                            if (responseObject != null && responseObject.transaction_details.external_resource_url != null)
                            {
                                ///Asigna valores para actualizar el pago en BD
                                pagos.Secuencia = responseObject.id;
                                var idPago = _contenedorTrabajo.Pagos.GetFirstOrDefault(filter: i => i.Secuencia == pagos.Secuencia);
                                pagos = idPago;

                                ///Datos generales del pago
                                getPago.id = responseObject.id;
                                getPago.status = responseObject.status;
                                getPago.date_created = responseObject.date_created;
                                getPago.description = responseObject.description;
                                getPago.date_created = responseObject.date_created;
                                getPago.date_approved = responseObject.date_approved;
                                getPago.payment_method_id = responseObject.payment_method_id;

                                ///Datos del cliente
                                getPago.payer.first_name = idPago.NombreR;
                                var tdoc = _contenedorTrabajo.TDocumento.GetFirstOrDefault(filter: i => i.Id == idPago.DocumentoIdR);
                                getPago.payer.identification.type = tdoc.tDocumento;
                                getPago.payer.identification.number = idPago.DocumentoR;
                                getPago.payer.last_name = idPago.ApellidoR;
                                getPago.payer.email = idPago.CorreoR;
                                getPago.payer.phone.number = idPago.CelularR;

                                pagos.Fechapago = getPago.date_created;
                                pagos.Estado = getPago.status;

                                _contenedorTrabajo.Pagos.Update(pagos);
                                _contenedorTrabajo.save();

                                switch (pagos.Estado)
                                {
                                    case "approved":
                                        if (pagos.ServicioId == 1)
                                        {
                                            Certificado certvm = new Certificado();
                                            Voluntario artivm = new Voluntario();

                                            artivm = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: i => i.Documento == idPago.DocumentoP);

                                            certvm.FechaExpedicion = DateTime.Now.ToString("yyyy/MM/dd");
                                            certvm.EstadoId = 5;
                                            certvm.codCertificado = Guid.NewGuid().ToString();
                                            certvm.Nombre = artivm.Nombre;
                                            certvm.Apellido = artivm.Apellido;
                                            certvm.DocumentoId = artivm.DocumentoId;
                                            var brigada = _contenedorTrabajo.Colegio.GetFirstOrDefault(filter: b => b.Id == artivm.ColegioId);
                                            certvm.Documento = artivm.Documento;
                                            certvm.BrigadaId = brigada.BrigadaId;
                                            int semestre = 0;
                                            if (DateTime.Now.Month >= 1 && DateTime.Now.Month <= 6)
                                            {
                                                semestre = 1;
                                            }
                                            else
                                            {
                                                semestre = 2;
                                            }
                                            certvm.SemestreId = semestre;
                                            certvm.AnoProceso = DateTime.Now.ToString("yyyy");
                                            certvm.ProcesoId = artivm.ProcesoId;
                                            _contenedorTrabajo.Certificado.Add(certvm);
                                            _contenedorTrabajo.save();
                                            //var acuFromDb = _contenedorTrabajo.Acudiente.GetFirstOrDefault(a => a.VoluntarioId == artivm.Id);
                                            //if (acuFromDb != null)
                                            //{
                                            //    _contenedorTrabajo.Acudiente.Remove(acuFromDb);
                                            //    _contenedorTrabajo.save();
                                            //}
                                            //var objFromDb = _contenedorTrabajo.Voluntario.Get(artivm.Id);
                                            //_contenedorTrabajo.Voluntario.Remove(objFromDb);
                                            //_contenedorTrabajo.save();
                                            //var asistencias = _contenedorTrabajo.Asistencia.GetFirstOrDefault(filter: i => i.VoluntarioId == objFromDb.Id);
                                            //if (asistencias is not null)
                                            //{
                                            //    _contenedorTrabajo.Asistencia.Remove(asistencias);
                                            //    _contenedorTrabajo.save();
                                            //}
                                        }

                                        break;
                                }

                                var salidas = _contenedorTrabajo.AsistenteSalida.GetFirstOrDefault(s => s.ReferenciaPago == pagos.Secuencia);
                                if (salidas != null)
                                {
                                    salidas.EstadoPago = pagos.Estado;
                                    _contenedorTrabajo.AsistenteSalida.Update(salidas);
                                    _contenedorTrabajo.save();
                                }

                                return StatusCode(StatusCodes.Status200OK);
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError);
                            }
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Procesarpago()");
                getPago.validate = "errorSdk";
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("DatosPago")]
        public async Task<ActionResult> DatosPago()
        {
            try
            {
                var session = _contenedorTrabajo.Session.GetFirstOrDefault(filter: s => s.sessionid == Convert.ToString(HttpContext.Request.Cookies["SessionId"]));
                var dataPayment = _contenedorTrabajo.Servicio.GetFirstOrDefault(filter: s => s.Id == Convert.ToInt32(session.sessionservice));
                var dataClient = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: c => c.Id == Convert.ToInt64(session.sessiondocument));
                if (dataClient == null)
                {
                    var dataAsistent = _contenedorTrabajo.AsistenteSalida.GetFirstOrDefault(filter: a => a.Id == Convert.ToInt32(session.sessiondocument));
                    dataClient = new Voluntario
                    {
                        Nombre = dataAsistent.Nombre,
                        Apellido = dataAsistent.Apellido,
                        Documento = dataAsistent.Documento,
                        DocumentoId = dataAsistent.DocumentoId
                    };
                }
                if (dataPayment.Id == 1)
                {
                    var voluntario = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: v => v.Documento == dataClient.Documento);
                    string anio = voluntario.FechaRegistro.Value.Year.ToString();
                    string mes = voluntario.FechaRegistro.Value.Month.ToString();
                    string date = string.Empty;
                    if (Convert.ToInt32(mes) >= 1 && Convert.ToInt32(mes) <= 6)
                    {
                        date = string.Format("{0}-{1}", anio, "1");
                    }
                    else
                    {
                        date = string.Format("{0}-{1}", anio, "2");
                    }
                    var vigencia = _contenedorTrabajo.VigenciaServicio.GetFirstOrDefault(filter: v => v.ServicioId == dataPayment.Id && v.Vigencia == date);
                    dataPayment.Valor = vigencia.Valor;
                }
               
                dataPayment.Detalle = dataPayment.Detalle + " del participante " + dataClient.Nombre + " " + dataClient.Apellido + ", Documento N°: " + dataClient.Documento;
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(5));
                HttpContext.Response.Cookies.Append("servicioDetalle", dataPayment.Detalle, cookieOptions);
                return Json(new { data = dataPayment });
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, new { data = "error", message = ex.Message });
            }

        }

        [HttpGet("ListarBancos")]
        public async Task<ActionResult> ListarBancos()
        {
            //MercadoPagoConfig.AccessToken = "APP_USR-4378491028291916-122522-d34b176566023da15341d7f0277ed1bc-1608202399";
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            string token = configuration["AccessToken"];
            MercadoPagoConfig.AccessToken = token;
            var client = new PaymentMethodClient();
            ResourcesList<PaymentMethod> paymentMethods = await client.ListAsync();

            var bank = paymentMethods
            .Where(pm => pm.Id == "pse")
            .Select(pm => new
            {
                FinancialInstitutions = pm.FinancialInstitutions.Select(d => new
                {
                    d.Id,
                    d.Description
                })

            });

            return Json(bank);
        }

        [HttpGet("UpdateState")]
        public async Task<ActionResult> UpdateState(int id)
        {
            Voluntario voluntario = new Voluntario();
            try
            {
                voluntario = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: i => i.Id == id);
                if (voluntario != null)
                {
                    switch (voluntario.EstadoId)
                    {
                        case 1:
                            voluntario.EstadoId = 2;
                            break;

                        case 4:
                            voluntario.EstadoId = 5;
                            break;
                    }
                    List<int> estadosIncompletos = new List<int> { 1,2,3,4 };
                    
                    if (estadosIncompletos.Contains(voluntario.EstadoId))
                    {
                        DateTime? registro = voluntario.FechaRegistro;
                        DateTime? fechalimite = DateTime.Now.AddMonths(-5);
                        if (registro <= fechalimite)
                        {
                            voluntario.EstadoId = 8;
                        }
                    }

                    _contenedorTrabajo.Voluntario.Update(voluntario);
                    _contenedorTrabajo.save();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ApiController.UpdateState()"); ;
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("SetStateBrigada")]
        public async Task<IActionResult> SetStateBrigada(DateTime update)
        {
            Brigada brigada = new Brigada();
            try
            {
                string email = string.Empty;
                int state = 0;
                if (User.Identity.Name != null)
                {
                    email = User.Identity.Name.ToString();
                }
                var usuario = _contenedorTrabajo.Usuario.GetFirstOrDefault(u => u.Email == email);
                brigada = _contenedorTrabajo.Brigada.GetFirstOrDefault(b => b.Id == usuario.BrigadaId);
                if (brigada.EstadoBrigadaId == 1)
                {
                    var param = _contenedorTrabajo.Params.GetFirstOrDefault(p => p.id == 4);
                    var voluntarios = _contenedorTrabajo.Asistencia.GetALL(filter: a => a.Voluntario.Colegio.BrigadaId == brigada.Id,includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio,Voluntario.Colegio.Brigada");
                    var count = 0;
                    foreach (var item in voluntarios)
                    {
                        var totalAsistencia = voluntarios.Where(t => t.VoluntarioId == item.VoluntarioId).Count(); 
                        if (totalAsistencia != Convert.ToInt32(param.Valor))
                        {
                            if (item.Voluntario.EstadoId != 8 || item.Voluntario.EstadoId != 5 || item.Voluntario.EstadoId != 6 || item.Voluntario.EstadoId != 7)
                            {
                                count++;
                            }
                        }
                    }
                    if (count == 0)
                    {
                        state = 2;
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status204NoContent);
                    }
                }
                else
                {
                    state = 1;
                }
                brigada.EstadoBrigadaId = state;
                brigada.fechaActualizacion = update;
                _contenedorTrabajo.Brigada.Update(brigada);
                _contenedorTrabajo.save();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet("GetStateBrigada")]
        public async Task<IActionResult> GetStateBrigada()
        {
            Brigada brigada = new Brigada();
            try
            {
                string email = string.Empty;
                if (User.Identity.Name != null)
                {
                    email = User.Identity.Name.ToString();
                }
                var usuario = _contenedorTrabajo.Usuario.GetFirstOrDefault(u => u.Email == email);
                brigada = _contenedorTrabajo.Brigada.GetFirstOrDefault(b => b.Id == usuario.BrigadaId);
                return Json(brigada.EstadoBrigadaId);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("GetCountPerson")]
        public async Task<IActionResult> GetCountPerson()
        {
            int count = 0;
            try
            {
                if (!User.IsInRole(CNT.Gestor) && !User.IsInRole(CNT.Colegio))
                {
                    count = _contenedorTrabajo.Voluntario.GetALL().Count();
                    return Json(count);
                }
                else
                {
                    string mail = User.Identity.Name.ToString() ?? "representacion.institucional@funsar.org.co";
                    var usuario = _contenedorTrabajo.Usuario.GetFirstOrDefault(u => u.Email == mail);
                    var voluntario = _contenedorTrabajo.Voluntario.GetALL(filter: v => v.Colegio.BrigadaId == usuario.BrigadaId && v.FechaRegistro.Value.Year == DateTime.Now.Year,includeProperties: "Colegio");
                    count = voluntario.Count();
                    return Json(count);
                }
                
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
