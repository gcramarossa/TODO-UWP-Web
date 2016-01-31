<?php
    include "../conf/database.php";
    if (isset($_GET["task"]) && isset($_GET["userID"]))
    {
        $task = strip_tags(addslashes($_GET["task"]));
        $userID = strip_tags(addslashes($_GET["userID"]));
        mysqli_query($connessione, "INSERT INTO tasks VALUES(NULL, '$task', 0, '$userID')");
        echo mysqli_insert_id($connessione);
        mysqli_close($connessione);
    }
?>