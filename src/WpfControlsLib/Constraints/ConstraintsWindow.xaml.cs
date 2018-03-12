using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfControlsLib.Constraints
{
    /// <summary>
    /// Interaction logic for ConstraintsWindow.xaml
    /// </summary>
    public partial class ConstraintsWindow : Window
    {
        private readonly RepoInfo info;
        private string objType;
        private Boolean userChange = true;

        public ConstraintsWindow(WpfControlsLib.Model.Model model)
        {
            this.InitializeComponent();
            this.info = new RepoInfo(model.Repo, model.ModelName);
        }

        public void ObjType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBoxItem)this.ObjType.SelectedItem).Content.ToString())
            {
                case "Node":
                    this.userChange = false;
                    this.ElementType.ItemsSource = new List<ConstraintsValues>(this.info.GetNodeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "Node";
                    break;
                case "EdgeViewModel":
                    this.userChange = false;
                    this.ElementType.ItemsSource = new List<ConstraintsValues>(this.info.GetEdgeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "EdgeViewModel";
                    break;
            }
            this.userChange = true;
        }

        private void ElementType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.userChange)
            {
                switch (((ConstraintsValues)this.ElementType.SelectedItem).ElementType)
                {
                    case "All":
                        this.ammountButton.Visibility = Visibility.Visible;
                        this.ammountBox.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void CreateConstraint_Click(object sender, RoutedEventArgs e)
        {
            var item = new ConstraintsItem();
            if (!string.IsNullOrEmpty(this.ammountBox.Text))
            {
                if (this.objType == "Node")
                {
                    WpfControlsLib.Constraints.Constraints.NodesAmount = Convert.ToInt32((string) this.ammountBox.Text);
                }
                else
                {
                    WpfControlsLib.Constraints.Constraints.EdgesAmount = Convert.ToInt32((string) this.ammountBox.Text);
                }
            }

            item.ElementType = this.ElementType.Text;
            item.ObjectType = this.objType;
            item.Initialize();
            this.ConstraintsPanel.Children.Add(item);
        }
    }
}
