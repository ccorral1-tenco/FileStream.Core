using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FileStream.Core
{
    /// <summary>
    /// This simple program exemplifies the use of filestream an a simple
    /// api call
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">The initialization arguments</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// Used by the ef core, this creates the host builder
        /// </summary>
        /// <param name="args">The initialization arguments</param>
        /// <returns><The built host builder/returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
