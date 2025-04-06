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
    public class CertificadoRepository : Repository<Certificado>, ICertificadoRepository
    {
        private readonly ApplicationDbContext _db;
        public CertificadoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Certificado certificado)
        {
            var objDesdeDb = _db.Certificado.FirstOrDefault(s => s.Id == certificado.Id);
            objDesdeDb.Nombre = certificado.Nombre;
            objDesdeDb.Apellido = certificado.Apellido;
            objDesdeDb.AnoProceso = certificado.AnoProceso;
            objDesdeDb.DocumentoId = certificado.DocumentoId;
            objDesdeDb.Documento = certificado.Documento;
            objDesdeDb.BrigadaId = certificado.BrigadaId;
            objDesdeDb.SemestreId = certificado.SemestreId;
            objDesdeDb.FechaExpedicion = certificado.FechaExpedicion;
            _db.SaveChanges();
        }
    }
}
