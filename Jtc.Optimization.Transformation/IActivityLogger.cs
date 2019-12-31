using System;

namespace Jtc.Optimization.Transformation
{
    public interface IActivityLogger
    {
        string Output { get; }

        void Add(string message, DateTime data);
        void Add(string message, double data);
        void Add(string message, double[] data);
        void Add(string message, int data);
        void Add(string message, TimeSpan data);
        void StateHasChanged();
    }
}