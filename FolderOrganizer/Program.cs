using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FolderOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Indicate source path: ");
            var sourceDirectoryPath = Console.ReadLine();

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
                //var files1 = Directory.GetFiles(sourceDirectoryPath, "*", SearchOption.AllDirectories);

                string sourceFilePath, destinationFileFolderPath, destinationFileFullPath;
                DateTime fileModifiedDate = new DateTime();
                DateTime fileCreateDate = new DateTime();
                DateTime originalDate = new DateTime();
                int movedFiles = 0;
                foreach (var file in files)
                {
                    //sourceFilePath = Path.Combine(sourceDirectoryPath, Path.GetFileName(file));
                    sourceFilePath = Path.GetFullPath(file);
                    fileModifiedDate = new FileInfo(sourceFilePath).LastWriteTimeUtc ;
                    fileCreateDate = new FileInfo(sourceFilePath).CreationTime ;

                    originalDate = fileModifiedDate < fileCreateDate ? fileModifiedDate : fileCreateDate;

                    //Handle gopro files
                    if (file.StartsWith("GH") || file.StartsWith("GOPR") || file.StartsWith("GX"))
                    {
                        originalDate = fileCreateDate;
                    }

                    //Create target directory path with [target directory + year + month]
                    destinationFileFolderPath = Path.Combine(targetDirectoryPath, originalDate.Year.ToString(), originalDate.ToString("MM"));

                    if (!Directory.Exists(destinationFileFolderPath))
                        Directory.CreateDirectory(destinationFileFolderPath);

                    //Destination path + file name
                    destinationFileFullPath = Path.Combine(destinationFileFolderPath, Path.GetFileName(file));

                    if (!Directory.Exists(destinationFileFullPath))
                    {
                        try
                        {
                            switch (copyOrMove.ToLower())
                            {
                                case "m":
                                    File.Move(sourceFilePath, destinationFileFullPath);
                                    break;
                                case "c":
                                default:
                                    File.Copy(sourceFilePath, destinationFileFullPath, false);
                                    break;

                            }

                            movedFiles++;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"Moved file: ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(Path.GetFileName(file));
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" from : ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(sourceFilePath);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" to ");
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(destinationFileFolderPath);
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
                        Console.WriteLine($"A file with the name:{Path.GetFileName(file)} already exists. Path: {sourceFilePath}");
                        Console.ResetColor();
                    }
                }

                Console.WriteLine($"Files moved: {movedFiles}");

            }
            else
                Console.WriteLine("Path does not exist");

            Console.WriteLine("Job Finished");
            Console.ReadLine();
            return;
        }
    }
}
