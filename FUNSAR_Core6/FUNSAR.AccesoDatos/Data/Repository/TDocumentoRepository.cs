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
    public class TDocumentoRepository : Repository<tipoDocumento>, ITDocumentoRepository
    {
        private readonly ApplicationDbContext _db;
        public TDocumentoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetIdDocumento(string Tdoc)
        {
            return _db.TDocumento.Where(t => t.tDocumento == Tdoc).Select(i => new SelectListItem()
            {
                Value = i.Id.ToString(),
            }
            );
        }

        public IEnumerable<SelectListItem> GetListaDocumento()
        {
            return _db.TDocumento.Select(i => new SelectListItem()
                {
                    Text = i.tDocumento,
                    Value = i.Id.ToString()
                }
            );
        }

        public IEnumerable<SelectListItem> GetTipoDocumento(int idTdoc)
        {
            return _db.TDocumento.Where(t => t.Id == idTdoc).Select(i => new SelectListItem()
            {
                Text = i.tDocumento.ToString()
            }
            );
        }


    }
}
