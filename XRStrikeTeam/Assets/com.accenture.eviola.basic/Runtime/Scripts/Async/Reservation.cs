
namespace Accenture.eviola.Async
{
    /// <summary>
    /// A class allowing to keep track when a variable is actually conumed.
    /// Useful in async contexts, multithreading etc
    /// </summary>
    public class GenericReservation<T> {
        bool _bReserved = false;
        T _value;

        /// <summary>
        /// returns true if the value was reserved and not consumed yet
        /// </summary>
        public bool IsReserved() {
            return _bReserved;
        }

        /// <summary>
        /// cancels a reservation
        /// </summary>
        public void Cancel() {
            _bReserved = false;
        }

        /// <summary>
        /// sets the value as reserved
        /// </summary>
        public void Reserve() {
            _bReserved = true;
        }

        /// <summary>
        /// sets the value and reserves it
        /// </summary>
        /// <param name="newVal"></param>
        public void Reserve(T newVal) {
            _value = newVal;
            Reserve();
        }

        /// <summary>
        /// consumes the value (and frees the reservation flag)
        /// </summary>
        public T Consume() {
            Cancel();
            return _value;
        }
    }

    /// <summary>
    /// <inheriddoc/>
    /// </summary>
    public class IntReservation : GenericReservation<int> { }
}