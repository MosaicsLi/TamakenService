using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;

namespace TamakenService.Services.TextFileFlter
{
    public class FilterCriteria
    {
        private string filePath;
        private NlogService nlogService;
        public FilterCriteria(string _filePath,NlogService _nlogService)
        {
            filePath = _filePath;
            nlogService = _nlogService;
        }

        #region 獲得判斷條件(順序固定)
        public List<string> GetSNPIndexList()
        {
            List<string> FilterCriteria = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim().Contains("\"SNP\"")) continue;
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = line.Split(',');
                        if (values.Length >= 2)
                        {
                            FilterCriteria.Add(values[0].Replace("\"", ""));
                        }
                    }
                }

                return FilterCriteria;
            }
            catch (Exception ex)
            {
                nlogService.LogError($"時間: {DateTime.Now.ToString("G")} 獲取GetSNPIndexList: {filePath} 發生錯誤:{ex.Message}");
                throw;
            }

        }
        #endregion
        #region 獲得判斷條件與顯示文字(Hashtable)
        public Hashtable GetSNPHashtable()
        {
            Hashtable ASASet = new Hashtable();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        if (line.Trim().Contains("\"SNP\"")) continue;

                            string[] values = line.Split(',');

                        if (values.Length >= 2)
                        {
                            ASASet.Add(values[0].Replace("\"", ""), values[1].Replace("\"", ""));
                        }
                    }
                }

                return ASASet;
            }
            catch (Exception ex)
            {
                nlogService.LogError($"時間: {DateTime.Now.ToString("G")} 獲取GetSNPHashtable: {filePath} 發生錯誤:{ex.Message}");
                return ASASet;
            }

        }
        #endregion

        #region 獲得判斷條件與數值化條件(Hashtable)
        public Hashtable GetSNPMathFeatureHashtable()
        {
            Hashtable ASASet = new Hashtable();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        if (line.Trim().Contains("\"SNP\"")) continue;
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] values = line.Split(',');
                        if (values.Length >= 3)
                        {
                            ASASet.Add(values[0].Replace("\"", ""), values[2].Replace("\"", ""));
                        }
                    }
                }

                return ASASet;
            }
            catch (Exception ex)
            {
                nlogService.LogError($"時間: {DateTime.Now.ToString("G")} 獲取SNPMathFeature: {filePath} 發生錯誤:{ex.Message}");
                return ASASet;
            }

        }
        #endregion
    }
}
