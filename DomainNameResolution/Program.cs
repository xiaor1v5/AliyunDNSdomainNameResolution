using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DomainNameResolution
{
    class Program
    {
        static async Task Main()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSetting.json");

            var configuration = builder.Build();

            DomainRecordOptions domainRecordOptions = new DomainRecordOptions();

            configuration.GetSection("DomainRecordOptions").Bind(domainRecordOptions);

            DomainRecord domainRecord = new DomainRecord(domainRecordOptions);
            while (true)
            {
                await domainRecord.CheckAndModify();
                Thread.Sleep(30000);
            }
        }
    }
}
