using APBD_3.Data;
using APBD_3.Repositories;
using APBD_3.Services;

namespace APBD_3;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        
        //Register of Connection for SQL Factory - why not? :D
        builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        //DB related
        builder.Services.AddScoped<ITripsRepository, TripsRepository>();
        builder.Services.AddScoped<ITripsService, TripsService>();

        //DB related
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IClientService, ClientService>();
        
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        //app.UseHttpsRedirection(); oh really??

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}