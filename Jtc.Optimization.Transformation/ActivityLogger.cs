using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.Optimization.Transformation
{
    public class ActivityLogger
    {

        private StringBuilder _builder { get; }
        private const string DateFormat = "yyyy-MM-dd hh:mm:ss";
        private readonly Action _stateHasChanged;
        private readonly Action<string> _showMessage;
        private int _lastShowSecond;

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

        public ActivityLogger(Action stateHasChanged, Action<string> showMessage)
        {

            _builder = new StringBuilder();
            _stateHasChanged = stateHasChanged;
            _showMessage = showMessage;
        }

        public void Add(string message, DateTime data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.ToString(DateFormat)).Append(Environment.NewLine);
            ShowMessage();
        }

        public void Add(string message, TimeSpan data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.TotalSeconds).Append(" secs").Append(Environment.NewLine);
            ShowMessage();
        }

        public void Add(string message, int data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data).Append(Environment.NewLine);
            ShowMessage();
        }

        public void Add(string message, double data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.ToString("N6")).Append(Environment.NewLine);
            ShowMessage();
        }

        public void Add(string message, double[] data)
        {
            _builder.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(string.Join(",", data.Select(s => s.ToString("N6")))).Append(Environment.NewLine);
            ShowMessage();
        }

        public void StateHasChanged()
        {
            _stateHasChanged.Invoke();
        }

        private void ShowMessage()
        {
            var now = DateTime.UtcNow.Second;
            //throttle to every few seconds
            if (_lastShowSecond != now && now % 2 > 0)
            {
                _lastShowSecond = now;
                var split = Output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                _showMessage.Invoke(string.Join(Environment.NewLine, split.Skip(Math.Max(0, split.Count() - 2))));
            }
        }

    }
}
