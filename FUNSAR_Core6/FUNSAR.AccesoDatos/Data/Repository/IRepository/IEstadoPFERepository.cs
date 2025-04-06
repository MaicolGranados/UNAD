using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IEstadoPFERepository : IRepository<EstadoPFE>
    {
        IEnumerable<SelectListItem> GetListaEstadoPFE();

        void Update(EstadoPFE estadoPFE);
    }
}
