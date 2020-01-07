using System;
using System.Collections.Generic;

namespace Jtc.Optimization.Transformation
{
    public interface IActivityLogger
    {
        string Status { get; }
        string Log { get; }

        void Add(string message, DateTime data);
        void Add(string message, double data);
        void Add(string message, double[] data);
        void Add(string message, int data);
        void Add(string message, TimeSpan data);
        void Add(string id, IEnumerable<string> keys, double[] parameters, double cost);
        void StateHasChanged();
    }
}