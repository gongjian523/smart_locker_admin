using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.SqliteHelper
{
    class SqliteHelper
    {
        public static SqliteHelper Instance
        {
            get
            {
                return iInstance;
            }

        }

        static private SqliteHelper iInstance = new SqliteHelper();
        private SQLiteConnection iConn;

        private SQLiteConnection SQLConnection
        {
            get
            {
                if (iConn == null)
                {
                    string dbFilename = @"CFLMedCab.db";
                    string cs = string.Format("Version=3;uri=file:{0}", dbFilename);

                    iConn = new SQLiteConnection(cs);
                }

                if (iConn.State != ConnectionState.Open)
                {
                    iConn.Open();
                }
                return iConn;
            }
        }

        public void CloseDataBase(SQLiteConnection iConn)
        {
            iConn.Close();
            return;
        }

        private bool ExecuteNonQuery(string sql)
        {
            SQLiteConnection _sqlConn = SQLConnection;

            bool ret = true;
            IDbCommand cmd = _sqlConn.CreateCommand();
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }

        private object ExecuteScalar(string sql)
        {
            SQLiteConnection _sqlConn = SQLConnection;

            object obj;

            IDbCommand cmd = _sqlConn.CreateCommand();
            cmd.CommandText = sql;

            try
            {
                obj = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return null;
            }

            return obj;
        }


        private IDataReader ExecuteReader(string sql)
        {
            SQLiteConnection _sqlConn = SQLConnection;

            IDataReader data;

            IDbCommand cmd = _sqlConn.CreateCommand();
            cmd.CommandText = sql;

            try
            {
                data = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                return null;
            }

            return data;
        }
    }
}
