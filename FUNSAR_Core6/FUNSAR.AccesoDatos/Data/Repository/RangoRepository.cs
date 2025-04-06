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
    public class RangoRepository : Repository<Rango>, IRangoRepository
    {
        private readonly ApplicationDbContext _db;
        public RangoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaRango()
        {
            return _db.Rango.Select(i => new SelectListItem()
            {
                Text = i.RangoNombre,
                Value = i.Id.ToString()
            }
            );
        }

    }
}
