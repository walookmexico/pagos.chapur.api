<?php
include_once 'lib/nusoap.php';
$servicio = new soap_server();

$servicio->soap_defencoding = 'UTF-8';
$servicio->decode_utf8 = false;
$servicio->encode_utf8 = true;

$namespace = "urn:serviciopruebapagowsdl";
$servicio->configureWSDL("ServicioPruebaPago",$namespace);
$servicio->schemaTargetNamespace = $namespace;


// Parametros de Salida
$servicio->wsdl->addComplexType('datos_persona_salidad', 
                                'complexType', 
                                'struct', 
                                'all', 
                                '',
                                array('edoCompra'   => array('name' => 'edoCompra','type' => 'xsd:string'),
                                      'IdPago'   => array('name' => 'IdPago','type' => 'xsd:int'),
                                      'IdUsuario'   => array('name' => 'IdUsuario','type' => 'xsd:int'),
                                      'IdMovimiento'   => array('name' => 'IdMovimiento','type' => 'xsd:int'),
                                      'Estado'   => array('name' => 'Estado','type' => 'xsd:boolean'),
                                      'mensaje' => array('name' => 'mensaje', 'type'=> 'xsd:string'),
                                      'CodError'   => array('name' => 'CodError','type' => 'xsd:string'),
                                      'EstadoError'   => array('name' => 'EstadoError','type' => 'xsd:boolean'))
                              );
//Registro del metodo
$servicio->register("ProcesaPago", 
                      array('NoTarjeta' => 'xsd:string', 
                            'cvv' => 'xsd:string', 
                            'password' => 'xsd:string',
                            'fechaRegistro' => 'xsd:date', 
                            'montoCompra' => 'xsd:float', 
                            'IdOrdenCompra' => 'xsd:int', 
                            'IdTienda' => 'xsd:int'), 
                      array('return' =>'tns:datos_persona_salidad'), 
                      $namespace);


//-------------------
function ProcesaPago($NoTarjeta, $cvv, $password, $fechaRegistro, $montoCompra, $IdOrdenCompra, $IdTienda)
{
  //validamos que no vengan campos vacios
  if(empty($NoTarjeta) || empty($cvv) || empty($password) || empty($fechaRegistro) || empty($montoCompra) || empty($IdOrdenCompra) || empty($IdTienda))
  {
    return array(
                  'mensaje' => 'Error interno',
                  'CodError' => '500',
                  'EstadoError' => 0
    );
  }

    //llaves para encriptar
    $key = "8080808080CHAPUR";
    $iv = "8080808080CHAPUR";
    $crypto = new CryptoMessage;
    // //Quitar la encriptacion al No. de tarjeta   
    $textTarjeta = $crypto->decrypt($key, $iv, $NoTarjeta);
    $strPad = ord($textTarjeta[strlen($textTarjeta)-1]);
    $tarjeta = substr($textTarjeta, 0, -$strPad);

    // // //Quitar la encriptacion al cvv   
    $textCvv = $crypto->decrypt($key, $iv, $cvv);
    $strPad = ord($textCvv[strlen($textCvv)-1]);
    $cvvTrjeta = substr($textCvv, 0, -$strPad);

    // //Quitar la encriptacion al password  
    $textPass = $crypto->decrypt($key, $iv, $password);
    $strPad = ord($textPass[strlen($textPass)-1]);
    $pass = substr($textPass, 0, -$strPad);

    //mensaje para verificar que se quito la encriptación correctamente
    $msg = ' Hemos procesado la siguiente informacion: No. de tarjeta: ' . $tarjeta . ', CVV:'. $cvvTrjeta.' y contraseña: ' . $pass . '.'; 
    //respuesta final a la petición del ser servicio
    return array('edoCompra' => 'exito',
                  'IdPago' => 10,
                  'IdUsuario' => 4000,
                  'IdMovimiento' =>1000,
                  'Estado' => true,
                  'mensaje' => $msg);
}

$HTTP_RAW_POST_DATA = isset($HTTP_RAW_POST_DATA) ? $HTTP_RAW_POST_DATA : '';
$servicio->service($HTTP_RAW_POST_DATA);
//Linea para php 7
//$servicio->service(file_get_contents("php://input")); 

//-------------------
class CryptoMessage
{
  protected $mcrypt_cipher = MCRYPT_RIJNDAEL_128;
  protected $mcrypt_mode = MCRYPT_MODE_CBC;

  public function decrypt($key, $iv, $encrypted)
  {
    $iv_utf = mb_convert_encoding($iv, 'UTF-8');
    return mcrypt_decrypt($this->mcrypt_cipher, $key, base64_decode($encrypted), $this->mcrypt_mode, $iv_utf);
  }
  
  public function encrypt($key, $iv, $text)
  {
    $block = mcrypt_get_block_size($this->mcrypt_cipher, $this->mcrypt_mode);
    $padding = $block - (strlen($text) % $block);
    $text .= str_repeat(chr($padding), $padding);
    $crypttext =  mcrypt_encrypt($this->mcrypt_cipher, $key, $text, $this->mcrypt_mode, $iv);

    $crypttext64=base64_encode($crypttext);
    return $crypttext64;
  }
}
?>
