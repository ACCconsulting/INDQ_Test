using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Dto
{
   public class EventsDto
    {
        public int EventsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Imagen { get; set; }

        public int Attendances { get; set; }
        public bool WillYouAttend { get; set; }

     
    }
}
