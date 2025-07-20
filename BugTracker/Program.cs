using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using BugTracker.UnitOfWork;
using BugTracker.Hubs;
using BugTracker.Services;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Only use Azure Key Vault if VaultUri is provided (for production)
var vaultUri = Environment.GetEnvironmentVariable("VaultUri");
if (!string.IsNullOrEmpty(vaultUri))
{
    var keyVaultEndpoint = new Uri(vaultUri);
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = true,
        ExcludeInteractiveBrowserCredential = true,
        ExcludeAzurePowerShellCredential = true,
        ExcludeSharedTokenCacheCredential = true,
        ExcludeVisualStudioCodeCredential = true,
        ExcludeAzureCliCredential = true,
        ExcludeManagedIdentityCredential = builder.Environment.IsDevelopment(),
        ExcludeVisualStudioCredential = !builder.Environment.IsDevelopment()
    }));
}
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<BugTracker.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BugTracker.Data.ApplicationDbContext>();

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
// Google OAuth disabled for local development
// builder.Services.AddAuthentication().AddGoogle(googleOptions =>
// {
//     googleOptions.ClientId = builder.Configuration["Authentication-Google-ClientId"];
//     googleOptions.ClientSecret = builder.Configuration["Authentication-Google-ClientSecret"];
// });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
