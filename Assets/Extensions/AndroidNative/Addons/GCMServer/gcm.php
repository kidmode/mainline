<?php


// API access key from Google API's Console
define( 'API_ACCESS_KEY', 'YOU_API_KEY' );
//define( 'API_ACCESS_KEY', 'AIzaSyC6MC2SvJSX7YeRw_FQnPieid6stE7TMTI' );

$registrationIds = array("YOUR_DEVICE_REGISTRATION_ID_HERE");
//$registrationIds = array("APA91bGjfYDAz8c-9eI6JNOpbQV6qswpuTG5okT0ChYtrLTgOKUwDjthkazzuPPO0W_A6vTfWk-RbrFfSERuVNqDFg9tozH_3Z5GgNLUtXU5Pi7l-5t45sJ2naVis3VGEUj8-m44LeX-oc6KvGxD8194ub7lT2wgqZK6Ey09wOC6Ixa-Y0hfRP4");

// prep the bundle
$msg = array
(
    'title'         => 'This is a title. title',
    'subtitle'      => 'This is a subtitle. subtitle',
    'message'       => 'here is a message. message'
);

$fields = array
(
    'registration_ids'  => $registrationIds,
    'data'              => $msg
);

$headers = array
(
    'Authorization: key=' . API_ACCESS_KEY,
    'Content-Type: application/json'
);

$ch = curl_init();
curl_setopt( $ch,CURLOPT_URL, 'https://android.googleapis.com/gcm/send' );
curl_setopt( $ch,CURLOPT_POST, true );
curl_setopt( $ch,CURLOPT_HTTPHEADER, $headers );
curl_setopt( $ch,CURLOPT_RETURNTRANSFER, true );
curl_setopt( $ch,CURLOPT_SSL_VERIFYPEER, false );
curl_setopt( $ch,CURLOPT_POSTFIELDS, json_encode( $fields ) );
$result = curl_exec($ch );
curl_close( $ch );

echo $result;