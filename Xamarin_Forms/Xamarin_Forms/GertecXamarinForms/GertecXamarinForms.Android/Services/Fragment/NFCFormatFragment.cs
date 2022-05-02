using Android.App;
using Android.Content;
using Android.Nfc.Tech;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.IO;

namespace GertecXamarinForms.Droid.Services.Fragment
{
    [Obsolete]
    class NFCFormatFragment : DialogFragment
    {
        private NfcLeituraGravacao nfcLeituraGravacao;

        public static string TAG = "NFCFormatFragment";

        private TextView texMensagem;
        private Nfc mListener;

        public static NFCFormatFragment NewInstance()
        {
            return new NFCFormatFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_formata, container, false);
            InitViews(view);
            return view;
        }

        private void InitViews(View view)
        {
            texMensagem = view.FindViewById<TextView>(Resource.Id.tv_message);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            mListener = (Nfc)context;
            mListener.OnDialogDisplayed();
        }

        public override void OnDetach()
        {
            base.OnDetach();
            mListener.OnDialogDismissed();
        }

        public void OnNfcDetected(Ndef ndef)
        {
            nfcLeituraGravacao = new NfcLeituraGravacao(ndef.Tag);
            FormatFromNFC(ndef);
        }

        /*
        * Este método irá tentar fazer a formatação do atual cartão que esta
        * sendo lido pela leitora.
        *
        * @param ndef = contém as informações do cartão que acabou de ser lido.
        *
        * @exception IOException
        * @exception FormatException
        **/
        private void FormatFromNFC(Ndef ndef){
            bool retorno;
            try
            {
                retorno = nfcLeituraGravacao.FormataCartao(ndef);

                if (retorno)
                {
                    texMensagem.Text = "Cartão formatado";
                } else {
                    texMensagem.Text = "Nao é necessário formatar este cartão.";
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            } 
            catch (FormatException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
