using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace FUNSAR.Areas.Admin.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    [Area("Admin")]
    public class UsuariosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public UsuariosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //Obtener todos los usuarios
            //return View(_contenedorTrabajo.Usuario.GetALL());
            //Obtener todos los usuarios sin el propio
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_contenedorTrabajo.Usuario.GetALL(u => u.Id != usuarioActual.Value));
        }

        [HttpGet]
        public IActionResult Bloquear(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.BloquearUsuario(Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Desbloquear(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.Desbloquearusuario(Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Eliminar(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            _contenedorTrabajo.Usuario.Desbloquearusuario(Id);
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public IActionResult Editar(string Id)
        //{
        //    if (Id == null)
        //    {
        //        return NotFound();
        //    }
        //    var users = new Identity.Pages.Account.RegisterModel();
        //    {
        //        new ApplicationUser()
        //    };

        //    foreach (var user in users)
        //    {
        //        user.Id = Id;
        //    }

        //    return View(users);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(IEnumerable<ApplicationUser> user)
        {
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (var item in user)
                    {
                        _contenedorTrabajo.Usuario.Editarusuario(item);
                        _contenedorTrabajo.save();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return View();
                }
                
            }
            return View();
        }
    }
}
