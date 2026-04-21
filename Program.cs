using System.Security.Claims;
using FutureTech.StudentManagement.Web.Options;
using FutureTech.StudentManagement.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Application Insights telemetry (connection string set via env var
// APPLICATIONINSIGHTS_CONNECTION_STRING in Azure App Service)
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
builder.Services.Configure<AdminAccessOptions>(builder.Configuration.GetSection(AdminAccessOptions.SectionName));
builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection(CosmosOptions.SectionName));
builder.Services.Configure<BlobStorageOptions>(builder.Configuration.GetSection(BlobStorageOptions.SectionName));

builder.Services.AddScoped<IAdminAccessService, AdminAccessService>();

var cosmosOptions = builder.Configuration
    .GetSection(CosmosOptions.SectionName)
    .Get<CosmosOptions>() ?? new CosmosOptions();

var useInMemoryRepository = builder.Environment.IsDevelopment()
    && (string.IsNullOrWhiteSpace(cosmosOptions.Endpoint)
        || string.IsNullOrWhiteSpace(cosmosOptions.Key)
        || cosmosOptions.Endpoint.Contains("your-cosmos-account", StringComparison.OrdinalIgnoreCase)
        || cosmosOptions.Key.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
        || cosmosOptions.Key.StartsWith("DEV_", StringComparison.OrdinalIgnoreCase));

var blobOptions = builder.Configuration
    .GetSection(BlobStorageOptions.SectionName)
    .Get<BlobStorageOptions>() ?? new BlobStorageOptions();

var useLocalImageStorage = builder.Environment.IsDevelopment()
    && (string.IsNullOrWhiteSpace(blobOptions.ConnectionString)
        || blobOptions.ConnectionString.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
        || blobOptions.ConnectionString.StartsWith("DEV_", StringComparison.OrdinalIgnoreCase)
        || blobOptions.ConnectionString.Contains("UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase));

if (useInMemoryRepository)
{
    // JsonFileStudentRepository persists to App_Data/students.json so data survives restarts
    builder.Services.AddSingleton<IStudentRepository, JsonFileStudentRepository>();
}
else
{
    builder.Services.AddScoped<IStudentRepository, CosmosStudentRepository>();
}

if (useLocalImageStorage)
{
    builder.Services.AddSingleton<IImageStorageService, LocalImageStorageService>();
}
else
{
    builder.Services.AddScoped<IImageStorageService, BlobImageStorageService>();
}

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ImageValidationService>();

var googleOptions = builder.Configuration
    .GetSection(GoogleAuthOptions.SectionName)
    .Get<GoogleAuthOptions>() ?? new GoogleAuthOptions();
var forceHttpsGoogleRedirectInDevelopment = builder.Environment.IsDevelopment();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "Google";
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    })
    .AddGoogle("Google", options =>
    {
        options.ClientId = googleOptions.ClientId;
        options.ClientSecret = googleOptions.ClientSecret;
        options.SaveTokens = true;
        if (!options.Scope.Any(scope => string.Equals(scope, "email", StringComparison.OrdinalIgnoreCase)))
        {
            options.Scope.Add("email");
        }

        options.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            if (forceHttpsGoogleRedirectInDevelopment)
            {
                var uri = new Uri(context.RedirectUri);
                var parsedQuery = QueryHelpers.ParseQuery(uri.Query);
                var queryBuilder = new QueryBuilder();

                foreach (var item in parsedQuery)
                {
                    if (string.Equals(item.Key, "redirect_uri", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    foreach (var value in item.Value)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            queryBuilder.Add(item.Key, value);
                        }
                    }
                }

                queryBuilder.Add("redirect_uri", "https://localhost:7159/signin-google");
                var redirectUrl = uri.GetLeftPart(UriPartial.Path) + queryBuilder.ToQueryString();
                context.Response.Redirect(redirectUrl);
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnCreatingTicket = context =>
        {
            var adminAccessService = context.HttpContext.RequestServices.GetRequiredService<IAdminAccessService>();
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email)
                ?? context.Identity?.FindFirst(ClaimTypes.Email)?.Value
                ?? context.Principal?.FindFirstValue("email")
                ?? context.Identity?.FindFirst("email")?.Value
                ?? context.Principal?.FindFirstValue("preferred_username")
                ?? context.Identity?.FindFirst("preferred_username")?.Value;

            if (adminAccessService.IsAdminEmail(email)
                && context.Identity is not null
                && !context.Identity.HasClaim(ClaimTypes.Role, "Admin"))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }

            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Security headers — OWASP best practice
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "default-src 'self'; "
        + "script-src 'self' 'unsafe-inline' 'unsafe-eval'; "
        + "style-src 'self' 'unsafe-inline'; "
        + "img-src 'self' data: https://futuretech*.blob.core.windows.net https://*.blob.core.windows.net; "
        + "font-src 'self' data:; "
        + "connect-src 'self'; "
        + "frame-ancestors 'none';");
    await next();
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
