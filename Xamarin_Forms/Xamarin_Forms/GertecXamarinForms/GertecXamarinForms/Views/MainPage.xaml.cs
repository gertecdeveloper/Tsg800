using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using GertecXamarinForms.Controls;
namespace GertecXamarinForms.Views
{

    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void codiBarras(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LeitorCodigoBarras());
        }

        private void codiBarrasV2(object sender, EventArgs e)
        {
            Navigation.PushAsync(new LeitorCodigoBarrasV2());
        }

        private void imprimir(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Impressão());
        }

        private void Nfc(object sender, EventArgs e)
        {
            DependencyService.Get<INfc>().Nfc();
        }
        private void sat(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MenuSat());
            //System.Console.WriteLine("Teste seleção Sat");            
        }


    }
}
