using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class VigenciaServicioRepository : Repository<VigenciaServicio>, IVigenciaServicioRepository
    {
        private readonly ApplicationDbContext _db;
        public VigenciaServicioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(VigenciaServicio VigenciaServicio)
        {
            var objDesdeDb = _db.VigenciaServicio.FirstOrDefault(s => s.Id == VigenciaServicio.Id);
            objDesdeDb.ServicioId = VigenciaServicio.ServicioId;
            objDesdeDb.Vigencia = VigenciaServicio.Vigencia;
            objDesdeDb.Valor = VigenciaServicio.Valor;

            _db.SaveChanges();
        }

    }
}
