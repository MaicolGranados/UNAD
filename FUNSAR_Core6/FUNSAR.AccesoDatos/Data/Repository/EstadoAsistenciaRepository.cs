using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class EstadoAsistenciaRepository : Repository<EstadoAsistencia>, IEstadoAsistenciaRepository
    {
        private readonly ApplicationDbContext _db;
        public EstadoAsistenciaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetListaEstadoAsistencia()
        {
            return _db.EstadoAsistencias.Select(i => new SelectListItem()
                {
                    Text = i.estadoAsistencia,
                    Value = i.Id.ToString()
                }
            );
        }

        public void Update(EstadoAsistencia estadoAsistencia)
        {
            var objDesdeDb = _db.EstadoAsistencias.FirstOrDefault(s => s.Id == estadoAsistencia.Id);
            objDesdeDb.estadoAsistencia = estadoAsistencia.estadoAsistencia;

            _db.SaveChanges();  
        }
    }
}
