using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Net;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo,Gestor,Division,Colegio")]
    [Area("Admin")]
    public class AsistenciasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public AsistenciasController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Masiv()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Daily()
        {
            AsistenciaVM asistencia = new AsistenciaVM()
            {
                EstadoAsistencia = new EstadoAsistencia(),
                Asistencia = new Asistencia(),
                ListaEstadoAsistencia = _contenedorTrabajo.EstadoAsistencia.GetListaEstadoAsistencia()
            };
            
            DateTime date = Convert.ToDateTime(HttpContext.Request.Cookies["dateAsistencia"]);
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            DateTime currentdate = new DateTime(year, month,day);
            asistencia.Asistencia.Fecha = currentdate;
            return View(asistencia);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Voluntario voluntario = new Voluntario();
            voluntario = _contenedorTrabajo.Voluntario.Get(id);
            return View(voluntario);
        }

        #region Llamadas a la API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (User.IsInRole(CNT.Gestor))
                {
                    int year = DateTime.Now.Year;
                    int month = DateTime.Now.Month;
                    DateTime ini = new DateTime();
                    DateTime end = new DateTime();
                    if (month >= 1 && month <= 6)
                    {
                        ini = new DateTime(year, 1, 1);
                        end = new DateTime(year, 6, 30);
                    }
                    else
                    {
                        ini = new DateTime(year, 7, 1);
                        end = new DateTime(year, 12, 31);
                    }

                    var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                    var idBrigada = _contenedorTrabajo.Usuario.GetFirstOrDefault(filter: u => u.Email == userEmail);

                    var asistencias = _contenedorTrabajo.Asistencia.GetALL(filter: a => (a.Voluntario.FechaRegistro.Value >= ini && a.Voluntario.FechaRegistro.Value <= end) && a.EstadoAsistenciaId != 3 && a.Voluntario.Colegio.BrigadaId == idBrigada.BrigadaId, includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio,Voluntario.Colegio.Brigada");

                    var resultado = asistencias.GroupBy(a => a.VoluntarioId)
                                               .Select(g => new
                                               {
                                                   Id = g.Key,
                                                   Nombre = g.First().Voluntario.Nombre,
                                                   Apellido = g.First().Voluntario.Apellido,
                                                   TipoDocumento = g.First().Voluntario.TDocumento,
                                                   Documento = g.First().Voluntario.Documento,
                                                   Colegio = g.First().Voluntario.Colegio.Nombre,
                                                   Asistencias = g.Count()
                                               }).ToList();

                    return Json(new { data = resultado });
                } else if (User.IsInRole(CNT.Colegio))
                {
                    var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                    var idColegio = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(filter: u => u.Correo == userEmail);

                    var asistencias = _contenedorTrabajo.Asistencia.GetALL(filter: a => a.EstadoAsistenciaId != 3 && a.Voluntario.Colegio.Id == idColegio.ColegioId && a.Voluntario.FechaRegistro.Value.Year == DateTime.Now.Year, includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio,Voluntario.Colegio.Brigada");

                    var resultado = asistencias.GroupBy(a => a.VoluntarioId)
                                               .Select(g => new
                                               {
                                                   Id = g.Key,
                                                   Nombre = g.First().Voluntario.Nombre,
                                                   Apellido = g.First().Voluntario.Apellido,
                                                   TipoDocumento = g.First().Voluntario.TDocumento,
                                                   Documento = g.First().Voluntario.Documento,
                                                   Colegio = g.First().Voluntario.Colegio.Nombre,
                                                   Asistencias = g.Count()
                                               }).ToList();

                    return Json(new { data = resultado });
                }
                else if (User.IsInRole(CNT.Division))
                {
                    List<int> division = new List<int>();
                    Params parametro = new Params();
                    int idParametro = 0;
                    string[] brigadas = null;
                    string user = User.Identity.Name;
                    switch (user.Substring(12, 6))
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

                    var asistencias = _contenedorTrabajo.Asistencia.GetALL(filter: a => a.Voluntario.FechaRegistro.Value.Year == DateTime.Now.Year && a.EstadoAsistenciaId != 3 && division.Contains(a.Voluntario.Colegio.BrigadaId), includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio,Voluntario.Colegio.Brigada");

                    var resultado = asistencias.GroupBy(a => a.VoluntarioId)
                                               .Select(g => new
                                               {
                                                   Id = g.Key,
                                                   Nombre = g.First().Voluntario.Nombre,
                                                   Apellido = g.First().Voluntario.Apellido,
                                                   TipoDocumento = g.First().Voluntario.TDocumento,
                                                   Documento = g.First().Voluntario.Documento,
                                                   Colegio = g.First().Voluntario.Colegio.Nombre,
                                                   Asistencias = g.Count()
                                               }).ToList();

                    return Json(new { data = resultado });
                }
                else
                {
                    var asistencias = _contenedorTrabajo.Asistencia.GetALL(includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio");

                    var resultado = asistencias.Where(a => a.EstadoAsistenciaId != 3)
                                               .GroupBy(a => a.VoluntarioId)
                                               .Select(g => new
                                               {
                                                   Id = g.Key,
                                                   Nombre = g.First().Voluntario.Nombre,
                                                   Apellido = g.First().Voluntario.Apellido,
                                                   TipoDocumento = g.First().Voluntario.TDocumento,
                                                   Documento = g.First().Voluntario.Documento,
                                                   Colegio = g.First().Voluntario.Colegio.Nombre,
                                                   Asistencias = g.Count()
                                               }).ToList();

                    return Json(new { data = resultado });
                }
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "AsistenciasController.SaveTableData");
                return Json(null); ;
            }
            
        }

        [HttpGet]
        public IActionResult GetEdit(int id)
        {
            var asistencias = _contenedorTrabajo.Asistencia.GetALL(filter: a => a.VoluntarioId == id,includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio");

            var resultado = asistencias
                                .Select(a => new
                                {
                                    a.Id,
                                    a.Fecha,
                                    a.EstadoAsistenciaId
                                })
                                .ToList();

            return Json(new { data = resultado });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDaily()
        {
            try
            {
                if (User.IsInRole(CNT.Gestor))
                {
                    var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                    var idBrigada = _contenedorTrabajo.Usuario.GetFirstOrDefault(filter: u => u.Email == userEmail);

                    var asistencias = _contenedorTrabajo.Asistencia.GetALL(filter: a => a.Voluntario.Colegio.BrigadaId == idBrigada.BrigadaId && a.Voluntario.FechaRegistro.Value.Year == DateTime.Now.Year,includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio,Voluntario.Colegio.Brigada");

                    var resultado = asistencias.GroupBy(a => a.VoluntarioId)
                                               .Select(g => new
                                               {
                                                   Id = g.Key,
                                                   Nombre = g.First().Voluntario.Nombre,
                                                   Apellido = g.First().Voluntario.Apellido,
                                                   TipoDocumento = g.First().Voluntario.TDocumento,
                                                   Documento = g.First().Voluntario.Documento
                                               }).ToList();

                    return Json(new { data = resultado });
                }
                else
                {
                    var asistencias = _contenedorTrabajo.Asistencia.GetALL(includeProperties: "Voluntario,Voluntario.TDocumento,Voluntario.Colegio,Voluntario.Colegio.Brigada");

                    var resultado = asistencias.GroupBy(a => a.VoluntarioId)
                                               .Select(g => new
                                               {
                                                   Id = g.Key,
                                                   Nombre = g.First().Voluntario.Nombre,
                                                   Apellido = g.First().Voluntario.Apellido,
                                                   TipoDocumento = g.First().Voluntario.TDocumento,
                                                   Documento = g.First().Voluntario.Documento
                                               }).ToList();

                    return Json(new { data = resultado });
                }
                
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "AsistenciasController.SaveTableData");
                return  Json(null); ;
            }
            
        }

        [HttpGet]
        public JsonResult GetDropdownData()
        {
            var dropdownData = _contenedorTrabajo.EstadoAsistencia.GetListaEstadoAsistencia()
                                .Select(i => i.Text)
                                .ToList(); 
            return Json(dropdownData);
        }

        [HttpPost]
        public async Task<JsonResult> SaveTableData([FromQuery] string dateIn,[FromBody] List<TableDataModel> tableData)
        {
            bool result;
            bool valida;
            int count = 0;
            try
            {
                if (dateIn == null || dateIn == string.Empty)
                {
                    result = false;
                    valida = false;
                    return Json(new { success = result, validate = valida });
                }

                foreach (var item in tableData)
                {
                    Asistencia asitencia = new Asistencia();
                    asitencia.VoluntarioId = item.ItemId;
                    var e = _contenedorTrabajo.EstadoAsistencia.GetFirstOrDefault(filter: a => a.estadoAsistencia == item.SelectedValue);

                    DateTime date = Convert.ToDateTime(dateIn);
                    int day = date.Day;
                    int month = date.Month;
                    int year = date.Year;

                    DateTime currentDate = new DateTime(year, month, day);
                    
                    var a = _contenedorTrabajo.Asistencia.GetALL(a => a.VoluntarioId == item.ItemId && a.Fecha == currentDate);
                    var b = _contenedorTrabajo.Asistencia.GetALL(a => a.VoluntarioId == item.ItemId);

                    var parametroDia = _contenedorTrabajo.Params.GetFirstOrDefault(filter: a => a.id == 1);
                    var parametroTotal = _contenedorTrabajo.Params.GetFirstOrDefault(filter: a => a.id == 4);

                    if (a.Count() > 0 || b.Count() > Convert.ToInt32(parametroTotal.Valor))
                    {
                        count = +1;
                    }
                    else
                    {
                        asitencia.EstadoAsistenciaId = e.Id;
                        asitencia.Fecha = currentDate;
                        _contenedorTrabajo.Asistencia.Add(asitencia);
                        _contenedorTrabajo.save();
                    }
                    
                }
                if (count > 0)
                {
                    valida = false;
                }
                else
                {
                    valida = true;
                }
                result = true;
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "AsistenciasController.SaveTableData");
                result = false;
                valida = false;
            }

            return Json(new { success = result , validate = valida });
        }

        [HttpPost]
        public async Task<JsonResult> EditTableData([FromBody] List<TableDataModel> tableData)
        {
            bool result;
            try
            {
                foreach (var item in tableData)
                {
                    Asistencia asitencia = new Asistencia();
                    var e = _contenedorTrabajo.EstadoAsistencia.GetFirstOrDefault(filter: a => a.estadoAsistencia == item.SelectedValue);
                    asitencia = _contenedorTrabajo.Asistencia.GetFirstOrDefault(filter: a => a.Id == item.ItemId);
                    asitencia.EstadoAsistenciaId = e.Id;
                    _contenedorTrabajo.Asistencia.Update(asitencia);
                    _contenedorTrabajo.save();
                }
                result = true;
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "AsistenciasController.EditTableData");
                result = false;
            }

            return Json(new { success = result });
        }

        public class TableDataModel
        {
            public int ItemId { get; set; }
            public string SelectedValue { get; set; }
        }

        #endregion
    }
}
