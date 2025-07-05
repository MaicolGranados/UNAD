// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using FUNSAR.Models;
using FUNSAR.Utilidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HerramientasComunes;
using FUNSAR.HerramientasComunes;
using System.ComponentModel.DataAnnotations.Schema;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.AccesoDatos.Data.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FUNSAR.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "SuperAdmin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender, 
            IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
            ListaBrigada = _contenedorTrabajo.Brigada.GetListaBrigadas();
            ListaRango = _contenedorTrabajo.Rango.GetListaRango();
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            //[Required]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            //[DataType(DataType.Password)]
            //[Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            //[DataType(DataType.Password)]
            //[Display(Name = "Confirm password")]
            //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "El nombre es obligatorio")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El documento es obligatorio")]
            public string Documento { get; set; }
            public int RangoId { get; set; }
            [ForeignKey("RangoId")]
            public Rango Rango { get; set; }

            public int BrigadaId { get; set; }
            [ForeignKey("BrigadaId")]
            public Brigada Brigada { get; set; }
        }

        public ApplicationUser applicationUser { get; set; }
        public IEnumerable<SelectListItem>? ListaRango { get; set; }
        public IEnumerable<SelectListItem>? ListaBrigada { get; set; }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            
            returnUrl ??= Url.Content("~/Admin/Usuarios");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (Input.Email.Contains("@funsar.org.co"))
            {
                if (ModelState.IsValid)
                {
                    Input.ConfirmPassword = "Funsar" + Input.Documento.ToString() + "*";
                    Input.Password = "Funsar" + Input.Documento.ToString() + "*";
                    var user = CreateUser();

                    user.Nombre = Input.Nombre;
                    user.Documento = Input.Documento;
                    user.RangoId = Input.RangoId;
                    user.BrigadaId = Input.BrigadaId;

                    //await _userStore.SetUserNameAsync(user, Input.Nombre.Substring(0,position), CancellationToken.None);
                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        //validar rol existente o no
                        if (!await _roleManager.RoleExistsAsync(CNT.SuperAdmin) || !await _roleManager.RoleExistsAsync(CNT.Administrativo) || !await _roleManager.RoleExistsAsync(CNT.Plataforma) || !await _roleManager.RoleExistsAsync(CNT.Gestor))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(CNT.SuperAdmin));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Administrativo));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Gestor));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Plataforma));
                        }

                        //Obtener el rol

                        string rol = Request.Form["radUsuarioRole"].ToString();

                        switch (rol)
                        {
                            case CNT.SuperAdmin:
                                await _userManager.AddToRoleAsync(user, CNT.SuperAdmin);
                                break;
                            case CNT.Administrativo:
                                await _userManager.AddToRoleAsync(user, CNT.Administrativo);
                                break;
                            case CNT.Plataforma:
                                await _userManager.AddToRoleAsync(user, CNT.Plataforma);
                                break;
                            case CNT.Gestor:
                                await _userManager.AddToRoleAsync(user, CNT.Gestor);
                                break;
                            case CNT.Division:
                                await _userManager.AddToRoleAsync(user, CNT.Division);
                                break;
                            default:
                                break;
                        }


                        string cuerpoCorreo = "<p>Se creo correctamente el usuario para el ingreso a https://www.funsar.org.co, valida en ingreso con las siguientes credenciales:</p>"
                            + "<p><strong>Usuario: </strong>" + Input.Email + " </p>"
                            + "<p><strong>Clave: </strong>" + Input.Password + " </p>"
                            + "<p>Cualquier novedad con el ingreso informar al correo: representacion.institucional@funsar.org.co</p>";

                        Correo correo = new Correo();
                        await correo.EnvioGmail("Fundación FUNSAR", Input.Email, "Creacion de Usuario Funsar.org.co", cuerpoCorreo);
                        _logger.LogInformation("Usuario " + Input.Email + " creado correctamente.");

                        return Redirect("~/Admin/Usuarios");

                        #region
                        //var userId = await _userManager.GetUserIdAsync(user);
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        //    protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        //{
                        //    //return LocalRedirect(returnUrl);

                        //    return RedirectToPage("~/Admin/Usuarios");
                        //}
                        //else
                        //{
                        //    await _signInManager.SignInAsync(user, isPersistent: false);
                        //    return LocalRedirect(returnUrl);
                        //}
                        #endregion
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ViewData["Error"] = "El correo debe pertenecer a la organizacion";
            }
            
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
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
    }
}
