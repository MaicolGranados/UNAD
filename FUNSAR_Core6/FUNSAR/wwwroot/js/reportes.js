function mostrarDiv() {
    var reporteSeleccionado = document.getElementById("reporteDropdown").value;
    document.getElementById("divProcesoParticipante").style.display = "none";
    document.getElementById("divPagos").style.display = "none";
    document.getElementById("divSalidas").style.display = "none";
    document.getElementById("divFiltroBrigada").style.display = "none";
    document.getElementById("fechapagos").style.display = "none";
    document.getElementById("fechaproceso").style.display = "none";
    document.getElementById("btnGenerar").disabled = true;

    if (reporteSeleccionado === "Pagos") {
        document.getElementById("divPagos").style.display = "block";
        document.getElementById("divFiltroBrigada").style.display = "block";
        document.getElementById("fechapagos").style.display = "block";
        document.getElementById("btnGenerar").disabled = false;
    } else if (reporteSeleccionado === "Procesos Participantes") {
        document.getElementById("divProcesoParticipante").style.display = "block";
        document.getElementById("divFiltroBrigada").style.display = "block";
        document.getElementById("fechaproceso").style.display = "block";
        document.getElementById("btnGenerar").disabled = false;
    }else if (reporteSeleccionado === "Salidas") {
        document.getElementById("divSalidas").style.display = "block";
        document.getElementById("fechapagos").style.display = "block";
        document.getElementById("btnGenerar").disabled = false;
    }
}

function toggleResponsablePago() {
    var checkbox = document.getElementById("responsablePagoCheckbox");
    var datosResponsableDiv = document.getElementById("datosResponsablePago");

    if (checkbox.checked) {
        datosResponsableDiv.style.display = "block";
    } else {
        datosResponsableDiv.style.display = "none";
    }
}

function toggleAcudiente() {
    var checkbox = document.getElementById("acudienteCheckbox");
    var datosAcudienteDiv = document.getElementById("datosAcudiente");

    if (checkbox.checked) {
        datosAcudienteDiv.style.display = "block";
    } else {
        datosAcudienteDiv.style.display = "none";
    }
}

function toggleFiltroBrigada() {
    var checkbox = document.getElementById("filtroBrigadaCheckbox");
    var brigada = document.getElementById("divBrigada");

    if (checkbox.checked) {
        brigada.style.display = "block";
    } else {
        brigada.style.display = "none";
    }
}


function resetForm() {
    resetCheckboxes();
    mostrarDiv();
}

function resetCheckboxes() {
    var responsablePagoCheckbox = document.getElementById("responsablePagoCheckbox");
    var asistenciaOpcionalCheckbox = document.getElementById("asistenciaOpcionalCheckbox");

    if (responsablePagoCheckbox) responsablePagoCheckbox.checked = false;
    if (asistenciaOpcionalCheckbox) asistenciaOpcionalCheckbox.checked = false;

    document.getElementById("datosResponsablePago").style.display = "none";
}

document.getElementById("formReporte").addEventListener('submit', function (event) {
    event.preventDefault();
    let semestreSeleccionado = document.getElementById("opcionesSemestre");
    var dateini = document.getElementById("fechaIni");
    var dateend = document.getElementById("fechaFin");
    if (dateini == null || dateend == null || dateini.value === '' || dateend.value === '' && semestreSeleccionado != null || semestreSeleccionado == '') {
        let textoSeleccionado = semestreSeleccionado.options[semestreSeleccionado.selectedIndex].text;
        const y = textoSeleccionado.substring(0,4);
        var ini = `${y}-01-01`;
        var end = `${y}-12-31`;
        textoSeleccionado = textoSeleccionado.substring(5, 6);
        if (textoSeleccionado == 1) {
            ini = `${y}-01-01`;
            end = `${y}-06-30`;
        } else if (textoSeleccionado == 2) {
            ini = `${y}-07-01`;
            end = `${y}-12-31`;
        }
        dateini.value = ini;
        dateend.value = end;
    }

    // Validación de fechas
    if (dateini.value === '' || dateend.value === '') {
        swal.fire({
            title: "Información",
            text: "Debes seleccionar el rango de fechas para la generación del reporte.",
            type: "info",
            showCancelButton: false,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "Entendido",
            closeOnConfirm: true
        });
        return;
    }

    // Mostrar pantalla de carga
    showLoading();

    // Crear el FormData y enviar la petición
    const formData = new FormData(this);
    fetch('Reportes/Generate', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error en la respuesta del servidor.');
                toastr.error('Error al generar el reporte.');
            } else {
                return response.blob();
            }
        })
        .then(blob => {
            // Crear y descargar el archivo
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'report.xlsx'; // Nombre del archivo
            document.body.appendChild(a);
            a.click();
            a.remove(); // Remover el elemento del DOM
            toastr.success('Reporte Generado correctamente.');
        })
        .catch(error => {
            toastr.error('Error al generar el reporte.');
        })
        .finally(() => {
            // Ocultar pantalla de carga
            hideLoading();
        });
});


function showLoading() {
    document.getElementById("loading").style.display = "block";
}

function hideLoading() {
    document.getElementById("loading").style.display = "none";
}

// Mostrar ventana de carga cuando se envíe un formulario
document.addEventListener("DOMContentLoaded", function () {
    var form = document.querySelector("form");
    if (form) {
        form.addEventListener("submit", function () {
            showLoading();
        });
    }
});

document.addEventListener("beforeunload", function () {
    hideLoading();
})

// Opcional: ocultar la ventana cuando la página esté completamente cargada
window.onload = function () {
    hideLoading();
};

function mostrarCargando() {
    // Muestra la ventana de carga
    document.getElementById("loading").style.display = "block";
    setTimeout(function () {
        window.location.href = '/Admin/Reportes/Index';  // Redirecciona al índice
    }, 15000);
    //fetch('https://localhost:7029/Admin/Reportes/Generate')
    //    .then(response => {
    //        if (response.ok) { // response.ok verifica si el estado es 200-299
    //            hideLoading();
    //            console.log("Request successful! Status:", response.status);
    //        } else {
    //            console.log("Request failed. Status:", response.status);
    //        }
    //    })
    //    .catch(error => {
    //        console.error("Network error or request failed", error);
    //    });
}