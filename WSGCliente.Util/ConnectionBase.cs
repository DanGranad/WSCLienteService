
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Util
{
    public class ConnectionBase
    {
        //static string strConexionOracle = null;
        static string strConexionOracleVTime = null;
        static string strConexionOracleVentasV = null;
        static string strConexionOracleLAFT = null;

        //private OracleConnection DataConnectionOracle = new OracleConnection(strConexionOracle);
        private OracleConnection DataConnectionOracleTIME = new OracleConnection(strConexionOracleVTime);
        private OracleConnection DataConnectionOracleVentasV = new OracleConnection(strConexionOracleVentasV);
        private OracleConnection DataConnectionOracleLAFT = new OracleConnection(strConexionOracleLAFT);
        

        public enum enuTypeDataBase
        {
            Oracle,
            OracleVTime,
            OracleVentasV
        }

        public enum enuTypeExecute
        {
            //ExecuteNonQuery,
            ExecuteReader
        }

        public static DbParameterCollection ParamsCollectionResult;
        public ConnectionBase()
        {

            //ConnectionBase.strConexionOracle = ConfigurationManager.ConnectionStrings["cnxStringOracle"].ConnectionString;
            //DataConnectionOracle.ConnectionString = ConnectionBase.strConexionOracle;

            ConnectionBase.strConexionOracleVTime = ConfigurationManager.ConnectionStrings["cnxStringOracleTIMEP"].ConnectionString;
            DataConnectionOracleTIME.ConnectionString = ConnectionBase.strConexionOracleVTime;

            ConnectionBase.strConexionOracleVentasV = ConfigurationManager.ConnectionStrings["cnxStringOracleRentasV"].ConnectionString;
            DataConnectionOracleVentasV.ConnectionString = ConnectionBase.strConexionOracleVentasV;

            ConnectionBase.strConexionOracleLAFT= ConfigurationManager.ConnectionStrings["cnxStringOracleLAFT"].ConnectionString;
            DataConnectionOracleLAFT.ConnectionString = ConnectionBase.strConexionOracleLAFT;


            
        }

        protected DbConnection ConnectionGet(enuTypeDataBase typeDataBase = enuTypeDataBase.Oracle)
        {
            DbConnection DataConnection = null;
            switch (typeDataBase)
            {
                //case enuTypeDataBase.Oracle:
                //    DataConnection = DataConnectionOracle;
                //    break;
                case enuTypeDataBase.OracleVTime:
                    DataConnection = DataConnectionOracleTIME;
                    break;
                case enuTypeDataBase.OracleVentasV:
                    DataConnection = DataConnectionOracleVentasV;
                    break;
                default:
                    break;
            }
            return DataConnection;
        }
        //protected DbDataReader ExecuteByStoredProcedure(string nameStore,
        //        IEnumerable<DbParameter> parameters = null,
        //        enuTypeDataBase typeDataBase = enuTypeDataBase.Oracle,
        //        enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader)
        //{

        //    DbConnection DataConnection = ConnectionGet(typeDataBase);
        //    DbCommand cmdCommand = DataConnection.CreateCommand();
        //    //var myTrans = cmdCommand.Connection.BeginTransaction();
        //    //cmdCommand.Transaction = myTrans;
        //    cmdCommand.CommandText = nameStore;
        //    cmdCommand.CommandType = CommandType.StoredProcedure;

        //    if (parameters != null)
        //    {
        //        foreach (DbParameter parameter in parameters)
        //        {
        //            cmdCommand.Parameters.Add(parameter);
        //        }
        //    }

        //    DataConnection.Open();
        //    DbDataReader myReader;

        //    //try
        //    //{
        //    if (((cmdCommand.Parameters.Contains("C_TABLE") || IsOracleReader(cmdCommand))) && typeExecute == enuTypeExecute.ExecuteReader)
        //    {
        //        myReader = cmdCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //    }
        //    else
        //    {
        //        cmdCommand.ExecuteNonQuery();
        //        //myTrans.Commit();
        //        ParamsCollectionResult = cmdCommand.Parameters;
        //        cmdCommand.Connection.Close();
        //        myReader = null;
        //    }

        //    return myReader;

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    myTrans.Rollback();
        //    //    Console.WriteLine(ex.ToString());
        //    //    return myReader = null;
        //    //}
        //}

        protected DbDataReader ExecuteByStoredProcedureVT_TRX(string nameStore,
               IEnumerable<DbParameter> parameters = null,
               DbConnection connection = null, DbTransaction trx = null,
               enuTypeDataBase typeDataBase = enuTypeDataBase.OracleVTime,
               enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader)
        {
            DbCommand cmdCommand = connection.CreateCommand();
            cmdCommand.Transaction = trx;
            cmdCommand.Connection = connection;

            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }

            DbDataReader myReader;

            cmdCommand.ExecuteNonQuery();
            ParamsCollectionResult = cmdCommand.Parameters;
            myReader = null;

            return myReader;
        }

        protected DbDataReader ExecuteByStoredProcedureVT(string nameStore,
                IEnumerable<DbParameter> parameters = null,
                enuTypeDataBase typeDataBase = enuTypeDataBase.OracleVTime,
                enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader)
        {
            DbConnection DataConnection = ConnectionGet(typeDataBase);
            DbCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }

            DataConnection.Open();
            DbDataReader myReader;
            if (((cmdCommand.Parameters.Contains("C_TABLE") || IsOracleReader(cmdCommand))) && typeExecute == enuTypeExecute.ExecuteReader)
            {
                myReader = cmdCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            else
            {
                cmdCommand.ExecuteNonQuery();
                ParamsCollectionResult = cmdCommand.Parameters;
                cmdCommand.Connection.Close();
                myReader = null;
            }
            return myReader;
        }

        protected  async Task<DbDataReader> ExecuteByStoredProcedureVTAsync(string nameStore,
              IEnumerable<DbParameter> parameters = null,
              enuTypeDataBase typeDataBase = enuTypeDataBase.OracleVTime,
              enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader)
        {
            DbConnection DataConnection = ConnectionGet(typeDataBase);
            DbCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }

            DataConnection.Open();
            DbDataReader myReader;
            if (((cmdCommand.Parameters.Contains("C_TABLE") || IsOracleReader(cmdCommand))) && typeExecute == enuTypeExecute.ExecuteReader)
            {
                myReader  =await cmdCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            else
            {
                await cmdCommand.ExecuteNonQueryAsync();
                ParamsCollectionResult = cmdCommand.Parameters;
                cmdCommand.Connection.Close();
                myReader = null;
            }
            return myReader;
        }

        protected DbDataReader ExecuteByStoredProcedureEX(string nameStore,
                IEnumerable<DbParameter> parameters = null,
                enuTypeDataBase typeDataBase = enuTypeDataBase.OracleVTime,
                enuTypeExecute typeExecute = enuTypeExecute.ExecuteReader)
        {
            DbConnection DataConnection = ConnectionGet(typeDataBase);
            DbCommand cmdCommand = DataConnection.CreateCommand();
            cmdCommand.CommandText = nameStore;
            cmdCommand.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    cmdCommand.Parameters.Add(parameter);
                }
            }

            DataConnection.Open();
            DbDataReader myReader;

            cmdCommand.ExecuteNonQuery();
            ParamsCollectionResult = cmdCommand.Parameters;
            cmdCommand.Connection.Close();
            myReader = null;

            return myReader;
        }

        private bool IsOracleReader(DbCommand cmdCommand)
        {
            bool isOracleReader = false;
            foreach (DbParameter item in cmdCommand.Parameters)
            {
                if (item is OracleParameter)
                {
                    if ((item as OracleParameter).OracleDbType == OracleDbType.RefCursor)
                    {
                        isOracleReader = true;
                        break;
                    }
                }
            }
            return isOracleReader;
        }
    }
}
