<?php
    error_reporting(E_ALL);
    if (!file_exists("conf/database.php"))
    {
        header("Location: configuraDatabase.php");
    }
    
    include "conf/database.php";
    if (isset($_POST["userid"]))
    {
        $user = strip_tags(addslashes($_POST["userid"]));
        $recordSet = mysqli_query($connessione, "SELECT * FROM tasks WHERE user = '$user'");
    }
    
?>
<!doctype html>
<html>
    <head>
        <title>CPP Online compiler</title>
        <link rel="stylesheet" href="css/general.css" />
    </head>
    <body>
        <div id="container" class="arrotondato">
            <h1>TODO: UWP+A4D</h1>
            <p>

                <form action="" method="post">
                    User ID:
                    <input type="text" name="userid"/>
                    <input type="submit" value="Search for User ID Tasks" />
                </form>
            </p>
            <p>
                <?php 
                    if (isset($_POST["userid"]))
                    {
                        if (mysqli_num_rows($recordSet) > 0)
                        {
                            echo "<table border='1'>";
                            echo "<tr>";
                            echo "<td> Task </td>";                           
                            echo "<td> Completed </td>";                            
                            echo "</tr>";
                            while ($record = mysqli_fetch_array($recordSet))
                            {
                                echo "<tr>";
                                echo "<td width='80%'> " . $record["task"] . "</td>";
                                echo "<td> " . $record["completed"] . "</td>";
                                echo "</tr>";
                            }
                            echo "</table>";
                        }
                        else
                        {
                            echo "No tasks found";
                        }                        
                    }

                ?>
            </p>
        </div>
    </body>
</html>