using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class SemestreRepository : Repository<Semestre>, ISemestreRepository
    {
        private readonly ApplicationDbContext _db;
        public SemestreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public IEnumerable<SelectListItem> GetListaSemestre()
        {
            return _db.Semestre.Select(i => new SelectListItem()
                {
                    Text = i.semestre,
                    Value = i.Id.ToString()
                }
            );
        }

       
    }
}
