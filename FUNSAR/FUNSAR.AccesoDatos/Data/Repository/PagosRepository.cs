using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class PagosRepository : Repository<Pagos>, IPagosRepository
    {
        private readonly ApplicationDbContext _db;
        public PagosRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Pagos pagos)
        {
            var objDesdeDb = _db.Pagos.FirstOrDefault(s => s.Id == pagos.Id);
            objDesdeDb.Fechapago = pagos.Fechapago;
            objDesdeDb.Estado = pagos.Estado;
            //objDesdeDb.DocumentoIdR = pagos.DocumentoIdR;
            //objDesdeDb.DocumentoR = pagos.DocumentoR;
            //objDesdeDb.ServicioId = pagos.ServicioId;
            //objDesdeDb.DocumentoP = pagos.DocumentoP;
            //objDesdeDb.CelularR = pagos.CelularR;
            //objDesdeDb.CorreoR = pagos.CorreoR;

            _db.SaveChanges();  
        }
    }
}
