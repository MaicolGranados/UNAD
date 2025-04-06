using FUNSAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IAsistenteSalidaRepository : IRepository<AsistenteSalida>
    {
        void Update(AsistenteSalida asistente);
    }
}
