var textbox = document.getElementById("tboxUpdate");
var button = document.getElementById("btnUpdatepfe");

if (textbox) {
    textbox.addEventListener("input", function () {
        var filepath = textbox.value;
        var filename = filepath.substring(filepath.lastIndexOf("\\") + 1);

        var caracteresEspeciales = /[!@#$%^&*()_+\-=\[\]{};':"\\|,<>\/?]+/;

        if (filename.trim() === "") {
            button.disabled = true;
        } else {
            if (caracteresEspeciales.test(filename.trim())) {
                button.disabled = true;
                document.getElementById("lblerror").textContent = "No puede contener caracteres especiales " + caracteresEspeciales.toString();
            } else {
                button.disabled = false;
            }
        }
    });
} 