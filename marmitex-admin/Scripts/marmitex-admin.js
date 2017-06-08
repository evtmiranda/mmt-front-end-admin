/**
 * evento da página de pedido, para marcar que uma determinada div ou objeto pode receber qualquer drop
 * @param {any} ev ev
 */
function allowDrop(ev) {
    ev.preventDefault();
}

/**
 * evento utilizado nos objetos que podem ser arrastados e soltados em outros lugares
 * @param {any} ev ev
 */
function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

/**
 * função utilizada na página de pedidos. quando um pedido é solto na sessão "entregue", este evento é executado.
    este evento é responsável por atualizar o pedido como entregue no banco de dados
 * @param {any} ev ev
 */
function dropEntregue(ev) {
    //captura o id do objeto que foi dropado aqui
    ev.preventDefault();
    var nomeDiv = ev.dataTransfer.getData("text");

    //faz um post no endpoint de pedidos para setar como entregue
    var trataIdPedido = nomeDiv.split('/');
    var tamVet = trataIdPedido.length-1;

    var dadosPedido = new Object();
    dadosPedido.IdPedido = trataIdPedido[tamVet];
    dadosPedido.IdStatusPedido = 1;

    var urlPost = "/Pedido/AtualizarStatusPedido";
    var dadosPedidoJson = JSON.stringify(dadosPedido, null, 0);

    $.post(urlPost, { dadosJson: dadosPedidoJson },
        function (data) {
            if (data == "erro") {
                swal(
                    'Oops...',
                    'não foi possível atualizar o pedido para entregue. por favor, tente novamente ou entre em contato com o administrador',
                    'error'
                )
            }
            else {
                //ev.target.appendChild(document.getElementById(nomeDiv));
                //recarrega o html da página
                window.location.reload();
            }
        }
    );
}

/**
 * função utilizada na página de pedidos. quando um pedido é solto na sessão "fila", este evento é executado.
    este evento é responsável por atualizar o pedido como "na fila" no banco de dados
 * @param {any} ev ev
 */
function dropFila(ev) {
    //captura o id do objeto que foi dropado aqui
    ev.preventDefault();
    var nomeDiv = ev.dataTransfer.getData("text");

    //faz um post no endpoint de pedidos para setar como fila
    var trataIdPedido = nomeDiv.split('/');
    var tamVet = trataIdPedido.length - 1;

    var dadosPedido = new Object();
    dadosPedido.IdPedido = trataIdPedido[tamVet];
    dadosPedido.IdStatusPedido = 0;

    var urlPost = "/Pedido/AtualizarStatusPedido";
    var dadosPedidoJson = JSON.stringify(dadosPedido, null, 0);

    $.post(urlPost, { dadosJson: dadosPedidoJson },
        function (data) {
            if (data == "erro") {
                swal(
                    'Oops...',
                    'não foi possível voltar o pedido para fila. por favor, tente novamente ou entre em contato com o administrador',
                    'error'
                )
            }
            else {
                //ev.target.appendChild(document.getElementById(nomeDiv));
                //recarrega o html da página
                window.location.reload();
            }
        }
    );
}

/**
 * função utilizada na página de pedidos. quando um pedido é solto na sessão "cancelado", este evento é executado.
    este evento é responsável por atualizar o pedido como "cancelado" no banco de dados
 * @param {any} ev ev
 */
function dropCancelado(ev) {

    //captura o id do objeto que foi dropado aqui
    ev.preventDefault();
    var nomeDiv = ev.dataTransfer.getData("text");

    //faz um post no endpoint de pedidos para setar como cancelado
    var trataIdPedido = nomeDiv.split('/');
    var tamVet = trataIdPedido.length - 1;

    var dadosPedido = new Object();
    dadosPedido.IdPedido = trataIdPedido[tamVet];
    dadosPedido.IdStatusPedido = 2;

    var urlPost = "/Pedido/AtualizarStatusPedido";
    var dadosPedidoJson = JSON.stringify(dadosPedido, null, 0);

    swal({
        title: 'Confirma o cancelamento do pedido?',
        text: "Ao cancelar, o cliente será notificado.",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#f9',
        cancelButtonColor: '#f9',
        cancelButtonText: 'Voltar',
        confirmButtonText: 'Sim, cancelar!'
    }).then(function () {
        $.post(urlPost, { dadosJson: dadosPedidoJson },
            function (data) {
                if (data == "erro") {
                    swal(
                        'Oops...',
                        'não foi possível cancelar o pedido. por favor, tente novamente ou entre em contato com o administrador',
                        'error'
                    )
                }
                else {
                    //ev.target.appendChild(document.getElementById(nomeDiv));
                    //recarrega o html da página
                    window.location.reload();
                }
            }
        );

    });


}

