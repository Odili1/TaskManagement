using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

// Create a new web application builder with preconfigured defaults
var builder = WebApplication.CreateBuilder(args);

// Add services to the container - components that will be used by our application

// Add controller services to handle HTTP requests
builder.Services.AddControllers()
    // Configure JSON options to convert enums to strings in API responses
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register database context with Entity Framework Core
// This tells our app to use SQL Server with the provided connection string
builder.Services.AddDbContext<TaskManagementDbContext>(options => options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=task_managementDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

// Add API explorerservices to help generate OpenAPI/Swagger documents
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger generation for API documentation
builder.Services.AddSwaggerGen(options =>
{
    // Define the security scheme for JWT Bearer tokens in Swaggger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // The name of the header
        Type = SecuritySchemeType.Http, // The type of security scheme
        Scheme = "Bearer", // The scheme name
        In = ParameterLocation.Header, // Where the token is placed (header)
        Description = "JWT Authorization header that uses a bearer scheme. Enter token in the text below"
    });

    // Add a global security requirement that applies to all API endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Reference the security scheme defined above
                }
            },
            Array.Empty<string>() // No specific scopes required
        }
    });
});

// Configure Authentication services to use JWT Bearer tokens
builder.Services.AddAuthentication(options =>
{
    // Set the default authentication scheme to JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => // Add JWT Bearer token handling
{
    // COnfigure how to validate incoming JWT tokens
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validate the token issuer
        ValidateAudience = true, // Validate the token audience
        ValidateLifetime = true, // Check if the token is not expired
        ValidateIssuerSigningKey = true, // Validate the signing key
        ValidIssuer = builder.Configuration["Issuer"], // Get valid issuer from configuration
        ValidAudience = builder.Configuration["Audience"], // Get valid audience from configuratio
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Key"])), // Get signing key from configuration
        ClockSkew = TimeSpan.Zero // No tolerance for expiration time differences
    };
});


// COnfigure Authorization policies based on user roles
builder.Services.AddAuthorizationBuilder()
    // Create an "Admin" policy that requires the user to have the Admin role
    .AddPolicy("Admin", policy => policy.RequireRole(AccountType.Admin.ToString()))
    // Create a "User" policy that requires the user to have the User role
    .AddPolicy("User", policy => policy.RequireRole(AccountType.User.ToString()));


// Build the application using all the configured services
var app = builder.Build();

// Configure the HTTP request pipeline - this determins how requests are handled

// If we are in development environment, enable Swagger for API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Generate OpenAPI specification
    app.UseSwaggerUI(); // Serve the Swagger UI tool
}

// Redirect HTTP requests to HTTPS for security
app.UseHttpsRedirection();

// Enable authentication - this must come before authorization
app.UseAuthentication();

// Enable authorization - determines what authenticated users can access
app.UseAuthorization();

// Map Controller routes to handle incoming requests
app.MapControllers();

// Run the application - this starts the erver and begins listening for requests
app.Run();
