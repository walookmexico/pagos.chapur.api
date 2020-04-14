$(document).ready(function(){
  var total = 1000;
  //se valida si viene parametro por url
  if(window.location.search.substring(1))
  {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
    sURLVariables = sPageURL.split('&'),
    sParameterName,
    i;
    var total = sURLVariables[3].split('=')[1];
  }
  
  $('#total').val(total);
  
  $.ajax("http://localhost:57350/Token", {
    type:'POST',
    dataType: 'JSON',
    headers: { 'Content-Type':'application/x-www-form-urlencoded'},
    data:{
        'username': 'delossantosam@outlook.com',
        'password': 'password',
        'grant_type': 'password',
        'client_secret' : 'lkfjldsfjkld',
        'client_id' : '987459827985'
    },
    success: function (res) {
       // console.log(res.access_token);
        $('#token').val(res.access_token);
      }
    });
});



$(document).on('click','#btnEnviar',function(){
    var token = 'Bearer ' + $('#token').val();
    var count = obtieneValores();
    if(count > 0)
    {
      alert('Debe ingresar todos los valores');
      return false;
    }
    datos = {
        'NoCreditCard':$('#tarjeta').val(),
        'password': $('#password').val(),
        'cvv': $('#cvv').val(),
        'CreateDate' : '04/24/2018',
        'total' : $('#total').val(),
        'IdPurchaseOrder' : '1000',
        'IdStore' :'1'
    };
    console.log(token);
$.ajax("http://localhost:57350/api/v0/payments/save", {
  type:'POST',
  dataType: 'JSON',
  headers: { 'Content-Type':'application/json', 'X-API-KEY': '12345678-90', 'Authorization': token },
  data:JSON.stringify(datos),
  success: function (res) {
    alert('Respuesta: ' + res.Messages);
      console.log(res.Messages);
      $('#tarjeta').val('');
      $('#cvv').val('');
      $('#password').val('');
    }
  });
});

function obtieneValores()
{
    var count = 0;
    
    if($('#tarjeta').val() == '')
    {
      $('#mensaje1').css('display','block');
      count = count + 1;
    }else{
      $('#mensaje1').css('display','none');
     
    }

    if($('#cvv').val() == '')
    {
      $('#mensaje2').css('display','block');
      count = count + 1;
    }else{
      $('#mensaje2').css('display','none');

    }

    if($('#password').val() == '')
    {
      $('#mensaje3').css('display','block');
      count = count + 1;
    }else{
      $('#mensaje3').css('display','none');
      
    }
console.log('count: '+ count);
return count;
}