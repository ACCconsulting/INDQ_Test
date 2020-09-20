using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Dto
{
  public  class PageResult<T>
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
