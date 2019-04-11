/* Copyright 2017-2018 REAL.NET group
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using System.Linq;

namespace ConstraintsPlugin
{
    using System;
    using EditorPluginInterfaces;
    using System.Windows.Controls;
    using ConstraintsMatcher;
    using WpfControlsLib.Controls.Scene;
    using System.Collections.Generic;

    /// <summary>
    /// This class is a sample plugin and is also used in unit tests to check plugin loading.
    /// </summary>
    public class ConstraintsPlugin : IPlugin<PluginConfig>
    {
        public event Action<string> ChangeModel;
        public event Action<bool> ChangeSelectorVisibility;

        /// <summary>
        /// Name of plugin
        /// </summary>
        public string Name => "constraintsPlugin";

        private Grid constraintsPanel;

        private Scene scene;

        private List<Tuple<string, ConstraintsCheckSystem>> checkSystems;
        private ConstraintsCheckSystem currentCheckSystem;
        private string currentModelName;
        //private ConstraintsScene constraintsScene;

        private ConstraintsColumn constraintsColumn;
        /// <summary>
        /// Field that contains reference to editor console.
        /// </summary>
        private IConsole console;

        private WpfControlsLib.Model.Model targetModel;
        private WpfControlsLib.Model.Model currentModel;

        /// <summary>
        /// Establishes connection with the rest of the system.
        /// </summary>
        /// <param name="config">Configuration from system core.</param>
        public void SetConfig(PluginConfig config)
        {
            if (config == null)
            {
                throw new ArgumentException("This is not correct type of configuration");
            }
            this.console = config.Console;
            this.console.SendMessage("Constraints add-on successfully launched");

            var repo = config.Model.Repo;

            this.targetModel = (WpfControlsLib.Model.Model)config.Model; //TODO сейчас мы можем работать только с роботстестмодел
            this.currentModelName = targetModel.ModelName;
            this.constraintsPanel = config.LeftPanelGrid;
            this.constraintsColumn = new ConstraintsColumn(config);

            this.constraintsColumn.NewButtonClicked += new Action<bool>(NewButtonClicked);
            this.constraintsColumn.CloseButtonClicked += new Action<bool>(CloseButtonClicked);
            this.constraintsColumn.SaveButtonClicked += new Action<bool>(SaveButtonClicked);
            this.constraintsColumn.CheckButtonClicked += new Action<bool>(CheckButtonClicked);
            config.OnMainModelChangedFunction = this.OnModelChanged;
            this.scene = (Scene)config.Scene;
            this.ChangeModel += new Action<string>(config.FuncCreateConstraintsModel);
            this.ChangeSelectorVisibility += new Action<bool>(config.FuncChangeSelectorVisibility);

            this.constraintsPanel.Children.Add(this.constraintsColumn);

            this.checkSystems = new List<Tuple<string, ConstraintsCheckSystem>>();
            if (targetModel.ModelName != null)
            {
                this.checkSystems.Add(new Tuple<string, ConstraintsCheckSystem>(this.targetModel.ModelName, new ConstraintsCheckSystem(this.targetModel, this.scene.Graph)));
            }

    }

        private void OnModelChanged(string modelName)
        {
            if (modelName != "ConstraintsTestModel")
            {
                this.currentModelName = modelName;
                var curScene = (Scene)scene;
                if (!this.checkSystems.Any(x => x.Item1 == targetModel.ModelName))
                {
                    this.checkSystems.Add(new Tuple<string, ConstraintsCheckSystem>(targetModel.ModelName, new ConstraintsCheckSystem(this.targetModel, this.scene.Graph)));
                }
                this.currentCheckSystem = this.checkSystems.Find(x => x.Item1 == targetModel.ModelName).Item2;
            }
        }

        private void SaveButtonClicked(bool i)
        {
            try
            {
                this.constraintsColumn.allowSave = false;
                var unit = new ConstraintsUnit();
                var rootName = this.currentCheckSystem.AddConstraint(this.targetModel, unit.GetHashCode());
                unit.RootName = rootName;
                unit.ConstraintsModelName = "ConstraintsTestModel"; //TODO 
                unit.DeleteButtonClicked += new Action<ConstraintsUnit>(this.DeleteConstraint);
                unit.EditButtonClicked += new Action<string>(this.EditConstraint);
                this.RemoveConstraintsSystemInterface();
               
                this.constraintsColumn.AddUnit(unit);
                this.constraintsColumn.allowSave = true;
            }
            catch(Exception ex)
            {
                this.console.SendMessage(ex.Message);
            }
        }

        private void EditConstraint(string constraintsModelName)
        {
            this.constraintsColumn.NewButtonClickedActions();
        }

        private void CheckButtonClicked(bool i)
        {
            if (this.currentCheckSystem.Check())
            {
                this.console.SendMessage("Constraints check successfully passed");
            }
            else
            {
                this.console.SendMessage("Constraints check did not passed successfully");
            }
        }

        private ConstraintsUnit CreateUnit(String rootName)
        {
            var unit = new ConstraintsUnit();
            
            return unit;
        }

        private void DeleteConstraint(ConstraintsUnit unit)
        {
            this.constraintsColumn.DeleteConstraintUnit(unit);
            this.currentCheckSystem.DeleteConstraint("ConstraintsTestModel", unit.GetHashCode());
        }
        private void NewButtonClicked(bool i)
        {
            this.CreateConstraintsSystemInterface();
        }

        private void CloseButtonClicked(bool i)
        {
            this.RemoveConstraintsSystemInterface();
        }

        private void CreateConstraintsSystemInterface()
        {
            this.ChangeSelectorVisibility(false);
            this.ChangeModel("ConstraintsTestModel");
        }

        private void RemoveConstraintsSystemInterface()
        {
            this.ChangeSelectorVisibility(true);
            this.ChangeModel(this.currentModelName);
        }
    }
}
