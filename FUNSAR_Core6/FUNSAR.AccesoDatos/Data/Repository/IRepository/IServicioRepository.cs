using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IServicioRepository : IRepository<Servicio>
    {
        IEnumerable<SelectListItem> GetListaServicio();
        IEnumerable<SelectListItem> GetListaValor();
        void Update(Servicio servicio);

    }
}
