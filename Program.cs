using Microsoft.EntityFrameworkCore;
using TobimSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// ربط قاعدة البيانات (SQLite أو SQL Server)
builder.Services.AddDbContext<TobimDbContext>(options =>
    options.UseSqlite("Data Source=tobim.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// تشغيل الواجهة التفاعلية الرئيسية (التي تم إنشاؤها مسبقاً)
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync("<!DOCTYPE html>... نظام TOBIM يعمل بنجاح ...");
});

app.Run();
