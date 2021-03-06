﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Dapper.DBContext.Test.Domain;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
namespace Dapper.DBContext.Test.Builder
{
     [TestClass]
   public class RowVersionTest
    {
         [TestMethod]
         public void TestTimespan()
         {
             SqlConnection conn = new SqlConnection("server=.;uid=sa;pwd=123456;database=temp2");
             SqlDataAdapter adapter = new SqlDataAdapter("select * from tb_time", conn);
             DataTable dt = new DataTable();
             adapter.Fill(dt);

           
             DataRow row = dt.Rows[0];
             SqlRowVersion_Test model = new SqlRowVersion_Test((int)row[0], (string)row[1]);
             //model.Id = (int)row[0];
             //model.Code = (string)row[1];
           //  model.RowVersion = (byte[])row[2];

             SqlCommand cmd = new SqlCommand("update tb_time set code='tttt' where Id=@Id and RowVersion=@RowVersion", conn);
             SqlParameter id = new SqlParameter("Id", 1);
             SqlParameter rowVersion = new SqlParameter("RowVersion", model.RowVersion);

             cmd.Parameters.Add(id);
             cmd.Parameters.Add(rowVersion);
             conn.Open();
             cmd.ExecuteNonQuery();
             conn.Close();

             Assert.AreEqual("tttt", "tttt");
         }
         [TestMethod]
         public void TestTimespan_MySql()
         {
             var strConn = ConfigurationManager.ConnectionStrings["ebsdb"].ConnectionString;

             DapperDBContext db = new DapperDBContext("ebsdb");
             var model = db.Table<temp_test>().FirstOrDefault(n=>n.Id == 1);

             var version1 = model.RowVersion;
             model.Quantity += 100;
             db.Update(model);
             db.SaveChange();
             var model2 = db.Table<temp_test>().FirstOrDefault(n => n.Id == 1);
              Assert.AreNotEqual(version1, model2.RowVersion);
         }
          [TestMethod]
         public void TestPrivatePeroperties()
         {
             IDbConnection conn = new SqlConnection("server=.;uid=sa;pwd=123456;database=temp2");
             var model= conn.Query<SqlRowVersion_Test>("select top 1 * from tb_time").FirstOrDefault();
             Assert.AreEqual(1, model.Id);
             Assert.AreEqual("tttt", model.Code);
            // Assert.AreEqual(1, model.Id);

         }
    }
}
