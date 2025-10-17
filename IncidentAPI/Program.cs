using Incident.Application.Interfaces;
using Incident.Application.Services;
using Incident.Infrastructure.Repositories;
 
var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
 
var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();