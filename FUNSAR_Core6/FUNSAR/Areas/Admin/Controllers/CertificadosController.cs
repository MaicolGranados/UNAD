using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using HerramientasComunes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.OpenXmlFormats.Vml.Office;
using System.Data;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo")]
    [Area("Admin")]
    public class CertificadosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public CertificadosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Masiva()
        {
            CertificadoVM artivm = new CertificadoVM()
            {
                CertificadoTemp = new FUNSAR.Models.CertificadoTemp(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                semestre = _contenedorTrabajo.Semestre.GetListaSemestre(),
                tipoDoc = _contenedorTrabajo.TDocumento.GetListaDocumento()
            };
            return View(artivm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            UTLCert utilidades = new UTLCert();
            CertificadoVM artivm = new CertificadoVM()
            {
                Certificado = new FUNSAR.Models.Certificado(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                semestre = _contenedorTrabajo.Semestre.GetListaSemestre(),
                tipoDoc = _contenedorTrabajo.TDocumento.GetListaDocumento(),
                ListaProceso = _contenedorTrabajo.Proceso.GetListaProceso()
            };
            return View(artivm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CertificadoVM artivm)
        {
            if (ModelState.IsValid)
            {
                //artivm.Certificado.codCertificado = artivm.ListaBrigadas .ToString().Substring(0,3)+Convert.ToString(artivm.Certificado.Documento).Substring((artivm.Certificado.Documento.Length)-4,4);
                artivm.Certificado.FechaExpedicion = DateTime.Now.ToString("yyyy/MM/dd");
                artivm.Certificado.EstadoId = 5;
                artivm.Certificado.codCertificado = Guid.NewGuid().ToString();
                _contenedorTrabajo.Certificado.Add(artivm.Certificado);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }
            artivm.semestre = _contenedorTrabajo.Semestre.GetListaSemestre();
            artivm.tipoDoc = _contenedorTrabajo.TDocumento.GetListaDocumento();
            artivm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas();
            return View(artivm);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            CertificadoVM artivm = new CertificadoVM()
            {
                Certificado = new FUNSAR.Models.Certificado(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                tipoDoc = _contenedorTrabajo.TDocumento.GetListaDocumento(),
                semestre = _contenedorTrabajo.Semestre.GetListaSemestre(),
                ListaProceso = _contenedorTrabajo.Proceso.GetListaProceso()
            };

            if (id != null)
            {
                artivm.Certificado = _contenedorTrabajo.Certificado.Get(id.GetValueOrDefault());
            }

            return View(artivm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CertificadoVM artivm)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Certificado.Update(artivm.Certificado);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }
            return View(artivm);
        }


        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            //Opcion1
            return Json(new { data = _contenedorTrabajo.Certificado.GetALL(includeProperties: "Brigada,proceso,Semestre,TDocumento") });
        }

        [HttpGet]
        public IActionResult GetAllTemp()
        {
            //Opcion1
            return Json(new { data = _contenedorTrabajo.CertificadoTemp.GetALL(includeProperties: "Brigada,proceso,Semestre,TDocumento") });
        }

        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Certificado.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando el certificado" });
            }
            _contenedorTrabajo.Certificado.Remove(objFromDb);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Certificado borrado correctamente" });
        }
        [HttpPost("Upload")]
        public IActionResult Upload(IFormFile file)
        {
            var builder = WebApplication.CreateBuilder();
            var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
            string conection = connectionString;
            CertificadoVM artivm = new CertificadoVM()
            {
                CertificadoTemp = new FUNSAR.Models.CertificadoTemp(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                semestre = _contenedorTrabajo.Semestre.GetListaSemestre(),
                tipoDoc = _contenedorTrabajo.TDocumento.GetListaDocumento(),
                ListaProceso = _contenedorTrabajo.Proceso.GetListaProceso()
            };
            try
            {
                if (file.Length == 0)
                {
                    return BadRequest();
                }
                var path = Path.Combine(_hostingEnviroment.ContentRootPath, "Files");
                if (!Directory.Exists(path))
                {
                        Directory.CreateDirectory(path);
                }
                string fullPath = Path.Combine(path, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                Excel CargaExcel = new Excel(_contenedorTrabajo);
                CargaExcel.ProcesaExcel(fullPath, artivm, conection);
                return RedirectToAction(nameof(Masiva));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("CargaSQL")]
        public IActionResult CargaSQL()
        {
            int cant = 0;
            var builder = WebApplication.CreateBuilder();
            var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
            string conection = connectionString;
            try
            {
                OperacionCertificado carga = new OperacionCertificado();
                cant = carga.UpdateMasivo(conection);
                return RedirectToAction(nameof(Masiva));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #endregion
    }
}
