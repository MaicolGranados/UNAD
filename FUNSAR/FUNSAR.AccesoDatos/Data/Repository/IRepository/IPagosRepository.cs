using FUNSAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IPagosRepository : IRepository<Pagos>
    {
        void Update(Pagos pagos);
    }
}
