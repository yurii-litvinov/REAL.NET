namespace WpfControlsLib.Constraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ConstraintsWindow.xaml
    /// </summary>
    public partial class ConstraintsWindow : Window
    {
        private readonly RepoInfo info;

        private string objType;
        private string elementType;

        private bool userChange = true;
        private WpfControlsLib.Model.Model model;

        public ConstraintsWindow(WpfControlsLib.Model.Model model)
        {
            this.model = model;
            this.InitializeComponent();
            this.info = new RepoInfo(model.Repo, model.ModelName);
        }

        public void ObjType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.errorLabel.Visibility = Visibility.Hidden;
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
            this.errorLabel.Visibility = Visibility.Hidden;
            if (this.userChange)
            {
                switch (((ConstraintsValues)this.ElementType.SelectedItem).ElementType)
                {
                    case "All":
                        this.ammountLabel.Visibility = Visibility.Visible;
                        this.ammountBox.Visibility = Visibility.Visible;
                        this.elementType = "All";
                        break;
                }
            }
        }

        private void CreateConstraint_Click(object sender, RoutedEventArgs e)
        {
            var item = new ConstraintsItem();
            if (!string.IsNullOrEmpty(this.ammountBox.Text))
            {
                try
                {
                    if (this.objType == "Node")
                    {
                        this.model.Constraints.NodesAmount = Convert.ToInt32((string)this.ammountBox.Text);
                    }
                    else
                    {
                        this.model.Constraints.EdgesAmount = Convert.ToInt32((string)this.ammountBox.Text);
                    }

                    item.Ammount = Convert.ToInt32(this.ammountBox.Text);
                }
                catch
                {
                    this.errorLabel.Visibility = Visibility.Visible;
                    this.errorLabel.Foreground = Brushes.Red;
                    this.errorLabel.Text = "Неправильно заполнено поле количества элементов";
                    return;
                }
            }

            item.ElementType = this.elementType;
            item.ObjectType = this.objType;
            item.Initialize();
            this.ConstraintsPanel.Children.Add(item);
        }
    }
}
