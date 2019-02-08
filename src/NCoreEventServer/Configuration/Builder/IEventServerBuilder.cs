
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Event Server Builder Interface
    /// </summary>
    public interface IEventServerBuilder
    {
        IServiceCollection Services { get; }
    }
}
