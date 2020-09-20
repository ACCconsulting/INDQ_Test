using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Aplicacion.Helpers
{
  public  class HandlereException:Exception
    {
        public HttpStatusCode Codigo { get; }
        public object Errores { get; }
        public HandlereException(HttpStatusCode code, object objErrores = null)
        {
            this.Codigo = code;
            this.Errores = objErrores;

        }
    }
}
