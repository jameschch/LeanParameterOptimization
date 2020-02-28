using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtc.Optimization.Transformation
{
    public class ActivityLogger : IActivityLogger
    {

        private StringBuilder _status;
        private StringBuilder _log;
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
        private const string DateWithMillisecondsFormat = "yyyy-MM-dd HH:mm:ss.ffff";
        private readonly Action _stateHasChanged;
        private readonly Action<string> _showMessage;
        private int _lastShowSecond;

        public string Status
        {
            get
            {
                return _status.ToString();
            }
        }

        public string Log
        {
            get
            {
                return _log.ToString();
            }
        }

        public ActivityLogger()
        {
            _status = new StringBuilder();
            _log = new StringBuilder();
        }

        public ActivityLogger(Action stateHasChanged, Action<string> showMessage) : this()
        {
            _stateHasChanged = stateHasChanged;
            _showMessage = showMessage;
        }

        public void Add(string message)
        {
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(Environment.NewLine);
            //ShowMessage();
        }

        public void Add(string message, DateTime data)
        {
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.ToString(DateFormat)).Append(Environment.NewLine);
            //ShowMessage();
        }

        public void Add(string message, TimeSpan data)
        {
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.TotalSeconds).Append(" secs").Append(Environment.NewLine);
            //ShowMessage();
        }

        public void Add(string message, int data)
        {
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data).Append(Environment.NewLine);
            //ShowMessage();
        }

        public void Add(string message, double data)
        {
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(data.ToString("N6")).Append(Environment.NewLine);
            //ShowMessage();
        }

        public void Add(string message, double[] data)
        {
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(message).Append(" ").Append(string.Join(",", data.Select(s => s.ToString("N6")))).Append(Environment.NewLine);
            //ShowMessage();
        }

        public void Add(string id, IEnumerable<string> keys, double[] parameters, double cost)
        {
            var paramText = string.Join(", ", Enumerable.Zip(keys, parameters, (k, v) => $"{k}: {v.ToString("N9").TrimEnd('0')}"));
            _log.Append(DateTime.Now.ToString(DateWithMillisecondsFormat)).Append(" ").Append($"id: {id}, ");
            _log.Append(paramText);

            var costText = $", cost: {cost.ToString("N9").TrimEnd('0')}\r\n";
            _status.Append(DateTime.Now.ToString(DateFormat)).Append(" ").Append(paramText).Append(costText);
            _log.Append(costText);
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
                var split = Status.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Last();
                _showMessage.Invoke(split);
            }
        }

    }
}
