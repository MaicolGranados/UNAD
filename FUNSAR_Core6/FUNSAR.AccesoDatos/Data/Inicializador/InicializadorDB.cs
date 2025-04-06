using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FUNSAR.Models;
using Microsoft.EntityFrameworkCore;
using FUNSAR.Utilidades;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using System.Drawing.Text;
using FUNSAR.Models.ViewModels;

namespace FUNSAR.AccesoDatos.Data.Inicializador
{
    public class InicializadorDB : IInicializadorDB
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public InicializadorDB(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IContenedorTrabajo contenedorTrabajo)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _contenedorTrabajo = contenedorTrabajo;
            _contenedorTrabajo = contenedorTrabajo;
        }

        public void Inicializar()
        {
            
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
            }

            if (_db.Roles.Any(ro => ro.Name == CNT.SuperAdmin)) return;

            _roleManager.CreateAsync(new IdentityRole(CNT.SuperAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Administrativo)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Gestor)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Division)).GetAwaiter().GetResult();

            //Creacion usuario inicial

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "representacion.institucional@funsar.org.co",
                Email = "representacion.institucional@funsar.org.co",
                EmailConfirmed = true,
                Nombre = "Maicol Steven Granados Rodriguez",
                Documento = "1000136197",
                BrigadaId = 1,
                RangoId = 4
            }, "Funsar123*").GetAwaiter().GetResult();

            ApplicationUser usuario = _db.ApplicationUser
                .Where(us => us.Email == "representacion.institucional@funsar.org.co")
                .FirstOrDefault();
            _userManager.AddToRoleAsync(usuario, CNT.SuperAdmin).GetAwaiter().GetResult();

        }
    }
}
