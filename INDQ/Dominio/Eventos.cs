using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dominio
{
  public class Events
    {
        public int EventsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Imagen { get; set; }

        public int Attendances { get; set; }
        public bool WillYouAttend { get; set; }

        public Point Location { get; set; }

    }
}