/**
 * 1. Exibe mensagem de confirmação de exclusão
   2. Se a exclusão for confirmada, faz um get na url. Se não for, sai da função
   3. Se o retorno do get for sucesso, exibe mensagem amigável e recarrega a página. Se for erro, exibe mensagem amigável
 * @param {any} url
 */
function Excluir(url) {
    swal({
        title: 'Confirma a exclusão?',
        html: "Ao excluir, não será possível desfazer a ação",
        type: 'warning',
        showCancelButton: true,
        cancelButtonText: 'Voltar',
        confirmButtonText: 'Sim, excluir!'
    }).then(function () {
        $.getJSON(url, { id: null }, function (result) {
            if (!result.success) {
                swal(
                    'Oops',
                    'não foi possível excluir. por favor, tente novamente ou entre em contato com o administrador',
                    'error'
                )
            }
            else {
                swal({
                    title: 'Sucesso',
                    html: "Exclusão realizada com sucesso.",
                    type: 'success',
                    confirmButtonText: 'Ok'
                }).then(function () {
                    //recarrega a pagina
                    window.location.reload();
                });
            }
        });
    });
}

/**
 * 1. vai para a url recebida
 * @param {any} url
 */
function IrParaUrl(url) {
    window.location.href = url;
}

/**
 * 1. Faz um get na url
   2. Se o retorno for sucesso, recarrega a página. Se for erro, exibe mensagem amigável
 * @param {any} url
 */
function Get(url) {
    $.getJSON(url, { id: null }, function (result) {
        if (!result.success) {
            swal(
                'Oops...',
                'Ocorreu um erro. por favor, tente novamente ou entre em contato com o administrador',
                'error'
            )
        }
        else {
            //recarrega o html da página
            window.location.reload();
        }
    })
}

function encode_utf8(s) {
    return unescape(encodeURIComponent(s));
}

function decode_utf8(s) {
    return decodeURIComponent(escape(s));
}

