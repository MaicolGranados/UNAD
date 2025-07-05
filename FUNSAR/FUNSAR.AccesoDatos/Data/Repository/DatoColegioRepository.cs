using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class DatoColegioRepository : Repository<DatoColegio>, IDatoColegioRepository
    {
        private readonly ApplicationDbContext _db;
        public DatoColegioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(DatoColegio datoColegio)
        {
            var objDesdeDb = _db.DatoColegio.FirstOrDefault(s => s.Id == datoColegio.Id);
            objDesdeDb.NombreEncargado = datoColegio.NombreEncargado;
            objDesdeDb.Correo = datoColegio.Correo;
            objDesdeDb.NumeroContacto = datoColegio.NumeroContacto;
            objDesdeDb.Cargo = datoColegio.Cargo;
            objDesdeDb.Jornada = datoColegio.Jornada;
            _db.SaveChanges();
        }
    }
}
