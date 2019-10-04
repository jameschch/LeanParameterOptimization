using System;
using System.Collections.Generic;
using System.Text;

namespace Jtc.Optimization.Transformation
{
    public class ActivityLogger
    {

        private StringBuilder _builder { get; }
        private const string DateFormat = "yyyy-MM-dd hh:mm:ss";
        
        public string Output
        {
            get
            {

                return _builder.ToString();
            }
        }

        public ActivityLogger()
        {
            _builder = new StringBuilder();
        }

        public void Add(string message, DateTime data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.ToString(DateFormat)).Append(Environment.NewLine);
        }

        public void Add(string message, TimeSpan data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.TotalSeconds).Append(" secs").Append(Environment.NewLine);
        }

        public void Add(string message, int data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data).Append(Environment.NewLine);
        }

    }
}
