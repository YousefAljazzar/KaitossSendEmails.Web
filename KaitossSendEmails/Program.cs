using Kaitoss;
using Kaitoss.DbModels.Models;
using Kaitoss.EmailService;
using Kaitoss.Implementation;

var builder = WebApplication.CreateBuilder(args);
var configer = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var emailConfig = configer.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddDbContext<KitoDbContext>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
