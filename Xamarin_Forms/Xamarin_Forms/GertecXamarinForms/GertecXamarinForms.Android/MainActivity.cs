using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using GertecXamarinForms.Droid;

using Plugin.DeviceInfo;


using System;
using GertecXamarinForms.Controls;
using Xamarin.Forms;
using GertecXamarinForms.Droid.Impressao;
using GertecXamarinForms.Droid.Services;
using System.Linq;
using System.IO;
using Android.Hardware.Usb;
using GertecXamarinForms.Droid.SAT.ServiceSat;
using GertecXamarinForms.Controls.Sat;
using GertecXamarinAndroid.Impressao;

[assembly: Android.App.UsesPermission(Android.Manifest.Permission.Flashlight)]
[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace GertecXamarinForms.Droid
{
    [Activity(Label = "GertecOne XamarinForms", Icon = "@drawable/Imagem1", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation),
         MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter"),
        IntentFilter(new[] { "android.hardware.usb.action.USB_DEVICE_ATTACHED", "android.intent.action.MAIN" })]
    public class MainActivity : 
        global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,
        INfc
    {
        public static Activity mContext;
        public Context context;

        public static ConfigPrint configPrint;
        public static GertecPrinter printer;
        public static SatFunctions satFunctions;

        public static string modelo;

        
        private Activity mainActivity;
        

        public MainActivity()
        {
            this.mainActivity = this;
            this.context = Android.App.Application.Context;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            
            printer = new GertecPrinter(mainActivity);
            configPrint = new ConfigPrint();
            printer.setConfigImpressao(configPrint);
            satFunctions = new SatFunctions(context);

            modelo = CrossDeviceInfo.Current.Model;

            // ZXing Inicialização
            global::ZXing.Net.Mobile.Forms.Android.Platform.Init();

            //LoadApplication is a Xamarin.Forms method
            LoadApplication(new App());

            mContext = this;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }             

        public void Nfc()
        {
            Intent myIntent = new Intent(this.context, typeof(Nfc));
            myIntent.AddFlags(ActivityFlags.NewTask);
            this.context.StartActivity(myIntent);
        }

        public void ativacaoSat(string txtCodAtivacao, string txtCNPJContribuinte, string txtCodConfirmacao)
        {
            throw new NotImplementedException();
        }
    }
}
