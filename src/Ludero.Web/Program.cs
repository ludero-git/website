using Ludero.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient<IEmailService, PostmarkEmailService>();
builder.Services.AddScoped<RazorViewRenderer>();
builder.Services.Configure<PostmarkOptions>(builder.Configuration.GetSection("Postmark"));

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
