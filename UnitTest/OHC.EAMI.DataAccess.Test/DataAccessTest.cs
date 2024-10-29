using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data.TestSupport;
using System.Collections.Generic;


namespace OHC.EAMI.DataAccess.Test
{
    [TestClass]
    public class DataAccessTest
    {
        //TESTS BELOW ARE NOT RELEVANT ANY LONGER
        //SHOULD BE REMOVED FROM AllTestSolution

        //string _connString = System.Configuration.ConfigurationManager.ConnectionStrings["EAMIData"].ConnectionString;

        //[TestMethod]
        //public void DataAccess_CreateSampleDatabase()
        //{
        //    //run this unit test ONCE to create a local version of database                        
        //    CreateSampleDatabase();

        //    //string connString = System.Configuration.ConfigurationManager.ConnectionStrings["EAMIData"].ConnectionString;

        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    using (DbConnection dbConnection =  db.CreateConnection())
        //    {
        //        dbConnection.Open();
        //        Assert.IsTrue(dbConnection.State == ConnectionState.Open);
        //        if (dbConnection.State == ConnectionState.Open)
        //        {
        //            dbConnection.Close();
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataSetSet_Select()
        //{
        //    //arrange
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    string sqlCommand = "SELECT RegionDescription FROM Region where regionId = @ID";
        //    DataSet ds = new DataSet();

        //    using (DbCommand command = db.GetSqlStringCommand(sqlCommand))
        //    {
        //        db.AddInParameter(command, "@ID", DbType.Int32, 4);

        //        //act
        //        using (DbConnection connection = db.CreateConnection())
        //        {
        //            connection.Open();
        //            using (DbTransaction transaction = connection.BeginTransaction())
        //            {
        //                db.LoadDataSet(command, ds, "Region", transaction);

        //                //assert
        //                Assert.AreEqual(1, ds.Tables[0].Rows.Count);
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataSet_Insert()
        //{
        //    //arrange
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    string sqlCommand = "SELECT RegionID, RegionDescription FROM Region WHERE RegionID > 90";
        //    DataSet ds = new DataSet();

        //    using (DbCommand command = db.GetSqlStringCommand(sqlCommand))
        //    {
        //        //act
        //        using (DbConnection connection = db.CreateConnection())
        //        {
        //            connection.Open();
        //            using (DbTransaction transaction = connection.BeginTransaction())
        //            {
        //                db.LoadDataSet(command, ds, "Region", transaction);

        //                DataTable dt = ds.Tables["Region"];
        //                object[] rowData = new object[] { 101, "Northwest" };
        //                dt.Rows.Add(rowData);

        //                //INSERT
        //                string addSQL = "INSERT INTO Region (RegionId, RegionDescription) VALUES (@RegionId, @RegionDescription)";
        //                DbCommand insertCommand = db.GetSqlStringCommand(addSQL);
        //                db.AddInParameter(insertCommand, "RegionDescription", DbType.String, "RegionDescription", DataRowVersion.Current);
        //                db.AddInParameter(insertCommand, "RegionId", DbType.String, "RegionId", DataRowVersion.Original);
        //                int rowsAffected = db.UpdateDataSet(ds, "Region", insertCommand, null, null, UpdateBehavior.Standard);

        //                //assert
        //                Assert.AreEqual(1, rowsAffected);
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataSet_Update()
        //{
        //    //arrange
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    string sqlCommand = "SELECT RegionID, RegionDescription FROM Region WHERE RegionID = 4";
        //    DataSet ds = new DataSet();

        //    using (DbCommand command = db.GetSqlStringCommand(sqlCommand))
        //    {
        //        //act
        //        using (DbConnection connection = db.CreateConnection())
        //        {
        //            connection.Open();
        //            using (DbTransaction transaction = connection.BeginTransaction())
        //            {
        //                db.LoadDataSet(command, ds, "Region", transaction);

        //                DataTable dt = ds.Tables["Region"];
        //                object[] rowData = rowData = dt.Rows[0].ItemArray;
        //                rowData[1] = "Southwest";
        //                dt.Rows[0].ItemArray = rowData;

        //                //UPDATE
        //                string updateSQL = "UPDATE Region SET RegionDescription = @RegionDescription WHERE RegionId = @RegionId";
        //                DbCommand updateCommand = db.GetSqlStringCommand(updateSQL);
        //                db.AddInParameter(updateCommand, "RegionDescription", DbType.String, "RegionDescription", DataRowVersion.Current);
        //                db.AddInParameter(updateCommand, "RegionId", DbType.String, "RegionId", DataRowVersion.Original);
        //                int rowsAffected = db.UpdateDataSet(ds, "Region", null, updateCommand, null, UpdateBehavior.Standard);

