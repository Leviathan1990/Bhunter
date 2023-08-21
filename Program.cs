/*
 *                  Bhunter *.bin extractor tool V1.0
 *                  By: Krisztian Kispeti.
 *                  Date: 2023.08.13
 *                  Version: V1.0
 *                  
 *                  TODO    :   Fix file structure for extracting .TGA files from *.bin archives!
 *                  TODO2   :   Info needed for .LEV and .PTH files!  
					TODO3	: Info needed for the .bsp files as well!
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

struct ArchiveFileEntry
{
    public string Filename;
    public int FileOffset;
    public int EndFileOffset;
}

class Program
{
    static void Main(string[] args)
    {
        List<ArchiveFileEntry> fileList = new List<ArchiveFileEntry>();
        Console.WriteLine("==========================================");
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("Bhunter *.bin extractor tool. V1.0");
        Console.ResetColor();
        Console.WriteLine("Moddb: https://www.moddb.com/games/bhunter");
        Console.WriteLine("By: Krisztian Kispeti");
        Console.WriteLine("==========================================\n");

        while (true)
        {
            Console.WriteLine("Make your selection:");
            Console.WriteLine("1. Display (list) files in *.bin archive");
            Console.WriteLine("2. Extract files from *.bin archive");
            Console.WriteLine("3. Exit");

            int choice = GetChoice(1, 3);

            if (choice == 1)
            {
                ListFiles(fileList);
            }
            else if (choice == 2)
            {
                ExtractFiles(fileList);
            }
            else if (choice == 3)
            {
                break;
            }
        }
    }

    static int GetChoice(int min, int max)
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < min || choice > max)
        {
            Console.WriteLine($"Invalid selection. Please type another number between {min} and {max}");
        }
        return choice;
    }

    static void ListFiles(List<ArchiveFileEntry> fileList)
    {
        Console.Write("Enter the *.bin file name: ");
        string binFilePath = Console.ReadLine();

        using (BinaryReader reader = new BinaryReader(File.Open(binFilePath, FileMode.Open), Encoding.Default))
        {
            int numFiles = reader.ReadInt32();

            Console.WriteLine("Available files:");
            for (int i = 0; i < numFiles; i++)
            {
                ArchiveFileEntry entry = new ArchiveFileEntry();
                entry.Filename = ReadNullTerminatedString(reader, 248);
                entry.FileOffset = reader.ReadInt32();
                entry.EndFileOffset = reader.ReadInt32();

                fileList.Add(entry); // Fájlnevek tárolása a listában

                Console.WriteLine($"{i + 1}. {entry.Filename}");
            }
        }
    }

    static void ExtractFiles(List<ArchiveFileEntry> fileList)
    {
        Console.Write("Enter the *.bin file name: ");
        string binFilePath = Console.ReadLine();

        Console.Write("Enter the output folder name: ");
        string outputDirectory = Console.ReadLine();

        using (BinaryReader reader = new BinaryReader(File.Open(binFilePath, FileMode.Open), Encoding.Default))
        {
            int numFiles = reader.ReadInt32();

            Directory.CreateDirectory(outputDirectory);

            foreach (ArchiveFileEntry entry in fileList)
            {
                string filename = entry.Filename;
                int fileOffset = entry.FileOffset;
                int endFileOffset = entry.EndFileOffset;

                int fileSize = endFileOffset - fileOffset;

                if (fileSize < 0)
                {
                    Console.WriteLine($"Error: Negative filesize in {filename} file.");
                    continue;
                }

                reader.BaseStream.Seek(fileOffset, SeekOrigin.Begin);
                byte[] fileData = reader.ReadBytes(fileSize);

                string cleanedFilename = CleanFilename(filename);
                var outputPath = Path.Combine(outputDirectory, cleanedFilename);
                string outputDirectoryPath = Path.GetDirectoryName(outputPath);
                Directory.CreateDirectory(outputDirectoryPath);
                File.WriteAllBytes(outputPath, fileData);

                Console.WriteLine($"Extracted files: {outputPath}");
            }
        }
    }

    static string ReadNullTerminatedString(BinaryReader reader, int maxLength)
    {
        StringBuilder sb = new StringBuilder();
        char c;

        while ((c = reader.ReadChar()) != '\0')
        {
            if (c == 'ĺ' || c == (char)0xE5)
            {
                reader.BaseStream.Seek(1, SeekOrigin.Current); // Ugrás az extra karakter felett
                continue;
            }

            if (sb.Length < maxLength)
            {
                sb.Append(c);
            }
        }

        reader.BaseStream.Seek(Math.Max(0, maxLength - sb.Length - 1), SeekOrigin.Current);
        return sb.ToString();
    }

    static string CleanFilename(string filename)
    {
        string cleanedFilename = new string(filename.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
        return cleanedFilename;
    }
}
