<?php

    if (!file_exists("conf/database.php"))
    {
        if (isset($_POST["username"]) && isset($_POST["password"]) && isset($_POST["server"]) && isset($_POST["database"]))
        {
            $server = strip_tags(addslashes($_POST["server"]));
            $database = strip_tags(addslashes($_POST["database"]));
            $user = strip_tags(addslashes($_POST["username"]));
            $password = strip_tags(addslashes($_POST["password"]));
            file_put_contents("conf/database.php", "<?php    const SERVER = \"$server\";
                    const USER = \"$user\";
                    const PASSWORD = \"$password\";
                    const DATABASE = \"$database\";
                    \$connessione = mysqli_connect(SERVER, USER, PASSWORD, DATABASE) or die(\"Impossibile connettersi al database\"); ?>");
        
            //header("Location:" . $_SERVER["PHP_SELF"]);
            $connessione = mysqli_connect($server, $user, $password, $database);
            $query = "CREATE TABLE tasks (idTask INT(11) PRIMARY KEY AUTO_INCREMENT, task VARCHAR(1000), completed TINYINT(1), user CHAR(25))";
            if(!mysqli_query($connessione, $query))
            {
                echo mysqli_error($connessione);
            }
            mysqli_close($connessione);
            header("Location:index.php");
        }        
    }

?>