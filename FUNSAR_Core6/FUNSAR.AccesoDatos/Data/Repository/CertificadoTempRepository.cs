using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class CertificadoTempRepository : Repository<CertificadoTemp>, ICertificadoTempRepository
    {
        private readonly ApplicationDbContext _db;
        public CertificadoTempRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
