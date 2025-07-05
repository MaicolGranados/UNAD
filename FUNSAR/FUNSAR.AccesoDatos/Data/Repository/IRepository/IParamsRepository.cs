using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IParamsRepository : IRepository<Params>
    {
        IEnumerable<SelectListItem> GetListaServicio();
        IEnumerable<SelectListItem> GetListaValor();

        void Update(Params parametros);
    }
}
