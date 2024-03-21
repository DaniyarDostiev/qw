using qw.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qw.util
{
    internal class DbWorker
    {
        private static qw2Entities context;

        public static qw2Entities GetContext()
        {
            if (context == null)
            {
                context = new qw2Entities();
            }
            return context;
        }

    }
}
