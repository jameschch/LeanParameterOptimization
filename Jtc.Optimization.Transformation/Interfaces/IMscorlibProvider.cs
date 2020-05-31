using System.Threading.Tasks;

namespace  Jtc.Optimization.Transformation
{
    public interface IMscorlibProvider
    {
        Task<byte[]> Get();
    }
}