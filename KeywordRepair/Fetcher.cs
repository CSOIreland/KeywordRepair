using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace KeywordRepair
{
    internal static class Fetcher
    {
        internal static string cstring = "Server=10.7.4.41;Initial Catalog=pxstat.net.dev;User ID=pxstat;Password=m5_Mp4?w;Persist Security Info=False;";
        
        
        public static ILanguagePlugin ReadLanguageResource(string lngIsocode, string path, string namespaceClass, string translationUrl)
        {

            var dll = Assembly.LoadFile(path);
            var languageType = dll.GetType(namespaceClass);

            string translation;
            using (WebClient wc = new())
            {
                translation = wc.DownloadString(translationUrl);
            }

            dynamic languageClass = Activator.CreateInstance(languageType, translation);

            //Impromptu will convert the dynamic to an instance of ILanguagePlugin. Must be of the same shape!
            ILanguagePlugin lngPlugin = Impromptu.ActLike<ILanguagePlugin>(languageClass);

            return lngPlugin;
        }

        public static List<Matrix> GetMatrixMetadata()
        {
            List<Matrix> matrixList = new List<Matrix>();
            try
            {
                string sql = GetFileContents("KeywordRepair.Sql.Matrix.sql");

                using (SqlConnection connection = new SqlConnection(cstring))
                {
                    // Creating the command object
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    // Opening Connection  
                    connection.Open();
                    // Executing the SQL query  
                    SqlDataReader sdr = cmd.ExecuteReader();
                    //Looping through each record
                    while (sdr.Read())
                    {
                        matrixList.Add(new Matrix()
                        {
                            RlsId = (int)sdr["RLS_ID"],
                            CprCode = (string)sdr["CPR_CODE"],
                            CprValue = (string)sdr["CPR_VALUE"],
                            MtrCode = (string)sdr["MTR_CODE"],
                            MtrTitle = (string)sdr["MTR_TITLE"],
                            LngIsoCode = (string)sdr["LNG_ISO_CODE"]

                        });

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMatrixData: " + ex.Message);
            }

            return matrixList;
        }

        public static List<Dimension> GetDimensionMetadata()
        {
            List<Dimension> dimensionList = new List<Dimension>();
            try
            {
                string sql = GetFileContents("KeywordRepair.Sql.Dimension.sql");
                using (SqlConnection connection = new SqlConnection(cstring))
                {
                    // Creating the command object
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    // Opening Connection  
                    connection.Open();
                    // Executing the SQL query  
                    SqlDataReader sdr = cmd.ExecuteReader();
                    //Looping through each record
                    while (sdr.Read())
                    {
                        dimensionList.Add(new Dimension()
                        {
                            LngIsoCode = (string)sdr["LNG_ISO_CODE"],
                            MdmCode = (string)sdr["MDM_CODE"],
                            MdmId = (int)sdr["MDM_ID"],
                            MdmValue = (string)sdr["MDM_VALUE"],
                            RlsId = (int)sdr["RLS_ID"]
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("GetDimensionData: " + ex.Message);
            }
            return dimensionList;
        }

        public static List<Variable> GetVariableMetadata()
        {
            List <Variable> variableList = new List<Variable>();
            try
            {
                string sql = GetFileContents("KeywordRepair.Sql.Variable.sql");
                using (SqlConnection connection = new SqlConnection(cstring))
                {
                    // Creating the command object
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    // Opening Connection  
                    connection.Open();
                    // Executing the SQL query  
                    SqlDataReader sdr = cmd.ExecuteReader();
                    //Looping through each record
                    while (sdr.Read())
                    {
                        variableList.Add(new Variable() 
                        {
                            DmtCode = (string)sdr["DMT_CODE"],
                            DmtValue = (string)sdr["DMT_VALUE"],
                            LngIsoCode = (string)sdr["LNG_ISO_CODE"],
                            MdmId = (int)sdr["MDM_ID"],
                            RlsId = (int)sdr["RLS_ID"]
                        });
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("GetVariableData: " + ex.Message);
            }
            return variableList;
        }

        public static List<Subject> GetSubjectMetadata() 
        {
            List<Subject> subjectList = new List<Subject>();
            try
            {
                string sql = GetFileContents("KeywordRepair.Sql.Subject.sql");
                using (SqlConnection connection = new SqlConnection(cstring))
                {
                    // Creating the command object
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    // Opening Connection  
                    connection.Open();
                    // Executing the SQL query  
                    SqlDataReader sdr = cmd.ExecuteReader();
                    //Looping through each record
                    while (sdr.Read())
                    {
                        subjectList.Add(new Subject()
                        {
                            SbjId = (int)sdr["SBJ_ID"],
                            SbjValue = (string)sdr["SBJ_VALUE"],
                            LngIsoCode = (string)sdr["LNG_ISO_CODE"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSubjectData: " + ex.Message);
            }
            return subjectList;
        }

        public static List<Product> GetProductMetadata()
        {
            List<Product> productList = new List<Product>();
            try
            {
                string sql = GetFileContents("KeywordRepair.Sql.Product.sql");
                using (SqlConnection connection = new SqlConnection(cstring))
                {
                    // Creating the command object
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    // Opening Connection  
                    connection.Open();
                    // Executing the SQL query  
                    SqlDataReader sdr = cmd.ExecuteReader();
                    //Looping through each record
                    while (sdr.Read())
                    {
                        productList.Add(new Product() 
                        {
                            LngIsoCode = (string)sdr["LNG_ISO_CODE"],
                            PrcId = (int)sdr["PRC_ID"],
                            PrcValue = (string)sdr["PRC_VALUE"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetProductData: " + ex.Message);
            }
            return productList;
        }

        public static string GetFileContents(string path)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            string readValue;
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                readValue = reader.ReadToEnd();

            }
            return readValue;   
        }





        internal static List<Keyword> ExtractSplitSingular(int id, string readString, ILanguagePlugin language)
        {

            readString = language.Sanitize(readString);

            //We need to treat spaces and non-breaking spaces the same, so we replace any space with a standard space
            Regex rx = new Regex("[\\s]");
            readString = rx.Replace(readString, " ");

            // convert the sentance to a list of words
            List<string> wordListInput = (readString.Split(' ')).Where(x => x.Length > 0).ToList();

            //create an output list
            List<Keyword> wordList = new List<Keyword>();


            foreach (string word in wordListInput)
            {
                //trim white spaces
                string trimWord = Regex.Replace(word, @"^\s+", "");
                trimWord = Regex.Replace(trimWord, @"\s+$", "");

                if (trimWord.Length > 0)
                {

                    //if the word is not in our list of excluded words
                    if (!language.GetExcludedTerms().Contains(trimWord))
                    {

                        //if the word may be changed from singular to plural
                        if (!language.GetDoNotAmend().Contains(trimWord))
                        {
                            //get the singular version if it's singular
                            string wordRead = language.Singularize(trimWord);
                            if (!String.IsNullOrEmpty(wordRead))
                                wordList.Add(new Keyword() { Id = id, Value = wordRead.ToLower(), Mandatory = true, Singularised = true }) ;
                        }
                        else
                        {
                            //add the word to the output list, but not if it's in the list already
                            if (!String.IsNullOrEmpty(trimWord))
                                wordList.Add(new Keyword() { Id = id, Value = trimWord.ToLower(), Mandatory = true, Singularised = false });
                        }
                    }

                }
            }
            return wordList;
        }

        internal static List<Keyword> ExtractSplit(int id, string readString, ILanguagePlugin language, bool sanitize = true)
        {
            if (sanitize)
                readString = language.Sanitize(readString);

            //We need to treat spaces and non-breaking spaces the same, so we replace any space with a standard space
            Regex rx = new Regex("[\\s]");
            readString = rx.Replace(readString, " ");
            // convert the sentence to a list of words
            List<string> wordListInput = readString.Split(' ').ToList<string>();

            //create an output list
            List<Keyword> wordList = new List<Keyword>();

            foreach (string word in wordListInput)
            {
                //trim white spaces
                string trimWord = Regex.Replace(word, @"^\s+", "");
                trimWord = Regex.Replace(trimWord, @"\s+$", "");

                if (trimWord.Length > 0)
                {

                    //if the word is not in our list of excluded words
                    if (!language.GetExcludedTerms().Contains(trimWord))
                    {

                        if (!String.IsNullOrEmpty(trimWord))
                            wordList.Add(new Keyword() { Id = id, Value = trimWord.ToLower(), Mandatory = true, Singularised = false });
                    }

                }
            }

            return wordList;
        }


    }
}
