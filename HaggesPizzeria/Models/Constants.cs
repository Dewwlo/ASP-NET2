using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HaggesPizzeria.Models
{
    public class Constants
    {
        public const string IngredientsSession = "IngredientsSession";
        public const string CartSession = "CartSession";
        public const string OrderSession = "OrderSession";
        public const string DevelopmentEnvironment = "Development";
        public const string ProductionEnvironment = "Production";
        public const string StagingEnvironment = "Staging";
        public const string AzureConnection = "Server=tcp:dewwlopizzeria.database.windows.net,1433;Initial Catalog=HaggesDb;Persist Security Info=False;User ID=Dewwlo;Password=Passw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    }
}
