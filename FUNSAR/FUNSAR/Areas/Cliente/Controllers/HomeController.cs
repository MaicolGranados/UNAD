using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.HerramientasComunes;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Diagnostics;
//MP
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using System.Threading.Tasks;
using MercadoPago.Client.Preference;
using MercadoPago.Client.PaymentMethod;
using MercadoPago.Resource;
using MercadoPago.Resource.PaymentMethod;
using MercadoPago.Resource.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.DotNet.MSIdentity.Shared;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using Microsoft.VisualStudio.Debugger.Contracts.EditAndContinue;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using FUNSAR.Areas.API;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NPOI.OpenXmlFormats.Dml;
using System.Linq;
using AspNetCoreGeneratedDocument;
using NPOI.HPSF;
using System.Net;

namespace FUNSAR.Areas.Cliente.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _env;

        public HomeController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment env)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    HerramientasComunes.Log log = new HerramientasComunes.Log();
                    HomeVM homeVm = new HomeVM()
                    {
                        Slider = _contenedorTrabajo.Slider.GetALL(),
                        ListaArticulos = _contenedorTrabajo.Articulo.GetALL(filter: a => a.Activo == true),
                        certificado = null
                    };
                    if (HttpContext.Session.GetString("Registro") == "Exitoso")
                    {
                        homeVm.resultado = "RegistroExitoso";
                        HttpContext.Session.Remove("Registro");
                    }
                    else if (HttpContext.Session.GetString("Registro") == "Existente")
                    {
                        homeVm.resultado = "Existente";
                        HttpContext.Session.Remove("Registro");
                    }
                    else if (HttpContext.Session.GetString("Registro") == "Error")
                    {
                        homeVm.resultado = "Error";
                        HttpContext.Session.Remove("Registro");
                    }

                    if (HttpContext.Session.GetString("PFE") == "Entregado")
                    {
                        homeVm.resultado = "PlanFalimiarEntregado";
                        HttpContext.Session.Remove("PFE");
                    }
                    else if (HttpContext.Session.GetString("PFE") == "Error")
                    {
                        homeVm.resultado = "PlanFalimiarError";
                        HttpContext.Session.Remove("RegiPFEstro");
                    }
                    else if (HttpContext.Session.GetString("Registro") == "Brigada")
                    {
                        homeVm.resultado = "Brigada";
                        HttpContext.Session.Remove("Registro");
                    }
                    else if (HttpContext.Session.GetString("Pago") == "Error")
                    {
                        homeVm.resultado = "ErrorPago";
                        HttpContext.Session.Remove("Pago");
                    }
                    else if (HttpContext.Session.GetString("Pago") == "Data")
                    {
                        homeVm.resultado = "DataPago";
                        HttpContext.Session.Remove("Pago");
                    }
                    if (homeVm.resultado == null)
                    {
                        homeVm.resultado = "SinData";
                    }
                    if (homeVm.ListaArticulos != null)
                    {
                        List<int> idActividad = new List<int>();
                        foreach (var actividad in homeVm.ListaArticulos)
                        {
                            idActividad.Add(actividad.CategoriaId);
                        }
                        homeVm.ListaCategorias = _contenedorTrabajo.Categoria.GetALL(filter: c => idActividad.Contains(c.Id));
                    }
                    return View(homeVm);
                }
                else
                {
                    return Redirect("/Admin");
                }
                
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Index");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var categoriaDesdeDb = _contenedorTrabajo.Categoria.GetFirstOrDefault(filter: c => c.Id == id);
                if (categoriaDesdeDb == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ArticuloVM articulo = new ArticuloVM();
                    articulo.Categoria = categoriaDesdeDb;
                    return View(articulo);
                }
                
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Details");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        public async Task<IActionResult> RegistroEvento(int id)
        {
            try
            {
                ArticuloVM articulo = new ArticuloVM();
                var sessionid = HttpContext.Session.GetString("SessionId");
                var voluntarioid = HttpContext.Session.GetString("IdVoluntario");
                var voluntario = _contenedorTrabajo.Voluntario.GetFirstOrDefault(v => v.Id == Convert.ToInt32(voluntarioid) , includeProperties: "Colegio");

                if (voluntario == null)
                {
                    voluntario = new Voluntario();
                    voluntario.Id = 0;
                }
                articulo.voluntario = voluntario.Id;
                if (articulo.voluntario > 0)
                {
                    articulo.Articulo = _contenedorTrabajo.Articulo.GetFirstOrDefault(a => a.ServicioId == id && a.BrigadaId.Contains(voluntario.Colegio.BrigadaId.ToString()));
                    if (articulo.Articulo == null)
                    {
                        HttpContext.Session.SetString("Pago", "Data");
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    articulo.ListaArticulo = _contenedorTrabajo.Articulo.GetListaArticulo();
                    if (articulo.ListaArticulo == null)
                    {
                        HttpContext.Session.SetString("Pago", "Data");
                        return RedirectToAction(nameof(Index));
                    }
                }

                articulo.ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetListaDocumento();
                return View(articulo);

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Details");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        [HttpPost]
        public async Task<IActionResult> RegistroEvento(ArticuloVM articulovm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AsistenteSalida asistente = new AsistenteSalida();
                    Encrypt encrypt = new Encrypt();
                    var servicioId = _contenedorTrabajo.Articulo.GetFirstOrDefault(filter: a => a.Id == articulovm.Articulo.Id); 
                    if (articulovm.AsistenteSalida != null)
                    {
                        asistente = articulovm.AsistenteSalida;
                        asistente.ServicioId = servicioId.ServicioId;
                        asistente.FechaRegistro = DateTime.Now;
                        asistente.EstadoPago = "Registrado";
                        _contenedorTrabajo.AsistenteSalida.Add(asistente);
                        _contenedorTrabajo.save();

                        var asistenteData = _contenedorTrabajo.AsistenteSalida.GetFirstOrDefault(filter: a => a.DocumentoId == articulovm.AsistenteSalida.DocumentoId && a.Documento == articulovm.AsistenteSalida.Documento);


                        var cookie1 = HttpContext.Session.GetString("SessionId");
                        if (cookie1 != null)
                        {
                            HttpContext.Session.SetString("SessionId", encrypt.sha256(Convert.ToString("Temporal")));
                            Thread.Sleep(60);
                        }
                        var cookie2 = HttpContext.Session.GetString("IdVoluntario");
                        if (cookie2 != null)
                        {
                            HttpContext.Session.SetString("IdVoluntario", encrypt.sha256(Convert.ToString("Temporal")));
                            Thread.Sleep(60);
                        }

                        HttpContext.Session.SetString("IdVoluntario", asistenteData.Id.ToString());
                        HttpContext.Session.SetString("SessionId","S-" + encrypt.sha256(Convert.ToString(asistenteData.Id)));
                        HttpContext.Session.SetString("ServiceId", Convert.ToString(servicioId.ServicioId));

                        CookieOptions options = new CookieOptions {
                            Expires = DateTimeOffset.Now.AddMinutes(5),
                            HttpOnly = false,
                            Secure = true,
                            SameSite = SameSiteMode.Strict
                        };
                        HttpContext.Response.Cookies.Append("ServiceId", Convert.ToString(servicioId.ServicioId),options);

                        string downloadUrl = Url.Action("DescargarArchivo", "Home", new { id = servicioId.Id });

                        return Json(new { success = true, downloadUrl });

                        //return RedirectToAction("GenerarPago", new { id = Convert.ToInt32(servicioId.ServicioId) });
                    }
                    else
                    {
                        HttpContext.Session.SetString("Pago", "Data");
                        return RedirectToAction(nameof(Index));
                    }
                    
                }
                return View(articulovm);

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Details");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }
        public IActionResult DescargarArchivo(int id)
        {
            var archivo = _contenedorTrabajo.Articulo.GetFirstOrDefault(a => a.Id == id);
            var strin = _env.WebRootPath;
            var filePath = _env.WebRootPath + archivo.UrlDocumento;
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", archivo.UrlDocumento);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> Index(HomeVM model, string documento)
        {
            try
            {
                if (documento == null)
                {
                    HomeVM home = new HomeVM()
                    {
                        Slider = _contenedorTrabajo.Slider.GetALL(),
                        ListaArticulos = _contenedorTrabajo.Articulo.GetALL(),
                        certificado = null
                    };
                    home.resultado = "SinDoc";
                    return View(home);
                }
                if (ModelState.IsValid)
                {
                    var builder = WebApplication.CreateBuilder();
                    var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                    string conection = connectionString;

                    FUNSAR.Utilidades.OperacionCertificado consultarCertificado = new Utilidades.OperacionCertificado();
                    var selectedValue = model.SelectTdocumento;
                    ViewBag.Tdocumento = selectedValue.ToString();
                    string tdocumento = ViewBag.Tdocumento;
                    int resultado = consultarCertificado.DatosCertificado(tdocumento, documento, conection);
                    HomeVM home = new HomeVM()
                    {
                        Slider = _contenedorTrabajo.Slider.GetALL(),
                        ListaArticulos = _contenedorTrabajo.Articulo.GetALL(),
                        certificado = _contenedorTrabajo.Certificado.Get(resultado),
                        resultado = "data"
                    };
                    if (resultado == 0)
                    {
                        Encrypt encrypt = new Encrypt();
                        int voluntario = consultarCertificado.DatosVoluntario(tdocumento, documento, conection);

                        CookieOptions cookieOptions = new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddMinutes(5),
                            Secure = true,
                            HttpOnly = false,
                            SameSite = SameSiteMode.Strict
                        };
                        HttpContext.Response.Cookies.Append("IdVoluntario", voluntario.ToString(),cookieOptions);
                        HttpContext.Session.SetString("IdVoluntario", voluntario.ToString());
                        HttpContext.Session.SetString("SessionId", encrypt.sha256(Convert.ToString(voluntario)));

                        if (voluntario == 0)
                        {
                            home.resultado = "SinResultado";
                            return View(home);
                        }
                        home.voluntario = _contenedorTrabajo.Voluntario.Get(voluntario);
                        var estadoVoluntario = home.voluntario;
                        if (estadoVoluntario.EstadoId == 1)
                        {
                            home.Slider = _contenedorTrabajo.Slider.GetALL();
                            home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                            home.certificado = null;
                            home.resultado = "Inscrito";
                        }
                        if (estadoVoluntario.EstadoId == 2)
                        {
                            home.Slider = _contenedorTrabajo.Slider.GetALL();
                            home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                            home.certificado = null;
                            home.resultado = "PlanFamiliar";
                        }
                        PFE pfe = new PFE();
                        var pfeVoluntario = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == voluntario);
                        if (home.resultado == "PlanFamiliar" && pfeVoluntario != null)
                        {
                            if (pfeVoluntario.EstadoPFEId == 1)
                            {
                                home.Slider = _contenedorTrabajo.Slider.GetALL();
                                home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                                home.certificado = null;
                                home.resultado = "PlanFalimiarAprobado";
                            }
                            if (pfeVoluntario.EstadoPFEId == 2)
                            {
                                home.Slider = _contenedorTrabajo.Slider.GetALL();
                                home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                                home.certificado = null;
                                home.resultado = "PlanFalimiarCorregir";
                                home.DetallePfe = pfeVoluntario.Detalle;
                            }
                            if (pfeVoluntario.EstadoPFEId == 3)
                            {
                                home.Slider = _contenedorTrabajo.Slider.GetALL();
                                home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                                home.certificado = null;
                                home.resultado = "PlanFalimiarEntregado";
                            }
                        }
                        if (estadoVoluntario.EstadoId == 3)
                        {
                            home.Slider = _contenedorTrabajo.Slider.GetALL();
                            home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                            home.certificado = null;
                            home.resultado = "nRequisitos";
                        }
                        if (estadoVoluntario.EstadoId == 4)
                        {
                            home.Slider = _contenedorTrabajo.Slider.GetALL();
                            home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                            home.certificado = null;
                            home.resultado = "sRequisitos";
                        }
                        if (estadoVoluntario.EstadoId == 5)
                        {
                            home.Slider = _contenedorTrabajo.Slider.GetALL();
                            home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                            home.certificado = null;
                            home.resultado = "PendientePago";
                            HttpContext.Session.SetString("idService", Convert.ToString(1));
                            HttpContext.Session.SetString("docVoluntario", documento);
                        }
                        if (estadoVoluntario.EstadoId == 8)
                        {
                            home.Slider = _contenedorTrabajo.Slider.GetALL();
                            home.ListaArticulos = _contenedorTrabajo.Articulo.GetALL();
                            home.certificado = null;
                            home.resultado = "Incompleto";
                        }
                        return View(home);
                    }

                    return View(home);

                }
                else
                {
                    HomeVM home = new HomeVM()
                    {
                        Slider = _contenedorTrabajo.Slider.GetALL(),
                        ListaArticulos = _contenedorTrabajo.Articulo.GetALL(),
                        certificado = null
                    };
                    home.resultado = "SinResultado";
                    return View(home);
                }
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Index(model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        public async Task<IActionResult> Registro(VoluntarioVM bri)
        {
            try
            {

                if (bri.Colegio == null)
                {
                    bri.Voluntario = new FUNSAR.Models.Voluntario();
                    bri.ListaColegio = _contenedorTrabajo.Colegio.GetListaColegios();
                    bri.ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio();
                    bri.valida = "Colegio";
                }
                else
                {
                    
                    HttpContext.Session.SetString("IdColegio", bri.Colegio.Id.ToString());
                    HttpContext.Session.SetString("IdJornada", bri.JornadaColegio.Id.ToString());
                    bri.Acudiente = new FUNSAR.Models.Acudiente();
                    bri.ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetListaDocumento();
                    bri.valida = "Acudiente";
                }

                return View(bri);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Registro(model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        [HttpGet]
        public IActionResult Colegio(string term)
        {
            var colegios = _contenedorTrabajo.Colegio.BuscarColegio(term);
            return Json(colegios);
        }

        [HttpPost]
        public IActionResult ValidarColegio(int id)
        {
            var colegios = _contenedorTrabajo.Colegio.GetFirstOrDefault(filter: c=>c.Id == id);
            return Json(colegios);
        }

        public async Task<IActionResult> RegistroParticipante(VoluntarioVM bri)
        {
            try
            {
                int brigada = Convert.ToInt32(HttpContext.Session.GetString("IdColegio"));
                int jornada = Convert.ToInt32(HttpContext.Session.GetString("IdJornada"));
                bri.Voluntario = new FUNSAR.Models.Voluntario();
                //bri.ListaColegio = _contenedorTrabajo.Colegio.GetListaColegios(brigada);
                bri.ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetListaDocumento();
                bri.valida = "Registro";

                return View(bri);

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.RegistroParticipante(model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoluntarioVM volvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //normalizar documento
                    if (volvm.Voluntario != null && volvm.Voluntario.Documento != null)
                    {
                        volvm.Voluntario.Documento = volvm.Voluntario.Documento.Trim();
                    }
                    //
                    HomeVM home = new HomeVM();
                    OperacionCertificado voluntario = new OperacionCertificado();
                    Asistencia asistencia = new Asistencia();
                    EstadoAsistencia estadoAsistencia = new EstadoAsistencia();
                    var builder = WebApplication.CreateBuilder();
                    var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                    string conection = connectionString;
                    int idVol = 0;
                    idVol = voluntario.IdVoluntario(volvm.Voluntario.Documento, volvm.Voluntario.DocumentoId, conection);

                    var colegio = _contenedorTrabajo.Colegio.GetFirstOrDefault(c => c.Id == Convert.ToInt32(HttpContext.Session.GetString("IdColegio")));
                    var jornada = _contenedorTrabajo.JornadaColegio.GetFirstOrDefault(j => j.Id == Convert.ToInt32(HttpContext.Session.GetString("IdJornada")));
                    if (colegio != null && jornada != null)
                    {
                        var brigada = _contenedorTrabajo.Brigada.GetFirstOrDefault(b => b.Id == colegio.BrigadaId);
                        if (brigada.EstadoBrigadaId == 1)
                        {
                            if (HttpContext.Session.GetString("IdAcudiente") != null)
                            {
                                if (idVol == 0)
                                {
                                    volvm.Voluntario.ColegioId = colegio.Id;
                                    volvm.Voluntario.JornadaId = jornada.Id;
                                    volvm.Voluntario.EstadoId = 1;
                                    volvm.Voluntario.ProcesoId = 1;
                                    volvm.Voluntario.FechaRegistro = DateTime.Now;
                                    _contenedorTrabajo.Voluntario.Add(volvm.Voluntario);
                                    _contenedorTrabajo.save();
                                    Acudiente acudiente = new Acudiente();

                                    acudiente.Id = Convert.ToInt32(HttpContext.Session.GetString("IdAcudiente"));
                                    acudiente.VoluntarioId = volvm.Voluntario.Id;
                                    _contenedorTrabajo.Acudiente.Update(acudiente);
                                    _contenedorTrabajo.save();
                                    
                                    HttpContext.Session.SetString("Registro", "Exitoso");

                                    asistencia.VoluntarioId = volvm.Voluntario.Id;
                                    asistencia.EstadoAsistenciaId = 1;
                                    asistencia.Fecha = brigada.fechaActualizacion;
                                    _contenedorTrabajo.Asistencia.Add(asistencia);
                                    _contenedorTrabajo.save();

                                    return RedirectToAction(nameof(Index));
                                }
                                else
                                {
                                    HttpContext.Session.SetString("Registro", "Existente");
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            if (idVol == 0)
                            {
                                volvm.Voluntario.ColegioId = colegio.Id;
                                volvm.Voluntario.JornadaId = jornada.Id;
                                volvm.Voluntario.EstadoId = 1;
                                volvm.Voluntario.ProcesoId = 1;
                                volvm.Voluntario.FechaRegistro = DateTime.Now;
                                _contenedorTrabajo.Voluntario.Add(volvm.Voluntario);
                                _contenedorTrabajo.save();
                                
                                HttpContext.Session.SetString("Registro", "Exitoso");

                                asistencia.VoluntarioId = volvm.Voluntario.Id;
                                asistencia.EstadoAsistenciaId = 1;
                                asistencia.Fecha = brigada.fechaActualizacion;
                                _contenedorTrabajo.Asistencia.Add(asistencia);
                                _contenedorTrabajo.save();

                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                HttpContext.Session.SetString("Registro", "Existente");
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            HttpContext.Session.SetString("Registro", "Brigada");
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        HttpContext.Session.SetString("Registro", "Error");
                        return RedirectToAction(nameof(Index));
                    }

                }
                ViewBag.msg = "Validar los datos ingresados";
                return RedirectToAction(nameof(RegistroParticipante));
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Create(model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateA(VoluntarioVM volvm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var builder = WebApplication.CreateBuilder();
                    var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                    string conection = connectionString;
                    _contenedorTrabajo.Acudiente.Add(volvm.Acudiente);
                    _contenedorTrabajo.save();
                    OperacionCertificado acudiente = new OperacionCertificado();
                    int idA = 0;
                    idA = acudiente.IdAcudiente(volvm.Acudiente.Documento, volvm.Acudiente.DocumentoId, conection);
                    CookieOptions cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddMinutes(5),
                        Secure = true,
                        HttpOnly = false,
                        SameSite = SameSiteMode.Strict
                    };
                    HttpContext.Session.SetString("IdAcudiente", idA.ToString());
                    volvm.valida = "AcudienteCreado";
                    return RedirectToAction(nameof(RegistroParticipante));
                }
                ViewBag.msg = "Validar los datos ingresados";
                return RedirectToAction(nameof(Registro));
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.CreateA(model)");
                return Error();
            }

        }

        public async Task<IActionResult> PDF(int Id)
        {
            try
            {
                Certificado home = new Certificado();

                home = _contenedorTrabajo.Certificado.Get(Id);
                home.TDocumento = _contenedorTrabajo.TDocumento.Get(home.DocumentoId);

                return new ViewAsPdf("PDF", home)
                {
                    FileName = $"CertificadoFunsar{home.Documento}.pdf",
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageMargins = new Rotativa.AspNetCore.Options.Margins(0, 0, 0, 0)
                };
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.PDF()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }


        }

        [HttpPost]
        public async Task<IActionResult> UploadPFE(IFormFile filePFE)
        {
            try
            {
                VoluntarioVM volun = new VoluntarioVM();
                int id = Convert.ToInt32(HttpContext.Session.GetString("IdVoluntario"));
                volun.Voluntario = _contenedorTrabajo.Voluntario.Get(id);
                HomeVM home = new HomeVM();
                PFE pfe = new PFE();
                if (filePFE is not null && volun.Voluntario is not null && filePFE.FileName.ToUpper().Contains(".PDF"))
                {
                    //pfe = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == id);
                    string cuerpoCorreo = "<p>El participante: <br><b>" + volun.Voluntario.Nombre + " " + volun.Voluntario.Apellido + "<br>Con numero de documento:" + volun.Voluntario.Documento + "</b> <br> Ha enviado el plan familiar de emergencias</p> <hr> <p>NOTA: Una vez validado, cambiar el estado y colocar las anotaciones correspondientes en la pagina para que el voluntario pueda visualizar el estado de su proceso.</p>";

                    if (filePFE.Length == 0)
                    {
                        return BadRequest();
                    }
                    var path = "FilesPFE";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fullPath = Path.Combine(path, filePFE.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        filePFE.CopyTo(stream);
                    }
                    fullPath = rename(fullPath, Path.Combine(path, volun.Voluntario.Documento));
                    var builder = WebApplication.CreateBuilder();
                    var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                    string conection = connectionString;
                    OperacionCertificado datos = new OperacionCertificado();
                    string correoDestino = datos.CorreoGestor(id, conection);
                    if (correoDestino == "NA")
                    {
                        correoDestino = "representacion.institucional@funsar.org.co";
                        cuerpoCorreo = "<p>No se encontro el correo asociado a la brigada, favor redireccionar con el area correspondiente.</p><p>El participante: <br><b>" + volun.Voluntario.Nombre + " " + volun.Voluntario.Apellido + "<br>Con numero de documento:" + volun.Voluntario.Documento + "</b> <br> Ha enviado el plan familiar de emergencias</p> <hr> <p>NOTA: Una vez validado, cambiar el estado y colocar las anotaciones correspondientes en la pagina para que el voluntario pueda visualizar el estado de su proceso.</p>";
                    }
                    Correo correo = new Correo();
                    await correo.EnvioGmailAdjunto("no-reply@funsar.org.co", correoDestino, "Plan Familiar de Emergencias: " + volun.Voluntario.Documento, cuerpoCorreo, fullPath);

                    pfe.VoluntarioId = id;
                    pfe.Detalle = "Plan Familiar de Emergencias entregado, pendiente por revisar.";
                    pfe.EstadoPFEId = 3;
                    _contenedorTrabajo.pFE.Add(pfe);
                    _contenedorTrabajo.save();

                    HttpContext.Session.SetString("PFE", "Entregado");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    HttpContext.Session.SetString("PFE", "Error");
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.UploadPDF()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePFE(IFormFile filePFE)
        {

            try
            {
                VoluntarioVM volun = new VoluntarioVM();
                int id = Convert.ToInt32(HttpContext.Session.GetString("IdVoluntario"));
                volun.Voluntario = _contenedorTrabajo.Voluntario.Get(id);
                HomeVM home = new HomeVM();
                PFE pfe = new PFE();

                if (filePFE is not null && volun.Voluntario is not null && filePFE.FileName.ToUpper().Contains(".PDF"))
                {
                    //pfe = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == id);
                    string cuerpoCorreo = "<p>El participante: <br><b>" + volun.Voluntario.Nombre + " " + volun.Voluntario.Apellido + "<br>Con numero de documento:" + volun.Voluntario.Documento + "</b> <br> Ha realizado correcciones al plan familiar de emergencias</p> <hr> <p>NOTA: Una vez validado, cambiar el estado y colocar las anotaciones correspondientes en la pagina para que el voluntario pueda visualizar el estado de su proceso.</p>";
                    //if (pfe.EstadoPFEId == 2)
                    //{
                    //    cuerpoCorreo = "<p>El participante: <br><b>" + volun.Voluntario.Nombre + " " + volun.Voluntario.Apellido + "<br>Con numero de documento:" + volun.Voluntario.Documento + "</b> <br> Ha realizado correcciones al plan familiar de emergencias</p> <hr> <p>NOTA: Una vez validado, cambiar el estado y colocar las anotaciones correspondientes en la pagina para que el voluntario pueda visualizar el estado de su proceso.</p>";
                    //}

                    if (filePFE.Length == 0)
                    {
                        return BadRequest();
                    }
                    var path = "FilesPFE";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fullPath = Path.Combine(path, filePFE.FileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        filePFE.CopyTo(stream);
                    }
                    fullPath = rename(fullPath, Path.Combine(path, volun.Voluntario.Documento));
                    var builder = WebApplication.CreateBuilder();
                    var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                    string conection = connectionString;
                    OperacionCertificado datos = new OperacionCertificado();
                    string correoDestino = datos.CorreoGestor(id, conection);
                    if (correoDestino == "NA")
                    {
                        correoDestino = "representacion.institucional@funsar.org.co";
                        cuerpoCorreo = "<p>No se encontro el correo asociado a la brigada, favor redireccionar con el area correspondiente.</p><p>El participante: <br><b>" + volun.Voluntario.Nombre + " " + volun.Voluntario.Apellido + "<br>Con numero de documento:" + volun.Voluntario.Documento + "</b> <br> Ha enviado el plan familiar de emergencias</p> <hr> <p>NOTA: Una vez validado, cambiar el estado y colocar las anotaciones correspondientes en la pagina para que el voluntario pueda visualizar el estado de su proceso.</p>";
                    }
                    Correo correo = new Correo();
                    await correo.EnvioGmailAdjunto("no-reply@funsar.org.co", correoDestino, "Plan Familiar de Emergencias: " + volun.Voluntario.Documento, cuerpoCorreo, fullPath);

                    pfe.VoluntarioId = id;
                    pfe.Detalle = "Plan familiar de emergencias Corregido.";
                    pfe.EstadoPFEId = 3;
                    _contenedorTrabajo.pFE.Update(pfe);
                    _contenedorTrabajo.save();

                    HttpContext.Session.SetString("PFE", "Entregado");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    HttpContext.Session.SetString("PFE", "Error");
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.UpdatePDF()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
        }

        public async Task<IActionResult> GenerarPago(int id)
        {
            try
            {
                session entity = new session();
                Articulo articulo = new Articulo();
                var sessionId = Convert.ToString(HttpContext.Session.GetString("SessionId"));
                var voluntario = Convert.ToString(HttpContext.Session.GetString("IdVoluntario"));
                if (sessionId != null)
                {
                    entity.sessionid = sessionId;
                }
                else
                {
                    if (voluntario != null)
                    {
                        Encrypt encrypt = new Encrypt();
                        sessionId = encrypt.sha256(Convert.ToString(voluntario));
                        entity.sessionid = sessionId;
                    }
                    else
                    {
                        HttpContext.Session.SetString("Pago", "Error");
                        return RedirectToAction(nameof(Index));
                    }
                    
                }
                if (voluntario != null)
                {
                    entity.sessiondocument = voluntario;

                    var service = _contenedorTrabajo.Servicio.GetFirstOrDefault(a => a.Id== id);

                    if (service != null)
                    {
                        entity.sessionservice = Convert.ToString(service.Id);
                        var getSession = _contenedorTrabajo.Session.GetFirstOrDefault(x => x.sessionid == sessionId);
                        if (getSession == null)
                        {
                            _contenedorTrabajo.Session.Add(entity);
                            _contenedorTrabajo.save();
                        }
                        else
                        {
                            getSession.sessionservice = entity.sessionservice;
                            _contenedorTrabajo.Session.Update(getSession);
                            _contenedorTrabajo.save();
                        }
                        return View();
                    }
                    else
                    {
                        HttpContext.Session.SetString("Pago", "Error");
                        return RedirectToAction(nameof(Index));
                    }
                    
                }
                else
                {
                    HttpContext.Session.SetString("Pago", "Error");
                    return RedirectToAction(nameof(Index));
                }
                
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.GenerarPago()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> GenerarSesion(ArticuloVM articulo, string CategoriaId)
        {
            try
            {
                Encrypt encrypt = new Encrypt();
                var cookie1 = HttpContext.Session.GetString("SessionId");
                if (cookie1 != null)
                {
                    HttpContext.Session.SetString("SessionId", encrypt.sha256("Temporal"));
                    Thread.Sleep(60);
                }
                var cookie2 = HttpContext.Session.GetString("IdVoluntario");
                if (cookie2 != null)
                {
                    CookieOptions co = new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddMinutes(5),
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };
                    Response.Cookies.Append("ServiceId", encrypt.sha256(Convert.ToString("Temporal")), co);
                    HttpContext.Session.SetString("IdVoluntario", encrypt.sha256(Convert.ToString("Temporal")));
                    Thread.Sleep(60);
                }

                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                string conection = connectionString;
                OperacionCertificado consultarCertificado = new OperacionCertificado();
                var selectedValue = articulo.SelectTdocumento;
                ViewBag.Tdocumento = selectedValue.ToString();
                string tdocumento = ViewBag.Tdocumento;
                int? voluntario = consultarCertificado.DatosVoluntario(tdocumento, articulo.Documento, conection);
                
                var datoVoluntario = _contenedorTrabajo.Voluntario.GetFirstOrDefault(v => v.Id == voluntario, includeProperties: "Colegio");
                if (datoVoluntario == null)
                {
                    datoVoluntario = new Voluntario();
                    datoVoluntario.Id = 0;
                }

                CookieOptions opciones = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddMinutes(5),
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("ServiceId", encrypt.sha256(Convert.ToString("Temporal")), opciones);
                HttpContext.Session.SetString("IdVoluntario", datoVoluntario.Id.ToString());
                HttpContext.Session.SetString("SessionId", encrypt.sha256(Convert.ToString(datoVoluntario.Id)));

                if (datoVoluntario.Id > 0)
                {
                    var actividad = _contenedorTrabajo.Articulo.GetFirstOrDefault(s => s.CategoriaId == Convert.ToInt32(CategoriaId) && s.BrigadaId.Contains(datoVoluntario.Colegio.BrigadaId.ToString()));
                    int ServicioId = actividad.ServicioId;

                    if (!ServicioId.Equals("1"))
                    {
                        return RedirectToAction("RegistroEvento", new { id = Convert.ToInt32(ServicioId)});
                    }
                    else
                    {
                        return RedirectToAction("GenerarPago", new { id = Convert.ToInt32(ServicioId) });
                    }
                }
                else
                {
                    return RedirectToAction("RegistroEvento");
                }
                
                    
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.GenerarSesion()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ValidarPago(long payment_id)
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
                                        var acuFromDb = _contenedorTrabajo.Acudiente.GetFirstOrDefault(a => a.VoluntarioId == artivm.Id);
                                        if (acuFromDb != null)
                                        {
                                            _contenedorTrabajo.Acudiente.Remove(acuFromDb);
                                            _contenedorTrabajo.save();
                                        }
                                        var objFromDb = _contenedorTrabajo.Voluntario.Get(artivm.Id);
                                        _contenedorTrabajo.Voluntario.Remove(objFromDb);
                                        _contenedorTrabajo.save();
                                        break;
                                }


                                getPago.validate = "OK";
                            }
                            else
                            {
                                getPago.validate = "NOK";
                            }
                            break;
                    }
                }
                else
                {
                    getPago.validate = "NOK";
                }
                
                return View(getPago);

                #region API
                //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                //IConfiguration configuration = builder.Build();
                //string obtenerEstado = configuration["APIGetEstadoPago"];
                //string token = configuration["BearerToken"];
                //using (HttpClient client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                //    HttpResponseMessage response = await client.GetAsync(obtenerEstado + payment_id);
                //    if (response.IsSuccessStatusCode)
                //    {
                //        var apiResponse = await response.Content.ReadAsStringAsync();
                //        GetPago result = (GetPago)Newtonsoft.Json.JsonConvert.DeserializeObject(apiResponse);
                //        Pagos datosPago = new Pagos();
                //        datosPago.Banco = result.payment_method_id;
                //        datosPago.TDocumentoR = (tipoDocumento?)_contenedorTrabajo.TDocumento.GetIdDocumento(result.payer.identification.type);
                //        datosPago.Secuencia = result.id;
                //        return View();
                //    }
                //    else
                //    {
                //        return View();
                //    }

                //}
                #endregion
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.Procesarpago()");
                getPago.validate = "errorSdk";
                return View(getPago);
            }

        }

        public async Task<IActionResult> ConsultarParticipante(string document)
        {
            return Json(new { data = _contenedorTrabajo.Voluntario.GetFirstOrDefault(filter: s => s.Documento == document) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprobante(long paymentId)
        {
            try
            {
                Pagos pagos = new Pagos();

                pagos = _contenedorTrabajo.Pagos.GetFirstOrDefault(filter: p => p.Secuencia == Convert.ToString(paymentId));
                pagos.TDocumentoR = _contenedorTrabajo.TDocumento.Get(pagos.DocumentoIdR);
                pagos.Servicio = _contenedorTrabajo.Servicio.Get(pagos.ServicioId);

                return new ViewAsPdf("Comprobante", pagos)
                {
                    FileName = $"Comprobante{pagos.Secuencia}.pdf",
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageSize = Rotativa.AspNetCore.Options.Size.A5,
                    PageMargins = new Rotativa.AspNetCore.Options.Margins(6, 0, 0, 0)
                };
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "HomeController.PDF()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }


        }

        private string rename(string path, string document)
        {
            string result = string.Format("{0}.pdf", document);
            if (System.IO.File.Exists(result))
            {
                System.IO.File.Delete(result);
            }
            System.IO.File.Move(path, result);
            return result;
        }

    }
}