        //                //assert
        //                Assert.AreEqual(1, rowsAffected);
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataSet_Delete()
        //{
        //    //arrange
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    string selectSQL = "SELECT RegionID, RegionDescription FROM Region WHERE RegionID > 90";
        //    string deleteSQL = "DELETE FROM Region WHERE RegionId = @RegionId";

        //    DataSet ds = new DataSet();

        //    using (DbCommand command = db.GetSqlStringCommand(selectSQL))
        //    {
        //        //act
        //        using (DbConnection connection = db.CreateConnection())
        //        {
        //            connection.Open();
        //            using (DbTransaction transaction = connection.BeginTransaction())
        //            {
        //                db.LoadDataSet(command, ds, "Region", transaction);

        //                DataTable dt = ds.Tables["Region"];
        //                dt.Rows[0].Delete();

        //                //DELETE
        //                DbCommand deleteCommand = db.GetSqlStringCommand(deleteSQL);
        //                db.AddInParameter(deleteCommand, "RegionId", DbType.Int32, "RegionId", DataRowVersion.Original);
        //                int rowsAffected = db.UpdateDataSet(ds, "Region", null, null, deleteCommand, UpdateBehavior.Standard);

        //                //assert
        //                Assert.AreEqual(1, rowsAffected);
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_SQLString_WithTransaction()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    string insertionString = "insert into Region values (77, 'Elbonia')";
        //    string countString = "select count(*) from Region";
        //    string cleanupString = "delete from Region where RegionId = 77";

        //    using (DbCommand insertionCommand = db.GetSqlStringCommand(insertionString))
        //    {
        //        using (DbConnection connection = db.CreateConnection())
        //        {
        //            connection.Open();
        //            using (DbTransaction transaction = connection.BeginTransaction())
        //            {
        //                db.ExecuteNonQuery(insertionCommand, transaction);
        //                transaction.Commit();
        //            }
        //        }
        //    }

        //    using (DbCommand countCommand = db.GetSqlStringCommand(countString))
        //    {
        //        int count = Convert.ToInt32(db.ExecuteScalar(countCommand));
        //        //assert
        //        Assert.AreEqual(5, count);
        //    }

        //    using(DbCommand cleanupCommand = db.GetSqlStringCommand(cleanupString))
        //    {
        //        int rowsAffected = db.ExecuteNonQuery(cleanupCommand);
        //        //assert
        //        Assert.AreEqual(1, rowsAffected);
        //    }

        //}
        
        //[TestMethod]
        //public void DataAccess_DataReader_StoredProc_InTransaction()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    using (DbConnection connection = db.CreateConnection())
        //    {
        //        connection.Open();
        //        using (DbTransaction transaction = connection.BeginTransaction())
        //        {
        //            using (var reader = db.ExecuteReader(transaction, "Ten Most Expensive Products")) 
        //            {
        //                if (reader.Read())
        //                {
        //                    Assert.IsNotNull(reader[0]);
        //                }
        //            }
        //            transaction.Commit();
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataReader_StoredProc()
        //{
        //    string storedProc = "Ten Most Expensive Products";
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    using (DbConnection connection = db.CreateConnection())
        //    {
        //        connection.Open();
        //        using (var reader = db.ExecuteReader(storedProc))
        //        {
        //            if (reader.Read())
        //            {
        //                Assert.IsNotNull(reader[0]);
        //            }
        //        }
        //    }   
        //}

        //[TestMethod]
        //public void DataAccess_DataSet_StoredProc()
        //{
        //    string storedProc = "Ten Most Expensive Products";
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    using (DataSet dataSet = db.ExecuteDataSet(storedProc))
        //    {
        //        Assert.AreEqual(1, dataSet.Tables.Count);
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataSet_StoredProc_InTransaction()
        //{
        //    string storedProc = "Ten Most Expensive Products";

        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    using (DbConnection connection = db.CreateConnection())
        //    {
        //        connection.Open();
        //        using (DbTransaction transaction = connection.BeginTransaction())
        //        {
        //            using (DataSet dataSet = db.ExecuteDataSet(transaction, storedProc))
        //            {
        //                Assert.AreEqual(1, dataSet.Tables.Count);
        //            }
        //        }
        //    }
        //}

        //[TestMethod]
        //public void DataAccess_DataSet_SelectOverTwoTables()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    DataSet dataSet = new DataSet();

        //    string selectSql = "Select * from Region; Select * from Orders";
        //    string[] tableNames = new string[] { "RegionData", "OrderData" };

        //    using (DbCommand command = db.GetSqlStringCommand(selectSql))
        //    {
        //        db.LoadDataSet(command, dataSet, tableNames);
        //    }

        //    Assert.IsNotNull(dataSet.Tables["RegionData"]);
        //    Assert.IsNotNull(dataSet.Tables["OrderData"]);
        //}

