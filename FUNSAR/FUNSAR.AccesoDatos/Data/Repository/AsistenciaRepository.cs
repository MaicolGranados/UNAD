using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class AsistenciaRepository : Repository<Asistencia>, IAsistenciaRepository
    {
        private readonly ApplicationDbContext _db;
        public AsistenciaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Asistencia asistencia)
        {
            var objDesdeDb = _db.Asistencia.FirstOrDefault(s => s.Id == asistencia.Id);
            objDesdeDb.EstadoAsistenciaId = asistencia.EstadoAsistenciaId;
            _db.SaveChanges();
        }
    }
}
