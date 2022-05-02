using System;
using System.Collections.Generic;
using System.Text;

namespace GertecXamarinForms.Controls.Sat
{
    public interface ISatAltera
    {
        void mostrarDialogo(string mensagem);

        void trocarCodAtivacao(string CodAtivacao, string opcao, string codAtivacaoNovo, string codConfirmacao, int numeroSessao);
    }
}
