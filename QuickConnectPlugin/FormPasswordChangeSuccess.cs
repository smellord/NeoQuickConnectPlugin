using System;
using System.Windows.Forms;

namespace QuickConnectPlugin {

    public partial class FormPasswordChangeSuccess : Form {

        public FormPasswordChangeSuccess(string message, string operationDetails) {
            InitializeComponent();

            this.labelMessage.Text = message;
            this.textBoxDetails.Text = operationDetails ?? string.Empty;
            this.buttonToggleDetails.Visible = !String.IsNullOrEmpty(operationDetails);
            this.textBoxDetails.Visible = false;
            this.Height = 150;
        }

        private void buttonOkClick(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonToggleDetailsClick(object sender, EventArgs e) {
            var showDetails = !this.textBoxDetails.Visible;
            this.textBoxDetails.Visible = showDetails;
            this.buttonToggleDetails.Text = showDetails ? "v View server log" : "> View server log";
            this.Height = showDetails ? 300 : 150;
        }
    }
}
