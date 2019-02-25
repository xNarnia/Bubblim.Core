using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bubblim.Core.Services
{
    public class InfoService
    {
        private Dictionary<string, InfoText> _info { get; set; }

        /// <summary>
        /// Service provider to load text files and retrieve them later.
        /// </summary>
        public InfoService()
        {
            PopulateInfo();
        }

        /// <summary>
        /// Loads and stores Info files into cache.
        /// </summary>
        public void PopulateInfo()
        {
            _info = new Dictionary<string, InfoText>();

            Directory.CreateDirectory(@"data/info");
            FileInfo[] files = new DirectoryInfo(@"data/info").GetFiles("*.txt");

            foreach (FileInfo file in files)
            {
                InfoText infoText = new InfoText(file);

                if (!infoText.Empty())
                    _info.Add(file.Name.Replace(".txt", "").ToLower(), infoText);
            }
        }

        /// <summary>
        /// Retrieves the InfoText for the selected file.
        /// </summary>
        /// <param name="name">Name of Info File</param>
        /// <returns>InfoText of file</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public InfoText GetInfo(string name)
        {
            name = name.ToLower();
            if (!_info.ContainsKey(name))
                throw new KeyNotFoundException($"Info File \"{name}\" could not be found.");

            return _info[name];
        }

        /// <summary>
        /// Lists all files loaded into cache.
        /// </summary>
        /// <param name="prefix">String to put directly before file name. Example: {prefix}FileName{suffix}</param>
        /// <param name="suffix">String to put directly after file name. Example: {prefix}FileName{suffix}</param>
        /// <returns></returns>
        public string GetList(string prefix = null, string suffix = null)
        {
            string output = "";
            foreach(var info in _info)
            {
                output += $"{prefix}{info.Key}{suffix} - {info.Value}\n";
            }

            return output;
        }
    }
}
