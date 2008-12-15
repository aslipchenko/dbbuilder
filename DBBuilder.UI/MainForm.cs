using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DBBuilder.UI
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private const string conStringTemplate = "Data source={0};Initial catalog={1};Integrated security=SSPI;";
		private void connectButton_Click(object sender, EventArgs e)
		{

			using (var con = new SqlConnection(string.Format(conStringTemplate, serverNameTextBox.Text, "master")))
			{
				try
				{
					con.Open();
					var com = new SqlCommand("select [name] from sys.databases where [state] = 0 and owner_sid <> 0x01 order by [name]", con);
					var result = new DataTable();
					result.Load(com.ExecuteReader());
					databaseComboBox.DisplayMember = "name";
					databaseComboBox.ValueMember = "name";
					databaseComboBox.DataSource = result;
				}
				catch(Exception ex)
				{
					MessageBox.Show(string.Format("We're encountered exception while connection to SQL Server at {0}\n{1}",
						serverNameTextBox.Text, ex));
				}
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			const DBBuilder.MSSQL.SqlObjectType logicTypes = DBBuilder.MSSQL.SqlObjectType.StoredProcedure | DBBuilder.MSSQL.SqlObjectType.Trigger | DBBuilder.MSSQL.SqlObjectType.UserDefinedFunction | DBBuilder.MSSQL.SqlObjectType.Assembly;
			const DBBuilder.MSSQL.SqlObjectType dataTypes = DBBuilder.MSSQL.SqlObjectType.UserTable | DBBuilder.MSSQL.SqlObjectType.View | DBBuilder.MSSQL.SqlObjectType.FTCatalog;
			const DBBuilder.MSSQL.SqlObjectType securityTypes = DBBuilder.MSSQL.SqlObjectType.Schema;

			dataObjectsCheckedListBox.Items.Clear();
			logicObjectsCheckedListBox.Items.Clear();
			foreach (var enumElementName in 
				Enum.GetNames(typeof(DBBuilder.MSSQL.SqlObjectType))
				.Where(s => s != "None")
				.OrderBy( s => s))
			{
				var enumElement = (DBBuilder.MSSQL.SqlObjectType)Enum.Parse(typeof(DBBuilder.MSSQL.SqlObjectType), enumElementName);
				dataObjectsCheckedListBox.Items.Add(enumElementName, (enumElement & dataTypes) > 0);
				logicObjectsCheckedListBox.Items.Add(enumElementName, (enumElement & logicTypes) > 0);
			}
		}

		private void serverNameTextBox_Validating(object sender, CancelEventArgs e)
		{
			if (string.IsNullOrEmpty(((TextBox)sender).Text))
				errorProvider.SetError((Control)sender, "You need to enter server name");
			else
				errorProvider.SetError((Control)sender, "");
		}

		private void scriptButton_Click(object sender, EventArgs e)
		{
			var dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				var outputDirName = dlg.SelectedPath + Path.DirectorySeparatorChar + "objects";
				Directory.CreateDirectory(outputDirName);

				var selectedObjectTypes = GetSelectedObjectTypes(dataObjectsCheckedListBox);
				DBBuilder.MSSQL.Helpers.SqlServerHelper.ScriptDB(
					serverNameTextBox.Text,
					databaseComboBox.Text,
					outputDirName,
					dataDependencyFileNameTextBox.Text,
					selectedObjectTypes,
					DBBuilder.MSSQL.ObjectSaveOptions.AddObjectTypePrefix | DBBuilder.MSSQL.ObjectSaveOptions.SaveEachObjectTypeToSeparateDir, 
					null);

				selectedObjectTypes = GetSelectedObjectTypes(logicObjectsCheckedListBox);
				DBBuilder.MSSQL.Helpers.SqlServerHelper.ScriptDB(
					serverNameTextBox.Text,
					databaseComboBox.Text,
					outputDirName,
					logicDependencyFileNameTextBox.Text,
					selectedObjectTypes,
					DBBuilder.MSSQL.ObjectSaveOptions.AddObjectTypePrefix | DBBuilder.MSSQL.ObjectSaveOptions.SaveEachObjectTypeToSeparateDir,
					null);
			}
		}

		private DBBuilder.MSSQL.SqlObjectType GetSelectedObjectTypes(CheckedListBox listBox)
		{
			var selectedObjectTypes = DBBuilder.MSSQL.SqlObjectType.None;
			foreach (var item in listBox.CheckedItems)
			{
				selectedObjectTypes = selectedObjectTypes | 
				                      (DBBuilder.MSSQL.SqlObjectType)Enum.Parse(typeof (DBBuilder.MSSQL.SqlObjectType), item.ToString());
			}
			return selectedObjectTypes;
		}
	}
}
