﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Census.Metering
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

        /// <summary>
        /// Meters a value to a provided name. Example: Meter("Score", score) will increment the score meter by the given value
        /// </summary>
        /// <param name="meterName">The name of the meter</param> 
        /// <param name="value">The amount to increment the meter by</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>
        /// Gets the value of a meter
        /// </summary>
        /// <param name="meterName">The name of the meter</param>
        /// <returns>The value of the meter (Null if meter doesn't exist)</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int? GetMeter(string meterName)
        {
            if (meterName is null) throw new ArgumentNullException("Meter name must not be null");

            if (_meterValues.TryGetValue(meterName, out int meterValue))
            {
                return meterValue;
            }

            return null;
        }

        /// <summary>
        /// Saves the local meter data to a text file at the given path.
        /// </summary>
        /// <param name="path">The path to save the meter data file to</param>
        /// <param name="clearLocalData">Whether to clear the local meter storage</param>
        public void SaveData(string path, bool clearLocalData = false)
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