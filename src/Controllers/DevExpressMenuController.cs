using Atomus.Control.Menu.Models;
using Atomus.Database;
using Atomus.Service;
using System.Threading.Tasks;

namespace Atomus.Control.Menu.Controllers
{
    internal static class DevExpressMenuController
    {
        internal static async Task<IResponse> SearchAsync(this ICore core, DevExpressMenuSearchModel search)
        {
            IServiceDataSet serviceDataSet;

            serviceDataSet = new ServiceDataSet
            {
                ServiceName = core.GetAttribute("ServiceName"),
                TransactionScope = false
            };
            serviceDataSet["LoadMenu"].ConnectionName = core.GetAttribute("DatabaseName");
            serviceDataSet["LoadMenu"].CommandText = core.GetAttribute("ProcedureMenu");
            serviceDataSet["LoadMenu"].AddParameter("@START_MENU_ID", DbType.Decimal, 18);
            serviceDataSet["LoadMenu"].AddParameter("@ONLY_PARENT_MENU_ID", DbType.Decimal, 18);
            serviceDataSet["LoadMenu"].AddParameter("@USER_ID", DbType.Decimal, 18);

            serviceDataSet["LoadMenu"].NewRow();
            serviceDataSet["LoadMenu"].SetValue("@START_MENU_ID", search.START_MENU_ID.MinusToDBNullValue());
            serviceDataSet["LoadMenu"].SetValue("@ONLY_PARENT_MENU_ID", search.ONLY_PARENT_MENU_ID.MinusToDBNullValue());
            serviceDataSet["LoadMenu"].SetValue("@USER_ID", Config.Client.GetAttribute("Account.USER_ID"));

            return await core.ServiceRequestAsync(serviceDataSet);
        }

        //internal static async Task<IResponse> SearchOpenControl(this ICore core, DefaultMenuSearchModel search)
        //{
        //    IServiceDataSet serviceDataSet;

        //    serviceDataSet = new ServiceDataSet
        //    {
        //        ServiceName = core.GetAttribute("ServiceName"),
        //        TransactionScope = false
        //    };
        //    serviceDataSet["OpenControl"].ConnectionName = core.GetAttribute("DatabaseName");
        //    serviceDataSet["OpenControl"].CommandText = core.GetAttribute("ProcedureMenuSelect");
        //    //serviceDataSet["OpenControl"].SetAttribute("DatabaseName", core.GetAttribute("DatabaseName"));
        //    //serviceDataSet["OpenControl"].SetAttribute("ProcedureID", core.GetAttribute("ProcedureMenuSelect"));
        //    serviceDataSet["OpenControl"].AddParameter("@MENU_ID", DbType.Decimal, 18);
        //    serviceDataSet["OpenControl"].AddParameter("@ASSEMBLY_ID", DbType.Decimal, 18);
        //    serviceDataSet["OpenControl"].AddParameter("@USER_ID", DbType.Decimal, 18);
                          
        //    serviceDataSet["OpenControl"].NewRow();
        //    serviceDataSet["OpenControl"].SetValue("@MENU_ID", search.MENU_ID);
        //    serviceDataSet["OpenControl"].SetValue("@ASSEMBLY_ID", search.ASSEMBLY_ID);
        //    serviceDataSet["OpenControl"].SetValue("@USER_ID", Config.Client.GetAttribute("Account.USER_ID"));

        //    return await core.ServiceRequestAsync(serviceDataSet);
        //}
    }
}