using Dynamitey;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordRepair
{

    internal class Repair
    {
        Dictionary<string, ILanguagePlugin> languages = new();

        //https://cdn.jsdelivr.net/gh/CSOIreland/PxLanguagePlugins@2.2.0/server/src/en/PxLanguagePlugin/Resources/language.json
        //https://cdn.jsdelivr.net/gh/CSOIreland/PxLanguagePlugins@2.2.0/server/src/ga/PxLanguagePlugin/Resources/language.json

        internal Repair()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var configSection = configBuilder.GetSection("AppSettings");

            // get the configuration values in the section.
            //string enLocation = configSection["pluginPathEn"] ?? null;
            var sections = configBuilder.GetSection("AppSettings:plugins").GetChildren();
            foreach (var item in sections)
            {
                string? lngIsoCode = item.GetSection("lngIsoCode").Value;
                string? location = item.GetSection("location").Value;
                string? translation = item.GetSection("translationUrl").Value;
                if(lngIsoCode != null && location != null)
                {
                    ILanguagePlugin lng = Fetcher.ReadLanguageResource(lngIsoCode, location, "PxLanguagePlugin." + lngIsoCode + ".Language", translation??"");
                    languages.Add(lngIsoCode, lng);
                }
            }
            


        }


        internal DataTable DoReleaseKeywords()
        {
            List<Matrix> matrixes = Fetcher.GetMatrixMetadata();
            List<Dimension> dimensions = Fetcher.GetDimensionMetadata();
            List<Variable> variables = Fetcher.GetVariableMetadata();
            DataTable dtKrl = new();
            List<Keyword> releaseKeywords = new();

            dtKrl.Columns.Add("RlsId", typeof(int));
            dtKrl.Columns.Add("KrlValue", typeof(string));
            dtKrl.Columns.Add("Mandatory", typeof(bool));
            dtKrl.Columns.Add("Singularised", typeof(bool));

            Console.WriteLine("Processing matrix data...");
            int counter = 0;
            foreach (Matrix matrix in matrixes)
            {
                
                releaseKeywords.AddRange(Fetcher.ExtractSplitSingular(matrix.RlsId, matrix.MtrTitle, languages[matrix.LngIsoCode]));
                releaseKeywords.Add(new Keyword() { Id = matrix.RlsId, Value = matrix.MtrCode.ToLower(), Mandatory = true, Singularised = false });
                releaseKeywords.AddRange(Fetcher.ExtractSplit(matrix.RlsId, matrix.CprCode, languages[matrix.LngIsoCode]));
                releaseKeywords.AddRange(Fetcher.ExtractSplit(matrix.RlsId, matrix.CprValue, languages[matrix.LngIsoCode]));
                counter++;
            }
            Console.WriteLine("Matrix data: " + counter + " items processed");

            Console.WriteLine("Processing dimension data...");
            counter = 0;
            foreach (Dimension dimension in dimensions)
            {
                releaseKeywords.AddRange(Fetcher.ExtractSplit(dimension.RlsId, dimension.MdmCode, languages[dimension.LngIsoCode]));
                releaseKeywords.AddRange(Fetcher.ExtractSplitSingular(dimension.RlsId, dimension.MdmValue, languages[dimension.LngIsoCode]));
                counter++;
            }
            Console.WriteLine("Dimension data: " + counter + " items processed");

            Console.WriteLine("Processing variable data...");
            counter = 0;
            foreach (Variable variable in variables)
            {
                releaseKeywords.AddRange(Fetcher.ExtractSplit(variable.RlsId, variable.DmtCode, languages[variable.LngIsoCode]));
                releaseKeywords.AddRange(Fetcher.ExtractSplitSingular(variable.RlsId, variable.DmtValue, languages[variable.LngIsoCode]));
                counter++;
            }

            Console.WriteLine("Variable data: " + counter + " items processed");

            var grpResult =
                from rk in releaseKeywords
                group rk by new { rk.Id, rk.Value, rk.Singularised } into g
                select new
                {
                    RlsId = g.Key.Id,
                    KrlValue = g.Key.Value,
                    Singularised = g.Key.Singularised

                };
            foreach (var item in grpResult)
            {
                DataRow dr = dtKrl.NewRow();
                dr["RlsId"] = item.RlsId;
                dr["KrlValue"] = item.KrlValue;
                dr["Mandatory"] = true;
                dr["Singularised"] = item.Singularised;
                dtKrl.Rows.Add(dr);
            }

            return dtKrl;

        }




        internal DataTable DoSubjectKeywords()
        {
            List<Subject> subjects = Fetcher.GetSubjectMetadata();
            DataTable dtSbj = new();
            List<Keyword> subjectKeywords = new();

            dtSbj.Columns.Add("SbjId", typeof(int));
            dtSbj.Columns.Add("SbjValue", typeof(string));
            dtSbj.Columns.Add("Mandatory", typeof(bool));
            dtSbj.Columns.Add("Singularised", typeof(bool));

            Console.WriteLine("Processing subject data...");
            int counter = 0;
            foreach (Subject subject in subjects)
            {
                subjectKeywords.AddRange(Fetcher.ExtractSplitSingular(subject.SbjId,subject.SbjValue, languages[subject.LngIsoCode]));
                counter++;
            }
            Console.WriteLine("Subject data: " + counter + " items processed");

            var grpResult =
                from rk in subjectKeywords
                group rk by new { rk.Id, rk.Value, rk.Singularised } into g
                select new
                {
                    Id = g.Key.Id,
                    Value = g.Key.Value,
                    Singularised = g.Key.Singularised

                };

            foreach (var item in grpResult)
            {
                
                DataRow dr = dtSbj.NewRow();
                dr["SbjId"] = item.Id;
                dr["SbjValue"] = item.Value;
                dr["Mandatory"] = true;
                dr["Singularised"] = item.Singularised;
                dtSbj.Rows.Add(dr);
                
            }
            

            Console.WriteLine("Subject Keywords created for " + subjects.Count + " subjects.");
            Console.WriteLine("Total unique Subject Keywords created: " + dtSbj.Rows.Count);
            return dtSbj;
        }

        internal DataTable DoProductKeywords()
        {
            List<Product> products = Fetcher.GetProductMetadata();
            DataTable dtPrc = new();
            List<Keyword> productKeywords = new();

            dtPrc.Columns.Add("PrcId", typeof(int));
            dtPrc.Columns.Add("PrcValue", typeof(string));
            dtPrc.Columns.Add("Mandatory", typeof(bool));
            dtPrc.Columns.Add("Singularised", typeof(bool));

            Console.WriteLine("Processing product data...");
            int counter = 0;
            foreach (Product product in products)
            {
                productKeywords.AddRange(Fetcher.ExtractSplitSingular(product.PrcId,product.PrcValue, languages[product.LngIsoCode]));
                counter++;
            }
            Console.WriteLine("Product data: " + counter + " items processed");

            var grpResult =
                from rk in productKeywords
                group rk by new { rk.Id, rk.Value, rk.Singularised } into g
                select new
                {
                    Id = g.Key.Id,
                    Value = g.Key.Value,
                    Singularised = g.Key.Singularised

                };

            foreach (var item in grpResult)
            {
                
                DataRow dr = dtPrc.NewRow();
                dr["PrcId"] = item.Id;
                dr["PrcValue"] = item.Value;
                dr["Mandatory"] = true;
                dr["Singularised"]= item.Singularised;
                dtPrc.Rows.Add(dr);
                
            }

            Console.WriteLine("Product Keywords created for " + products.Count + " products.");
            Console.WriteLine("Total unique Product Keywords created: " + dtPrc.Rows.Count);

            return dtPrc;
        }

    }



    internal class Matrix
    {
        internal int RlsId { get; set; }
        internal string MtrCode { get; set; }
        internal string MtrTitle { get; set; }
        internal string CprCode { get; set; }
        internal string CprValue { get; set; }
        internal string LngIsoCode { get; set; }
    }

    internal class Dimension
    {
        internal int RlsId { get; set; }
        internal int MdmId { get; set; }
        internal string MdmCode { get; set; }
        internal string MdmValue { get; set; }
        internal string LngIsoCode { get; set; }
    }

    internal class Variable
    {
        internal int RlsId { get; set; }
        internal int MdmId { get; set; }
        internal string LngIsoCode { get; set; }
        internal string DmtCode { get; set; }
        internal string DmtValue { get; set; }

    }

    internal class Subject
    {
        internal int SbjId { get; set; }
        internal string SbjValue { get; set; }
        internal string LngIsoCode { get; set; }
    }

    internal class Product
    {
        internal int PrcId { get; set; }
        internal string PrcValue { get; set; }
        internal string LngIsoCode { get; set; }
    }

    internal class Keyword
    {
        internal int Id { get; set; }
        internal string Value { get; set; }
        internal bool Mandatory { get; set; }
        internal bool Singularised { get; set; }
    }



}
