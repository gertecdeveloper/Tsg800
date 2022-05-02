using GertecXamarinForms.Controls.Sat;
using GertecXamarinForms.Controls.Sat.ServicesSat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GertecXamarinForms.Views.Sat
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Associar : ContentPage
    {
        public Associar()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            cnpjContribuinte.Text = "03.654.119/0001-76";
            cnpjSH.Text = "16.716.114/0001-72";
            txtAssinatura.Text = "SGR-SAT SISTEMA DE GESTAO E RETAGUARDA DO SAT";
            txtCodAtivacao.Text = GlobalValuesSat.codigoAtivacao;
        }

        private void btnAssociar(object sender, EventArgs e)
        {
            if (!UteisSat.VerificaCodigoAtivacao(txtCodAtivacao.Text))
            {
                DependencyService.Get<ISatAssociar>().mostrarDialogo("Código de Ativação deve ter entre 8 a 32 caracteres!");
            }

            if (string.IsNullOrEmpty(txtAssinatura.Text))
            {
                DependencyService.Get<ISatAssociar>().mostrarDialogo("Assinatura AC Inválida!");
            }

            DependencyService.Get<ISatAssociar>().associarSat(UteisSat.SomenteNumeros(cnpjContribuinte.Text),
                                                    UteisSat.SomenteNumeros(cnpjSH.Text), txtCodAtivacao.Text,
                                                    txtAssinatura.Text, UteisSat.GerarNumeroSessao);
        
        }
    }
}