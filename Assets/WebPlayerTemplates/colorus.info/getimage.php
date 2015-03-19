<?php
    // add this to cron
    // */5 *  *  *  *  root   /bin/find /usr/share/nginx/img/img/ -mmin +5 -exec /bin/rm -f '{}' \;

    // add this to nginx
    //  rewrite ^/img/(.*\.png)$ /getimage.php?imagename=$1 last;
    $dir='tmpimg';
    $filename = $_GET["imagename"];
    header('content-type: image/gif');
    $fullPath=$dir . '/' . $filename;
    $im = file_get_contents($fullPath);
    echo $im;
    unlink($fullPath);
?>