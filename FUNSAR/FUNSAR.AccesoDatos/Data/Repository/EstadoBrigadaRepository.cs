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
    public class EstadoBrigadaRepository : Repository<EstadoBrigada>, IEstadoBrigadaRepository
    {
        private readonly ApplicationDbContext _db;
        public EstadoBrigadaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Brigada brigada)
        {
            var objDesdeDb = _db.Brigada.FirstOrDefault(s => s.Id == brigada.Id);
            objDesdeDb.Nombre = brigada.Nombre;
            _db.SaveChanges();
        }
    }
}
