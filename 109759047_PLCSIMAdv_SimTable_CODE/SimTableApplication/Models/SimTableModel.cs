using GalaSoft.MvvmLight;
using SimTableApplication.PLCSIM_Advanced.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SimTableApplication.Models
{
    [Serializable]
    [XmlRoot(ElementName ="SimulationTable")]
    public class SimTableModel: ObservableObject
    {
        #region Properties

        /// <summary>
        /// name of the simulation table
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// list of simulation tags
        /// </summary>
        [XmlElement(ElementName ="SimulationsTag")]
        public Collection<SimTag> SimTags { get; set; }

        #endregion

        #region Ctor

        public SimTableModel()
        {
            SimTags = new Collection<SimTag>();
        }

        #endregion
        
        #region Methods

        #endregion

    }


}
