using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Log;
using TamakenService.Models.TextFileFilter;

namespace TamakenService.Services.TextFileFlter
{
    public class ExportFile
    {
        private List<string> SNPIndexList;
        private Hashtable ASAToGWASTable;
        private NlogService nlogService;
        private static readonly object fileWriteLock = new object();
        public ExportFile(List<string> _SNPIndexList, Hashtable _ASAToGWASTable, NlogService _nlogService)
        {
            SNPIndexList = _SNPIndexList;
            nlogService = _nlogService;
            ASAToGWASTable = _ASAToGWASTable;
        }
        public void SaveSetToCSV(HashSet<ReadSampleData> sampleSet, string outputPath)
        {
            foreach (var sample in sampleSet)
            {
                nlogService.WriteLine($"執行續開始輸出 基因資料");
                SaveToCSV(sample, outputPath);
                nlogService.WriteLine($"輸出 {sample.SampleData.SampleID} 基因完畢");
            }
        }
        public void SaveSetToMathCSV(HashSet<ReadSampleData> sampleSet, Hashtable SNPMathFeature, string outputPath)
        {
            foreach (var sample in sampleSet)
            {
                nlogService.WriteLine($"執行續開始輸出 數據資料");
                SaveToMathCSV(sample, SNPMathFeature, outputPath);
                nlogService.WriteLine($"輸出 {sample.SampleData.SampleID} 數據資料完畢");
            }
        }
        public void SaveToCSV(ReadSampleData sample, string outputPath)
        {
            try
            {
                var SampleSNPSet = sample.SNPDataToHash();
                bool fileExists = File.Exists(outputPath);
                using (StreamWriter writer = new StreamWriter(outputPath, true))
                {
                    if (!fileExists)
                    {
                        writer.Write($"SampleID_GWAS");
                        foreach (string SNP in SNPIndexList)
                        {
                            writer.Write($",{ASAToGWASTable[SNP]}");
                        }
                        writer.WriteLine();

                        writer.Write($"SampleID_ASA");
                        foreach (string SNP in SNPIndexList)
                        {
                            writer.Write($",{SNP}");
                        }
                        writer.WriteLine();
                    }
                    writer.Write($"{sample.SampleData.SampleID}");
                    nlogService.WriteLine($"正在輸出 Sample:{sample.SampleData.SampleID} 的基因資料 Sample位置 {sample.FilePath}");
                    foreach (string SNP in SNPIndexList)
                    {
                        writer.Write($",");
                        if (SampleSNPSet.ContainsKey(SNP))
                        {
                            writer.Write($"{SampleSNPSet[SNP]}");
                        }
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                nlogService.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                nlogService.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                nlogService.WriteLine($"outputPath:{outputPath}");
                nlogService.WriteLine($"Exception:{ex.Message}");
            }
        }
        public void SaveToMathCSV(ReadSampleData sample, Hashtable SNPMathFeature, string outputPath)
        {
            try
            {
                var SampleSNPSet = sample.SNPDataToMathHashtable(SNPMathFeature);
                bool fileExists = File.Exists(outputPath);
                using (StreamWriter writer = new StreamWriter(outputPath, true))
                {
                    if (!fileExists)
                    {
                        writer.Write($"SampleID_GWAS");
                        foreach (string SNP in SNPIndexList)
                        {
                            writer.Write($",{ASAToGWASTable[SNP]}");
                        }
                        writer.WriteLine();

                        writer.Write($"SampleID_ASA");
                        foreach (string SNP in SNPIndexList)
                        {
                            writer.Write($",{SNP}");
                        }
                        writer.WriteLine();
                    }
                    writer.Write($"{sample.SampleData.SampleID}");
                    nlogService.WriteLine($"正在輸出 Sample:{sample.SampleData.SampleID} 的數據資料 Sample位置 {sample.FilePath}");
                    foreach (string SNP in SNPIndexList)
                    {
                        writer.Write($",");
                        if (SampleSNPSet.ContainsKey(SNP))
                        {
                            writer.Write($"{SampleSNPSet[SNP]}");
                        }
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                nlogService.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                nlogService.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                nlogService.WriteLine($"outputPath:{outputPath}");
                nlogService.WriteLine($"Exception:{ex.Message}");
            }
        }
        public void SaveToCSV_WithLock(ReadSampleData sample, string outputPath)
        {
            try
            {
                var SampleSNPSet = sample.SNPDataToHash();
                lock (fileWriteLock)
                {
                    bool fileExists = File.Exists(outputPath);
                    using (StreamWriter writer = new StreamWriter(outputPath, true))
                    {
                        if (!fileExists)
                        {
                            writer.Write($"SampleID_GWAS");
                            foreach (string SNP in SNPIndexList)
                            {
                                writer.Write($",{ASAToGWASTable[SNP]}");
                            }
                            writer.WriteLine();

                            writer.Write($"SampleID_ASA");
                            foreach (string SNP in SNPIndexList)
                            {
                                writer.Write($",{SNP}");
                            }
                            writer.WriteLine();
                        }
                        writer.Write($"{sample.SampleData.SampleID}");
                        nlogService.WriteLine($"正在輸出 Sample:{sample.SampleData.SampleID} 的基因資料 Sample位置 {sample.FilePath}");
                        foreach (string SNP in SNPIndexList)
                        {
                            writer.Write($",");
                            if (SampleSNPSet.ContainsKey(SNP))
                            {
                                writer.Write($"{SampleSNPSet[SNP]}");
                            }
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                nlogService.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                nlogService.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                nlogService.WriteLine($"outputPath:{outputPath}");
                nlogService.WriteLine($"Exception:{ex.Message}");
            }
        }
        public void SaveToMathCSV_WithLock(ReadSampleData sample, Hashtable SNPMathFeature, string outputPath)
        {
            try
            {
                var SampleSNPSet = sample.SNPDataToMathHashtable(SNPMathFeature);
                lock (fileWriteLock)
                {
                    bool fileExists = File.Exists(outputPath);
                    using (StreamWriter writer = new StreamWriter(outputPath, true))
                    {
                        if (!fileExists)
                        {
                            writer.Write($"SampleID_GWAS");
                            foreach (string SNP in SNPIndexList)
                            {
                                writer.Write($",{ASAToGWASTable[SNP]}");
                            }
                            writer.WriteLine();

                            writer.Write($"SampleID_ASA");
                            foreach (string SNP in SNPIndexList)
                            {
                                writer.Write($",{SNP}");
                            }
                            writer.WriteLine();
                        }
                        writer.Write($"{sample.SampleData.SampleID}");
                        nlogService.WriteLine($"正在輸出 Sample:{sample.SampleData.SampleID} 的數據資料 Sample位置 {sample.FilePath}");
                        foreach (string SNP in SNPIndexList)
                        {
                            writer.Write($",");
                            if (SampleSNPSet.ContainsKey(SNP))
                            {
                                writer.Write($"{SampleSNPSet[SNP]}");
                            }
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                nlogService.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                nlogService.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                nlogService.WriteLine($"outputPath:{outputPath}");
                nlogService.WriteLine($"Exception:{ex.Message}");
            }
        }
    }
}
