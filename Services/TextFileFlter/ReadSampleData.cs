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
            nlogService = _nlogService;
            ReadSNPDataFromFile();
        }
        public SampleModel SampleData
        {
            get { return sampleData; }
        }
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                ReadSNPDataFromFile();
            }
        }
        private void ReadSNPDataFromFile()
        {
            HashSet<SNPData> snpDataList = new HashSet<SNPData>();
            sampleData = new SampleModel();
            bool foundDataSection = false;
            int SNPIndex = 0, Allele1Index = 0, Allele2Index = 0, index = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!foundDataSection)
                    {
                        foundDataSection = CheckForDataSection(line);
                        if (foundDataSection)
                        {
                            nlogService.WriteLine($"已讀取到標籤: {line.Trim()} ,後續讀取到的資料將做為Sample資料");
                            continue;
                        }
                    }

                    if (foundDataSection)
                    {
                        ProcessDataLine(line, ref index, ref SNPIndex, ref Allele1Index, ref Allele2Index, snpDataList);
                    }
                }

                nlogService.WriteLine($"SampleID: {sampleData.SampleID} 資料已讀取完畢 共有 {snpDataList.Count} 組基因資料");
                sampleData.SNPData = snpDataList;
            }
        }

        private bool CheckForDataSection(string line)
        {
            return line.Trim() == "[Data]";
        }

        private void ProcessDataLine(string line, ref int index, ref int SNPIndex, ref int Allele1Index, ref int Allele2Index, HashSet<SNPData> snpDataList)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            string[] values = line.Split('\t');
            if (values.Length < 8) return;
            if (values[1].Replace(" ", "").ToLower().Equals("sampleid"))
            {
                foreach (string value in values)
                {
                    switch (value.Replace(" ", "").ToLower())
                    {
                        case "snpname":
                            nlogService.WriteLine($"SNPIndex: {index}");
                            SNPIndex = index;
                            break;
                        case "allele1-forward":
                            nlogService.WriteLine($"Allele1Index: {index}");
                            Allele1Index = index;
                            break;
                        case "allele2-forward":
                            nlogService.WriteLine($"Allele2Index: {index}");
                            Allele2Index = index;
                            break;
                        default: break;
                    }
                    index++;
                }
                return;
            }

            if (string.IsNullOrEmpty(sampleData.SampleID))
            {
                sampleData.SampleID = values[1];
                nlogService.WriteLine($"正在讀取SampleID: {sampleData.SampleID} 資料");
            }
            SNPData snpData = new SNPData
            {
                SNP = values[SNPIndex],
                Allele1 = values[Allele1Index],
                Allele2 = values[Allele2Index]
            };
            snpDataList.Add(snpData);
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
                    if (Allele1 > -1 && Allele2 > -1)
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
