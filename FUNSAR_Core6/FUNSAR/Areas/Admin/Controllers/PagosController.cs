using FUNSAR.AccesoDatos.Data.Repository;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo,Division,Gestor")]
    [Area("Admin")]
    public class PagosController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public PagosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region Llamadas a la API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Opcion1
            try
            {
                if (User.IsInRole(CNT.Gestor))
                {
                    var userMail = User.Claims.FirstOrDefault(e => e.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                    var brigada = _contenedorTrabajo.Usuario.GetFirstOrDefault(b => b.Email == userMail);

                    var dataCertificado = _contenedorTrabajo.Certificado.GetALL().Where(v => v.BrigadaId == brigada.BrigadaId);
                    var dataVoluntario = _contenedorTrabajo.Voluntario.GetALL(includeProperties: "Colegio").Where(v => v.Colegio.BrigadaId == brigada.BrigadaId);

                    List<string> dataList = new List<string>();

                    if (dataCertificado is not null)
                    {
                        foreach (var item in dataCertificado)
                        {
                            if (item.Documento is not null)
                            {
                                dataList.Add(item.Documento);
                            }
                        }
                    }

                    if (dataVoluntario is not null)
                    {
                        foreach (var item in dataVoluntario)
                        {
                            if (item.Documento is not null)
                            {
                                dataList.Add(item.Documento);
                            }
                        }
                    }

                    return Json(new { 
                        data = _contenedorTrabajo.Pagos.GetALL().Where(p => dataList.Contains(p.DocumentoP))
                    });
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

                    var dataCertificado = _contenedorTrabajo.Certificado.GetALL().Where(v => division.Contains(v.BrigadaId));
                    var dataVoluntario = _contenedorTrabajo.Voluntario.GetALL(includeProperties: "Colegio").Where(v => division.Contains(v.Colegio.BrigadaId));

                    List<string> dataList = new List<string>();

                    if (dataCertificado is not null)
                    {
                        foreach (var item in dataCertificado)
                        {
                            if (item.Documento is not null)
                            {
                                dataList.Add(item.Documento);
                            }
                        }
                    }

                    if (dataVoluntario is not null)
                    {
                        foreach (var item in dataVoluntario)
                        {
                            if (item.Documento is not null)
                            {
                                dataList.Add(item.Documento);
                            }
                        }
                    }

                    return Json(new
                    {
                        data = _contenedorTrabajo.Pagos.GetALL().Where(p => dataList.Contains(p.DocumentoP))
                    });
                }
                else
                {
                    return Json(new { data = _contenedorTrabajo.Pagos.GetALL() });
                }
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "pagosController.GetAll()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
                
        }

        #endregion
    }
}
