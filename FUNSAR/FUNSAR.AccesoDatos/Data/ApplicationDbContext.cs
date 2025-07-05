using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FUNSAR.Models;

namespace FUNSAR.AccesoDatos.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Agregar Modelos
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Slider> Slider { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Brigada> Brigada { get; set; }
        public DbSet<Certificado> Certificado { get; set; }
        public DbSet<tipoDocumento> TDocumento { get; set; }
        public DbSet<Semestre> Semestre { get; set; }
        public DbSet<CertificadoTemp> CertificadoTemp { get; set; }
        public DbSet<Colegio> Colegio { get; set; }
        public DbSet<Voluntario> Voluntario { get; set; }
        public DbSet<Rango> Rango { get; set; }
        public DbSet<EstadoPersona> EstadoPersona { get; set; }
        public DbSet<PFE> PFE { get; set; }
        public DbSet<EstadoPFE> EstadoPFE { get; set; }
        public DbSet<Acudiente> Acudiente { get; set; }
        public DbSet<Proceso> Proceso { get; set; }
        public DbSet<Pagos> Pagos { get; set; }
        public DbSet<Servicio> Servicio { get; set; }
        public DbSet<session> Session { get; set; }
        public DbSet<Asistencia> Asistencia { get; set; }
        public DbSet<EstadoAsistencia> EstadoAsistencias { get; set; }
        public DbSet<Params> Params { get; set; }
        public DbSet<OTP> Otp { get; set; }
        public DbSet<DatoColegio> DatoColegio { get; set; }
        public DbSet<JornadaColegio> JornadaColegio { get; set; }
        public DbSet<EstadoBrigada> EstadoBrigadas { get; set; }
        public DbSet<VigenciaServicio> VigenciaServicio { get; set; }
        public DbSet<AsistenteSalida> AsistenteSalida { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}