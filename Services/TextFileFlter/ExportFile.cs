using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TamakenService.Models.TextFileFilter;

namespace TamakenService.Services.TextFileFlter
{
    public class ExportFile
    {
        private List<string> SNPIndexList;
        private Hashtable ASAToGWASTable;
        public ExportFile(List<string> _SNPIndexList, Hashtable _ASAToGWASTable)
        {
            SNPIndexList= _SNPIndexList;
            ASAToGWASTable= _ASAToGWASTable;
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
                    Console.WriteLine($"正在輸出 Sample:{sample.SampleData.SampleID} 的基因資料 Sample位置 {sample.FilePath}");
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
                Console.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                Console.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                Console.WriteLine($"outputPath:{outputPath}");
                Console.WriteLine($"Exception:{ex.Message}");
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
                    Console.WriteLine($"正在輸出 Sample:{sample.SampleData.SampleID} 的數據資料 Sample位置 {sample.FilePath}");
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
                Console.WriteLine($"SNPs:{SNPIndexList.ToString()}");
                Console.WriteLine($"ASAToGWASTable:{ASAToGWASTable.Count}");
                Console.WriteLine($"outputPath:{outputPath}");
                Console.WriteLine($"Exception:{ex.Message}");
            }
        }
    }
}
