# Dapper.DBContext
Dapper.DBContext is a orm library based on dapper.

# nuget install
Install-Package Dapper.DBContext

# QuickStart
```C#
public enum OrderStatus
{
     Create, WaitToPay, Paid
}
// entity
[Table("tb_Order")]
public class Order:IEntity
{
   // default primary key : rule is propertie name Id ,type is int 
    public int Id { get; set; }
    public string Code { get; set; }
    [Column("Total_Amount")]
    public decimal TotalAmount { get; set; }
    public int TotalQuantity { get; set; }
    public DateTime PaidTime { get; set; }
    public OrderStatus Status { get; set; }
    // foreign object 
    public List<OrderItem> list { get; set; }
    
    // row version,    auto add rowVersion = @rowVersion when update   // 
   //  public byte[] RowVersion { get; private set; }  // sqlserver
    // public DateTime RowVersion { get; private set; }  // mysql
}

public class OrderItem:IEntity
{
    [Key]   //  primary key
    public int Id { get; set; }
    puublic string ProductName {get;set;}
    public int Quantity { get; set; }
    public decmail Price { get; set; }
}
// query dto 
[table("tb_Order")]
public class OrderDto
{   
    public int Id { get; set; }
    puublic string OrderCode {get;set;}
    public DateTime PaidTime { get; set; }
    public decmail Price { get; set; }
}

```
# Detailed usage introduction
1.  web.config or app.config  connectionStrings
```XML
   ...
    <configuration>
  <connectionStrings>
    <add name="mssqldb" connectionString="database=db;server=.;uid=sa;pwd=111;" providerName="System.Data.SqlClient"/>
    <add name="mysqldb" connectionString="database=db;server=.;uid=sa;pwd=111;" providerName="MySql.Data.MySqlClient"/>
  </connectionStrings>
</configuration>
  ...    
```
2. create a instance of IDBContext
```C#
   IDBContext _db = new DapperDBContext("mssqldb");
```
3. Query a Entity
```C#
// query a row
var entity= _db.Table<Order>().FirstOrDefault(n => n.Id == 123);   //return order instance or null
var entity= _db.Table<Order>().FirstOrDefault(n => n.Code == 'ABC');   //return order instance or null
var count= _db.Table<Order>().Count(n => n.TotalQuantity > 1);      
var exists= _db.Table<Order>().Exists(n => n.Id == 123);   //return true or false

// Query list
var list =_db.Table<Order>().Where(n => n.Id == 123).ToList(); 
// Order By  asc
var list =_db.Table<Order>().Where(n => n.Id == 123).OrderBy(n => n.Id).ToList();  
// order by desc
var list =_db.Table<Order>().Where(n => n.Id == 123).OrderByDesc(n => n.Id).ToList();  

// select column 
  var list =_db.Table<Order>().Where(n => n.Id == 123).OrderByDesc(n => n.Id).Select(n => n.Code).ToList();  
 // sql = SELECT [Code] FROM [Order] Where Id =@P0 Order BY Id desc

 // select Multiple  columns 
  var list =_db.Table<OrderDto>().Where(n => n.Id == 123).OrderByDesc(n => n.Id).Select(n => new { n.Id, n.PaidTime }).ToList();  
 // sql = SELECT [Id] AS Id, [PaidTime] AS PaidTime FROM [Order] Where Id = @P0 Order BY Id desc

 // Column Alias
   var list =_db.Table<OrderDto>().Where(n => n.Id == 123).OrderByDesc(n=>n.Id).Select(n=> new { OrderId = 1, OrderCode = n.Code, n.Price }).ToList();  
 // sql = SELECT 1 AS OrderId, [Code] AS OrderCode,[Price] AS Price FROM [Order] Where Id =@P0 Order BY Id desc
 //  sql Query
 var list = _db.Table<OrderDto>.Query("select * from order where id = @id",new{ id > 123});
 var entity = _db.Table<OrderDto>.QuerySingle("select * from order where id = @id",new{ id = 123});

 // Dapper sql Query
 var list = _db.DataBase.Query<OrderDto>("select * from order where id = @id",new{id > 123});  // Same as Dapper
 var entity = _db.DataBase.QuerySingle<OrderDto>("select * from order where id = @id",new{ id = 123 }); 
```
4. Insert ,update ,delete , with transaction   just like EF
```C#
// one entity insert
   IDBContext _db = new DapperDBContext("mssqldb");
     Order model = new Order()
        {
            Code = "abc123",
            TotalAmount = 10.1f,
            PaidTime = DateTime.Now,
            Status = OrderState.Paid
        };
    _db.Insert<Order>(model);
    _db.SaveChange();   // realy begin to execute sql with transaction    
    var id = model.Id //   get AutoIncrement Id value
```
5 .Insert  Aggregate entity 
```C#
  IDBContext _db = new DapperDBContext("mssqldb");
     Order model = new Order()
        {
            Code = "abc123",
            TotalAmount = 10.1f,
            PaidTime = DateTime.Now,
            Status = OrderState.Paid
        };
        OrderItem item = new OrderItem()
            {           
                Price = 12.33m,
                ProductName = "haha",
                Quantity = 10
            };
            model.Items.Add(item);
    _db.Insert(model);
    _db.SaveChange();   
    // realy begin to execute sql with transaction  
    //  will be save Order and OrderItem
    var id = model.Id //   get AutoIncrement Id value
```
6.  update 
```C#
 // UPDATE ONE
  IDBContext _db = new DapperDBContext("mssqldb");
     Order model = _db.Table<Order>().FirstOrDefault(n=>n.Id == 123);
     model.Code ="ABC123";
    _db.Update(model);
    _db.SaveChange();   
    // realy begin to execute sql with transaction  
   
   // UPDATE LIST
    var rows = _db.Table<Order>().Where(n=>n.Id == 123).ToArray();
     rows.ForEach(n=>n.Code = "123");
    _db.Update(rows);
    _db.SaveChange();   
    
   // update some column with condition
    _db.Update(n=> new Order(){Code ="123",TotalAmount = TotalAmount+100},m=>m.Id = 123);
    _db.SaveChange();   
    
    // update all column with condition
     Order model = _db.Table<Order>().FirstOrDefault(n=>n.Id = 123);
     model.TotalAmount = 999;
     model.TotalQuantity=10;
    _db.Update(model,m=>m.TotalAmount < 100);
    _db.SaveChange();   
    
    
```
7. delete
```C#
 IDBContext _db = new DapperDBContext("mssqldb");
     Order model = _db.Table<Order>().FirstOrDefault(n=>n.Id == 123);  // Or  Order model = new Order(){Id = 123} 
    _db.Delete(model);
    _db.SaveChange();   
    // realy begin to execute sql with transaction  
   
   // delete LIST
    var rows = _db.Table<Order>().Where(n=>n.Id == 123).ToArray();
    _db.Delete(rows);
    _db.SaveChange();  
    
    // delete with condition    
     _db.Delete(n=>n.Id>10);
    _db.SaveChange();  
    
```
# Entity Attribute
1. TableAttribute   has a name parameter
```C#
   [Table("table_name")]
   public class Entity { }
```
2 .KeyAttribute   
   primary key rule: 1:  property name is   Id or ID , 2:  property has Key Attribute .
  
```C#  
   Inheriting ID primary key
   public class Entity:IEntity { 
      // Inheriting  id primary key and AutoIncrement
   }
   // or
    public class Entity:Entity<string> { 
                // Inheriting  id primary key
   }
   // or
    public class Entity { 
       public int Id    //  id primary key and AutoIncrement
   }
   //  or 
   public class Entity { 
      [key]
       public int entityId  
   }

```
3. ColumnAttribute                     
```C#
   [Table("table_name")]
   public class Entity {
     [Column("order_code")]
      public string Code {ge;set;}
   }
```
4 NotMappedAttribute   // not mapper to table column