//mascaras
function formataMascara(campo, evt, formato) {
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;

    var result = "";
    var maskIdx = formato.length - 1;
    var error = false;
    var valor = campo.value;
    var posFinal = false;
    if (campo.setSelectionRange) {
        if (campo.selectionStart == valor.length)
            posFinal = true;
    }
    valor = valor.replace(/[^0123456789Xx]/g, '');
    for (var valIdx = valor.length - 1; valIdx >= 0 && maskIdx >= 0; --maskIdx) {
        var chr = valor.charAt(valIdx);
        var chrMask = formato.charAt(maskIdx);
        switch (chrMask) {
            case '#':
                if (!(/\d/.test(chr)))
                    error = true;
                result = chr + result;
                --valIdx;
                break;
            case '@':
                result = chr + result;
                --valIdx;
                break;
            default:
                result = chrMask + result;
        }
    }
    campo.value = result;
    campo.style.color = error ? 'red' : '';
    if (posFinal) {
        campo.selectionStart = result.length;
        campo.selectionEnd = result.length;
    }
    return result;
}
// Formata o campo valor monetÃ¡rio
function formataValor(campo, evt) {
    //1.000.000,00
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    if (vr.length > 0) {
        vr = parseFloat(vr.toString()).toString();
        tam = vr.length;
        if (tam == 1)
            campo.value = "0,0" + vr;
        if (tam == 2)
            campo.value = "0," + vr;
        if ((tam > 2) && (tam <= 5)) {
            campo.value = vr.substr(0, tam - 2) + ',' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 6) && (tam <= 8)) {
            campo.value = vr.substr(0, tam - 5) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 9) && (tam <= 11)) {
            campo.value = vr.substr(0, tam - 8) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 12) && (tam <= 14)) {
            campo.value = vr.substr(0, tam - 11) + '.' + vr.substr(tam - 11, 3) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
        }
        if ((tam >= 15) && (tam <= 18)) {
            campo.value = vr.substr(0, tam - 14) + '.' + vr.substr(tam - 14, 3) + '.' + vr.substr(tam - 11, 3) + '.' + vr.substr(tam - 8, 3) + '.' + vr.substr(tam - 5, 3) + ',' + vr.substr(tam - 2, tam);
        }
    }
    MovimentaCursor(campo, xPos);
}
// Formata data no padrÃ£o DD/MM/YYYY
function formataData(campo, evt) {
    var xPos = PosicaoCursor(campo);
    //dd/MM/yyyy
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2 && tam < 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2);
    if (tam == 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/';
    if (tam > 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4);
    MovimentaCursor(campo, xPos);

    /*Valida a Data*/
    day = data.substring(0, 2);
    month = data.substring(3, 5);
    year = data.substring(6, 10);

    if (year <= 1920) {
        alert('Data Nascimento Inválida');
    }

    if ((month == 01) || (month == 03) || (month == 05) || (month == 07) || (month == 08) || (month == 10) || (month == 12)) {//mes com 31 dias
        if ((day < 01) || (day > 31)) {
            alert('Data Nascimento Inválida');
        }
    } else

        if ((month == 04) || (month == 06) || (month == 09) || (month == 11)) {//mes com 30 dias
            if ((day < 01) || (day > 30)) {
                alert('Data Nascimento Inválida');
            }
        } else

            if ((month == 02)) {//February and leap year
                if ((year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0))) {
                    if ((day < 01) || (day > 29)) {
                        alert('Data Nascimento Inválida');
                    }
                } else {
                    if ((day < 01) || (day > 28)) {
                        alert('Data Nascimento Inválida');
                    }
                }
            }

}
//descobre qual a posiÃ§Ã£o do cursor no campo
function PosicaoCursor(textarea) {
    var pos = 0;
    if (typeof (document.selection) != 'undefined') {
        //IE
        var range = document.selection.createRange();
        var i = 0;
        for (i = textarea.value.length; i > 0; i--) {
            if (range.moveStart('character', 1) == 0)
                break;
        }
        pos = i;
    }
    if (typeof (textarea.selectionStart) != 'undefined') {
        //FireFox
        pos = textarea.selectionStart;
    }
    if (pos == textarea.value.length)
        return 0; //retorna 0 quando nÃ£o precisa posicionar o elemento
    else
        return pos; //posiÃ§Ã£o do cursor
}
// move o cursor para a posiÃ§Ã£o pos
function MovimentaCursor(textarea, pos) {
    if (pos <= 0)
        return; //se a posiÃ§Ã£o for 0 nÃ£o reposiciona
    if (typeof (document.selection) != 'undefined') {
        //IE
        var oRange = textarea.createTextRange();
        var LENGTH = 1;
        var STARTINDEX = pos;
        oRange.moveStart("character", -textarea.value.length);
        oRange.moveEnd("character", -textarea.value.length);
        oRange.moveStart("character", pos);
        //oRange.moveEnd("character", pos);
        oRange.select();
        textarea.focus();
    }
    if (typeof (textarea.selectionStart) != 'undefined') {
        //FireFox
        textarea.selectionStart = pos;
        textarea.selectionEnd = pos;
    }
}
//Formata data e hora no padrÃ£o DD/MM/YYYY HH:MM
function formataDataeHora(campo, evt) {
    xPos = PosicaoCursor(campo);
    //dd/MM/yyyy
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2 && tam < 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2);
    if (tam == 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/';
    if (tam > 4)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4);
    if (tam > 8 && tam < 11)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4, 4) + ' ' + vr.substr(8, 2);
    if (tam >= 11)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2, 2) + '/' + vr.substr(4, 4) + ' ' + vr.substr(8, 2) + ':' + vr.substr(10);
    campo.value = campo.value.substr(0, 16);
    //    if(xPos == 2 || xPos == 5)
    //        xPos = xPos +1;
    //    if(xPos == 11 || xPos == 14)
    //        xPos = xPos +2;
    MovimentaCursor(campo, xPos);
}
// Formata sÃ³ nÃºmeros
function formataInteiro(campo, evt) {
    //1234567890
    xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    campo.value = filtraNumeros(filtraCampo(campo));
    MovimentaCursor(campo, xPos);
}
// Formata hora no padrao HH:MM
function formataHora(campo, evt) {
    //HH:mm
    xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    if (tam == 2)
        campo.value = vr.substr(0, 2) + ':';
    if (tam > 2 && tam < 5)
        campo.value = vr.substr(0, 2) + ':' + vr.substr(2);
    //    if(xPos == 2)
    //        xPos = xPos + 1;
    MovimentaCursor(campo, xPos);
}
// limpa todos os caracteres especiais do campo solicitado
function filtraCampo(campo) {
    var s = "";
    var cp = "";
    vr = campo.value;
    tam = vr.length;
    for (i = 0; i < tam; i++) {
        if (vr.substring(i, i + 1) != "/"
            && vr.substring(i, i + 1) != "-"
            && vr.substring(i, i + 1) != "."
            && vr.substring(i, i + 1) != "("
            && vr.substring(i, i + 1) != ")"
            && vr.substring(i, i + 1) != ":"
            && vr.substring(i, i + 1) != ",") {
            s = s + vr.substring(i, i + 1);
        }
    }
    return s;
    //return campo.value.replace("/", "").replace("-", "").replace(".", "").replace(",", "")
}
// limpa todos caracteres que nÃ£o sÃ£o nÃºmeros
function filtraNumeros(campo) {
    var s = "";
    var cp = "";
    vr = campo;
    tam = vr.length;
    for (i = 0; i < tam; i++) {
        if (vr.substring(i, i + 1) == "0" ||
            vr.substring(i, i + 1) == "1" ||
            vr.substring(i, i + 1) == "2" ||
            vr.substring(i, i + 1) == "3" ||
            vr.substring(i, i + 1) == "4" ||
            vr.substring(i, i + 1) == "5" ||
            vr.substring(i, i + 1) == "6" ||
            vr.substring(i, i + 1) == "7" ||
            vr.substring(i, i + 1) == "8" ||
            vr.substring(i, i + 1) == "9") {
            s = s + vr.substring(i, i + 1);
        }
    }
    return s;
    //return campo.value.replace("/", "").replace("-", "").replace(".", "").replace(",", "")
}
// limpa todos caracteres que nÃ£o sÃ£o letras
function filtraCaracteres(campo) {
    vr = campo;
    for (i = 0; i < tam; i++) {
        //Caracter
        if (vr.charCodeAt(i) != 32 && vr.charCodeAt(i) != 94 && (vr.charCodeAt(i) < 65 ||
            (vr.charCodeAt(i) > 90 && vr.charCodeAt(i) < 96) ||
            vr.charCodeAt(i) > 122) && vr.charCodeAt(i) < 192) {
            vr = vr.replace(vr.substr(i, 1), "");
        }
    }
    return vr;
}
// limpa todos caracteres que nÃ£o sÃ£o nÃºmeros, menos a vÃ­rgula
function filtraNumerosComVirgula(campo) {
    var s = "";
    var cp = "";
    vr = campo;
    tam = vr.length;
    var complemento = 0; //flag paga contar o nÃºmero de virgulas
    for (i = 0; i < tam; i++) {
        if ((vr.substring(i, i + 1) == "," && complemento == 0 && s != "") ||
            vr.substring(i, i + 1) == "0" ||
            vr.substring(i, i + 1) == "1" ||
            vr.substring(i, i + 1) == "2" ||
            vr.substring(i, i + 1) == "3" ||
            vr.substring(i, i + 1) == "4" ||
            vr.substring(i, i + 1) == "5" ||
            vr.substring(i, i + 1) == "6" ||
            vr.substring(i, i + 1) == "7" ||
            vr.substring(i, i + 1) == "8" ||
            vr.substring(i, i + 1) == "9") {
            if (vr.substring(i, i + 1) == ",")
                complemento = complemento + 1;
            s = s + vr.substring(i, i + 1);
        }
    }
    return s;
}
function formataMesAno(campo, evt) {
    //MM/yyyy
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2)
        campo.value = vr.substr(0, 2) + '/' + vr.substr(2);
    MovimentaCursor(campo, xPos);
}
function formataCNPJ(campo, evt) {
    //99.999.999/9999-99
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 2 && tam < 5)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2);
    else if (tam >= 5 && tam < 8)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5);
    else if (tam >= 8 && tam < 12)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8);
    else if (tam >= 12)
        campo.value = vr.substr(0, 2) + '.' + vr.substr(2, 3) + '.' + vr.substr(5, 3) + '/' + vr.substr(8, 4) + '-' + vr.substr(12);
    MovimentaCursor(campo, xPos);
}
function formataCPF(campo, evt) {
    //999.999.999-99
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam >= 3 && tam < 6)
        campo.value = vr.substr(0, 3) + '.' + vr.substr(3);
    else if (tam >= 6 && tam < 9)
        campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6);
    else if (tam >= 9)
        campo.value = vr.substr(0, 3) + '.' + vr.substr(3, 3) + '.' + vr.substr(6, 3) + '-' + vr.substr(9);
    MovimentaCursor(campo, xPos);
}
function formataDouble(campo, evt) {
    //18,53012
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    campo.value = filtraNumerosComVirgula(campo.value);
    MovimentaCursor(campo, xPos);
}
function formataTelefone(campo, evt) {
    //(00) 0000-0000
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam == 1)
        campo.value = '(' + vr;
    else if (tam >= 2 && tam < 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2);
    else if (tam >= 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2, 5) + '-' + vr.substr(7, 4);
    //(
    //    if(xPos == 1 || xPos == 3 || xPos == 5 || xPos == 9)
    //        xPos = xPos +1
    MovimentaCursor(campo, xPos);
}

