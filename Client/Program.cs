using MeepleSystemClient.Services;
using Microsoft.AspNetCore.Authentication.Cookies; // Required for cookie-based authentication

// This Initializes the app set up (The Configuration Phase)
// Reads the appsettings.json file and prepares dependency injection, logging, envioronemt and other things
var builder = WebApplication.CreateBuilder(args);

// This Service Enables Razor Pages (Required for the .cshtml files to work)
builder.Services.AddRazorPages();

// This adds temporary storage in memory (used by session and caching data)
builder.Services.AddDistributedMemoryCache();

// Session options for timout and cookie settings
builder.Services.AddSession(options =>
{
    // Session times out after 30 minutes
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Keeps the sesssion http accessable only (not by JavaScript)
    options.Cookie.HttpOnly = true;

    // Ensures required cookies are sent
    options.Cookie.IsEssential = true;
});


// ========================= AUTHENTICATION SETUP =========================

// This sets up cookie-based authentication for login sessions
// The cookie will store the user's identity after they log in
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        // If a user tries to access a protected page without logging in,
        // they will be redirected to this page
        options.LoginPath = "/Login";
        options.Events.OnRedirectToLogin = context =>
        {
            // Prevent redirect loop if already on login page
            if (context.Request.Path.StartsWithSegments("/Login"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };

        // If access is denied (ex: roles later), redirect here
        options.AccessDeniedPath = "/Login";
    });

// This enables authorization rules across the app
builder.Services.AddAuthorization();

// ========================= API CONFIGURATION =========================

// This tries to read config and falls back to default if not found
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? "https://localhost:7093/";

// Creates a HttpClient for GameService and its interface
builder.Services.AddHttpClient<IGameService, GameService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Creates a HttpClient for AuthService and its interface
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Creates a HttpClient for UserService and its interface
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


// Finishes the configuration and builds the app (The Build Phase)
var app = builder.Build();


// ========================= HTTP PIPELINE =========================

// This configures the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// This forces the http to redirect to https
app.UseHttpsRedirection();

// Allows access to static files (css, js, images, pretty much anything in wwwroot folder)
app.UseStaticFiles();

// Enables Routing (Matches URLs (/page) to pages (page.cshtml))
app.UseRouting();

// Session Support (Allows the Authorization session to work)
app.UseSession();


// ========================= AUTH MIDDLEWARE =========================

// This checks if a user is logged in and reads their authentication cookie
// MUST come before UseAuthorization
app.UseAuthentication();

// Creates User based Access (Checks if the user is allowed to access a page)
app.UseAuthorization();


// Connects URLs to Razor Pages (Allows the .cshtml files to be served when requested)
app.MapRazorPages();

// Runs the app (The Run Phase)
app.Run();