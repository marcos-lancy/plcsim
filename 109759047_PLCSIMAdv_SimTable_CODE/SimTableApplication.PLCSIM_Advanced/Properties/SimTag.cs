using GalaSoft.MvvmLight;
using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Xml.Serialization;

namespace SimTableApplication.PLCSIM_Advanced.Models
{
    [Serializable]
    [XmlRoot(ElementName ="SimulationTag")]
    public class SimTag : ObservableObject, IEquatable<SimTag>, ICloneable
    {
        #region Fields

        private object _value;
        private object _modifyValue;
        private bool _isValid;

        #endregion

        #region Properties

        /// <summary>
        /// name of the simulation tag
        /// </summary>
        [XmlElement(ElementName ="Name")]
        public string TagName { get; set; }

        /// <summary>
        /// Datatype of the simulation tag
        /// </summary>
        [XmlElement(ElementName ="Datatype")]
        public SimDataType DataType { get; set; }

        /// <summary>
        /// Area of the simulation tag (input/Output...)
        /// </summary>
        [XmlElement(ElementName = "Area")]
        public string Area { get; set; }

        /// <summary>
        /// actual value of the tag
        /// </summary>
        [XmlIgnore]
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }
        
        /// <summary>
        /// new value of the tag
        /// </summary>
        [XmlIgnore]
        public object ModifyValue
        {
            get
            {
                return _modifyValue;
            }
            set
            {
                _modifyValue = value;
                RaisePropertyChanged(nameof(ModifyValue));
            }
        }

        /// <summary>
        /// indicates if the tag is valid within memory of controller
        /// </summary>        
        [XmlIgnore]
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        /// indicates if the tag should be modified or not
        /// </summary>
        [XmlElement(ElementName = "IsSelected")]
        public bool IsSelected { get; set; }


        #endregion

        public bool Equals(SimTag other)
        {
            if (other == null) return false;
            if (string.IsNullOrWhiteSpace(other.TagName)) return false;
            return TagName.Equals(other.TagName) && DataType.Equals(other.DataType);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
