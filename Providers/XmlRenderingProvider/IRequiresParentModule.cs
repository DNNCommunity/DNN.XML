using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.Xml.Providers.XmlRenderingProvider
{
    public interface IRequiresParentModule
    {
        void Setup(PortalModuleBase moduleControl);
    }
}