using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompanyEmployeesSQL
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CompanyEmployees.Properties." +
        "Settings.LocalDBConnectionString"].ConnectionString);
        SqlDataAdapter adapterEmployee = new SqlDataAdapter();
        SqlDataAdapter adapterDepartment = new SqlDataAdapter();

        DataTable dtEmployee = new DataTable();
        DataTable dtDepartments = new DataTable();

        Service serv;
        public MainWindow()
        {
            InitializeComponent();

            //Реакция главного окна
            this.Closing += (s, e) => { { serv.SaveDB(); } };
            //Реакция главного окна

            BtnDepartmentsEdit.Click += (s,e) => { { serv.OpenDepartmentsEdit(); } };
            CbDepartmentSet.SelectionChanged += (s, e) => { { serv.DepartmentSet(CbDepartmentSet.SelectedItem as DataRowView, 
                DgEmployee.SelectedItems.Cast<DataRowView>().ToList<DataRowView>()); } };
            BtnRemoveEmployee.Click += (s, e) => { { serv.RemoveEmployee(DgEmployee.SelectedItems.Cast<DataRowView>().ToList<DataRowView>()); } };
            BtnAddNewEmployee.Click += (s, e) => { { serv.AddNewEmployee(CbDepartmentSet.Items.Cast<DataRowView>().ToList<DataRowView>()); } };

            serv = new Service(this, adapterEmployee, dtEmployee, adapterDepartment, dtDepartments);
            serv.INI(connection);
        }
    }
}