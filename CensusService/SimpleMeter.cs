using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CensusService
{
    public class SimpleMeter
    {
        private Dictionary<string, int> _meterValues = new Dictionary<string, int>();

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
    }
}
