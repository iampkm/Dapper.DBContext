using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.DBContext.Helper;
using Dapper.DBContext;
using Dapper.DBContext.Test.Domain;
namespace Dapper.DBContext.Test.Helper
{
    [TestClass]
    public class ReflectionServiceTest
    {
      
        [TestInitialize]
        public void Init()
        {
           
        }

        [TestMethod]
        public void IsForeignColumn_One_Mapping_More()
        {
            //Arrange
            Entity_Parent_Test entity = new Entity_Parent_Test();
            var pi = entity.GetType().GetProperty("Childs");
            //Action
            var actual = ReflectionHelper.IsForeignColumn(pi);
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsForeignColumn_One_Mapping_One()
        {
            //Arrange
            Entity_Parent_Test entity = new Entity_Parent_Test();
            var pi = entity.GetType().GetProperty("One");
            //Action
            var actual = ReflectionHelper.IsForeignColumn(pi);
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsNotMappedColumn_Test()
        {
            //Arrange
            Entity_Parent_Test entity = new Entity_Parent_Test();
            var pi = entity.GetType().GetProperty("NotMappedColumn");
            //Action
            var actual = ReflectionHelper.IsNotMappedColumn(pi);
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsAutoIncrementColumn_DefaultKeyName_IntType()
        {
            //Arrange
            Entity_Parent_Test entity = new Entity_Parent_Test();
            var pi = entity.GetType().GetProperty("Id");
            //Action
            var actual = ReflectionHelper.IsKeyAndAutoIncrement(pi);
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsAutoIncrementColumn_DefaultKeyName_LongType()
        {
            //Arrange
            Entity_AutoInCreament_longType entity = new Entity_AutoInCreament_longType();
            var pi = entity.GetType().GetProperty("Id");
            //Action
            var actual = ReflectionHelper.IsKeyAndAutoIncrement(pi);
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsAutoIncrementColumn_DefaultKeyName_stringType()
        {
            //Arrange
            Entity_Not_AutoInCreament_stringType entity = new Entity_Not_AutoInCreament_stringType();
            var pi = entity.GetType().GetProperty("Id");
            //Action
            var actual = ReflectionHelper.IsKeyAndAutoIncrement(pi);
            // Assert
            var expected = false;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsAutoIncrementColumn_KeyAttribute_intType()
        {
            //Arrange
            Entity_AutoInCreament_KeyAttribute_intType entity = new Entity_AutoInCreament_KeyAttribute_intType();
            var pi = entity.GetType().GetProperty("SysNo");
            //Action
            var actual = ReflectionHelper.IsKeyAndAutoIncrement(pi);
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsAutoIncrementColumn_KeyAttribute_intType_Not()
        {
            //Arrange
            Entity_Not_AutoInCreament_KeyAttribute_intType entity = new Entity_Not_AutoInCreament_KeyAttribute_intType();
            var pi = entity.GetType().GetProperty("SysNo");
            //Action
            var actual = ReflectionHelper.IsKeyAndAutoIncrement(pi);
            // Assert
            var expected = false;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsAutoIncrementColumn_KeyAttribute_stringType_Not()
        {
            //Arrange
            Entity_Not_AutoInCreament_KeyAttribute_stringType entity = new Entity_Not_AutoInCreament_KeyAttribute_stringType();
            var pi = entity.GetType().GetProperty("SysNo");
            //Action
            var actual = ReflectionHelper.IsKeyAndAutoIncrement(pi);
            // Assert
            var expected = false;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void IsAutoIncrement_Entity_Has()
        {
            //Arrange
            Entity_Parent_Test entity = new Entity_Parent_Test();
            //Action
            var actual = ReflectionHelper.ExistsAutoIncrementKey(entity.GetType());
            // Assert
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsAutoIncrement_Entity_Has_Not()
        {
            //Arrange
            Entity_Not_AutoInCreament_KeyAttribute_intType entity = new Entity_Not_AutoInCreament_KeyAttribute_intType();
            //Action
            var actual = ReflectionHelper.ExistsAutoIncrementKey(entity.GetType());
            // Assert
            var expected = false;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetPrimaryKey_Insert_Entity_then_Get_Id_Value()
        {
            //Arrange
            Entity_Parent_Test entity = new Entity_Parent_Test();
            //Action
            ReflectionHelper.SetPrimaryKey(entity,100);
            var actual = entity.Id;
            // Assert
            var expected = 100;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SetPrimaryKey_Insert_Entity_List_then_Get_Id_Value()
        {
            //Arrange
            IList<Entity_Parent_Test> entitys = new List<Entity_Parent_Test>();
            for (var i=0;i<3;i++)
            {
                entitys.Add(new Entity_Parent_Test());
            }
            //Action
            ReflectionHelper.SetPrimaryKey(entitys, 100);
            foreach (var entity in entitys)
            {
                var actual = entity.Id;
                // Assert
                var expected = 100;
                Assert.AreEqual(expected, actual);
            }            
        }
        [TestMethod]
        public void SetPrimaryKey_Insert_Entity_then_Not_Get_Id_Value()
        {
            //Arrange
            Entity_Not_AutoInCreament_KeyAttribute_intType entity = new Entity_Not_AutoInCreament_KeyAttribute_intType();
            //Action
            ReflectionHelper.SetPrimaryKey(entity, 100);
            var actual = entity.SysNo;
            // Assert           
            var expected = 0;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetPrimaryKey_Insert_Entity_then_Get_String_Id_Value()
        {
            //Arrange
            Entity_Not_AutoInCreament_KeyAttribute_stringType entity = new Entity_Not_AutoInCreament_KeyAttribute_stringType();
            //Action
            ReflectionHelper.SetPrimaryKey(entity, 100);
            var actual = entity.SysNo;
            // Assert
            Assert.IsNull(actual);
           // var expected = null;
           // Assert.AreEqual(expected, actual);
        }

    }
}
