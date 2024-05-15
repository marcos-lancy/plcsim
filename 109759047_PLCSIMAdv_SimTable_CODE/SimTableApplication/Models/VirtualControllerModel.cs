using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SimTableApplication.Models
{
    [Serializable]    
    public class VirtualControllerModel
    {
        #region Properties

        /// <summary>
        /// Name of the controller
        /// </summary>
        [XmlAttribute("Name")]
        public string VirtualControllerName { get; set; }

        /// <summary>
        /// type of the controller
        /// </summary>
        [XmlAttribute("Type")]
        public VirtualControllerType VirtualControllerType { get; set; }

        /// <summary>
        /// Path where the controller is located
        /// </summary>
        [XmlAttribute("Path")]
        public string VirtualControllerPath { get; set; }

        /// <summary>
        /// List of simulation tables
        /// </summary>
        [XmlElement(ElementName ="SimulationTables")]
        public Collection<SimTableDirectoryModel> SimTableDirectoryModels { get; set; }

        #endregion

        #region Ctor
        public VirtualControllerModel()
        {
            SimTableDirectoryModels = new Collection<SimTableDirectoryModel>();
        }

        #endregion

        #region Methods

        public SimTableDirectoryModel AddNewSimTableDirectory()
        {
            SimTableDirectoryModel model = new SimTableDirectoryModel();
                       
            SimTableDirectoryModels.Add(model);

            return model;
        }

        #endregion

    }
}