using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class EstadoPFERepository : Repository<EstadoPFE>, IEstadoPFERepository
    {
        private readonly ApplicationDbContext _db;
        public EstadoPFERepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetListaEstadoPFE()
        {
            return _db.EstadoPFE.Select(i => new SelectListItem()
                {
                    Text = i.Detalle,
                    Value = i.Id.ToString()
                }
            );
        }

        public void Update(EstadoPFE estadoPFE)
        {
            var objDesdeDb = _db.EstadoPFE.FirstOrDefault(s => s.Id == estadoPFE.Id);
            objDesdeDb.Detalle = estadoPFE.Detalle;

            _db.SaveChanges();  
        }
    }
}
