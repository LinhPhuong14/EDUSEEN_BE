using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Sep490_Eduseen_BE.Extensions
{
    public static class StaticFileExtensions
    {
        public static IApplicationBuilder UseStaticUploads(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var uploadPath = Path.Combine(env.ContentRootPath, "Uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            return app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadPath),
                RequestPath = "/uploads"
            });
        }
    }
}
