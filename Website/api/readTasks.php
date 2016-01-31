<?php
    include "../conf/database.php";
    if (isset($_GET["userID"]))
    {
        $userID = strip_tags(addslashes($_GET["userID"]));
        $recordSet = mysqli_query($connessione, "SELECT * FROM tasks WHERE user = '$userID'");
        if (mysqli_num_rows($recordSet) > 0)
        {
            while ($record = mysqli_fetch_array($recordSet))
            {
                echo $record["idTask"] . ":" . $record["completed"] . ":" .$record["task"];
                echo "#ENDTASK#";
            }
        }
        mysqli_close($connessione);
    }
?>