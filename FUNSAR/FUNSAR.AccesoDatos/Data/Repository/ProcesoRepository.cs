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
    public class ProcesoRepository : Repository<Proceso>, IProcesoRepository
    {
        private readonly ApplicationDbContext _db;
        public ProcesoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetListaProceso()
        {
            return _db.Proceso.Select(i => new SelectListItem()
            {
                Text = i.proceso,
                Value = i.Id.ToString()
            }
            );
        }

    }
}
