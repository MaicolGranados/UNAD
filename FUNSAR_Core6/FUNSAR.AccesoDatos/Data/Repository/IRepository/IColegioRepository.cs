using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IColegioRepository : IRepository<Colegio>
    {
        IEnumerable<SelectListItem> GetListaColegios();
        IEnumerable<SelectListItem> GetListaColegios(int idBrigada);
        void Update(Colegio colegio);

        public List<Colegio> BuscarColegio(string nombre);

    }
}
