using Ixnas.AltchaNet;
using Ludero.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient<IEmailService, PostmarkEmailService>();
builder.Services.AddScoped<RazorViewRenderer>();
builder.Services.AddScoped<NewsService>();
builder.Services.Configure<PostmarkOptions>(builder.Configuration.GetSection("Postmark"));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAltchaChallengeStore, MemoryCaptchaStore>();
var selfHostedKeyBase64 = builder.Configuration.GetValue<string>("AltchaKey");
var selfHostedKey = Convert.FromBase64String(selfHostedKeyBase64!);
builder.Services.AddScoped(sp => Altcha.CreateService(new AltchaSha256Configuration
{
    Key = AltchaKey.FromBytes(selfHostedKey),
    StoreFactory = sp.GetRequiredService<IAltchaChallengeStore>,
    Expiry = AltchaExpiry.FromSeconds(900)
}));
builder.Services.AddScoped(_ => Altcha.CreateSolver());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
