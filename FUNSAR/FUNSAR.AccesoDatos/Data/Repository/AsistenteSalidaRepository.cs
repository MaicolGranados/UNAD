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
    public class AsistenteSalidaRepository : Repository<AsistenteSalida>, IAsistenteSalidaRepository
    {
        private readonly ApplicationDbContext _db;
        public AsistenteSalidaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AsistenteSalida voluntario)
        {
            var objDesdeDb = _db.AsistenteSalida.FirstOrDefault(s => s.Id == voluntario.Id);
            objDesdeDb.Nombre = voluntario.Nombre;
            objDesdeDb.Apellido = voluntario.Apellido;
            objDesdeDb.Documento = voluntario.Documento;
            objDesdeDb.DocumentoId = voluntario.DocumentoId;
            objDesdeDb.ReferenciaPago = voluntario.ReferenciaPago;
            _db.SaveChanges();
        }
    }
}