function formataTelefonefixo(campo, evt) {
    //(00) 0000-0000
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam == 1)
        campo.value = '(' + vr;
    else if (tam >= 2 && tam < 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2);
    else if (tam >= 6)
        campo.value = '(' + vr.substr(0, 2) + ') ' + vr.substr(2, 4) + '-' + vr.substr(6, 4);
    //(
    //    if(xPos == 1 || xPos == 3 || xPos == 5 || xPos == 9)
    //        xPos = xPos +1
    MovimentaCursor(campo, xPos);
}

function validaAno(campo) {
    day = data.substring(0, 2);
    month = data.substring(3, 5);
    year = data.substring(6, 10);

    if ((month == 01) || (month == 03) || (month == 05) || (month == 07) || (month == 08) || (month == 10) || (month == 12)) {//mes com 31 dias
        if ((day < 01) || (day > 31)) {
            alert('invalid date');
        }
    } else

        if ((month == 04) || (month == 06) || (month == 09) || (month == 11)) {//mes com 30 dias
            if ((day < 01) || (day > 30)) {
                alert('invalid date');
            }
        } else

            if ((month == 02)) {//February and leap year
                if ((year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0))) {
                    if ((day < 01) || (day > 29)) {
                        alert('invalid date');
                    }
                } else {
                    if ((day < 01) || (day > 28)) {
                        alert('invalid date');
                    }
                }
            }

}

