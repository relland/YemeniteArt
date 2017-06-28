
using Nop.Web.Framework.Themes;
namespace Nop.Plugin.BrainStation.QuickView.ViewEngines
{
    class CustomViewEngine : ThemeableRazorViewEngine 
    {
        
        public  CustomViewEngine()
        {



            ViewLocationFormats = new[]
                                             {
                                                 "~/Plugins/BrainStation.QuickView/Views/{0}.cshtml"
                                             };

            PartialViewLocationFormats = new[]
                                             {

                                                 "~/Plugins/BrainStation.QuickView/Views/{0}.cshtml"
                                             };

            
        }
    }
}
