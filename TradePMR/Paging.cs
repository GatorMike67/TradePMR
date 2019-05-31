using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradePMR
{
   
    public class Paging
    {       
        const int maxPageSize = 20;
        private int _pageSize { get; set; } = 10;
        public int Page { get; set; } = 0;

      

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}