using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FUNSAR.Utilidades
{
    public class OperacionCertificado
    {
        public int DatosCertificado(string tdocumento,string documento,string strConection)
        {
            int resultado = 0;

            try
            {
                //string conectarDB = ConfigurationManager.ConnectionStrings["FunsarDB"].ConnectionString;
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_Certificado", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("tdocumento", SqlDbType.VarChar).Value = tdocumento;
                cmd.Parameters.Add("documento", SqlDbType.VarChar).Value = documento;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    
                        resultado =  Convert.ToInt32(dr["Id"].ToString());
                    
                }
                else
                {
                    resultado = 0;
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                resultado = 0;
            }

            return resultado;
        }

        public int DatosVoluntario(string tdocumento, string documento, string strConection)
        {
            int resultado = 0;

            try
            {
                //string conectarDB = ConfigurationManager.ConnectionStrings["FunsarDB"].ConnectionString;
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_Voluntario", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("tdocumento", SqlDbType.VarChar).Value = tdocumento;
                cmd.Parameters.Add("documento", SqlDbType.VarChar).Value = documento;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {

                    resultado = Convert.ToInt32(dr["Id"].ToString());

                }
                else
                {
                    resultado = 0;
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                resultado = 0;
            }

            return resultado;
        }

        public int IdVoluntario(string documento, int tdocumento, string strConection)
        {
            int resultado = 0;

            try
            {
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_IdVoluntario", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("documento", SqlDbType.VarChar).Value = documento;
                cmd.Parameters.Add("tdocumento", SqlDbType.Int).Value = tdocumento;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {

                    resultado = Convert.ToInt32(dr["Id"].ToString());

                }
                else
                {
                    resultado = 0;
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                resultado = 0;
            }

            return resultado;
        }

        public int IdAcudiente(string documento, int tdocumento, string strConection)
        {
            int resultado = 0;

            try
            {
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_IdAcudiente", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("documento", SqlDbType.VarChar).Value = documento;
                cmd.Parameters.Add("tdocumento", SqlDbType.Int).Value = tdocumento;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {

                    resultado = Convert.ToInt32(dr["Id"].ToString());

                }
                else
                {
                    resultado = 0;
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                resultado = 0;
            }

            return resultado;
        }

        public string CorreoGestor(int voluntario, string strConection)
        {
            string Correo = "";

            try
            {
                //string conectarDB = ConfigurationManager.ConnectionStrings["FunsarDB"].ConnectionString;
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_CorreoGestor", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("idVoluntario", SqlDbType.Int).Value = voluntario;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {

                    Correo = dr["Email"].ToString();

                }
                else
                {
                    Correo = "NA";
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                Correo = "ERR";
            }

            return Correo;
        }

        public int[] Parametros(string tdocumento, string brigada, string semestre, string proceso,string strConection)
        {
            int[] resultado;
            resultado = new int[4];

            try
            {
                //string conectarDB = ConfigurationManager.ConnectionStrings["FunsarDB"].ConnectionString;
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_Parametros", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@tdocumento", SqlDbType.VarChar).Value = tdocumento;
                cmd.Parameters.Add("@brigada", SqlDbType.VarChar).Value = brigada;
                cmd.Parameters.Add("@semestre", SqlDbType.VarChar).Value = semestre;
                cmd.Parameters.Add("@proceso", SqlDbType.VarChar).Value = proceso;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    resultado[0] = Convert.ToInt32(dr[0]);
                    resultado[1] = Convert.ToInt32(dr[1]);
                    resultado[2] = Convert.ToInt32(dr[2]);
                    resultado[3] = Convert.ToInt32(dr[3]);
                }
                else
                {
                    resultado[0] = 0;
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                resultado[0] = 0;
            }

            return resultado;
        }

        public int UpdateMasivo(string strConection)
        {
            int resultado = 0;

            try
            {
                //string conectarDB = ConfigurationManager.ConnectionStrings["FunsarDB"].ConnectionString;
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SP_InsertCertificados", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                cmd.Connection.Close();
                resultado = 1;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                resultado = 0;
            }

            return resultado;
        }

        public int BrigadaxGestor(string user,string strConection)
        {
            int result = 0;
            try
            {
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_BrigadaxGestor", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = Convert.ToInt32(dr[0]);
                }
                else
                {
                    result = 0;
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                result = 0;
            }
            return result;
        }

        public string BrigadaxGestor(int idBrigada, string strConection)
        {
            string result = "";
            try
            {
                SqlConnection sqlConectar = new SqlConnection(strConection);
                SqlCommand cmd = new SqlCommand("SEL_NomBrigada", sqlConectar)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("@idBrigada", SqlDbType.VarChar).Value = idBrigada;
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = dr[0].ToString();
                }
                else
                {
                    result = "NoData";
                }
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                result = "Err";
            }
            return result;
        }


    }
}
