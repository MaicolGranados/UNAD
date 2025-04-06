using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface ITDocumentoRepository : IRepository<tipoDocumento>
    {
        IEnumerable<SelectListItem> GetListaDocumento();
        IEnumerable<SelectListItem> GetTipoDocumento(int idTdoc);
        IEnumerable<SelectListItem> GetIdDocumento(string Tdoc);

    }
}
