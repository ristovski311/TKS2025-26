using Backend.Repositories;
using Backend.Services;
using Supabase;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var supabaseUrl = builder.Configuration["Supabase:Url"]
    ?? throw new InvalidOperationException("Supabase URL is not configured. Please add it to User Secrets or appsettings.json");
var supabaseKey = builder.Configuration["Supabase:Key"]
    ?? throw new InvalidOperationException("Supabase Key is not configured. Please add it to User Secrets or appsettings.json");

var options = new SupabaseOptions
{
    AutoConnectRealtime = true
};

builder.Services.AddScoped(_ =>
    new Client(supabaseUrl, supabaseKey, options));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProfessorRepository>();
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<TaskRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var supabaseClient = scope.ServiceProvider.GetRequiredService<Client>();
    await supabaseClient.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
