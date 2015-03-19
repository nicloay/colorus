<?php
    $dir='tmpimg';
    $tmpfname = $dir . '/' . uniqid() . '.png';
    $fp = fopen($tmpfname, 'w+');
    fwrite($fp, base64_decode($_POST["imgdata"]));
    fclose($fp);
    echo $tmpfname;
?>