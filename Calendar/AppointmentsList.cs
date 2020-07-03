using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class AppointmentsList
    {
        #region Fields
        private List<Appointment> appointments;
        #endregion

        #region Properties
        public List<Appointment> Appointments
        {
            get { return appointments; }
        }
        #endregion

        #region Methods
        public AppointmentsList()
        {
            appointments = new List<Appointment>();
        }

        public void AddAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
        }

        public void RemoveAppointment(Appointment appointment)
        {
            Appointments.Remove(appointment);
        }
        #endregion
    }
}
