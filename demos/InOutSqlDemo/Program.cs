using Microsoft.Data.SqlClient;
using UkrGuru.Sql;

DbHelper.ConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=true";

// UkrGuru.Sql used @Data as default name for single parameter

var boolResult = DbHelper.Exec<bool>("SELECT @Data", true);
Console.WriteLine($"Boolean Result: {boolResult}");

var intResult = DbHelper.Exec<int>("SELECT @Data", 123);
Console.WriteLine($"Integer Result: {intResult}");

var stringResult = DbHelper.Exec<string>("SELECT @Data", "Hello, World!");
Console.WriteLine($"String Result: {stringResult}");

var fieldResult = DbHelper.Exec<string>("SELECT @Name", new { Name = "John" });
Console.WriteLine($"Field Result: {fieldResult}");

var dateResult = DbHelper.Exec<DateOnly>("SELECT @Data", DateTime.Today);
Console.WriteLine($"DateOnly Result: {dateResult}");

var timeResult = DbHelper.Exec<TimeOnly>("SELECT @Data", new TimeSpan(23, 59, 0));
Console.WriteLine($"TimeOnly Result: {timeResult}");

var decimalResult = DbHelper.Exec<decimal>("SELECT @Data", 123.45m);
Console.WriteLine($"Decimal Result: {decimalResult}");

var guidResult = DbHelper.Exec<Guid>("SELECT @Data", Guid.NewGuid());
Console.WriteLine($"Guid Result: {guidResult}");

var enumResult = DbHelper.Exec<UserType>("SELECT @Data", UserType.Admin);
Console.WriteLine($"Enum Result: {enumResult}");

var jsonObject = DbHelper.Exec<NamedType>("SELECT @Id Id, @Name Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER", new { Id = 1, Name = "Test" });
Console.WriteLine($"Json Object Result: Id = {jsonObject?.Id}, Name = {jsonObject?.Name}");

var namedObject = DbHelper.Read<NamedType>("SELECT @Id Id, @Name Name", new { Id = 1, Name = "Test" }).FirstOrDefault();
Console.WriteLine($"Named Object Result: Id = {namedObject?.Id}, Name = {namedObject?.Name}");

var sqlBooleanParam = new SqlParameter("@Data", true);
var booleanResult = DbHelper.Exec<bool>("SELECT @Data", sqlBooleanParam);
Console.WriteLine($"Sql Boolean Result: {booleanResult}");

var sqlNamedParameters = new SqlParameter[]
{
    new SqlParameter("@Id", 1),
    new SqlParameter("@Name", "John")
};
var sqlNamedResult = DbHelper.Read<NamedType>("SELECT @Id Id, @Name Name", sqlNamedParameters).FirstOrDefault(); 
Console.WriteLine($"Sql Named Result: Id = {sqlNamedResult?.Id}, Name = {sqlNamedResult?.Name}");

enum UserType
{
    Guest,
    User,
    Manager,
    Admin,
    SysAdmin
}

class NamedType
{
    public int Id { get; set; }
    public string? Name { get; set; }
}