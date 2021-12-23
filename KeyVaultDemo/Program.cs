using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultDemoWithCertificate.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace KeyVaultDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                var builtConfig = builder.Build();
                var URL = builtConfig["KeyVault:Vault"];
                var TenantId = builtConfig["KeyVault:TenantId"];
                var ClientId = builtConfig["KeyVault:ClientId"];
                var ThumbPrint = builtConfig["KeyVault:ThumbPrint"];
                var Prefix = builtConfig["KeyVault:Prefix"];
                if (!string.IsNullOrEmpty(URL))
                {
                    var certificate = GetCertificate(ThumbPrint);

                    builder.AddAzureKeyVault(new Uri(URL),
                                            new ClientCertificateCredential(TenantId, ClientId, certificate.OfType<X509Certificate2>().Single()),
                                            new PrefixKeyVaultSecretManager(Prefix));
                }
            })
           
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             });

        private static X509Certificate2Collection GetCertificate(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                if (certificateCollection.Count == 0)
                    throw new Exception("Certificate is not installed"); //<------------- FAILS HERE

                return certificateCollection;
            }
            finally
            {
                store.Close();
            }
        }

    }
}
