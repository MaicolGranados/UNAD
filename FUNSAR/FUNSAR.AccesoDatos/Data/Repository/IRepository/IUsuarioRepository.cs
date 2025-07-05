using FUNSAR.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IUsuarioRepository : IRepository<ApplicationUser>
    {
        void BloquearUsuario(string IdUsuario);
        void Desbloquearusuario(string IdUsuario);
        void Eliminarusuario(string IdUsuario);
        void Editarusuario(ApplicationUser user);
    }
}
