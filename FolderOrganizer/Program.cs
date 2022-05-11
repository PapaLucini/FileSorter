using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FolderOrganizer.Models;

namespace FolderOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Indicate source path: ");
            var sourceDirectoryPath = Console.ReadLine();

            if (!Directory.Exists(sourceDirectoryPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Source path not found.");
                Console.ResetColor();

                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Files under source directory:");

            var allFiles = Directory.GetFiles(sourceDirectoryPath, "*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{Path.GetFileNameWithoutExtension(file)}");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(Path.GetExtension(file));
                Console.ResetColor();
            }

            Console.WriteLine($"Total Files: {allFiles.Count()}");

            if (Directory.Exists(sourceDirectoryPath))
            {
                Console.Write("Indicate destination path: ");
                var targetDirectoryPath = Console.ReadLine();

                Console.Write("Indicate the extension of the files you would like to move: ");
                var fileExtension = Console.ReadLine();

                Console.Write("Would you like to Copy (C) or move (M) the files?: ");
                var copyOrMove = Console.ReadLine();

                var files = Directory.GetFiles(sourceDirectoryPath, $"*.{fileExtension}", SearchOption.AllDirectories);

                var fileSettings = new FileSettings();

                foreach (var file in files)
                {
                    fileSettings.SourcePath = Path.GetFullPath(file);
                    fileSettings.ModifiedDate = new FileInfo(fileSettings.SourcePath).LastWriteTimeUtc;
                    fileSettings.CreationDate = new FileInfo(fileSettings.SourcePath).CreationTime;

                    //Handle gopro files -- GOPRO file set modified date as 2016
                    if (file.StartsWith("GH") || file.StartsWith("GOPR") || file.StartsWith("GX"))
                        fileSettings.OriginalDate = fileSettings.CreationDate;
                    else
                        fileSettings.OriginalDate = fileSettings.ModifiedDate < fileSettings.CreationDate ? fileSettings.ModifiedDate : fileSettings.CreationDate;

                    //Create target directory path with [target directory + year + month]
                    fileSettings.DestinationFolderPath = Path.Combine(targetDirectoryPath, fileSettings.OriginalDate.Year.ToString(), fileSettings.OriginalDate.ToString("MM"));

                    if (!Directory.Exists(fileSettings.DestinationFolderPath))
                        Directory.CreateDirectory(fileSettings.DestinationFolderPath);

                    //Destination path + file name
                    fileSettings.FullDestinationPath = Path.Combine(fileSettings.DestinationFolderPath, Path.GetFileName(file));

                    if (!Directory.Exists(fileSettings.FullDestinationPath))
                    {
                        try
                        {
                            switch (copyOrMove.ToLower())
                            {
                                case "m":
                                    File.Move(fileSettings.SourcePath, fileSettings.FullDestinationPath);
                                    break;
                                case "c":
                                default:
                                    File.Copy(fileSettings.SourcePath, fileSettings.FullDestinationPath, false);
                                    break;
                            }

                            fileSettings.MovedFiles++;

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"Moved file: ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(Path.GetFileName(file));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" from : ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(fileSettings.SourcePath);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" to ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(fileSettings.FullDestinationPath);
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Could not copy file --> {Path.GetFullPath(file)}");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Message: {ex.Message}");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"A file with the name:{Path.GetFileName(file)} already exists. Path: {fileSettings.SourcePath}");
                        Console.ResetColor();
                    }
                }

                Console.WriteLine($"Files moved: {fileSettings.MovedFiles}");
            }
            else
                Console.WriteLine("Path does not exist");

            Console.WriteLine("Job Finished");

            return;
        }
    }
}
