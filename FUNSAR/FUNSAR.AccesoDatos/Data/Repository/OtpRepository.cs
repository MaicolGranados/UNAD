using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNSAR.AccesoDatos.Data.Repository
{
    public class OtpRepository : Repository<OTP>, IOtpRepository
    {
        private readonly ApplicationDbContext _db;
        public OtpRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OTP otp)
        {
            var objDesdeDb = _db.Otp.FirstOrDefault(s => s.Mail == otp.Mail);
            if (objDesdeDb != null)
            {
                objDesdeDb.Code = otp.Code;
            }
            _db.SaveChanges();  
        }

    }
}
