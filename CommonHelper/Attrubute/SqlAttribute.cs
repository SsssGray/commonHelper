using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper.Attrubute
{
    public class SqlAttribute:Attribute
    {
        public Boolean primaryKey;

        public String tableName;

        public String fieldName;
    }
}
