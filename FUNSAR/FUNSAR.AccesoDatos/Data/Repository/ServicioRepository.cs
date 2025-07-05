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
    public class ServicioRepository : Repository<Servicio>, IServicioRepository
    {
        private readonly ApplicationDbContext _db;
        public ServicioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaServicio()
        {
            return _db.Servicio.Select(i => new SelectListItem()
            {
                Text = i.Detalle,
                Value = i.Id.ToString()
            }
            );
        }

        public IEnumerable<SelectListItem> GetListaValor()
        {
            return _db.Servicio.Select(i => new SelectListItem()
            {
                Text = i.Valor,
                Value = i.Id.ToString()
            }
            );
        }

        public void Update(Servicio servicio)
        {
            var objDesdeDb = _db.Servicio.FirstOrDefault(s => s.Id == servicio.Id);
            objDesdeDb.Detalle = servicio.Detalle;
            objDesdeDb.Valor = servicio.Valor;

            _db.SaveChanges();
        }

    }
}
