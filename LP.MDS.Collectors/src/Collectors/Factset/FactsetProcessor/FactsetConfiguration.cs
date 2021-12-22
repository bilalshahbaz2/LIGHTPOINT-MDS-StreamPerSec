using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace gRPCFactset
{
    public class FactsetConfiguration
    {
        IConfiguration configuration;
        public string ContentRoot { get; private set; }
        public string SubscriptionConnectionString { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string ApiKeyUser { get; private set; }
        public string ApiKeyPassword { get; private set; }

        public String RTFieldFile { get; private set; }

        public FactsetConfiguration(IConfiguration _configuration)
        {
            configuration = _configuration;
            var Factset = configuration.GetSection("Factset");
            SubscriptionConnectionString = Factset["SubscriptionConnection"];
            Username = Factset.GetSection("ReferenceData").GetValue<string>("UserName");
            Password = Factset.GetSection("ReferenceData").GetValue<string>("Password");
            ApiKeyUser = Factset.GetSection("ApiKey").GetValue<string>("Username");
            ApiKeyPassword = Factset.GetSection("ApiKey").GetValue<string>("Password");

            string execPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            RTFieldFile = Path.Combine(execPath, "rt_fields.xml");
        }
    }
}
