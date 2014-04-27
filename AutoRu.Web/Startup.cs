using System;
using System.Collections.Generic;
using System.Linq;
using AutoRu.Common;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AutoRu.Web.Startup))]

namespace AutoRu.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
