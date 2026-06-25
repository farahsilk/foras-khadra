using ForasKhadra.API.Services;
using ForasKhadra.API.Services.Providers;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor views (UI) and API controllers (JSON endpoints) live together.
builder.Services.AddControllersWithViews();

// DataService is a singleton: load the JSON once and keep it in memory.
builder.Services.AddSingleton<DataService>();

// Each provider gets its own typed HttpClient (distinct named client + config).
builder.Services.AddHttpClient<ClaudeProvider>(c => c.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddHttpClient<OpenAIProvider>(c => c.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddHttpClient<GeminiProvider>(c => c.Timeout = TimeSpan.FromSeconds(60));

// Expose all three as ILlmProvider so ChatService receives them via IEnumerable<>.
builder.Services.AddTransient<ILlmProvider>(sp => sp.GetRequiredService<ClaudeProvider>());
builder.Services.AddTransient<ILlmProvider>(sp => sp.GetRequiredService<OpenAIProvider>());
builder.Services.AddTransient<ILlmProvider>(sp => sp.GetRequiredService<GeminiProvider>());

builder.Services.AddScoped<ChatService>();

var app = builder.Build();

// Force the data to load at startup so problems surface immediately.
app.Services.GetRequiredService<DataService>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Map both the MVC page routes and the [ApiController] attribute routes.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
