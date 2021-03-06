﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace A03.API
{
    /// <exclude />
    public class Program
    {
        /// <exclude />
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
