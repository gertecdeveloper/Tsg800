using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Util;
using Android.Widget;
using GertecXamarinForms.Droid.Services.Fragment;
using System;
using Xamarin.Forms.Platform.Android;

namespace GertecXamarinForms.Droid.Services
{
    [Activity(Label = "Nfc", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class Nfc : FormsAppCompatActivity

    {
        private EditText editMesagemPadrao;
        private Button btn_ler;
        private Button btn_gravar;
        private Button btn_teste;
        private Button btn_formatarCartao;
        private NfcAdapter mNfcAdapter;

        private bool isDialogDisplayed = false;
        private bool isWrite = false;
        private bool isRead = false;
        private bool isFormat = false;        
        private bool isForceTeste = false;

        private NFCWriteFragment mNfcWriteFragment;
        private NFCReadFragment mNfcReadFragment;
        private NFCWriteReadFragment nfcWriteReadFragment;
        private NFCFormatFragment nfcFormatFragment;

        private static string MENSAGEM_PADRAO = "GERTEC";
        private int processo = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Nfc);

            editMesagemPadrao = FindViewById<EditText>(Resource.Id.editMensagemPadrao);
            btn_ler = FindViewById<Button>(Resource.Id.btnRead);
            btn_gravar = FindViewById<Button>(Resource.Id.btnRecord);
            btn_formatarCartao = FindViewById<Button>(Resource.Id.btnFormat);
            btn_teste = FindViewById<Button>(Resource.Id.btnTest);

            InitViews();
            InitNFC();
        }

        private void InitViews() {

            btn_gravar.Click += delegate
            {
                ShowWriteFragment();
            };

            btn_ler.Click += delegate
            {
                ShowReadFragment();
            };

            btn_formatarCartao.Click += delegate {
                ShowFormatFragment();
            };

            btn_teste.Click += delegate {
                ShowReadWriteFragment();
            };
        }

        private void InitNFC()
        {
            mNfcAdapter = NfcAdapter.GetDefaultAdapter(this);
        }

        private void ShowWriteFragment()
        {
            isWrite = true;
            isRead = false;
            isForceTeste = false;
            isFormat = false;

            mNfcWriteFragment = (NFCWriteFragment)FragmentManager.FindFragmentByTag(NFCWriteFragment.TAG);

            if (mNfcWriteFragment == null)
            {
                mNfcWriteFragment = NFCWriteFragment.newInstance();
            }
            mNfcWriteFragment.Show(FragmentManager, NFCWriteFragment.TAG);

        }

        private void ShowReadFragment()
        {
            isRead = true;
            isWrite = false;
            isForceTeste = false;
            isFormat = false;

            mNfcReadFragment = (NFCReadFragment)FragmentManager.FindFragmentByTag(NFCReadFragment.TAG);

            if (mNfcReadFragment == null)
            {
                mNfcReadFragment = NFCReadFragment.NewInstance();
            }
            mNfcReadFragment.Show(FragmentManager, NFCReadFragment.TAG);

        }

        private void ShowFormatFragment()
        {
            isFormat = true;
            isRead = false;
            isWrite = false;
            isForceTeste = false;

            nfcFormatFragment = (NFCFormatFragment)FragmentManager.FindFragmentByTag(NFCFormatFragment.TAG);

            if (nfcFormatFragment == null)
            {
                nfcFormatFragment = NFCFormatFragment.NewInstance();
            }
            nfcFormatFragment.Show(FragmentManager, NFCFormatFragment.TAG);

        }

        private void ShowReadWriteFragment()
        {
            isForceTeste = true;
            isFormat = false;
            isRead = false;
            isWrite = false;
            processo = 1000;

            nfcWriteReadFragment = (NFCWriteReadFragment)FragmentManager.FindFragmentByTag(NFCWriteReadFragment.TAG);

            if (nfcWriteReadFragment == null)
            {
                nfcWriteReadFragment = NFCWriteReadFragment.NewInstance();
            }
            nfcWriteReadFragment.Show(FragmentManager, NFCWriteReadFragment.TAG);
        }

        public void OnDialogDisplayed()
        {
            isDialogDisplayed = true;
        }

        public void OnDialogDismissed()
        {
            isDialogDisplayed = false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            IntentFilter tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            IntentFilter ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
            IntentFilter techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
            IntentFilter idDetected = new IntentFilter((NfcAdapter.ExtraAid));
            IntentFilter[] nfcIntentFilter = new IntentFilter[] { techDetected, tagDetected, ndefDetected, idDetected };

            PendingIntent pendingIntent = PendingIntent.GetActivity(
               this, 0, new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), 0);

            if (mNfcAdapter != null) { 
                mNfcAdapter.EnableForegroundDispatch(this, pendingIntent, nfcIntentFilter, null);
            }
        }

        public void OnPause()
        {
            base.OnPause();
            if (mNfcAdapter != null)
            {
                mNfcAdapter.DisableForegroundDispatch(this);
            }
        }

        [Obsolete]
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Tag tag = (Android.Nfc.Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);

            Log.Debug("TAG", "onNewIntent: " + intent.Action);

            if (tag != null)
            {
                var ndef = Ndef.Get(tag);
                if (isDialogDisplayed)
                {
                    if (ndef == null)
                    {
                        Toast.MakeText(Android.App.Application.Context, "Tipo de cartão não suportado.", ToastLength.Short).Show();
                    } 
                    else if (isWrite)
                    {
                        string messageToWrite = editMesagemPadrao.Text;
                        if(messageToWrite.Equals(""))
                        {
                            Toast.MakeText(Android.App.Application.Context, "Preencha uma mensagem", ToastLength.Short).Show();
                        } 
                        else
                        {
                            mNfcWriteFragment = (NFCWriteFragment)FragmentManager.FindFragmentByTag(NFCWriteFragment.TAG);
                            mNfcWriteFragment.OnNfcDetected(ndef, messageToWrite);
                        }
                    } 
                    else if (isRead)
                    {
                        mNfcReadFragment = (NFCReadFragment)FragmentManager.FindFragmentByTag(NFCReadFragment.TAG);
                        mNfcReadFragment.OnNfcDetected(ndef);
                    } 
                    else if (isFormat)
                    {
                        nfcFormatFragment = (NFCFormatFragment)FragmentManager.FindFragmentByTag(NFCFormatFragment.TAG);
                        nfcFormatFragment.OnNfcDetected(ndef);
                    }
                    else if (isForceTeste)
                    {
                        nfcWriteReadFragment = (NFCWriteReadFragment)FragmentManager.FindFragmentByTag(NFCWriteReadFragment.TAG);
                        nfcWriteReadFragment.onNfcDetected(ndef, MENSAGEM_PADRAO + processo);
                        processo--;
                    }
                }
            }
        }
    }
}
