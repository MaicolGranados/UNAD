using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo")]
    [Area("Admin")]
    public class VigenciaServiciosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        //private readonly ApplicationDbContext _context;

        public VigenciaServiciosController(IContenedorTrabajo contenedorTrabajo)
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
            VigenciaVM vigencia = new VigenciaVM { ListaServicio = _contenedorTrabajo.Servicio.GetListaServicio()};
            return View(vigencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VigenciaServicio vigenciaServicio)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.VigenciaServicio.Add(vigenciaServicio);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(vigenciaServicio);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            VigenciaVM servicio = new VigenciaVM();
            servicio.VigenciaServicio = _contenedorTrabajo.VigenciaServicio.Get(id);
            servicio.ListaServicio = _contenedorTrabajo.Servicio.GetListaServicio();
            if (servicio == null)
            {
                return NotFound();
            }

            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(VigenciaServicio vigenciaServicio)
        {
            if (ModelState.IsValid)
            {
                _contenedorTrabajo.VigenciaServicio.Update(vigenciaServicio);
                _contenedorTrabajo.save();
                return RedirectToAction(nameof(Index));
            }

            return View(vigenciaServicio);
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            //Opcion1
            return Json(new {data = _contenedorTrabajo.VigenciaServicio.GetALL(includeProperties: "Servicio")});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.VigenciaServicio.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error Borrando Servicio" });
            }
            _contenedorTrabajo.VigenciaServicio.Remove(objFromDb);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Servicio borrado correctamente" });
        }
        #endregion
    }
}
