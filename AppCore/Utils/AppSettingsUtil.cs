using AppCore.Utils.Bases;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Utils
{
    public class AppSettingsUtil: AppSettingsUtilBase
    {
        public AppSettingsUtil(IConfiguration configuration): base(configuration)
        {
            
        }
    }
}
