using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("Admin")]
    public class ParamsController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public ParamsController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
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
        public async Task<IActionResult> Create(Params parametros)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _contenedorTrabajo.Params.Add(parametros);
                    _contenedorTrabajo.save();
                    return RedirectToAction(nameof(Index));
                }

                return View(parametros);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ParamsController.Create()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Params parametros = new Params();
                parametros = _contenedorTrabajo.Params.Get(id);
                if (parametros == null)
                {
                    return NotFound();
                }

                return View(parametros);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ParamsController.Edit()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Params parametros)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Params.Update(parametros);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(parametros);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            //Opcion1
            return Json(new { data = _contenedorTrabajo.Params.GetALL() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Params.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando el parametro" });
            }
            _contenedorTrabajo.Params.Remove(objFromDb);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Parametro borradao correctamente" });
        }

        #endregion
    }
}
