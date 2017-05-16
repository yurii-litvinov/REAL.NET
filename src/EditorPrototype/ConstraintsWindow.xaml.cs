﻿namespace EditorPrototype
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Логика взаимодействия для ConstraintsWindow.xaml
    /// </summary>
    public partial class ConstraintsWindow : Window
    {
        private RepoInfo info;
        private string objType;

        public ConstraintsWindow(Repo.IRepo repo, string modelName)
        {
            this.InitializeComponent();
            this.info = new RepoInfo(repo, modelName);
        }

        public void ObjType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ComboBoxItem)this.ObjType.SelectedItem).Content.ToString())
            {
                case "Node":
                    this.ElementType.ItemsSource = new List<ConstraintsValues>(this.info.GetNodeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "Node";
                    break;
                case "Edge":
                    this.ElementType.ItemsSource = new List<ConstraintsValues>(this.info.GetEdgeTypes().Select(x => new ConstraintsValues { ElementType = x }));
                    this.objType = "Edge";
                    break;
            }
        }

        private void ElementType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((ConstraintsValues)this.ElementType.SelectedItem).ElementType)
            {
                case "All":
                    this.ammountButton.Visibility = Visibility.Visible;
                    this.ammountBox.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void CreateConstraint_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ammountBox.Text))
            {
                if (this.objType == "Node")
                {
                    Constraints.NodesAmmount = Convert.ToInt32(this.ammountBox.Text);
                }
                else
                {
                    Constraints.EdgesAmmount = Convert.ToInt32(this.ammountBox.Text);
                }
            }
        }
    }
}
