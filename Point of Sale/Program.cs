using pos.Infrastructure;
using pos.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/EnterPin", "");
});

builder.Services.AddScoped<DataAccessHelper>();
builder.Services.AddScoped<SaveCategory>();
builder.Services.AddScoped<SaveProductRep>();
builder.Services.AddScoped<SaveOrderRep>();
builder.Services.AddScoped<GetAllListRep>();
builder.Services.AddScoped<GetInventoryRep>();
builder.Services.AddScoped<GetReportRep>();
builder.Services.AddScoped<PinCodeRep>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthorization();
app.MapRazorPages();
app.Run();
