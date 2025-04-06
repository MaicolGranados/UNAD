using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IEstadoPersonaRepository : IRepository<EstadoPersona>
    {
        IEnumerable<SelectListItem> GetListaEstado(List<int> id);
        IEnumerable<SelectListItem> GetListaEstado();
        void Update(EstadoPersona estadoPersona);
    }
}
