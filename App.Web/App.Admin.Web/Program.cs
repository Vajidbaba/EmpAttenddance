using Common.Core.Services;
using Common.Data.Context;
using Common.Data.Repositories;
using Common.Data.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using static Common.Core.Services.IEmployeeService;
using static Common.Core.Services.ILeaveService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LogisticContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequiredRole",
    policy => policy.RequireRole("Admin"));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    option => { option.LoginPath = "/Access/Login"; });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUsersService, UsresService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<ISalariesService, SalariesService>();
builder.Services.AddScoped<IOvertimeRecordsService, OvertimeRecordsService>();
builder.Services.AddScoped<IMasterOvertimeService, MasterOvertimeService>();
builder.Services.AddScoped<IMasterDepartmentService, MasterDepartmentService>();
builder.Services.AddScoped<IHolidayService, HolidayService>();



builder.Services.AddSingleton<IContextHelper, ContextHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Dashboard}/{action=List}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Login}/{id?}");
});


app.Run();
