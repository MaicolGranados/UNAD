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
    public class AcudienteRepository : Repository<Acudiente>, IAcudienteRepository
    {
        private readonly ApplicationDbContext _db;
        public AcudienteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Acudiente acudiente)
        {
            var objDesdeDb = _db.Acudiente.FirstOrDefault(s => s.Id == acudiente.Id);
            objDesdeDb.VoluntarioId = acudiente.VoluntarioId;
            _db.SaveChanges();
        }
    }
}
