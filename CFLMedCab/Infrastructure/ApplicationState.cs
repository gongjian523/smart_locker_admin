using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure
{
    enum ApplicationKey{ CurUser, CurGoods, CodeMCab, CodeSCab, COM_MLocker, COM_SLocker, COM_MRFid, COM_SRFid, COM_MVein };

    public static class ApplicationState
    {

        //private static Dictionary<string, object> _values =
        //    new Dictionary<string, object>();

        //public static void SetValue(string key, object value)
        //{
        //    _values.Add(key, value);
        //}

        //public static T GetValue<T>(string key)
        //{
        //    return (T)_values[key];
        //}

        private static Dictionary<int, object> _values =
            new Dictionary<int, object>();

        public static void SetValue(int key, object value)
        {
            object sValue;

            if (_values.TryGetValue(key, out sValue))
                _values.Remove(key);

            _values.Add(key, value);
        }

        public static T GetValue<T>(int key)
        {
            return (T)_values[key];
        }
    }
}
