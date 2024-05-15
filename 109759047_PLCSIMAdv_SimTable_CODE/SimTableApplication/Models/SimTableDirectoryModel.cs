using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SimTableApplication.Models
{
    [Serializable]   
    public class SimTableDirectoryModel
    {
        #region Fields

        private string _name = "SIM tables";

        #endregion

        #region Properties

        /// <summary>
        /// name of the simulation table
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// List of simulation tables
        /// </summary>
        [XmlElement(ElementName = "SimulationTable")]
        public Collection<SimTableModel> SimTableModels { get; set; }

        #endregion

        #region Ctor

        public SimTableDirectoryModel()
        {
            SimTableModels = new Collection<SimTableModel>();
        }

        #endregion
        
        #region Methods

        public SimTableModel AddNewSimTable(string name)
        {
            SimTableModel model = new SimTableModel()
            {
                Name = name

            };
            SimTableModels.Add(model);

            return model;
        }

        #endregion

    }
}
