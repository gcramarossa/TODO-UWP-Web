<?php
    include "../conf/database.php";
    if (isset($_GET["taskID"]) && isset($_GET["userID"]) && isset($_GET["status"]))
    {
        $task = (int) $_GET["taskID"];
        $userID = strip_tags(addslashes($_GET["userID"]));
        $status = (int) $_GET["status"];
        mysqli_query($connessione, "UPDATE tasks SET completed = '$status' WHERE idTask = $task AND user = '$userID'");
        mysqli_close($connessione);
    }
?>