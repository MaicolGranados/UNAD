using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IBrigadaRepository : IRepository<Brigada>
    {
        IEnumerable<SelectListItem> GetListaBrigadas(IEnumerable<int> idsExcluidos = null);
        IEnumerable<SelectListItem> GetListaBrigadas(int idBrigada);
        void Update(Brigada brigada);
    }
}
