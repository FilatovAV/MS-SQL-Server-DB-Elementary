using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace CompanyEmployeesSQL
{
    /// <summary>
    /// Логика взаимодействия для WinEditDepartments.xaml
    /// </summary>
    public partial class WinEditDepartments : Window
    {
        SqlDataAdapter adapterDepartment;
        DataTable dtDepartments;
        Service sService;
        SqlDataAdapter adapterEmployees;
        DataTable dtEmployees;

        public WinEditDepartments(SqlDataAdapter aDepartment, DataTable dtDeps, Service sServ)
        {
            InitializeComponent();

            adapterDepartment = aDepartment;
            dtDepartments = dtDeps;
            sService = sServ;

            DgDepartments.ItemsSource = dtDepartments.DefaultView;
        }

        private void BtnAddNewDepartment_Click(object sender, RoutedEventArgs e)
        {
            DataRow newRow = dtDepartments.NewRow();

            newRow.BeginEdit();
            newRow["DepartmentName"] = $"Новый-{DgDepartments.Items.Count}";
            newRow.EndEdit();

            dtDepartments.Rows.Add(newRow);
            adapterDepartment.Update(dtDepartments);
        }

        private void BtnRemoveDepartment_Click(object sender, RoutedEventArgs e)
        {

            sService.RemoveDepartment(DgDepartments.SelectedItem as DataRowView);

        }
    }
}
