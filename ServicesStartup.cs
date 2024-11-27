using GohMdlExpert.Models.GatesOfHell.Resources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GohMdlExpert {
    public static class ServicesStartup {
        public static void Startup(IServiceCollection services) {
            services.AddSingleton<GohResourceLoader>();
            services.AddSingleton<GohResourceLocations>();
        }
    }
}
