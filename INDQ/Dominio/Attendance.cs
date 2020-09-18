using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
     

        public User User { get; set; }

        public int EventsId { get; set; }
        public Events Events { get; set; }
    }
}
