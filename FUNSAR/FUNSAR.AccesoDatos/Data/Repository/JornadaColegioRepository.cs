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
    public class JornadaColegioRepository : Repository<JornadaColegio>, IJornadaColegioRepository
    {
        private readonly ApplicationDbContext _db;
        public JornadaColegioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaJornadaColegio()
        {
            return _db.JornadaColegio.Select(i => new SelectListItem()
            {
                Text = i.Jornada,
                Value = i.Id.ToString()
            }
            );
        }

        public IEnumerable<SelectListItem> GetListaJornadaColegio(int idJornada)
        {
            return _db.JornadaColegio.Where(b => b.Id == idJornada).Select(i => new SelectListItem()
            {
                Text = i.Jornada,
                Value = i.Id.ToString()
            }
            );
        }

        public void Update(JornadaColegio jornada)
        {
            var objDesdeDb = _db.JornadaColegio.FirstOrDefault(s => s.Id == jornada.Id);
            objDesdeDb.Jornada = jornada.Jornada;
            _db.SaveChanges();
        }
    }
}
