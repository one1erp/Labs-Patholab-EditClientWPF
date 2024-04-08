using LSSERVICEPROVIDERLib;
using Patholab_DAL_V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Patholab_XmlService;
using System.Windows.Forms;

namespace EditClientUserCont
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditClientControl : System.Windows.Window
    {
        private INautilusServiceProvider _sp;
        private Dictionary<string, string> _genderDic;
        private CLIENT client;
        public event Action<CLIENT> PatientEdited;

        public EditClientControl(CLIENT client, Dictionary<string, string> genderDic, INautilusServiceProvider sp)
        {
            InitializeComponent();

            _genderDic = genderDic;
            _sp = sp;
            var cu = client.CLIENT_USER;
            textBoxID.Text = client.NAME;
            textBoxFirstName.Text = cu.U_FIRST_NAME;
            textBoxLastName.Text = cu.U_LAST_NAME;
            textBoxPrevLastName.Text = cu.U_PREV_LAST_NAME;

            if (cu.U_DATE_OF_BIRTH != null)
                datePicker.SelectedDate = cu.U_DATE_OF_BIRTH.Value;

            this.client = client;

            comboBoxGender.ItemsSource = new System.Windows.Forms.BindingSource(genderDic, null);
            comboBoxGender.DisplayMemberPath = "Value";
            comboBoxGender.SelectedValuePath = "Key";
            comboBoxGender.SelectedItem = _genderDic.FirstOrDefault(x => x.Key == cu.U_GENDER);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            var axc = ((System.Collections.Generic.KeyValuePair<string, string>)
                (comboBoxGender.SelectedItem))
                     .Key;

            UpdateStaticEntity upcl = new UpdateStaticEntity(_sp);
            upcl.Login("CLIENT", "Client", FindBy.Name, textBoxID.Text.Trim());
            upcl.AddProperties("U_FIRST_NAME", textBoxFirstName.Text);
            upcl.AddProperties("U_LAST_NAME", textBoxLastName.Text);
            upcl.AddProperties("U_PREV_LAST_NAME", textBoxPrevLastName.Text);

            upcl.AddProperties("U_DATE_OF_BIRTH", datePicker.SelectedDate.ToString());
            upcl.AddProperties("U_GENDER", axc);// radDropDownList1.SelectedValue.ToString());//להכניס KEY TODO:

            var s = upcl.ProcssXml();
            if (!s)
            {

                MessageBox.Show(string.Format("Error on Edit Patient  {0}", upcl.ErrorResponse), "Edit Patient", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("הפציינט עודכן במערכת.");

                var cu = client.CLIENT_USER;

                cu.U_FIRST_NAME = upcl.GetValueByTagName("U_FIRST_NAME");
                cu.U_LAST_NAME = upcl.GetValueByTagName("U_LAST_NAME");
                cu.U_PREV_LAST_NAME = upcl.GetValueByTagName("U_PREV_LAST_NAME");
                cu.U_GENDER = upcl.GetValueByTagName("U_GENDER");
                cu.U_DATE_OF_BIRTH = datePicker.SelectedDate;//DateTime.Parse(upcl.GetValueByTagName("U_DATE_OF_BIRTH"); 

                if (PatientEdited != null)
                    PatientEdited(client);

                this.Close();

            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            var dg = MessageBox.Show("האם אתה בטוח שברצונך לצאת?", "Nautilus - עדכון פציינט", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dg == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }

        }
    }
}
