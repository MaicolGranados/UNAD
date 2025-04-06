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
    public class EstadoPersonaRepository : Repository<EstadoPersona>, IEstadoPersonaRepository
    {
        private readonly ApplicationDbContext _db;
        public EstadoPersonaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaEstado(List<int> id)
        {
            return _db.EstadoPersona.Where(e => id.Contains(e.Id)).Select(i => new SelectListItem()
            {
                Text = i.estadoPersona,
                Value = i.Id.ToString()
            }
            );
        }

        public IEnumerable<SelectListItem> GetListaEstado()
        {
            return _db.EstadoPersona.Select(i => new SelectListItem()
            {
                Text = i.estadoPersona,
                Value = i.Id.ToString()
            }
            );
        }

        public void Update(EstadoPersona estadoPersona)
        {
            var objDesdeDb = _db.EstadoPersona.FirstOrDefault(s => s.Id == estadoPersona.Id);
            objDesdeDb.estadoPersona = estadoPersona.estadoPersona;
            _db.SaveChanges();
        }
    }
}
