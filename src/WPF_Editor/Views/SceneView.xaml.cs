using System.Windows.Controls;
using System.Windows.Input;
/* Copyright 2017
 * Yurii Litvinov
 * Ivan Yarkov
 * Egor Zainullin
 * Denis Sushentsev
 * Arseniy Zavalishin
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */

using WPF_Editor.ViewModels;
using WPF_Editor.ViewModels.Interfaces;

namespace WPF_Editor.Views
{
    /// <summary>
    ///     Interaction logic for SceneView.xaml
    /// </summary>
    public partial class SceneView : UserControl
    {
        private readonly ISceneViewModel _scene;

        public SceneView()
        {
            this.InitializeComponent();
            this._scene = SceneViewModel.CreateScene();
            this._scene.InitializeScene(this.zoomctrl);
            this.DataContext = this._scene;
        }

        private void HandleSingleLeftClick(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(this);
            position = this.TranslatePoint(position, this.graphArea);
            this._scene.HandleSingleLeftClick(position);
        }
    }
}