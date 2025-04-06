var dataTable;

$(document).ready(function () {
    $('#dailyAttendanceLink').click(function (event) {
        event.preventDefault();

        var currentDate = new Date();
        var day = currentDate.getDay();

        if (day == 0 || day == 6 || day == 5 || day == 4) {
            Swal.fire({
                title: 'Selecciona la fecha de asistencia',
                html: '<input type="date" id="fechaAsistencia" class="swal2-input">',
                icon: 'info',
                showCancelButton: false,
                confirmButtonColor: '#DD6B55',
                confirmButtonText: 'Continuar',
                preConfirm: () => {
                    // Obtener el valor del datebox
                    const fecha = document.getElementById('fechaAsistencia').value;
                    if (!fecha) {
                        Swal.showValidationMessage('Por favor selecciona una fecha');
                        return false;  // Evitar que se cierre sin una fecha
                    }
                    return fecha;
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    // Almacenar la fecha en una variable
                    const fechaSeleccionada = result.value;
                    console.log("Fecha seleccionada:", fechaSeleccionada);
                    document.cookie = "dateAsistencia="+fechaSeleccionada+"; max-age=30; path=/";
                    window.location.href = $(this).attr('href');; // Cambia "tu-enlace" por el enlace deseado
                }
            });
        } else {
            swal.fire({
                title: "Acción no permitida",
                text: "Solo se puede agregar asistencias los sábados, domingos o lunes.",
                type: "info",
                showCancelButton: false,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Entendido",
                closeOnConfirm: true
            });
        }
    });
    cargarDatatable();
});


function cargarDatatable() {
    dataTable = $("#tblAsistencias").DataTable({
        "ajax": {
            "url": "/admin/Asistencias/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "paging": false,
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "nombre", "width": "10%" },
            { "data": "apellido", "width": "10%" },
            { "data": "tipoDocumento.tDocumento", "width": "5%" },
            { "data": "documento", "width": "10%" },
            { "data": "colegio", "width": "10%" },
            { "data": "asistencias", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Asistencias/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                <i class="far fa-edit"></i>Editar
                                </a>
                            </div>
                            `;
                }, "width": "10%"
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "width": "100%"
    });

}

function Delete(url) {
    Swal.fire({
        title: "¿Está seguro de borrar?",
        text: "Este contenido no se puede recuperar.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sí, borrar!",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) { // Si el usuario confirma
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload(); // Recargar tabla de datos
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Ocurrió un error al realizar la operación.");
                }
            });
        }
    });
}