using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class VoluntarioRepository : Repository<Voluntario>, IVoluntarioRepository
    {
        private readonly ApplicationDbContext _db;
        public VoluntarioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Voluntario voluntario)
        {
            var objDesdeDb = _db.Voluntario.FirstOrDefault(s => s.Id == voluntario.Id);
            objDesdeDb.Nombre = voluntario.Nombre;
            objDesdeDb.Apellido = voluntario.Apellido;
            objDesdeDb.Documento = voluntario.Documento;
            objDesdeDb.DocumentoId = voluntario.DocumentoId;
            objDesdeDb.ColegioId = voluntario.ColegioId;
            objDesdeDb.EstadoId = voluntario.EstadoId;
            objDesdeDb.ProcesoId = voluntario.ProcesoId;
            _db.SaveChanges();
        }
    }
}
