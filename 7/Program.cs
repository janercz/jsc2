using EShopEf.Data;
using EShopEf.Services;

using (var ctx = new AppDbContext())
{
    ctx.Database.EnsureDeleted();
    
    ctx.Database.EnsureCreated(); 
}

var service = new EShopService();

service.CreateData();
service.ReadData();
service.UpdateData();
service.PrintCustomerStats();
service.DeleteData();