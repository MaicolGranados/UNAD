using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class SessionRepository : Repository<session>, ISessionRepository
    {
        private readonly ApplicationDbContext _db;
        public SessionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(session session)
        {
            var objDesdeDb = _db.Session.FirstOrDefault(s => s.sessionid == session.sessionid);
            objDesdeDb.sessionid = session.sessionid;
            objDesdeDb.sessiondocument = session.sessiondocument;
            objDesdeDb.sessionservice = session.sessionservice;
            
            _db.SaveChanges();
        }

    }
}