function formataTexto(campo, evt, sMascara) {
    //Nome com Inicial Maiuscula.
    evt = getEvent(evt);
    xPos = PosicaoCursor(campo);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraCaracteres(filtraCampo(campo));
    tam = vr.length;
    if (sMascara == "Aa" || sMascara == "Xx") {
        var valor = campo.value.toLowerCase();
        var count = campo.value.split(" ").length - 1;
        var i;
        var pos = 0;
        var valorIni;
        var valorMei;
        var valorFim;
        valor = valor.substring(0, 1).toUpperCase() + valor.substring(1, valor.length);
        for (i = 0; i < count; i++) {
            pos = valor.indexOf(" ", pos + 1);
            valorIni = valor.substring(0, valor.indexOf(" ", pos - 1)) + " ";
            valorMei = valor.substring(valor.indexOf(" ", pos) + 1, valor.indexOf(" ", pos) + 2).toUpperCase();
            valorFim = valor.substring(valor.indexOf(" ", pos) + 2, valor.length);
            valor = valorIni + valorMei + valorFim;
        }
        campo.value = valor;
    }
    if (sMascara == "Aaa" || sMascara == "Xxx") {
        var valor = campo.value.toLowerCase();
        var count = campo.value.split(" ").length - 1;
        var i;
        var pos = 0;
        var valorIni;
        var valorMei;
        var valorFim;
        var ligacao = false;
        var chrLigacao = Array("de", "da", "do", "para", "e")
        valor = valor.substring(0, 1).toUpperCase() + valor.substring(1, valor.length);
        for (i = 0; i < count; i++) {
            ligacao = false;
            pos = valor.indexOf(" ", pos + 1);
            valorIni = valor.substring(0, valor.indexOf(" ", pos - 1)) + " ";
            for (var a = 0; a < chrLigacao.length; a++) {
                if (valor.substring(valorIni.length, valor.indexOf(" ", valorIni.length)).toLowerCase() == chrLigacao[a].toLowerCase()) {
                    ligacao = true;
                    break;
                }
                else if (ligacao == false && valor.indexOf(" ", valorIni.length) == -1) {
                    if (valor.substring(valorIni.length, valor.length).toLowerCase() == chrLigacao[a].toLowerCase()) {
                        ligacao = true;
                        break;
                    }
                }
            }
            if (ligacao == true) {
                valorMei = valor.substring(valor.indexOf(" ", pos) + 1, valor.indexOf(" ", pos) + 2).toLowerCase();
            }
            else {
                valorMei = valor.substring(valor.indexOf(" ", pos) + 1, valor.indexOf(" ", pos) + 2).toUpperCase();
            }
            valorFim = valor.substring(valor.indexOf(" ", pos) + 2, valor.length);
            valor = valorIni + valorMei + valorFim;
        }
        campo.value = valor;
    }
    MovimentaCursor(campo, xPos);
    return true;
}
// Formata o campo CEP
function formataCEP(campo, evt) {
    //312555-650
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    vr = campo.value = filtraNumeros(filtraCampo(campo));
    tam = vr.length;
    if (tam < 5)
        campo.value = vr;
    else if (tam == 5)
        campo.value = vr + '-';
    else if (tam > 5)
        campo.value = vr.substr(0, 5) + '-' + vr.substr(5);
    MovimentaCursor(campo, xPos);
}