        //[TestMethod]
        //public void DataAccess_InsertUsingTransaction()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    string insertionString = "insert into Region values (77, 'Elbonia')";
        //    string countString = "select count(*) from Region";

        //    using (DbCommand insertionCommand = db.GetSqlStringCommand(insertionString))
        //    {
        //        using (DbConnection connection = db.CreateConnection())
        //        {
        //            connection.Open();
        //            using (RollbackTransactionWrapper transaction = new RollbackTransactionWrapper(connection.BeginTransaction()))
        //            {
        //                int rowsAffected = db.ExecuteNonQuery(insertionCommand, transaction.Transaction);

        //                using (DbCommand countCommand = db.GetSqlStringCommand(countString))
        //                {
        //                    int count = Convert.ToInt32(db.ExecuteScalar(countCommand, transaction.Transaction));
        //                    Assert.IsNotNull(count);
        //                }
        //                //assert
        //                Assert.IsNotNull(rowsAffected);
        //            }
        //        }
        //    }
        //}
        
        //[TestMethod]
        //public void DataAccess_OneTransactionLocksOutAnother()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);
        //    DbCommand firstCommand = db.GetSqlStringCommand("insert into region values (99, 'Midwest')");
        //    DbCommand secondCommand = db.GetSqlStringCommand("Select * from Region");

        //    DbConnection connection1 = db.CreateConnection();
        //    connection1.Open();
        //    DbTransaction transaction1 = connection1.BeginTransaction(IsolationLevel.Serializable);

        //    DbConnection connection2 = db.CreateConnection();
        //    connection2.Open();
        //    DbTransaction transaction2 = connection2.BeginTransaction(IsolationLevel.Serializable);
        //    DataSet dataSet2 = new DataSet();
        //    secondCommand.CommandTimeout = 1;

        //    try
        //    {
        //        db.ExecuteNonQuery(firstCommand, transaction1);
        //        db.LoadDataSet(secondCommand, dataSet2, "Foo", transaction2);
        //        Assert.Fail("should have thrown some funky exception");
        //    }
        //    catch (SqlException) { }
        //    finally
        //    {
        //        transaction1.Rollback();
        //        transaction1.Dispose();
        //        transaction2.Dispose();
        //        connection1.Dispose();
        //        connection2.Dispose();
        //    }
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void DataAccess_NullTableNameArrayCausesException()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    string selectSql = "Select * from Region; Select * from Orders";

        //    DbCommand command = db.GetSqlStringCommand(selectSql);
        //    DataSet dataSet = new DataSet();
        //    db.LoadDataSet(command, dataSet, (string[])null);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void DataAccess_TableNameArrayWithNoEntriesCausesException()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    string selectSql = "Select * from Region; Select * from Orders";
        //    string[] tableNames = new string[0];

        //    DbCommand command = db.GetSqlStringCommand(selectSql);
        //    DataSet dataSet = new DataSet();
        //    db.LoadDataSet(command, dataSet, tableNames);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void DataAccess_NullTableNameInArrayCausesException()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    string selectSql = "Select * from Region; Select * from Orders";
        //    string[] tableNames = new string[] { "foo", null, "bar" };

        //    DbCommand command = db.GetSqlStringCommand(selectSql);
        //    DataSet dataSet = new DataSet();
        //    db.LoadDataSet(command, dataSet, tableNames);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void DataAccess_EmptyTableNameInArrayCausesException()
        //{
        //    Microsoft.Practices.EnterpriseLibrary.Data.Database db = new SqlDatabase(_connString);

        //    string selectSql = "Select * from Region; Select * from Orders";
        //    string[] tableNames = new string[] { "foo", "", "bar" };

        //    DbCommand command = db.GetSqlStringCommand(selectSql);
        //    DataSet dataSet = new DataSet();
        //    db.LoadDataSet(command, dataSet, tableNames);
        //}

        //private void CreateSampleDatabase()
        //{
        //    System.Diagnostics.Process process = new System.Diagnostics.Process();
        //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        //    startInfo.FileName = "sqlcmd.exe";
        //    string scriptPath = string.Format("{0}\\TestSupport\\instnwnd.sql", Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
        //    startInfo.Arguments = string.Format("-E -i {0}", scriptPath);            
        //    process.StartInfo = startInfo;
        //    process.Start();
        //    process.WaitForExit();
        //}

        //[TestMethod]
        //public void DataAccess_GetConfiguration()
        //{
        //    Assert.IsNotNull(System.Configuration.ConfigurationManager.ConnectionStrings["EAMIData"].ConnectionString);
        //}

        //[TestMethod]
        //public void DataAccess_TestDatabaseFactory_DataReader()
        //{
        //    Assert.IsNotNull(DataManager.GetRegions_DataReader());
        //}
    }
}
