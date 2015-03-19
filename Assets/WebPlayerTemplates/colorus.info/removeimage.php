<?php
    $dir='tmpimg';
    $imageName = $_GET["imageName"];
    $fullPath=$dir . '/' . $imageName;
    unlink($fullPath);
?>