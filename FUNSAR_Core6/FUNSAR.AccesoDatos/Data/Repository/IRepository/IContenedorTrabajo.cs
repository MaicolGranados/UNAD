using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IContenedorTrabajo : IDisposable
    {
        ICategoriaRepository Categoria { get; }
        // Aqui se deben ir agregando los repositorios
        IArticuloRepository Articulo { get; }
        ISliderRepository Slider { get; }
        IUsuarioRepository Usuario { get; }
        IBrigadaRepository Brigada { get; }
        ICertificadoRepository Certificado { get; }
        ISemestreRepository Semestre { get; }
        ITDocumentoRepository TDocumento { get; }
        ICertificadoTempRepository CertificadoTemp { get; }
        IColegioRepository Colegio { get; }
        IVoluntarioRepository Voluntario { get; }
        IRangoRepository Rango { get; }
        IEstadoPersonaRepository estadoPersona { get; }
        IPFERepository pFE { get; }
        IEstadoPFERepository estadoPFE { get; }
        IAcudienteRepository Acudiente { get; }
        IProcesoRepository Proceso { get; }
        IPagosRepository Pagos { get; }
        IServicioRepository Servicio { get; }
        ISessionRepository Session { get; }
        IAsistenciaRepository Asistencia { get; }
        IEstadoAsistenciaRepository EstadoAsistencia { get; }
        IParamsRepository Params { get; }
        IOtpRepository Otp { get; }
        IDatoColegioRepository DatoColegio { get; }
        IJornadaColegioRepository JornadaColegio { get; }
        IEstadoBrigadaRepository EstadoBrigada { get; }
        IVigenciaServicioRepository VigenciaServicio { get; }
        IAsistenteSalidaRepository AsistenteSalida { get; }
        void save();

    }
}
