using System;
using System.Collections.Generic;

namespace Auth0Glue
{
    public class EmailCollator
    {
        public static ISet<string> Collate(List<string> list1, List<string> list2)
        {
            var result = new HashSet<string>(list1);
            result.UnionWith(list2);
            result.Remove(null);
            return result;
        }
    }
}