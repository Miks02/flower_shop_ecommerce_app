using System.Reflection;

namespace FlowerShop.Application;

public class AssemblyReference
{
    public readonly Assembly Assembly = typeof(AssemblyReference).Assembly; 
}