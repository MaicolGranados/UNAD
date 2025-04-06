using FUNSAR.AccesoDatos.Data.Repository;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Data.Migrations;
using FUNSAR.Models;
using FUNSAR.Models.Reportes;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Debugger.Contracts.EditAndContinue;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using HerramientasComunes;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo,Gestor,Division,Colegio")]
    [Area("Admin")]
    public class ReportesController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;

        private string result;
        public ReportesController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<string> tipeReport;
                if (User.IsInRole(CNT.SuperAdmin) || User.IsInRole(CNT.Administrativo))
                {
                    tipeReport = new List<string> { "Pagos", "Procesos Participantes", "Salidas" };
                }
                else if (User.IsInRole(CNT.Colegio))
                {
                    tipeReport = new List<string> { "Procesos Participantes" };
                }
                else
                {
                    tipeReport = new List<string> { "No tiene reportes asignados" };
                }

                var listaReportes = tipeReport.Select(r => new SelectListItem
                {
                    Value = r,   
                    Text = r     
                });

                ReporteVM viewReport = new ReporteVM()
                {
                    ListaReporte = listaReportes,
                    listaBrigada = _contenedorTrabajo.Brigada.GetListaBrigadas()
                };
                return View(viewReport);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ReportesController.Index()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Generate(ReporteVM report)
        {
            try
            {
                var typeReport = report.ReporteSeleccionado.ToString();
                if (typeReport != null)
                {
                    switch (typeReport)
                    {
                        case "Pagos":
                            
                            if (report.fechaIni != null && report.fechaFin != null)
                            {
                                IEnumerable<Models.Pagos> datosPagos;
                                List<string> documentosParticipantes;
                                List<string> documentosPagos;
                                IEnumerable<Voluntario> datosParticipante;
                                IEnumerable<Certificado> datosCertificado;
                                IEnumerable<Servicio> datosServicio;

                                datosPagos = _contenedorTrabajo.Pagos
                                        .GetALL()
                                        .AsEnumerable()
                                        .Where(p => DateTime.ParseExact(p.Fechapago, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                                        >= report.fechaIni &&
                                        DateTime.ParseExact(p.Fechapago, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                                        <= report.fechaFin);

                                if (report.filtraBrigada)
                                {
                                    //Obtener los registros de los participantes
                                    documentosParticipantes = datosPagos.Select(p => p.DocumentoP).ToList();
                                    datosParticipante = _contenedorTrabajo.Voluntario.GetALL(v => documentosParticipantes.Contains(v.Documento) && v.Colegio.BrigadaId == report.brigada.Id, includeProperties: "TDocumento,Colegio.Brigada");

                                    //Obtener los registros de los certificados
                                    documentosPagos = datosPagos.Select(p => p.DocumentoP).ToList();
                                    datosCertificado = _contenedorTrabajo.Certificado.GetALL(c => documentosPagos.Contains(c.Documento) && c.BrigadaId == report.brigada.Id, includeProperties: "TDocumento,Brigada");
                                }
                                else
                                {
                                    //Obtener los registros de los participantes
                                    documentosParticipantes = datosPagos.Select(p => p.DocumentoP).ToList();
                                    datosParticipante = _contenedorTrabajo.Voluntario.GetALL(v => documentosParticipantes.Contains(v.Documento), includeProperties: "TDocumento,Colegio.Brigada");

                                    //Obtener los registros de los certificados
                                    documentosPagos = datosPagos.Select(p => p.DocumentoP).ToList();
                                    datosCertificado = _contenedorTrabajo.Certificado.GetALL(c => documentosPagos.Contains(c.Documento), includeProperties: "TDocumento,Brigada");
                                }

                                datosServicio = _contenedorTrabajo.Servicio.GetALL();

                                //Obtener los registros de los acudientes
                                var idsParticipantes = datosParticipante.Select(dp => (int?)dp.Id).ToList();
                                var datosAcudiente = _contenedorTrabajo.Acudiente.GetALL(a => idsParticipantes.Contains(a.VoluntarioId), includeProperties: "TDocumento");

                                var datosConsolidado = new List<FUNSAR.Models.Reportes.Pagos>();

                                foreach (var pago in datosPagos)
                                {

                                    Voluntario? voluntario = new Voluntario();
                                    Acudiente? acudiente = new Acudiente();

                                    pago.Servicio = datosServicio.FirstOrDefault(s => s.Id == pago.ServicioId);

                                    Certificado? certificado = datosCertificado.FirstOrDefault(c => c.Documento == pago.DocumentoP);
                                    
                                    if (certificado == null)
                                    {
                                        voluntario = datosParticipante.FirstOrDefault(v => v.Documento == pago.DocumentoP);

                                        if (voluntario != null)
                                        {
                                            certificado = new Certificado
                                            {
                                                TDocumento = voluntario.TDocumento,
                                                Documento = pago.DocumentoP,
                                                Nombre = voluntario.Nombre,
                                                Apellido = voluntario.Apellido,
                                                Brigada = voluntario.Colegio.Brigada
                                            };
                                        }
                                    }

                                    if (report.acudiente)
                                    {
                                        voluntario = datosParticipante.FirstOrDefault(v => v.Documento == pago.DocumentoP);

                                        if (voluntario != null)
                                        {
                                            acudiente = datosAcudiente.FirstOrDefault(a => a.VoluntarioId == voluntario.Id);
                                        }
                                        else
                                        {
                                            acudiente = null;
                                        }

                                        if (acudiente == null)
                                        {
                                            acudiente = new Acudiente
                                            {
                                                parentesco = "NA",
                                                telefono = pago.CelularR,
                                                Documento = pago.DocumentoR,
                                                nombre = pago.NombreR,
                                                apellido = pago.ApellidoR,
                                                correo = pago.CorreoR,
                                            };

                                        }
                                    }

                                    if (certificado != null && report.acudiente)
                                    {
                                        datosConsolidado.Add(new Models.Reportes.Pagos
                                        {
                                            datoPagos = pago,
                                            datoParticipante = certificado,
                                            datoAcudiente = acudiente
                                        });
                                    }
                                    else if (certificado != null && !report.acudiente)
                                    {
                                        datosConsolidado.Add(new Models.Reportes.Pagos
                                        {
                                            datoPagos = pago,
                                            datoParticipante = certificado,
                                            datoAcudiente = null
                                        });
                                    }
                                }
                               
                                var valores = new List<string> {
                                "dr.datoParticipante.TDocumento.tDocumento",
                                "dr.datoPagos.DocumentoP",
                                "dr.datoParticipante.Nombre",
                                "dr.datoParticipante.Apellido",
                                "dr.datoPagos.Servicio.Detalle",
                                "dr.datoPagos.Valor",
                                "dr.datoPagos.Secuencia",
                                "dr.datoPagos.Fechapago",
                                "dr.datoPagos.Estado",
                                "dr.datoParticipante.Brigada.Nombre"
                                };
                                var encabezados = new List<string>
                                {
                                    "Tipo_Documento",
                                    "Numero_Documento",
                                    "Nombres",
                                    "Apellidos",
                                    "Servicio",
                                    "Valor",
                                    "Numero_Referencia",
                                    "Fecha_Pago",
                                    "Estado_Pago",
                                    "Brigada_Participante"
                                };

                                if (report.responsablePago)
                                {
                                    //Datos Responsable del pago
                                    if (report.rTipoDoc) valores.Add("dr.datoPagos.TDocumentoR.tDocumento");
                                    if (report.rDocumento) valores.Add("dr.datoPagos.DocumentoR");
                                    if (report.rNombre) valores.Add("dr.datoPagos.NombreR");
                                    if (report.rApellidos) valores.Add("dr.datoPagos.ApellidoR");
                                    if (report.rCorreo) valores.Add("dr.datoPagos.CorreoR");
                                    if (report.rBanco) valores.Add("dr.datoPagos.Banco");

                                    if (report.rTipoDoc) encabezados.Add("Tipo_Documento_ResponsablePago");
                                    if (report.rDocumento) encabezados.Add("Numero_Documento_ResponsablePago");
                                    if (report.rNombre) encabezados.Add("Nombres_ResponsablePago");
                                    if (report.rApellidos) encabezados.Add("Apellidos_ResponsablePago");
                                    if (report.rCorreo) encabezados.Add("Correo_ResponsablePago");
                                    if (report.rBanco) encabezados.Add("Banco_ResponsablePago");

                                }

                                if (report.acudiente)
                                {
                                    //Datos Acudiente
                                    if (report.aTipoDoc) valores.Add("dr.datoAcudiente.TDocumento.tDocumento");
                                    if (report.aDocumento) valores.Add("dr.datoAcudiente.Documento");
                                    if (report.aNombre) valores.Add("dr.datoAcudiente.nombre");
                                    if (report.aApellidos) valores.Add("dr.datoAcudiente.apellido");
                                    if (report.aCorreo) valores.Add("dr.datoAcudiente.correo");
                                    if (report.aCelular) valores.Add("dr.datoAcudiente.telefono");

                                    if (report.aTipoDoc) encabezados.Add("Tipo_Documento_Acudiente");
                                    if (report.aDocumento) encabezados.Add("Numero_Documento_Acudiente");
                                    if (report.aNombre) encabezados.Add("Nombres_Acudiente");
                                    if (report.aApellidos) encabezados.Add("Apellidos_Acudiente");
                                    if (report.aCorreo) encabezados.Add("Correo_Acudiente");
                                    if (report.aCelular) encabezados.Add("Celular_Acudiente");
                                }
                                
                                var encabezado = string.Join(",", encabezados);

                                var datosFiltrados = datosConsolidado.Select(dr =>
                                {
                                    var resultado = new Dictionary<string, object>();

                                    foreach (var campo in valores)
                                    {
                                        var partes = campo.Split('.');
                                        object valorActual = dr;

                                        foreach (var parte in partes)
                                        {
                                            if (valorActual == null)
                                                break;

                                            var tipo = valorActual.GetType();

                                            var propiedad = tipo.GetProperty(parte);
                                            if (propiedad != null)
                                            {
                                                valorActual = propiedad.GetValue(valorActual);
                                            }
                                        }

                                        resultado[campo] = valorActual ?? "N/A";
                                    }

                                    return resultado;
                                }).ToList();

                                Excel excel = new Excel(_contenedorTrabajo);
                                var excelBytes = excel.GenerarExcel(datosFiltrados, report, valores, encabezados);

                                // Enviar el archivo Excel al cliente
                                Response.Headers.Add("X-Download-Token", "download-complete");
                                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
                            }

                            break;
                        case "Procesos Participantes":

                            if (report.fechaIni != null && report.fechaFin != null)
                            {
                                IEnumerable<Asistencia>? datosAsistencia;
                                List<int> idParticipantes;
                                List<string> documentosParticipantes;

                                IEnumerable<Voluntario> datosParticipante;
                                IEnumerable<Certificado> datosCertificado;

                                IEnumerable<PFE> datosPFE = null;
                                IEnumerable<Models.Pagos> datosPagos = null;
                                IEnumerable<Servicio> servicios = null;
                                List<string> Salidas = new List<string>();

                                #region CamposObligatorios

                                if (User.IsInRole(CNT.Colegio))
                                {
                                    var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                                    var datoColegio = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(filter: dc => dc.Correo == email);

                                    datosParticipante = _contenedorTrabajo.Voluntario.GetALL(v => v.FechaRegistro >= report.fechaIni && v.FechaRegistro <= report.fechaFin && v.ColegioId == datoColegio.ColegioId, includeProperties: "TDocumento,Colegio.Brigada,estadoPersona");
                                    
                                    idParticipantes = datosParticipante.Select(d => d.Id).ToList();
                                    datosAsistencia = _contenedorTrabajo.Asistencia
                                        .GetALL(a => idParticipantes.Contains(a.VoluntarioId) && a.Voluntario.ColegioId == datoColegio.ColegioId, includeProperties: "Voluntario")
                                        .OrderBy(a => a.Fecha);
                                }
                                else
                                {
                                    if (report.filtraBrigada)
                                    {
                                        datosParticipante = _contenedorTrabajo.Voluntario.GetALL(v => v.FechaRegistro >= report.fechaIni && v.FechaRegistro <= report.fechaFin && v.Colegio.BrigadaId == report.brigada.Id, includeProperties: "TDocumento,Colegio,Colegio.Brigada,estadoPersona");

                                        idParticipantes = datosParticipante.Select(d => d.Id).ToList();
                                        datosAsistencia = _contenedorTrabajo.Asistencia
                                            .GetALL(a => idParticipantes.Contains(a.VoluntarioId) && a.Voluntario.Colegio.BrigadaId == report.brigada.Id, includeProperties: "Voluntario,Voluntario.Colegio")
                                            .OrderBy(a => a.Fecha);
                                    }
                                    else
                                    {
                                        //Obtener asistencias
                                        datosParticipante = _contenedorTrabajo.Voluntario.GetALL(v => v.FechaRegistro >= report.fechaIni && v.FechaRegistro <= report.fechaFin, includeProperties: "TDocumento,Colegio,Colegio.Brigada,estadoPersona");

                                        idParticipantes = datosParticipante.Select(d => d.Id).ToList();
                                        datosAsistencia = _contenedorTrabajo.Asistencia
                                            .GetALL(a => idParticipantes.Contains(a.VoluntarioId), includeProperties: "Voluntario,Voluntario.Colegio")
                                            .OrderBy(a => a.Fecha);

                                    }

                                }

                                //Obtener los registros de los certificados
                                documentosParticipantes = datosParticipante.Select(p => p.Documento).ToList();
                                #endregion

                                if (report.filtraBrigada)
                                {
                                    //Se consulta en certificados en caso que hubiera quedado trocada la informacion
                                    datosCertificado = _contenedorTrabajo.Certificado.GetALL(filter: c => documentosParticipantes.Contains(c.Documento) && c.BrigadaId == report.brigada.Id && c.AnoProceso == report.fechaFin.Value.Year.ToString());

                                }
                                else
                                {
                                    //Se consulta en certificados en caso que hubiera quedado trocada la informacion
                                    datosCertificado = _contenedorTrabajo.Certificado.GetALL(filter: c => documentosParticipantes.Contains(c.Documento) && c.AnoProceso == report.fechaFin.Value.Year.ToString());

                                }

                                //Obtener datos salida
                                if (report.salida)
                                {
                                    List<int> servicio = new List<int> { 2, 3 };
                                    servicios = _contenedorTrabajo.Servicio.GetALL(filter: s => servicio.Contains(s.Id));
                                }

                                #region CamposOpcionales
                                //Obtener datos Plan Familiar
                                if (report.pfe)
                                {
                                    datosPFE = _contenedorTrabajo.pFE.GetALL(p => idParticipantes.Contains((int)p.VoluntarioId), includeProperties: "EstadoPFE");
                                }

                                //Obtener datos Pago
                                if (report.pago)
                                {
                                    datosPagos = _contenedorTrabajo.Pagos.GetALL(p => documentosParticipantes.Contains(p.DocumentoP)).Where(p => p.Estado == "approved");
                                }

                                #endregion

                                var datosConsolidado = new List<FUNSAR.Models.Reportes.ProcesoParticipante>();

                                foreach (var proceso in datosParticipante)
                                {
                                    Voluntario? voluntario = new Voluntario();
                                    PFE? pfe = new PFE { EstadoPFE = new EstadoPFE() };
                                    Models.Pagos? pagos = new Models.Pagos();
                                    Certificado? certificado = new Certificado();
                                    List<string> FechaAsistencias = new List<string>();
                                    List<string> EstadoAsistencias = new List<string>();

                                    //Certificado? certificado = datosCertificado.FirstOrDefault(c => c.Documento == asistencia.Voluntario.Documento);

                                    voluntario = datosParticipante.FirstOrDefault(v => v.Documento == proceso.Documento);

                                    certificado = datosCertificado.FirstOrDefault(c => c.Documento == proceso.Documento);

                                    switch (voluntario.EstadoId)
                                    {
                                        case 3:
                                            voluntario.estadoPersona.estadoPersona = "No Aprobado";
                                            break;
                                        case 4:
                                            voluntario.estadoPersona.estadoPersona = "En aprobación";
                                            break;
                                        case 5:
                                            voluntario.estadoPersona.estadoPersona = "En aprobación";
                                            break;
                                        case 6:
                                            voluntario.estadoPersona.estadoPersona = "En aprobación";
                                            break;
                                        default:
                                            break;
                                    }

                                    if (report.asistenciaDia)
                                    {
                                        List<int> asistencia = datosAsistencia.OrderBy(a => a.Fecha).Where(a => a.VoluntarioId == proceso.Id).Select(a => a.EstadoAsistenciaId).ToList();
                                        for (int i = 0; i < asistencia.Count; i++)
                                        {
                                            switch (asistencia[i])
                                            {
                                                case 1:
                                                    EstadoAsistencias.Add("Asistio");
                                                    break;
                                                case 2:
                                                    EstadoAsistencias.Add("Retardo");
                                                    break;
                                                case 3:
                                                    EstadoAsistencias.Add("Falla");
                                                    break;
                                                case 4:
                                                    EstadoAsistencias.Add("Excusa");
                                                    break;
                                                default :
                                                    EstadoAsistencias.Add("No registra");
                                                    break;
                                            }
                                        }
                                    }

                                    if (report.pfe)
                                    {
                                        pfe = datosPFE.FirstOrDefault(p => p.VoluntarioId == proceso.Id);
                                        if (pfe == null)
                                        {
                                            pfe = new PFE
                                            {
                                                EstadoPFE = new EstadoPFE()
                                            };
                                            pfe.EstadoPFE.Detalle = "Pendiente Entregar";
                                        }
                                    }

                                    if (report.pago)
                                    {
                                        pagos = datosPagos.FirstOrDefault(p => p.DocumentoP == proceso.Documento);

                                        if (pagos != null && certificado == null)
                                        {
                                            pagos.Estado = "pending";
                                        }
                                        else if (pagos == null)
                                        {
                                            pagos = new Models.Pagos();
                                            pagos.Estado = "pending";
                                        }
                                    }

                                    if (report.salida)
                                    {
                                        if (servicios != null && datosPagos != null)
                                        {
                                            var idServicios = servicios.Select(s => s.Id).ToList();
                                            List<int> salidasPagos = datosPagos.Where(p => idServicios.Contains(p.ServicioId) && p.DocumentoP == proceso.Documento).Select(p => p.ServicioId).ToList();
                                            if (salidasPagos.Count() > 0)
                                            {
                                                for (int i = 0; i < servicios.Count(); i++)
                                                {
                                                    if (salidasPagos[i] != null)
                                                    {
                                                        Salidas.Add("Asistio");
                                                    }
                                                    else
                                                    {
                                                        Salidas.Add("No Asistio");
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Salidas.Add("No registra");
                                            }
                                        }
                                        else
                                        {
                                            Salidas.Add("No registra");
                                        }
                                    }

                                    if (certificado != null)
                                    {
                                        voluntario.estadoPersona.estadoPersona = "Certificado";

                                        if (report.pfe)
                                        {
                                            pfe = new PFE
                                            {
                                                EstadoPFE = new EstadoPFE()
                                            };

                                            pfe.EstadoPFE.Detalle = "Aprobado";
                                        }

                                        if (report.pago)
                                        {
                                            pagos = new Models.Pagos();
                                            pagos.Estado = "approved";
                                        }

                                        voluntario.estadoPersona = null;
                                    }

                                    datosConsolidado.Add(new Models.Reportes.ProcesoParticipante
                                    {
                                        datoParticipante = voluntario,
                                        estadoAsistencia = EstadoAsistencias,
                                        datoPfe = pfe,
                                        datoPagos = pagos,
                                        salidas = Salidas
                                    });
                                }


                                var valores = new List<string> {
                                            "dr.datoParticipante.TDocumento.tDocumento",
                                            "dr.datoParticipante.Documento",
                                            "dr.datoParticipante.Nombre",
                                            "dr.datoParticipante.Apellido",
                                            "dr.datoParticipante.estadoPersona.estadoPersona",
                                            "dr.datoParticipante.Colegio.Brigada.Nombre"
                                        };
                                var encabezados = new List<string>
                                        {
                                            "Tipo_Documento",
                                            "Numero_Documento",
                                            "Nombres",
                                            "Apellidos",
                                            "Estado_Proceso",
                                            "Brigada"
                                        };

                                if (report.asistenciaDia)
                                {
                                    var param = _contenedorTrabajo.Params.GetFirstOrDefault(filter: p => p.id == 4);

                                    for (int i = 0; i < Convert.ToInt32(param.Valor); i++)
                                    {
                                        string a = string.Format("dr.estadoAsistencia[{0}]", i);
                                        valores.Add(a);
                                    }
                                    if (User.IsInRole(CNT.Administrativo) || User.IsInRole(CNT.SuperAdmin))
                                    {
                                        for (int i = 1; i <= Convert.ToInt32(param.Valor); i++)
                                        {
                                            string b = string.Format("Dia_{0}", i);
                                            encabezados.Add(b);
                                        }
                                    }
                                    else
                                    {
                                        var fecha = datosParticipante.Select(f => f.Colegio.Brigada.fechaActualizacion);
                                        var dia = fecha.First();
                                        var encabezadoFecha = new List<string>();
                                        encabezadoFecha.Add(dia.ToString("dd/MM/yyyy"));
                                        for (int i = 0; i <= Convert.ToInt32(param.Valor); i++)
                                        {
                                            dia = dia.AddDays(7);
                                            if (encabezadoFecha.Count == Convert.ToInt32(param.Valor)) break;
                                            encabezadoFecha.Add(dia.ToString("dd/MM/yyyy"));
                                        }
                                        for (int i = 0; i < encabezadoFecha.Count; i++)
                                        {
                                            encabezados.Add(encabezadoFecha[i].ToString());
                                        }
                                    }
                                    
                                    
                                }

                                if (report.salida)
                                {
                                    if (servicios != null)
                                    {
                                        for (int i = 0; i < servicios.Count(); i++)
                                        {
                                            if (Salidas[i].ToLower() == "no registra")
                                            {
                                                encabezados.Add("Salida de campo");
                                            }
                                            else
                                            {
                                                encabezados.Add(string.Format("Salida de campo {0}", i));
                                            }
                                            valores.Add(string.Format("dr.salidas[{0}]", i));
                                        }
                                    }
                                    else
                                    {
                                        encabezados.Add("Salida de campo");
                                        valores.Add("No registra");
                                    }
                                    
                                }

                                if (report.pfe)
                                {
                                    //Datos pfe
                                    valores.Add("dr.datoPfe.EstadoPFE.Detalle");
                                    encabezados.Add("Estado_Plan_Familiar");
                                }

                                if (report.pago)
                                {
                                    valores.Add("dr.datoPagos.Estado");
                                    encabezados.Add("Estado_Pago");
                                }

                                var encabezado = string.Join(",", encabezados);

                                var datosFiltrados = datosConsolidado.Select(dr =>
                                {
                                    var resultado = new Dictionary<string, object>();

                                    foreach (var campo in valores)
                                    {
                                        object valorActual = dr;
                                        string[] partes = campo.Split('.');

                                        foreach (var parte in partes)
                                        {
                                            if (valorActual == null)
                                                break;

                                            var tipo = valorActual.GetType();

                                            // Detectar si el campo contiene un índice, como "fechaAsistencia[0]"
                                            if (parte.Contains("[") && parte.Contains("]"))
                                            {
                                                // Extraer el nombre de la propiedad (por ejemplo, "fechaAsistencia")
                                                var nombrePropiedad = parte.Substring(0, parte.IndexOf("["));

                                                // Extraer el índice (por ejemplo, "0")
                                                var indice = int.Parse(parte.Substring(parte.IndexOf("[") + 1, parte.IndexOf("]") - parte.IndexOf("[") - 1));

                                                // Obtener la propiedad (por ejemplo, la lista de asistencias)
                                                var propiedad = tipo.GetProperty(nombrePropiedad);
                                                if (propiedad != null)
                                                {
                                                    // Obtener el valor de la propiedad (la lista)
                                                    var lista = propiedad.GetValue(valorActual) as IList;

                                                    // Si es una lista y el índice es válido, accedemos al valor por índice
                                                    if (lista != null && lista.Count > indice)
                                                    {
                                                        valorActual = lista[indice];
                                                    }
                                                    else
                                                    {
                                                        valorActual = "N/A"; // Valor por defecto si no hay suficiente asistencia
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Acceso normal si no es una lista
                                                var propiedad = tipo.GetProperty(parte);
                                                if (propiedad != null)
                                                {
                                                    valorActual = propiedad.GetValue(valorActual);
                                                }
                                            }
                                        }

                                        resultado[campo] = valorActual ?? "N/A";
                                    }

                                    return resultado;
                                }).ToList();

                                Excel excel = new Excel(_contenedorTrabajo);
                                var excelBytes = excel.GenerarExcel(datosFiltrados, report, valores, encabezados);

                                Response.Headers.Add("X-Download-Token", "download-complete");
                                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
                            }
                            break;
                        case "Salidas":

                            if (report.fechaIni != null && report.fechaFin != null)
                            {
                                IEnumerable<Models.AsistenteSalida> asistenteSalidas;

                                asistenteSalidas = _contenedorTrabajo.AsistenteSalida
                                        .GetALL(includeProperties: "TDocumento,Servicio")
                                        .AsEnumerable()
                                        .Where(p => p.FechaRegistro
                                        >= report.fechaIni &&
                                        p.FechaRegistro
                                        <= report.fechaFin);

                                var datosConsolidado = new List<FUNSAR.Models.Reportes.AsistenteSalida>();

                                foreach (var asistente in asistenteSalidas)
                                {
                                    datosConsolidado.Add(new Models.Reportes.AsistenteSalida
                                    { 
                                        asistenteSalida = asistente
                                    });
                                }

                                var valores = new List<string> {
                                "dr.asistenteSalida.TDocumento.tDocumento",
                                "dr.asistenteSalida.Documento",
                                "dr.asistenteSalida.Nombre",
                                "dr.asistenteSalida.Apellido",
                                "dr.asistenteSalida.telefono",
                                "dr.asistenteSalida.correo",
                                "dr.asistenteSalida.Servicio.Detalle",
                                "dr.asistenteSalida.EstadoPago",
                                "dr.asistenteSalida.ReferenciaPago",
                                "dr.asistenteSalida.FechaRegistro"
                                };
                                var encabezados = new List<string>
                                {
                                    "Tipo_Documento",
                                    "Numero_Documento",
                                    "Nombres",
                                    "Apellidos",
                                    "Celular",
                                    "Correo",
                                    "Salida",
                                    "Estado_Pago",
                                    "Referencia_Pago",
                                    "Fecha_Registro"
                                };

                                var encabezado = string.Join(",", encabezados);

                                var datosFiltrados = datosConsolidado.Select(dr =>
                                {
                                    var resultado = new Dictionary<string, object>();

                                    foreach (var campo in valores)
                                    {
                                        var partes = campo.Split('.');
                                        object valorActual = dr;

                                        foreach (var parte in partes)
                                        {
                                            if (valorActual == null)
                                                break;

                                            var tipo = valorActual.GetType();

                                            var propiedad = tipo.GetProperty(parte);
                                            if (propiedad != null)
                                            {
                                                valorActual = propiedad.GetValue(valorActual);
                                            }
                                        }

                                        resultado[campo] = valorActual ?? "N/A";
                                    }

                                    return resultado;
                                }).ToList();

                                Excel excel = new Excel(_contenedorTrabajo);
                                var excelBytes = excel.GenerarExcel(datosFiltrados, report, valores, encabezados);

                                // Enviar el archivo Excel al cliente
                                Response.Headers.Add("X-Download-Token", "download-complete");
                                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
                            }

                            break;
                        default:
                            break;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ReportesController.Generate()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

    }
}
