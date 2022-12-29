using System;
using System.IO;
using Signals.UiSignals;
using UnityEngine;
using Zenject;

namespace FilesRead
{
    public class FileReader
    {
        private const string SEPARATOR = "\r\n";

        private SignalBus _signalBus;
        private FileData _data;

        public FileReader(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public FileData Data => _data;

        public string[] GetFileText(int fileCount)
        {
            var variants = Resources.LoadAll<TextAsset>("FileVariants");
            string[] fileText;
            switch (fileCount)
            {
                case 1:
                    fileText = variants[fileCount-1].text.Split(SEPARATOR);
                    CreateNewFileData(fileText);
                    return fileText;
                case 2:
                    fileText = variants[fileCount-1].text.Split(SEPARATOR);
                    CreateNewFileData(fileText);
                    return fileText;
                case 3:
                    fileText = variants[fileCount-1].text.Split(SEPARATOR);
                    CreateNewFileData(fileText);
                    return fileText;
            }

            return null;
        }

        private void CreateNewFileData(string[] textFromFile)
        {
            try
            {
                _data = new FileData(textFromFile);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _signalBus.Fire<FileErrorSignal>();
            }
        }
    }
}