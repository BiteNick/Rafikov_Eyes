using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Rafikov_Eyes
{
    /// <summary>
    /// Логика взаимодействия для EyesPage.xaml
    /// </summary>
    public partial class EyesPage : Page
    {
        private int CountRecords;
        private int CountPage;
        private int CurrentPage = 0;

        private List<Agent> CurrentPageList = new List<Agent>();
        private List<Agent> TableList;

        public EyesPage()
        {
            
            InitializeComponent();
            Sorting.SelectedIndex = 0;
            Types.SelectedIndex = 0;
            var currentDBList = Rafikov_eyesEntities.GetContext().Agent.ToList();
            UpdateServices();
            EyesListView.ItemsSource = currentDBList;

        }

        public void UpdateServices()
        {
            var currentDBList = Rafikov_eyesEntities.GetContext().Agent.ToList();
            var AgentTypesDBList = Rafikov_eyesEntities.GetContext().AgentType.ToList();


            if (Types.SelectedIndex > 0)
            {
                var CurrentType = (TextBlock)Types.SelectedValue;
                string CurrentTypeContent = CurrentType.Text;
                currentDBList = currentDBList.Where(p => AgentTypesDBList[p.AgentTypeID - 1].GetTypeByID == CurrentTypeContent).ToList();
            }

            switch (Sorting.SelectedIndex)
            {
                case 1:
                    currentDBList = currentDBList = currentDBList.OrderBy(p => p.Title).ToList();
                    break;
                case 2:
                    currentDBList = currentDBList = currentDBList.OrderByDescending(p => p.Title).ToList();
                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:
                    currentDBList = currentDBList.OrderBy(p => p.Priority).ToList();
                    break;
                case 6:
                    currentDBList = currentDBList.OrderByDescending(p => p.Priority).ToList();
                    break;
            }
            Regex regex = new Regex(@"\D");
            currentDBList = currentDBList.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || regex.Replace(p.Phone, "").Contains(TBoxSearch.Text)).ToList();

            

            EyesListView.ItemsSource = currentDBList.ToList();
            TableList = currentDBList;

            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean IfUpdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            IfUpdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            IfUpdate = false;
                        }
                        break;
                }
            }
            if (IfUpdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                EyesListView.ItemsSource = CurrentPageList;

                EyesListView.Items.Refresh();
            }
            TBCount.Text = (CurrentPage+1).ToString();
            TBRecords.Text = CountPage.ToString();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void Types_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString())-1);
        }
    }
}
