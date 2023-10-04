using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace KeywordRepair
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            // get the section to read
            var configSection = configBuilder.GetSection("AppSettings");

            // get the configuration values in the section.
            string connectionString = configSection["connectionString"] ?? null;


            Stopwatch sw = Stopwatch.StartNew();
            Repair repair = new Repair();

            Fetcher.cstring = connectionString;

            DataTable dtRelease=repair.DoReleaseKeywords();
            DataTable dtSubject = repair.DoSubjectKeywords();
            DataTable dtProduct = repair.DoProductKeywords();

            Writer writer = new();
            writer.cstring = connectionString;

            writer.ClearKeywordRelease();
            writer.ClearKeywordSubject();
            writer.ClearKeywordProduct();

            writer.WriteKeywordRelease(dtRelease);

            writer.WriteKeywordSubject(dtSubject);
            writer.WriteKeywordProduct(dtProduct);


            sw.Stop();
            long seconds = sw.ElapsedMilliseconds / 1000;
            Console.WriteLine("End of process. Time taken: " + seconds + " seconds.");

        }
    }
}