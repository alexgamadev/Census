using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusService
{
    public class CensusMeter
    {
        private readonly Dictionary<string, int> _meterValues = new Dictionary<string, int>();
        private readonly IFileSystem _fileSystem;

        public CensusMeter()
        {

        }

        public CensusMeter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public int Meter( string meterName, int value = 1 )
        {
            if (meterName is null) throw new ArgumentNullException("Meter name must not be null");

            if (_meterValues.TryGetValue(meterName, out int meterValue))
            {
                _meterValues[meterName] = meterValue + value;
            }
            else
            {
                _meterValues.Add(meterName, value);
            }

            return _meterValues[meterName];
        }

        public int? GetMeter(string meterName)
        {
            if (meterName is null) throw new ArgumentNullException("Meter name must not be null");

            if (_meterValues.TryGetValue(meterName, out int meterValue))
            {
                return meterValue;
            }

            return null;
        }

        
        public void SaveData(string path, bool clearLocalData = true)
        {
            _fileSystem.File.WriteAllText(path, ConvertDataToText());

            if( clearLocalData )
            {
                _meterValues.Clear();
            }
        }

        private string ConvertDataToText()
        {
            string text = "";
            foreach( var item in _meterValues )
            {
                text += $"{item.Key}:{item.Value};";
            }

            return text;
        }
    }
}
