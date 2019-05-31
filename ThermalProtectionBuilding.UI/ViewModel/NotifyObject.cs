using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThermalProtectionBuilding.UI.ViewModel
{
    /// <summary>
    /// Observable object with INotifyPropertyChanged implemented
    /// </summary>
    public class NotifyObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие происходить, если изменилось какое-либо свойство. Событие слабое.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChangedWeakEvent.Add(value); }
            remove { _propertyChangedWeakEvent.Remove(value); }
        }

        private WeakEvent<PropertyChangedEventHandler> _propertyChangedWeakEvent = new WeakEvent<PropertyChangedEventHandler>();

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="oldValue">Backing store.</param>
        /// <param name="newValue">Value.</param>
        /// <param name="changedBeforeNotifying">On changed.</param>
        /// <param name="propertyName">Property name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected bool SetValueNotifyProperty<T>(ref T oldValue, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return false;

            oldValue = newValue;

            _propertyChangedWeakEvent?.GetRaiseDelegate()?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            _propertyChangedWeakEvent?.GetRaiseDelegate()?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void NotifyAllPropertyChanged()
        {
            _propertyChangedWeakEvent?.GetRaiseDelegate()?.Invoke(this, new PropertyChangedEventArgs(""));
        }
    }
}
