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
    public class PFERepository : Repository<PFE>, IPFERepository
    {
        private readonly ApplicationDbContext _db;
        public PFERepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(PFE pFE)
        {
            var objDesdeDb = _db.PFE.FirstOrDefault(s => s.VoluntarioId == pFE.VoluntarioId);
            objDesdeDb.Detalle = pFE.Detalle;
            objDesdeDb.EstadoPFEId = pFE.EstadoPFEId;
            _db.SaveChanges();
        }
    }
}
