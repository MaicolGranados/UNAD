﻿using FUNSAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository.IRepository
{
    public interface IAcudienteRepository : IRepository<Acudiente>
    {
        void Update(Acudiente acudiente);
    }
}
