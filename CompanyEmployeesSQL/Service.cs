using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CompanyEmployeesSQL
{
    public class Service
    {
        MainWindow WinMainWindow;
        DataTable dtE;
        DataTable dtD;
        SqlDataAdapter adapterE;
        SqlDataAdapter adapterD;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mWindow"></param>
        /// <param name="adapterEmp"></param>
        /// <param name="dtEmp"></param>
        /// <param name="adapterDep"></param>
        /// <param name="dtDep"></param>
        public Service(System.Windows.Window mWindow, SqlDataAdapter adapterEmp, DataTable dtEmp, SqlDataAdapter adapterDep, DataTable dtDep )
        {
            if (mWindow.GetType()==typeof(MainWindow))
            { WinMainWindow = (MainWindow)mWindow; }
            dtE = dtEmp;
            dtD = dtDep;
            adapterE = adapterEmp;
            adapterD = adapterDep;
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void SaveDB()
        {
            adapterE.Update(dtE);
            adapterD.Update(dtD);
        }

        /// <summary>
        /// Удалить департамент
        /// </summary>
        /// <param name="rowV"></param>
        public void RemoveDepartment(DataRowView rowV)
        {
            if (rowV == null) { return; }
            int inx = (int)rowV.Row["Id"];

            foreach (DataRow item in dtE.Rows)
            {
                if ((int)item["DepartmentId"] == inx) { item.Delete(); }
            }

            rowV.Row.Delete();
            adapterE.Update(dtE);
            adapterD.Update(dtD);
        }

        /// <summary>
        /// Открыть окно редактирования сотрудников
        /// </summary>
        public void OpenDepartmentsEdit()
        {
            WinEditDepartments wed = new WinEditDepartments(adapterD, dtD, this) { Owner = WinMainWindow };
            wed.ShowDialog();

            SaveDB();

            dtE.Clear();
            adapterE.Fill(dtE);
        }

        /// <summary>
        /// Изменение департамента для сотрудника
        /// </summary>
        /// <param name="drvDep"></param>
        /// <param name="dataRowViews"></param>
        public void DepartmentSet(DataRowView drvDep, List<DataRowView> dataRowViews)
        {
            if (drvDep == null) { return; }
            int iId = (int)drvDep["Id"];
            string DepName = (string)drvDep["DepartmentName"];

            foreach (DataRowView item in dataRowViews)
            {
                item["DepartmentId"] = iId;
                item["DepartmentName"] = DepName;
            }
            adapterE.Update(dtE);
            WinMainWindow.CbDepartmentSet.SelectedIndex = -1;
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="dataRowViews"></param>
        public void RemoveEmployee(List<DataRowView> dataRowViews)
        {
            foreach (DataRowView item in dataRowViews)
            {
                item.Row.Delete();
            }
            adapterE.Update(dtE);
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <param name="dataRowViews"></param>
        public void AddNewEmployee(List<DataRowView> dataRowViews)
        {
            if (dataRowViews.Count == 0)
            {
                MessageBox.Show("Сначала необходимо добавить отдел!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            DataRowView drvDep = dataRowViews[0] as DataRowView;
            if (drvDep == null)
            { return; }
            int inx = (int)drvDep["Id"];
            string n = drvDep["DepartmentName"].ToString();

            DataRow newRow = dtE.NewRow();

            newRow.BeginEdit();
            newRow["Name"] = "Новый";
            newRow["Age"] = 0;
            newRow["Salary"] = 0;
            newRow["DepartmentId"] = inx;
            newRow["DepartmentName"] = n;
            newRow.EndEdit();

            dtE.Rows.Add(newRow);
            adapterE.Update(dtE);
        }


        /// <summary>
        /// Инициализация адаптеров, таблиц параметро базы данных
        /// </summary>
        /// <param name="connection"></param>
        public void INI(SqlConnection connection)
        {
            SqlCommand command;
            SqlParameter param;

            //adapter = new SqlDataAdapter();
            command = new SqlCommand("SELECT * FROM Employee, Department where Employee.DepartmentId = Department.Id OR Employee.DepartmentId = 0", connection);
            adapterE.SelectCommand = command;

            //insert
            command = new SqlCommand(@"INSERT INTO Employee (Name, Age, Salary, DepartmentId) 
                          VALUES (@Name, @Age, @Salary, @DepartmentId); SET @Id = @@IDENTITY;",
                          connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, -1, "Name");
            command.Parameters.Add("@Age", SqlDbType.Int, 0, "Age");
            command.Parameters.Add("@Salary", SqlDbType.Float, 0, "Salary");
            command.Parameters.Add("@DepartmentId", SqlDbType.Int, 0, "DepartmentId");

            param = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            param.Direction = ParameterDirection.Output;

            adapterE.InsertCommand = command;

            // update
            command = new SqlCommand(@"UPDATE Employee SET Name = @Name,
            Age = @Age, Salary = @Salary, DepartmentId = @DepartmentId WHERE Id = @Id", connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, -1, "Name");
            command.Parameters.Add("@Age", SqlDbType.Int, 0, "Age");
            command.Parameters.Add("@Salary", SqlDbType.Float, 0, "Salary");
            command.Parameters.Add("@DepartmentId", SqlDbType.Int, 0, "DepartmentId");

            param = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            param.SourceVersion = DataRowVersion.Original;

            adapterE.UpdateCommand = command;
            //delete
            command = new SqlCommand("DELETE FROM Employee WHERE Id = @Id", connection);

            param = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            param.SourceVersion = DataRowVersion.Original;

            adapterE.DeleteCommand = command;

            adapterE.Fill(dtE);
            WinMainWindow.DgEmployee.ItemsSource = dtE.DefaultView;


            //Departments
            command = new SqlCommand("SELECT * FROM Department", connection);
            adapterD.SelectCommand = command;

            //insert
            command = new SqlCommand(@"INSERT INTO Department (DepartmentName) 
                          VALUES (@DepartmentName); SET @Id = @@IDENTITY;",
                          connection);

            command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, -1, "DepartmentName");

            param = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            param.Direction = ParameterDirection.Output;

            adapterD.InsertCommand = command;

            // update
            //command = new SqlCommand(@"UPDATE Employee SET Name = @Name, Age = @Age, Salary = @Salary, DepartmentId = @DepartmentId WHERE Id = @Id", connection);
            command = new SqlCommand(@"UPDATE Department SET DepartmentName = @DepartmentName WHERE Id = @Id", connection);

            command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar, -1, "DepartmentName");

            param = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            param.SourceVersion = DataRowVersion.Original;

            adapterD.UpdateCommand = command;
            //delete
            command = new SqlCommand("DELETE FROM Department WHERE Id = @Id", connection);

            param = command.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
            param.SourceVersion = DataRowVersion.Original;

            adapterD.DeleteCommand = command;

            adapterD.Fill(dtD);

            WinMainWindow.CbDepartmentSet.ItemsSource = dtD.DefaultView;
        }
    }
}