function formataCartaoCredito(campo, evt) {
    //0000.0000.0000.0000
    var xPos = PosicaoCursor(campo);
    evt = getEvent(evt);
    var tecla = getKeyCode(evt);
    if (!teclaValida(tecla))
        return;
    var vr = campo.value = filtraNumeros(filtraCampo(campo));
    var tammax = 16;
    var tam = vr.length;
    if (tam < tammax && tecla != 8)
    { tam = vr.length + 1; }
    if (tam < 5)
    { campo.value = vr; }
    if ((tam > 4) && (tam < 9))
    { campo.value = vr.substr(0, 4) + '.' + vr.substr(4, tam - 4); }
    if ((tam > 8) && (tam < 13))
    { campo.value = vr.substr(0, 4) + '.' + vr.substr(4, 4) + '.' + vr.substr(8, tam - 4); }
    if (tam > 12)
    { campo.value = vr.substr(0, 4) + '.' + vr.substr(4, 4) + '.' + vr.substr(8, 4) + '.' + vr.substr(12, tam - 4); }
    MovimentaCursor(campo, xPos);
}

//recupera tecla
//evita criar mascara quando as teclas sÃ£o pressionadas
function teclaValida(tecla) {
    if (tecla == 8 //backspace
        //Esta evitando o post, quando sÃ£o pressionadas estas teclas.
        //Foi comentado pois, se for utilizado o evento texchange, Ã© necessario o post.
        || tecla == 9 //TAB
        || tecla == 27 //ESC
        || tecla == 16 //Shif TAB 
        || tecla == 45 //insert
        || tecla == 46 //delete
        || tecla == 35 //home
        || tecla == 36 //end
        || tecla == 37 //esquerda
        || tecla == 38 //cima
        || tecla == 39 //direita
        || tecla == 40)//baixo
        return false;
    else
        return true;
}
// recupera o evento do form
function getEvent(evt) {
    if (!evt) evt = window.event; //IE
    return evt;
}
//Recupera o cÃ³digo da tecla que foi pressionado
function getKeyCode(evt) {
    var code;
    if (typeof (evt.keyCode) == 'number')
        code = evt.keyCode;
    else if (typeof (evt.which) == 'number')
        code = evt.which;
    else if (typeof (evt.charCode) == 'number')
        code = evt.charCode;
    else
        return 0;
    return code;
}