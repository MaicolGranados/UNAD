using FUNSAR.AccesoDatos.Data;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.HerramientasComunes;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo,Gestor,Division,Colegio")]
    [Area("Admin")]
    public class VoluntariosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;
        private readonly ApplicationDbContext _db;
        public VoluntariosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment, ApplicationDbContext db)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                List<int> idEstado = new List<int>();
                if (User.IsInRole(CNT.Administrativo))
                {
                    idEstado.Add(5);
                    idEstado.Add(6);
                }
                else if (User.IsInRole(CNT.Gestor))
                {
                    idEstado.Add(1);
                    idEstado.Add(2);
                }
                else if (User.IsInRole(CNT.SuperAdmin))
                {
                    idEstado.Add(1);
                    idEstado.Add(2);
                    idEstado.Add(4);
                    idEstado.Add(5);
                    idEstado.Add(6);
                }
                else if (User.IsInRole(CNT.Division))
                {
                    idEstado.Add(5);
                }


                VoluntarioVM artivm = new VoluntarioVM()
                {
                    Voluntario = new FUNSAR.Models.Voluntario(),
                    ListaColegio = _contenedorTrabajo.Colegio.GetListaColegios(),
                    ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetListaDocumento(),
                    ListaEstado = _contenedorTrabajo.estadoPersona.GetListaEstado(idEstado),
                    ListaProceso = _contenedorTrabajo.Proceso.GetListaProceso(),
                    PFE = new PFE()
                };

                if (id != null)
                {
                    artivm.Acudiente = _contenedorTrabajo.Acudiente.GetFirstOrDefault(filter: a => a.VoluntarioId == id);
                    artivm.Voluntario = _contenedorTrabajo.Voluntario.Get(id.GetValueOrDefault());
                    if ((artivm.PFE = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == artivm.Voluntario.Id)) != null)
                    {
                        artivm.Voluntario = _contenedorTrabajo.Voluntario.Get(id.GetValueOrDefault());
                        artivm.PFE = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == artivm.Voluntario.Id);
                        artivm.ListaEstadoPFE = _contenedorTrabajo.estadoPFE.GetListaEstadoPFE();
                    }
                    else
                    {
                        artivm.Voluntario = _contenedorTrabajo.Voluntario.Get(id.GetValueOrDefault());
                        artivm.PFE = null;
                    }

                }

                return View(artivm);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.Edit()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VoluntarioVM artivm)
        {
            try
            {
                if (artivm.PFE.VoluntarioId == null)
                {
                    if ((artivm.PFE = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == artivm.Voluntario.Id)) != null)
                    {
                        artivm.PFE = _contenedorTrabajo.pFE.GetFirstOrDefault(filter: p => p.VoluntarioId == artivm.Voluntario.Id);
                        artivm.ListaEstadoPFE = _contenedorTrabajo.estadoPFE.GetListaEstadoPFE();
                    }
                    else
                    {
                        artivm.PFE = null;
                    }
                    _contenedorTrabajo.Voluntario.Update(artivm.Voluntario);
                    _contenedorTrabajo.save();
                    return RedirectToAction(nameof(Index));
                }
                if (ModelState.IsValid && artivm.PFE.VoluntarioId != null)
                {
                    PFE pfe = new PFE();
                    _contenedorTrabajo.Voluntario.Update(artivm.Voluntario);
                    _contenedorTrabajo.save();

                    pfe.VoluntarioId = artivm.Voluntario.Id;
                    artivm.PFE.VoluntarioId = pfe.VoluntarioId;
                    _contenedorTrabajo.pFE.Update(artivm.PFE);
                    _contenedorTrabajo.save();

                    Voluntario voluntario = new Voluntario();
                    voluntario = _contenedorTrabajo.Voluntario.Get(artivm.Voluntario.Id);


                    if (artivm.PFE.EstadoPFEId == 2)
                    {

                        //Envio de correo con los detalles del PFE:
                        string cuerpoCorreo = "<p>Cordial saludo.</p<br><br>" +
                            "<p>De acuerdo al plan familiar de emergencias entregado, el gestor a realizado las siguientes observaciones:</p>" +
                            "<p>" + artivm.PFE.Detalle + "</p>" +
                            "<p>Favor realizar las correcciones correspondientes y subir de nuevo el documento a la pagina www.funsar.org.co desde el apartado \"Consultar Proceso\"</p><br>" +
                            "<p><b>Cordialmente.</b></p>" +
                            "<p>Fundacion de busqueda y rescate FUNSAR</p>" +
                            "<p>Puede hacer seguimiento de su proceso en www.funsar.org.co</p>";

                        Correo correo = new Correo();
                        await correo.EnvioGmail("Servicio Social FUNSAR", artivm.Voluntario.correo, "Plan Familiar de Emergencias", cuerpoCorreo);
                    }

                    if (artivm.PFE.EstadoPFEId == 1)
                    {
                        artivm.Voluntario.EstadoId = 4;
                        _contenedorTrabajo.Voluntario.Update(artivm.Voluntario);
                        _contenedorTrabajo.save();
                        //Envio de correo con los detalles del PFE:
                        string cuerpoCorreo = "<p>Cordial saludo.</p<br><br>" +
                            "<p>De acuerdo al plan familiar de emergencias entregado, el gestor a realizado las siguientes observaciones:</p>" +
                            "<p>" + artivm.PFE.Detalle + "</p>" +
                            "<p>En caso de requerir correcciones volver a subir el documento a la pagina www.funsar.org.co desde el apartado \"Consultar Proceso\"</p><br>" +
                            "<p><b>Cordialmente.</b></p>" +
                            "<p>Fundacion de busqueda y rescate FUNSAR</p>" +
                            "<p>Puede hacer seguimiento de su proceso en www.funsar.org.co</p>";

                        Correo correo = new Correo();
                        await correo.EnvioGmail("Servicio Social FUNSAR", artivm.Voluntario.correo, "Plan Familiar de Emergencias", cuerpoCorreo);
                    }

                    if (voluntario.EstadoId == 6)
                    {
                        Certificado certvm = new Certificado();
                        certvm.FechaExpedicion = DateTime.Now.ToString("yyyy/MM/dd");
                        certvm.EstadoId = 5;
                        certvm.codCertificado = Guid.NewGuid().ToString();
                        certvm.Nombre = artivm.Voluntario.Nombre;
                        certvm.Apellido = artivm.Voluntario.Apellido;
                        certvm.DocumentoId = artivm.Voluntario.DocumentoId;
                        var brigada = _contenedorTrabajo.Colegio.GetFirstOrDefault(filter: b => b.Id == artivm.Voluntario.ColegioId);
                        certvm.Documento = artivm.Voluntario.Documento;
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
                        certvm.ProcesoId = artivm.Voluntario.ProcesoId;
                        _contenedorTrabajo.Certificado.Add(certvm);
                        _contenedorTrabajo.save();
                    }

                    return RedirectToAction(nameof(Index));
                }

                artivm.Voluntario = new FUNSAR.Models.Voluntario();
                artivm.ListaColegio = _contenedorTrabajo.Colegio.GetListaColegios();
                artivm.ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetListaDocumento();
                artivm.ListaEstado = _contenedorTrabajo.estadoPersona.GetListaEstado();

                return RedirectToAction(nameof(Edit));
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.Edit(model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Acudiente(int? id)
        {
            try
            {
                VoluntarioVM acudiente = new VoluntarioVM()
                {
                    ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetListaDocumento(),
                };

                if (id != null)
                {
                    acudiente.Acudiente = _contenedorTrabajo.Acudiente.Get(id.GetValueOrDefault());
                    acudiente.ListaTipoDocumento = _contenedorTrabajo.TDocumento.GetTipoDocumento(acudiente.Acudiente.DocumentoId);
                }

                return View(acudiente);

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.Acudiente()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

            
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Json(new { data = _contenedorTrabajo.Voluntario.GetALL(includeProperties: "Colegio,TDocumento,proceso,estadoPersona") });
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.GetAll()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllxGestor()
        {
            try
            {
                OperacionCertificado data = new OperacionCertificado();
                var builder = WebApplication.CreateBuilder();
                var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
                string conection = connectionString;
                string user = User.Identity.Name;
                int brigada = data.BrigadaxGestor(user, conection);
                List<int> estado = new List<int> {1,2,3};
                ViewData["NomBrigada"] = data.BrigadaxGestor(brigada, conection);
                //Opcion1
                return Json(new { data = _contenedorTrabajo.Voluntario.GetALL(filter: i => i.Colegio.BrigadaId == brigada && estado.Contains(i.estadoPersona.Id) && i.FechaRegistro.Value.Year == DateTime.Now.Year, includeProperties: "Colegio,TDocumento,proceso,estadoPersona") });

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.GetAllxGestor()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> GetAllxDivision()
        {
            try
            {
                List<int> division = new List<int>();
                Params parametro = new Params();
                int idParametro = 0;
                string[] brigadas = null;
                string user = User.Identity.Name;
                switch (user.Substring(12,6))
                {
                    case "zonal1":
                        idParametro = 7;
                        break;
                    case "zonal2":
                        idParametro = 8;
                        break;
                }
                parametro = _contenedorTrabajo.Params.GetFirstOrDefault(b => b.id == idParametro);
                brigadas = parametro.Valor.Split(',');
                foreach (var item in brigadas)
                {
                    division.Add(Convert.ToInt32(item));
                }
                return Json(new { data = _contenedorTrabajo.Voluntario.GetALL(filter: i => division.Contains(i.Colegio.Brigada.Id) && i.FechaRegistro.Value.Year == DateTime.Now.Year, includeProperties: "Colegio,TDocumento,proceso,estadoPersona") });

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.GetAllxGestor()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var objFromDb = _contenedorTrabajo.Voluntario.Get(id);
                var objAcudiente = _contenedorTrabajo.Acudiente.GetFirstOrDefault(filter: a => a.VoluntarioId == id);
                var objAsistencia = _contenedorTrabajo.Asistencia.GetFirstOrDefault(filter: a => a.VoluntarioId == id);
                if (objFromDb == null)
                {
                    return Json(new { success = false, message = "Error Borrando el voluntario" });
                }
                if (objAcudiente != null)
                {
                    _contenedorTrabajo.Acudiente.Remove(objAcudiente);
                    _contenedorTrabajo.save();
                }
                if (objAsistencia != null)
                {
                    _contenedorTrabajo.Asistencia.Remove(objAsistencia);
                    _contenedorTrabajo.save();
                }
                _contenedorTrabajo.Voluntario.Remove(objFromDb);
                _contenedorTrabajo.save();
                return Json(new { success = true, message = "Voluntario borrado correctamente" });
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "VoluntariosController.Delete()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }
    }
}
