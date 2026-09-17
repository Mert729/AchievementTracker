using AchievementTracker.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Çoklu dil sözlüklerinin nerede aranacaðýný belirtiyoruz (Birazdan bu klasörü açacaðýz)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Arayüz (View) ve DataAnnotations (Model uyarýlarý) için dil desteðini aktif ediyoruz
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Uygulamaya veritabaný adresimizi (ConnectionString) ve iletiþim kuracak köprüyü (DbContext) tanýtýyoruz.
builder.Services.AddDbContext<AchievementTrackerDB>(options =>  options.UseSqlServer
                   (builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

//Uygulama ilk ayaða kalktýðýnda veritabanýný kontrol edip, varsayýlan oyunlarýmýzý (Seed Data) ekler.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AchievementTrackerDB>();
    DbCreator.Init(context);
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseStaticFiles();

// Desteklenen dilleri belirliyoruz (Ýngilizce ve Türkçe)
var supportedCultures = new[] { "en-US", "tr-TR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US") // Sitenin varsayýlan açýlýþ dili
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Dil ayarlarýný boru hattýna (pipeline) ekliyoruz
app.UseRequestLocalization(localizationOptions);

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
