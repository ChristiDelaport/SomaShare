using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomaShare.Components;
using SomaShare.Components.Model;
using SomaShare.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register a scoped service to store user session data
builder.Services.AddScoped<UserSession>();

// Add support for mvc controllers and views 
builder.Services.AddControllersWithViews();

// Register DbContext configure SQL Server connection
builder.Services.AddDbContext<SomaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

// Register DbContext Factory
builder.Services.AddScoped<IDbContextFactory<SomaContext>>(provider =>
{
    var options = provider.GetRequiredService<DbContextOptions<SomaContext>>();
    return (IDbContextFactory<SomaContext>)new DesignTimeDbContextFactory(options);
});

// Register custom application services
builder.Services.AddScoped<IListingService, ListingService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IWantedAdService, WantedAdService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<RoleSeederService>();
builder.Services.AddScoped<UserSession>();

// Add SignalR for real-time communication (chat)
builder.Services.AddSignalR();

// Add authentication and authorization services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Enables authentication state to flow through blazor component
builder.Services.AddCascadingAuthenticationState();

// Add auth and authorization services
builder.Services.AddScoped<FormValidator>();

// Service used to seed roles into the database
builder.Services.AddScoped<RoleSeederService>();

// Configure ASP.NET Identity (user management system)
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    // Lockout settings
    options.Lockout.AllowedForNewUsers = true;
    // Require unique email for each user
    options.User.RequireUniqueEmail = true;
})
    // Use Entity Framework for storing identity data
    .AddEntityFrameworkStores<SomaContext>()
    .AddDefaultTokenProviders();

// Configure session settings (for temp user data)
builder.Services.AddSession(options =>
{
    // Session expires after 30 minutes of inactivity
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Configure auth cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    // Redirect to this path if user is not logged in
    options.LoginPath = "/";
    // Redirect here if user is not authorized
    options.AccessDeniedPath = "/access-denied";
    // Logout endpoint
    options.LogoutPath = "/logout";
    // Extend cookie expiration on activity
    options.SlidingExpiration = true;
    // Cookie expires after 7 days
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Global error handler for production
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Seed roles before running the application
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeederService>();
    await roleSeeder.SeedRolesAsync(); // Creates Admin, Seller, Buyer roles if they don't exist
}

app.Run();