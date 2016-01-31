<?php
    include "../conf/database.php";
    if (isset($_GET["taskID"]) && isset($_GET["userID"]))
    {
        $task = (int) $_GET["taskID"];
        $userID = strip_tags(addslashes($_GET["userID"]));
        mysqli_query($connessione, "DELETE FROM tasks WHERE idTask = $task AND user = '$userID'");
        mysqli_close($connessione);
    }
?>