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
    public class ColegioRepository : Repository<Colegio>, IColegioRepository
    {
        private readonly ApplicationDbContext _db;
        public ColegioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public List<Colegio> BuscarColegio(string nombre)
        {
            return _db.Colegio.Where(c => c.Nombre.Contains(nombre)).ToList();
        }

        public IEnumerable<SelectListItem> GetListaColegios()
        {
            return _db.Colegio.Select(i => new SelectListItem()
            {
                Text = i.Nombre,
                Value = i.Id.ToString(),
            }
            );
        }
        public IEnumerable<SelectListItem> GetListaColegios(int idBrigada)
        {
            return _db.Colegio.Where(i => i.BrigadaId == idBrigada).Select(i => new SelectListItem()
            {
                Text = i.Nombre,
                Value = i.Id.ToString(),
            }
            );
        }


        public void Update(Colegio colegio)
        {
            var objDesdeDb = _db.Colegio.FirstOrDefault(s => s.Id == colegio.Id);
            objDesdeDb.Nombre = colegio.Nombre;
            objDesdeDb.BrigadaId = colegio.BrigadaId;
            _db.SaveChanges();
        }
    }
}
