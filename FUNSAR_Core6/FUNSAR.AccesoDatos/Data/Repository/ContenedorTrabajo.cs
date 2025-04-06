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
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _db;

        public ContenedorTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Categoria = new CategoriaRepository(_db);
            Articulo = new ArticuloRepository(_db);
            Slider = new SliderRepository(_db);
            Usuario = new UsuarioRepository(_db);
            Brigada = new BrigadaRepository(_db);
            Certificado = new CertificadoRepository(_db);
            Semestre = new SemestreRepository(_db);
            TDocumento = new TDocumentoRepository(_db);
            CertificadoTemp = new CertificadoTempRepository(_db);
            Colegio = new ColegioRepository(_db);
            Voluntario = new VoluntarioRepository(_db);
            Rango = new RangoRepository(_db);
            estadoPersona = new EstadoPersonaRepository(_db);
            pFE = new PFERepository(_db);
            estadoPFE = new EstadoPFERepository(_db);
            Acudiente = new AcudienteRepository(_db);
            Proceso = new ProcesoRepository(_db);
            Pagos = new PagosRepository(_db);
            Servicio = new ServicioRepository(_db);
            Session = new SessionRepository(_db);
            Asistencia = new AsistenciaRepository(_db);
            EstadoAsistencia = new EstadoAsistenciaRepository(_db);
            Params = new ParamsRepository(_db);
            Otp = new OtpRepository(_db);
            DatoColegio = new DatoColegioRepository(_db);
            JornadaColegio = new JornadaColegioRepository(_db);
            EstadoBrigada = new EstadoBrigadaRepository(_db);
            VigenciaServicio = new VigenciaServicioRepository(_db);
            AsistenteSalida = new AsistenteSalidaRepository(_db);
        }

        public ICategoriaRepository Categoria { get; private set; }
        public IArticuloRepository Articulo { get; private set; }
        public ISliderRepository Slider { get; private set; }
        public IUsuarioRepository Usuario { get; private set; }
        public IBrigadaRepository Brigada { get; private set; }
        public ICertificadoRepository Certificado { get; private set; }
        public ISemestreRepository Semestre { get; private set; }
        public ITDocumentoRepository TDocumento { get; private set; }
        public ICertificadoTempRepository CertificadoTemp { get; private set; }
        public IColegioRepository Colegio { get; private set; }
        public IVoluntarioRepository Voluntario { get; private set; }
        public IRangoRepository Rango { get; private set; }
        public IEstadoPersonaRepository estadoPersona { get; private set; }
        public IPFERepository pFE { get; private set; }
        public IEstadoPFERepository estadoPFE { get; private set; }
        public IAcudienteRepository Acudiente { get; private set; }
        public IProcesoRepository Proceso { get; private set; }
        public IPagosRepository Pagos { get; private set; }
        public IServicioRepository Servicio { get; private set; }
        public ISessionRepository Session { get; private set; }
        public IAsistenciaRepository Asistencia { get; private set; }
        public IEstadoAsistenciaRepository EstadoAsistencia { get; private set; }
        public IParamsRepository Params { get; private set; }
        public IOtpRepository Otp { get; private set; }
        public IDatoColegioRepository DatoColegio { get; private set; }
        public IJornadaColegioRepository JornadaColegio { get; private set; }
        public IEstadoBrigadaRepository EstadoBrigada { get; private set; }
        public IVigenciaServicioRepository VigenciaServicio { get; private set; }
        public IAsistenteSalidaRepository AsistenteSalida { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void save()
        {
            _db.SaveChanges();
        }
    }
}
