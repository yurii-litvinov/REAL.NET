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

        private string currentModelName;
        private Scene scene;

        private ConstraintsCheckSystem checkSystem;
        //private ConstraintsScene constraintsScene;

        private ConstraintsColumn constraintsColumn;
        /// <summary>
        /// Field that contains reference to editor console.
        /// </summary>
        private IConsole console;

        private WpfControlsLib.Model.Model targetModel;

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

            this.currentModelName = config.Model.ModelName;
            this.constraintsPanel = config.ConstraintsGrid;
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

            this.checkSystem = new ConstraintsCheckSystem(this.targetModel, this.scene.Graph);

    }

        private void OnModelChanged(String modelName)
        {
            if (modelName != "ConstraintsTestModel")
                this.currentModelName = modelName;
        }

        private void SaveButtonClicked(bool i)
        {
            try
            {
                this.constraintsColumn.allowSave = false;
                var rootName = this.checkSystem.AddConstraint("ConstraintsTestModel");
                this.RemoveConstraintsSystemInterface();
                var unit = this.CreateUnit(rootName);
                this.constraintsColumn.AddUnit(unit);
                this.constraintsColumn.allowSave = true;
            }
            catch(Exception ex)
            {
                this.console.SendMessage(ex.Message);
            }
        }

        private void CheckButtonClicked(bool i)
        {
            if (this.checkSystem.Check())
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
            var unit = new ConstraintsUnit(rootName);
            unit.DeleteButtonClicked += new Action<ConstraintsUnit>(this.constraintsColumn.DeleteConstraintUnit);
            return unit;
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
