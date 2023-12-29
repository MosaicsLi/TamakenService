using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Models.TextFileFilter;
using TamakenService.Services.Sqlite;

namespace TamakenService.Services.TextFileFlter
{
    public class ReadSampleData
    {
        private string filePath;
        private SampleModel sampleData;
        public ReadSampleData()
        {
            filePath = @"C:\cubic\Chip\";
        }
        public ReadSampleData(string _filePath)
        {
            filePath = _filePath;
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
            string validLetters = "ATGC";
            foreach (SNPData sample in sampleData.SNPData)
            {
                if (!validLetters.Contains(sample.Allele1) || !validLetters.Contains(sample.Allele2))
                {
                    snpDataSet.Add(sample.SNP, "");
                    continue;
                }
                if (SNPMathFeature.ContainsKey(sample.SNP))
                {
                    int Allele1 = SNPMathFeature[sample.SNP].Equals(sample.Allele1) ? 1 : 0;
                    int Allele2 = SNPMathFeature[sample.SNP].Equals(sample.Allele2) ? 1 : 0;
                    int AlleleSum = Allele1 + Allele2;
                    snpDataSet.Add(sample.SNP, $"{AlleleSum}");
                }
            }
            return snpDataSet;
        }
        /*
        public Hashtable ReadSNPDataHashtable()
        {
            Hashtable snpDataSet = new Hashtable();
            bool foundDataSection = false;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (foundDataSection)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] values = line.Split('\t'); // 假設資料以制表符分隔

                            // 確保有足夠的資料列來創建 SNPData 物件
                            if (values.Length >= 8)
                            {
                                SampleID = values[1];
                                snpDataSet.Add(values[0], $"{values[5]}/{values[6]}");
                            }
                        }
                    }
                    else if (line.Trim() == "[Data]")
                    {
                        foundDataSection = true;
                    }
                }
            }
            return snpDataSet;
        }

        public Hashtable ReadSNPMathHashtable(Hashtable SNPMathFeature)
        {
            Hashtable snpDataSet = new Hashtable();
            bool foundDataSection = false;
            string validLetters = "ATGC";

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (foundDataSection)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] values = line.Split('\t'); // 假設資料以制表符分隔

                            // 確保有足夠的資料列來創建 SNPData 物件
                            if (values.Length >= 8)
                            {
                                if (!validLetters.Contains(values[5]) || !validLetters.Contains(values[6]))
                                {
                                    snpDataSet.Add(values[0], "");
                                    continue;
                                }
                                SampleID = values[1];
                                if (SNPMathFeature.ContainsKey(values[0]))
                                {
                                    int Allele1 = (SNPMathFeature[values[0]].Equals(values[5])) ? 1 : 0;
                                    int Allele2 = (SNPMathFeature[values[0]].Equals(values[6])) ? 1 : 0;
                                    int AlleleSum = Allele1 + Allele2;
                                    snpDataSet.Add(values[0], $"{AlleleSum}");
                                }
                            }
                        }
                    }
                    else if (line.Trim() == "[Data]")
                    {
                        foundDataSection = true;
                    }
                }
            }
            return snpDataSet;
        }
        */

        
    }
}
