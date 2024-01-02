using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;
using TamakenService.Models.TextFileFilter;
using TamakenService.Services.Sqlite;

namespace TamakenService.Services.TextFileFlter
{
    public class ReadSampleData
    {
        private string filePath;
        private SampleModel sampleData;
        private NlogService nlogService;
        public ReadSampleData(NlogService _nlogService)
        {
            filePath = @"C:\cubic\Chip\";
            nlogService = _nlogService;
        }
        public ReadSampleData(string _filePath, NlogService _nlogService)
        {
            filePath = _filePath;
            nlogService= _nlogService;
            ReadSNPDataFromFile();
        }
        public SampleModel SampleData
        {
            get { return sampleData; }
        }
        public string FilePath
        {
            get { return filePath; }
            set {
                filePath = value;
                ReadSNPDataFromFile();
            }
        }

        private void ReadSNPDataFromFile()
        {
            List<SNPData> snpDataList = new List<SNPData>();
            sampleData = new SampleModel();
            bool foundDataSection = false;
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (foundDataSection)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] values = line.Split('\t'); // 假設資料以制表符分隔

                        // 確保有足夠的資料列來創建 SNPData 物件
                        if (values.Length < 8) continue;
                        if (values[1].ToLower().Equals("sample id")) continue;

                        if (string.IsNullOrEmpty(sampleData.SampleID))
                        {
                            sampleData.SampleID = values[1];
                            nlogService.WriteLine($"正在讀取SampleID: {sampleData.SampleID} 資料");
                        }
                        SNPData snpData = new SNPData
                        {
                            SNP = values[0],
                            Allele1 = values[5],
                            Allele2 = values[6]
                        };
                        snpDataList.Add(snpData);
                    }
                    else if (line.Trim() == "[Data]")
                    {
                        foundDataSection = true;
                        nlogService.WriteLine($"已讀取到標籤: {line.Trim()} ,後續讀取到的資料將做為Sample資料");
                    }
                }
                sampleData.SNPData = snpDataList;
            }
        }
        public Hashtable SNPDataToHash()
        {
            Hashtable snpDataSet = new Hashtable();
            foreach (SNPData sample in sampleData.SNPData)
            {
                snpDataSet.Add(sample.SNP, $"{sample.Allele1}/{sample.Allele2}");
            }
            return snpDataSet;
        }
        public Hashtable SNPDataToMathHashtable(Hashtable SNPMathFeature)
        {
            Hashtable snpDataSet = new Hashtable();
            string validLetters = "-";
            string geneValidLetters = "ATCG";
            foreach (SNPData sample in sampleData.SNPData)
            {
                if (validLetters.Contains(sample.Allele1) || validLetters.Contains(sample.Allele2))
                {
                    snpDataSet.Add(sample.SNP, "");
                    //nlogService.WriteLine($"Sample資料異常: {sample.Allele1}/{sample.Allele2} 此筆資料將不會被轉為數據並將資料留空");
                    continue;
                }
                if (SNPMathFeature.ContainsKey(sample.SNP))
                {
                    int Allele1 = -1;
                    int Allele2 = -1;
                    if (geneValidLetters.Contains(sample.Allele1))
                    {
                        Allele1 = SNPMathFeature[sample.SNP].Equals(sample.Allele1) ? 1 : 0;
                    }
                    if (geneValidLetters.Contains(sample.Allele2))
                    {
                        Allele2 = SNPMathFeature[sample.SNP].Equals(sample.Allele2) ? 1 : 0;
                    }
                    if(Allele1>-1 && Allele2 > -1)
                    {
                        int AlleleSum = Allele1 + Allele2;
                        snpDataSet.Add(sample.SNP, $"{AlleleSum}");
                    }
                    else
                    {
                        snpDataSet.Add(sample.SNP, $"{sample.Allele1}/{sample.Allele2}");
                    }
                }
            }
            return snpDataSet;
        }
        
    }
}
