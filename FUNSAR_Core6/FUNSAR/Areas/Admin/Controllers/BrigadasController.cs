using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo")]
    [Area("Admin")]
    public class BrigadasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public BrigadasController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brigada brigada)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _contenedorTrabajo.Brigada.Add(brigada);
                    _contenedorTrabajo.save();
                    return RedirectToAction(nameof(Index));
                }

                return View(brigada);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "BrigadasController.Create()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Brigada brigada = new Brigada();
                brigada = _contenedorTrabajo.Brigada.Get(id);
                if (brigada == null)
                {
                    return NotFound();
                }

                return View(brigada);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "BrigadasController.Create()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Brigada brigada)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Brigada.Update(brigada);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(brigada);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            //Opcion1
            return Json(new { data = _contenedorTrabajo.Brigada.GetALL()});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Brigada.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando la Brigada" });
            }
            _contenedorTrabajo.Brigada.Remove(objFromDb);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Brigada borrada correctamente" });
        }

        #endregion
    }
}
