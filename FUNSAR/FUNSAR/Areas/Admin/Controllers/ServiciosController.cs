using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo")]
    [Area("Admin")]
    public class ServiciosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        //private readonly ApplicationDbContext _context;

        public ServiciosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
            //_context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[AllowAnonymous] -> permitir acceso desde fuera
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Servicio.Add(servicio);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(servicio);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Servicio servicio = new Servicio();
            servicio = _contenedorTrabajo.Servicio.Get(id);
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.Servicio.Update(servicio);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(servicio);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            //Opcion1
            return Json(new {data = _contenedorTrabajo.Servicio.GetALL()});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Servicio.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Servicio" });
            }
            _contenedorTrabajo.Servicio.Remove(objFromDb);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Servicio borrado correctamente" });
        }
        #endregion
    }
}
