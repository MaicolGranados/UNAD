using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.HerramientasComunes;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Net;
using System.Threading;
using FUNSAR.Areas.Identity.Pages.Account;
using System.Text.RegularExpressions;
using MercadoPago.Resource.User;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrativo,Gestor")]
    [Area("Admin")]
    public class ColegiosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public ColegiosController(
            IContenedorTrabajo contenedorTrabajo,
            IWebHostEnvironment hostingEnviroment,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _roleManager = roleManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ColegioVM colevm = new ColegioVM()
            {
                Colegio = new FUNSAR.Models.Colegio(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio()
            };
            int id = Convert.ToInt32(HttpContext.Request.Cookies["Cargo"]);
            if (id > 0)
            {
                colevm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(id);
            }

            return View(colevm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ColegioVM coleVm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (validEmail(coleVm.DatoColegio.Correo))
                    {
                        //artivm.Certificado.codCertificado = artivm.ListaBrigadas .ToString().Substring(0,3)+Convert.ToString(artivm.Certificado.Documento).Substring((artivm.Certificado.Documento.Length)-4,4);
                        var validate = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(filter: d => d.Correo == coleVm.DatoColegio.Correo);

                        if (validate == null)
                        {
                            if (await CreateUser(coleVm))
                            {
                                _contenedorTrabajo.Colegio.Add(coleVm.Colegio);
                                _contenedorTrabajo.save();
                                coleVm.DatoColegio.ColegioId = coleVm.Colegio.Id;
                                _contenedorTrabajo.DatoColegio.Add(coleVm.DatoColegio);
                                _contenedorTrabajo.save();
                                return RedirectToAction(nameof(Index));
                            }
                            coleVm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas();
                            coleVm.ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio();
                            return View(coleVm);
                        }
                        coleVm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas();
                        coleVm.ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio();
                        return View(coleVm);
                    }
                    else
                    {
                        coleVm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas();
                        coleVm.ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio();
                        return View(coleVm);
                    }
                    
                }
                coleVm.ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio();
                coleVm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas();
                return View(coleVm);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                coleVm.ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas();
                coleVm.ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio();
                return View(coleVm);
            }
            
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ColegioVM colevm = new ColegioVM()
            {
                Colegio = new FUNSAR.Models.Colegio(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio()
            };

            if (id != null)
            {
                colevm.Colegio = _contenedorTrabajo.Colegio.Get(id.GetValueOrDefault());
                colevm.DatoColegio = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(filter: d => d.ColegioId == id);
            };

            return View(colevm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ColegioVM coleVM)
        {
            ColegioVM colevm = new ColegioVM()
            {
                Colegio = new FUNSAR.Models.Colegio(),
                ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(),
                ListaJornada = _contenedorTrabajo.JornadaColegio.GetListaJornadaColegio()
            };

            try
            {
                

                if (ModelState.IsValid)
                {
                    var user = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(filter: d => d.Colegio.Id == coleVM.Colegio.Id);
                    if (validEmail(coleVM.DatoColegio.Correo))
                    {

                        if (user == null)
                        {
                            if (await CreateUser(coleVM))
                            {
                                _contenedorTrabajo.Colegio.Update(coleVM.Colegio);
                                _contenedorTrabajo.save();
                                coleVM.DatoColegio.ColegioId = coleVM.Colegio.Id;
                                _contenedorTrabajo.DatoColegio.Add(coleVM.DatoColegio);
                                _contenedorTrabajo.save();
                                return RedirectToAction(nameof(Index));
                            }

                        }
                        else if (user.Correo != coleVM.DatoColegio.Correo)
                        {
                            if (await CreateUser(coleVM))
                            {
                                _contenedorTrabajo.Colegio.Update(coleVM.Colegio);
                                _contenedorTrabajo.save();
                                coleVM.DatoColegio.Id = user.Id;
                                coleVM.DatoColegio.ColegioId = coleVM.Colegio.Id;
                                _contenedorTrabajo.DatoColegio.Update(coleVM.DatoColegio);
                                _contenedorTrabajo.save();
                                return RedirectToAction(nameof(Index));
                            }
                        }
                        else
                        {
                            _contenedorTrabajo.Colegio.Update(coleVM.Colegio);
                            _contenedorTrabajo.save();
                            coleVM.DatoColegio.Id = user.Id;
                            coleVM.DatoColegio.ColegioId = coleVM.Colegio.Id;
                            _contenedorTrabajo.DatoColegio.Update(coleVM.DatoColegio);
                            _contenedorTrabajo.save();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        coleVM = colevm;

                        return View(coleVM);
                    }


                }

                coleVM = colevm;

                return View(coleVM);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ColegiosController.Edit");
                coleVM = colevm;

                return View(coleVM);

            }


        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Colegio.GetALL(includeProperties: "Brigada") });
        }

        [HttpGet]
        public IActionResult GetAllxGestor()
        {
            
            OperacionCertificado data = new OperacionCertificado();
            var builder = WebApplication.CreateBuilder();
            var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
            string conection = connectionString;
            string user = User.Identity.Name;
            int brigada = data.BrigadaxGestor(user, conection);
            ViewData["NomBrigada"] = data.BrigadaxGestor(brigada, conection);
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(5));
            HttpContext.Response.Cookies.Append("Cargo", brigada.ToString(), cookieOptions);
            //Opcion1
            return Json(new { data = _contenedorTrabajo.Colegio.GetALL(filter: i => i.BrigadaId == brigada, includeProperties: "Brigada") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Colegio.Get(id);
            var datoColegio = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(filter: c => c.ColegioId == id);
            if (objFromDb == null || datoColegio == null)
            {
                return Json(new { success = false, message = "Error Borrando el Colegio" });
            }
            _contenedorTrabajo.Colegio.Remove(objFromDb);
            _contenedorTrabajo.save();
            _contenedorTrabajo.DatoColegio.Remove(datoColegio);
            _contenedorTrabajo.save();
            return Json(new { success = true, message = "Colegio borrada correctamente" });
        }

        [HttpPost]
        public async Task<bool> CreateUser(ColegioVM input)
        {
            try
            {
                if (input != null)
                {
                    if (input.Colegio != null && input.DatoColegio != null)
                    {
                        var validador = _contenedorTrabajo.DatoColegio.GetFirstOrDefault(v => v.Correo == input.DatoColegio.Correo);

                        var user = UserInstance();
                        user.Nombre = input.DatoColegio.NombreEncargado;
                        user.Documento = input.DatoColegio.NumeroContacto;
                        user.RangoId = 8;
                        user.BrigadaId = input.Colegio.BrigadaId;

                        if (validador != null)
                        {
                            await _userManager.DeleteAsync(user);
                        }

                        string password = setPassword(input.DatoColegio.NumeroContacto);


                        await _userStore.SetUserNameAsync(user, input.DatoColegio.Correo.ToLower(),CancellationToken.None);
                        await _emailStore.SetEmailAsync(user, input.DatoColegio.Correo.ToLower(), CancellationToken.None);
                        var result = await _userManager.CreateAsync(user, password);

                        if (result.Succeeded)
                        {
                            if (!await _roleManager.RoleExistsAsync(CNT.Colegio))
                            {
                                await _roleManager.CreateAsync(new IdentityRole(CNT.Colegio));
                            }

                            await _userManager.AddToRoleAsync(user, CNT.Colegio);

                        }

                        string cuerpoCorreo = "<p>Se creo correctamente el usuario para el ingreso a https://www.funsar.org.co, valida en ingreso con las siguientes credenciales:</p>"
                            + "<p><strong>Usuario: </strong>" + input.DatoColegio.Correo + " </p>"
                            + "<p><strong>Clave: </strong>" + password + " </p>"
                            + "<p>Con el podra conocer el estado del proceso de los estudiantes, en el apartado Administracion</p>"
                            + "<p>Cualquier novedad con el ingreso informar al correo: soporte@funsar.org.co</p>";

                        Correo correo = new Correo();
                        await correo.EnvioGmail("Fundación FUNSAR", input.DatoColegio.Correo, "Creacion de Usuario Funsar.org.co", cuerpoCorreo);
                        _logger.LogInformation("Usuario " + input.DatoColegio.Correo + " creado correctamente.");

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                string err = ex.Message;
                return false;
            }
            
            
        }
        
        private ApplicationUser UserInstance()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private string setPassword(string password)
        {
            string inverted = new string(password.Reverse().ToArray());

            string first = inverted.Substring(0, 4);

            int length = inverted.Length - 4;

            string second = length > 0 ? inverted.Substring(4, length) : "";

            password = "Ingress" + first + second + "*";

            return password;
        }

        private bool validEmail(string mail)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(mail, emailPattern);
        }

        #endregion
    }
}
