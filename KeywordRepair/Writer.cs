using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeywordRepair
{
    internal class Writer
    {
        public string cstring;

        string krlTable = "TD_KEYWORD_RELEASE";
        string sbjTable = "TD_KEYWORD_SUBJECT";
        string prcTable = "TD_KEYWORD_PRODUCT";
        internal Writer()
        {

        }

        internal void ClearKeywordRelease()
        {
            string sql = "delete from " + krlTable + " where krl_mandatory_flag=1";
            try
            {
                using (SqlConnection connection = new SqlConnection(cstring))
                {
                    
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.CommandTimeout = 0;
                    connection.Open();
                    int counter = cmd.ExecuteNonQuery();
                    Console.WriteLine("KeywordRelease " + counter + " rows deleted");
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Clear KeywordRelease error: " + ex.Message);
            }
        }

        internal void ClearKeywordSubject()
        {
            string sql = "delete from " + sbjTable + " where ksb_mandatory_flag=1";
            try
            {
                using (SqlConnection connection = new SqlConnection(cstring))
                {

                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.CommandTimeout = 0;
                    connection.Open();
                    int counter = cmd.ExecuteNonQuery();
                    Console.WriteLine("KeywordSubject " + counter + " rows deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Clear KeywordSubject error: " + ex.Message);
            }
        }

        internal void ClearKeywordProduct()
        {
            string sql = "delete from " + prcTable + " where kpr_mandatory_flag=1";
            try
            {
                using (SqlConnection connection = new SqlConnection(cstring))
                {

                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.CommandTimeout = 0;
                    connection.Open();
                    int counter = cmd.ExecuteNonQuery();
                    Console.WriteLine("KeywordProduct " + counter + " rows deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Clear KeywordProduct error: " + ex.Message);
            }
        }

        internal void WriteKeywordRelease(DataTable dtKeywordRelease)
        {
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cstring))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.BatchSize = 0;
                    bulkCopy.DestinationTableName = krlTable;
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("RlsId", "KRL_RLS_ID"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("KrlValue", "KRL_VALUE"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Mandatory", "KRL_MANDATORY_FLAG"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Singularised", "KRL_SINGULARISED_FLAG"));

                    bulkCopy.WriteToServer(dtKeywordRelease);
                    Console.WriteLine("WriteKeywordRelease bulk copy complete");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("WriteKeywordRelease bulk copy error:" + ex.Message);
            }
        }

        internal void WriteKeywordSubject(DataTable dtKeywordSubject) 
        {
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cstring))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.BatchSize = 0;
                    bulkCopy.DestinationTableName = sbjTable;
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SbjId", "KSB_SBJ_ID"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("SbjValue", "KSB_VALUE"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Mandatory", "KSB_MANDATORY_FLAG"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Singularised", "KSB_SINGULARISED_FLAG"));

                    bulkCopy.WriteToServer(dtKeywordSubject);
                    Console.WriteLine("WriteKeywordSubject bulk copy complete");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WriteKeywordSubject bulk copy error:" + ex.Message);
            }
        }

        internal void WriteKeywordProduct(DataTable dtKeywordProduct) 
        {
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(cstring))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.BatchSize = 0;
                    bulkCopy.DestinationTableName = prcTable;
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("PrcId", "KPR_PRC_ID"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("PrcValue", "KPR_VALUE"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Mandatory", "KPR_MANDATORY_FLAG"));
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("Singularised", "KPR_SINGULARISED_FLAG"));

                    bulkCopy.WriteToServer(dtKeywordProduct);
                    Console.WriteLine("WriteKeywordProduct bulk copy complete");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WriteKeywordProduct bulk copy error:" + ex.Message);
            }

        } 
        

    }
}
