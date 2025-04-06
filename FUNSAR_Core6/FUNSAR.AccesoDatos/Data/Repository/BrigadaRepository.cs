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
    public class BrigadaRepository : Repository<Brigada>, IBrigadaRepository
    {
        private readonly ApplicationDbContext _db;
        public BrigadaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaBrigadas(IEnumerable<int> idsExcluidos = null)
        {
            idsExcluidos ??= new List<int>(); // Si es nulo, inicializamos como lista vacía

            return _db.Brigada
                .Where(i => !idsExcluidos.Contains(i.Id)) // Filtrar los IDs excluidos
                .Select(i => new SelectListItem()
                {
                    Text = i.Nombre,
                    Value = i.Id.ToString()
                });
        }

        public IEnumerable<SelectListItem> GetListaBrigadas(int idBrigada)
        {
            return _db.Brigada.Where(b => b.Id == idBrigada).Select(i => new SelectListItem()
            {
                Text = i.Nombre,
                Value = i.Id.ToString()
            }
            );
        }

        public void Update(Brigada brigada)
        {
            var objDesdeDb = _db.Brigada.FirstOrDefault(s => s.Id == brigada.Id);
            objDesdeDb.Nombre = brigada.Nombre;
            objDesdeDb.EstadoBrigadaId = brigada.EstadoBrigadaId;
            objDesdeDb.fechaActualizacion = brigada.fechaActualizacion;
            _db.SaveChanges();
        }
    }
}
