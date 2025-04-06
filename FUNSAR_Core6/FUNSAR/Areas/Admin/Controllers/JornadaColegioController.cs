using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo")]
    [Area("Admin")]
    public class JornadaColegioController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public JornadaColegioController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
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
        public async Task<IActionResult> Create(JornadaColegio jornadaColegio)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _contenedorTrabajo.JornadaColegio.Add(jornadaColegio);
                    _contenedorTrabajo.save();
                    return RedirectToAction(nameof(Index));
                }

                return View(jornadaColegio);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "JornadaColegioController.Create()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                JornadaColegio jornada = new JornadaColegio();
                jornada = _contenedorTrabajo.JornadaColegio.Get(id);
                if (jornada == null)
                {
                    return NotFound();
                }

                return View(jornada);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "JornadaColegioController.Create()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(JornadaColegio jornada)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.JornadaColegio.Update(jornada);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(jornada);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            //Opcion1
            return Json(new { data = _contenedorTrabajo.JornadaColegio.GetALL()});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.JornadaColegio.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando la Jornada" });
            }
            _contenedorTrabajo.JornadaColegio.Remove(objFromDb);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Jornada borrada correctamente" });
        }

        #endregion
    }
}
