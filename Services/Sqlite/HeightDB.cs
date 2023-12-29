using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Reflection.PortableExecutable;
using TamakenService.Models.TextFileFilter;
using System.Reflection;
using Dapper;
using System.Security.Policy;

namespace TamakenService.Services.Sqlite
{
    public class HeightDB
    {
        private string connectionString = $"Data Source=C:\\cubic\\DB\\Height.db;Version=3;";


        #region 建構子
        public HeightDB(string _connectionString)
        {
            connectionString = _connectionString;
        }
        #endregion

        #region
        public Dictionary<string, ASAModel> getASAModel()
        {
            Dictionary<string, ASAModel> ASASet = new Dictionary<string, ASAModel>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    Console.WriteLine($"DB已連接");
                    string query = @"SELECT DISTINCT GWAS.""BP(HG38)"", ASA.MapInfo, GWAS.SNP 
                             FROM ""GWAS_COJO-SNPS_List"" GWAS 
                             INNER JOIN ""ASA-24v1-0_E2"" ASA 
                             ON GWAS.""BP(HG38)"" = ASA.MapInfo;";
                    // 使用 Dapper 查詢資料庫
                    int i = 0;
                    IEnumerable<ASAModel> gwasList = connection.Query<ASAModel>(query);
                    foreach (ASAModel model in gwasList)
                    {
                        i++;
                        //Console.WriteLine($"第{i}筆");
                        ASASet.Add(model.SNP, model);
                    }

                    return ASASet;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"連接資料庫或查詢時發生錯誤：{ex.Message}");
                    return ASASet;
                }
            }
        }
        #endregion
        #region
        public List<string> getASAList()
        {
            List<string> ASASet = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    Console.WriteLine($"DB已連接");
                    string query = @"SELECT DISTINCT GWAS.SNP 
                             FROM ""GWAS_COJO-SNPS_List"" GWAS 
                             INNER JOIN ""ASA-24v1-0_E2"" ASA 
                             ON GWAS.""BP(HG38)"" = ASA.MapInfo
                             ORDER BY GWAS.SNP ;";
                    // 使用 Dapper 查詢資料庫
                    int i = 0;
                    IEnumerable<string> gwasList = connection.Query<string>(query);
                    ASASet = new List<string>(gwasList);

                    return ASASet;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"連接資料庫或查詢時發生錯誤：{ex.Message}");
                    return ASASet;
                }
            }
        }
        #endregion


    }
}
