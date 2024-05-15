using SimTableApplication.PLCSIM_Advanced.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace SimTableApplication.Models
{
    [Serializable]
    [XmlRoot(ElementName="Project")]
    public class ProjectModel
    {
        #region Properties


        /// <summary>
        /// Name of the Project
        /// </summary>
        [XmlAttribute("Name")]
        public string ProjectName { get; set; }

        /// <summary>
        /// Path where the project is located
        /// </summary>
        [XmlAttribute("Path")]
        public string ProjectPath { get; set; }

        /// <summary>
        /// Version of the project
        /// </summary>
        [XmlAttribute("Version")]
        public string Version { get; set; }

        /// <summary>
        /// Author of the project
        /// </summary>
        [XmlAttribute("Author")]
        public string Author { get; set; }

        /// <summary>
        /// Comment of the project
        /// </summary>
        [XmlElement(ElementName ="Comment")]        
        public string Comment { get; set; }

        /// <summary>
        /// List of controller
        /// </summary>
        [XmlElement(ElementName ="Controller")]
        public Collection<VirtualControllerModel> VirtualControllerModels { get; set; }

        #endregion

        #region Ctor

        public ProjectModel()
        {
            VirtualControllerModels = new Collection<VirtualControllerModel>();
        }


        #endregion

        #region Methods

        public VirtualControllerModel AddNewController(string name, VirtualControllerType type, string path)
        {
            VirtualControllerModel model = new VirtualControllerModel()
            {
                VirtualControllerName = name,
                VirtualControllerType = type,
                VirtualControllerPath = path

            };
            VirtualControllerModels.Add(model);

            return model;
        }

        #endregion
    }


}
