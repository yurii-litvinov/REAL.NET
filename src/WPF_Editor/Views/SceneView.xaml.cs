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
            InitializeComponent();
            _scene = SceneViewModel.CreateScene();
            _scene.InitializeScene(zoomctrl);
            DataContext = _scene;
        }

        private void HandleSingleLeftClick(object sender, MouseButtonEventArgs e)
        {
            var position = Mouse.GetPosition(this);
            position = TranslatePoint(position, graphArea);
            _scene.HandleSingleLeftClick(position);
        }
    }
}