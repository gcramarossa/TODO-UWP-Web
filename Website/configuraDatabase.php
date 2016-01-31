<?php
    if (file_exists("conf/database.php"))
    {
        header("Location:index.php");
    }
    else
    {
        include "api/creaDatabase.php";
    }
?>
<!doctype html>
<html>
    <head>
        <title>CPP Online compiler</title>
        <link rel="stylesheet" href="css/general.css" />
        <script type="text/javascript" src="js/AJAX.js"></script>
    </head>
    <body>
        <div id="container" class="arrotondato">
            <h1>Database Configurator</h1>
            <p>
                <form action="" method="post">
                <p>Server: <input name="server" type="text"/></p>
                <p>Database Name:  <input name="database" type="text"/></p>
                <p>Username: <input name="username" type="text"/></p>
                <p>Password: <input name="password" type="password"/></p> 
                <p><input type="submit" value="Configure Database"/></p>
                </form>
            </p>
        </div>
    </body>
</html>