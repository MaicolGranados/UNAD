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
    public class ParamsRepository : Repository<Params>, IParamsRepository
    {
        private readonly ApplicationDbContext _db;
        public ParamsRepository(ApplicationDbContext db) : base(db)
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

        public void Update(Params parametros)
        {
            var objDesdeDb = _db.Params.FirstOrDefault(s => s.id == parametros.id);
            if (objDesdeDb != null) {
                objDesdeDb.Concepto = parametros.Concepto;
                objDesdeDb.Valor = parametros.Valor;
            }
            _db.SaveChanges();
        }

    }
}
