﻿using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Lzw.DemoWithBwt
{
    public class PbvCompressor
    {
        private readonly ICompressorAlgorithm _compressorAlgorithm;

        private ICompressorAlgorithm CompressorAlgorithm
        {
            // allows us to set the algorithm at runtime, also lets us set the algorithm dynamically if we create a factory class
            init => _compressorAlgorithm = value;
        }

        private PbvCompressor()
        {
            // setting this by default but we could create a factory class to let us set the algorithm based on arguments passed to the main method
            // IE. "Compress.exe zip -c blah.txt" would use the zip algorithm as opposed to the default one.
            CompressorAlgorithm = new PbvCompressorLzw();
        }

        private void Start(string argument, string pInFile, string pOutFile)
        {
            var newOutFile = FileNameSelector.GetFileName(pOutFile);

            if (newOutFile == null)
            {
                Console.WriteLine("Exiting...");
                Environment.Exit(1);
            }

            switch (argument)
            {
                case "-c":
                    _compressorAlgorithm.Compress(pInFile, pOutFile);
                    break;
                case "-d":
                    _compressorAlgorithm.Decompress(pInFile, pOutFile);
                    break;
            }

            return;
        }

        private static void Main(string[] args)
        {
            var validCommands = new Regex("-[cdCD]");

            if (args.Length != 3)
            {
                Console.WriteLine("Too many or too few arguments! Exiting.");
                return;
            }

            if (validCommands.IsMatch(args[0])) //if valid arguments given proceed
            {
                var pc = new PbvCompressor();
                var transformation = Bwt.Bwt.Transform(File.ReadAllBytes(args[1]));
                using var fileStream = new FileStream("tmp.tmp", FileMode.Create);
                using var writer = new BinaryWriter(fileStream);
                writer.Write(transformation);
                pc.Start(args[0].ToLower(), "tmp.tmp", args[2]);
            }
            else
                Console.WriteLine("Invalid argument command given. Exiting.");
        }
    }
}