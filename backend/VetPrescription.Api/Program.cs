using Microsoft.Extensions.FileProviders;
using Serilog;
using VetPrescription.Api.Features.Prescriptions.Create;
using VetPrescription.Api.Features.Prescriptions.GeneratePdf;
using VetPrescription.Api.Features.Prescriptions.GetById;
using VetPrescription.Api.Features.Prescriptions.GetPrescriptionPdfUrl;
using VetPrescription.Api.Features.Prescriptions.List;
using VetPrescription.Api.Features.Vets.GetVetProfile;
using VetPrescription.Api.Features.Vets.SaveVetProfile;
using VetPrescription.Api.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, config) =>
        config.ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddSingleton<MongoDbContext>();

    builder.Services.AddScoped<IListPrescriptionsRepository, ListPrescriptionsRepository>();
    builder.Services.AddScoped<ListPrescriptionsHandler>();

    builder.Services.AddScoped<ICreatePrescriptionRepository, CreatePrescriptionRepository>();
    builder.Services.AddScoped<CreatePrescriptionHandler>();

    builder.Services.AddScoped<IGetPrescriptionByIdRepository, GetPrescriptionByIdRepository>();
    builder.Services.AddScoped<GetPrescriptionByIdHandler>();

    builder.Services.AddScoped<IGeneratePdfRepository, GeneratePdfRepository>();
    builder.Services.AddScoped<GeneratePdfHandler>();

    builder.Services.AddScoped<IGetPrescriptionPdfUrlRepository, GetPrescriptionPdfUrlRepository>();
    builder.Services.AddScoped<GetPrescriptionPdfUrlHandler>();

    builder.Services.AddScoped<ISaveVetProfileRepository, SaveVetProfileRepository>();
    builder.Services.AddScoped<SaveVetProfileHandler>();

    builder.Services.AddScoped<IGetVetProfileRepository, GetVetProfileRepository>();
    builder.Services.AddScoped<GetVetProfileHandler>();

    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins(builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "VetPrescription API", Version = "v1" });
    });

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();
    app.UseCors();

    var pdfsPath = builder.Configuration["PdfsPath"] ?? "/app/pdfs";
    Directory.CreateDirectory(pdfsPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(pdfsPath),
        RequestPath = "/pdfs",
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    ListPrescriptionsEndpoint.Map(app);
    CreatePrescriptionEndpoint.Map(app);
    GetPrescriptionByIdEndpoint.Map(app);
    GeneratePdfEndpoint.Map(app);
    GetPrescriptionPdfUrlEndpoint.Map(app);
    SaveVetProfileEndpoint.Map(app);
    GetVetProfileEndpoint.Map(app);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
